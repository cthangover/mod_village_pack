namespace Cthangover.Mods.VillagePack.Tests
{
    using System;
    using System.Collections.Generic;
    using Cthangover.Core.Bootstrap;
    using Cthangover.Core.Mods.Resolvers;
    using Cthangover.Core.Scenes;
    using Cthangover.Core.Utils;
    using Cthangover.Mods.Dialog;
    using Cthangover.Mods.Rpg;
    using Cthangover.Mods.Rpg.Model;
    using Cthangover.Mods.Rpg.Render;
    using Godot;

    /// <summary>Walks through seven real entrances and exits, checks source geometry and records both projections.</summary>
    public partial class BiomeExpansionDriver : Node
    {
        private static readonly string[] Exteriors = { "rpg_village", "rpg_village", "rpg_forest", "rpg_desert", "rpg_desert", "rpg_winter", "rpg_winter" };
        private static readonly string[] Buildings = { "wood_townhouse", "forest_lodge", "forest_lodge", "desert_house", "desert_inn", "snow_cabin", "snow_lodge" };
        private static readonly string[] Rooms = { "wood_home_room", "forest_hall_room", "forest_hall_room", "desert_home_room", "desert_inn_room", "snow_home_room", "snow_lodge_room" };
        private static readonly Vector2I[] Doors = { new(37,11),new(52,32),new(38,12),new(49,12),new(62,12),new(49,12),new(62,12) };
        private readonly string shots;
        private int stage, index, passed, failed;
        private double elapsed;
        private ulong waitStarted, readyFrame;
        private Key? release;
        private Vector2I activeDoor;
        private static RpgWorldView View => RpgWorldHost.Current?.View;
        private static DialogBox Box => RpgInput.FindDialogBox();
        private string RoomId => "rpg_plus_" + Exteriors[index].Substring(4) + "_" + Buildings[index];

        /// <summary>Waits for a mounted, idle map and two actual frames before checking geometry or capturing.</summary>
        public BiomeExpansionDriver(string shots)
        {
            this.shots = shots;
            Name = "BiomeExpansionDriver";
            var timer = new Timer { WaitTime = .1, Autostart = true };
            timer.Timeout += Tick;
            AddChild(timer);
        }

        private void Tick()
        {
            if (stage < 0) return;
            if (release.HasValue)
            {
                Input.ParseInputEvent(new InputEventKey { PhysicalKeycode=release.Value, Keycode=release.Value, Pressed=false });
                release=null;
            }
            elapsed += .1;
            if (elapsed > 180) { Check(false, "architecture journey timeout at " + stage + "/" + index); Finish(); return; }
            try { Run(); }
            catch (Exception e) { Check(false, e.ToString()); Finish(); }
        }

        private void Run()
        {
            if (!Boot.IsReady || string.IsNullOrEmpty(GetTree().Root.GetNodeOrNull<SceneManager>("SceneManager")?.CurrentSceneName)) return;
            if (Box?.IsRunning == true)
            {
                if (Box.IsWaitingAnswer) Box.TrySelectVariant(0); else Box.NextAction();
                return;
            }
            if (GodotSceneService.IsTransitioning) return;
            if (!MapIsReady()) return;
            switch (stage)
            {
                case 0:
                    if (View == null) return;
                    VerifyCatalog();
                    Next(); return;
                case 1:
                    RpgWorldHost.Teleport(new RpgTeleport { Map=Exteriors[index], X=Doors[index].X, Y=Doors[index].Y+1, Dir=RpgDirection.Up });
                    Next(); return;
                case 2:
                    if (View?.Map.ID != Exteriors[index]) return;
                    var door=View.Find(Exteriors[index]+"/rpg_plus_enter_"+Buildings[index]);
                    Check(door?.Current.Interact?.Teleport?.Map==RoomId, "facade door names its own interior: " + RoomId);
                    activeDoor=door.Actor.Cell;
                    var canEnter = View.CanEnter(activeDoor);
                    Check(canEnter, "entrance overrides the facade footprint");
                    if (!canEnter) { Finish(); return; }
                    View.PlacePlayer(activeDoor.X,activeDoor.Y+1,RpgDirection.Up);
                    VerifyScene(false);
                    Shot(Exteriors[index]+"_"+Buildings[index]);
                    Tap(Key.Up);Next();return;
                case 3:
                    if (View?.Map.ID != RoomId) return;
                    Check(View.Player.Cell==new Vector2I(5,8), "real movement enters at the safe indoor spawn");
                    Check(View.CanEnter(new Vector2I(5,8)) && View.CanEnter(new Vector2I(5,9)) && View.CanEnter(new Vector2I(6,9)), "floor and both threshold cells are walkable");
                    Check(!View.CanEnter(new Vector2I(0,5)) && !View.CanEnter(new Vector2I(6,0)), "walls block movement in either projection");
                    Check(!View.CanEnter(new Vector2I(2,5)) && !View.CanEnter(new Vector2I(8,6)) && !View.CanEnter(new Vector2I(9,4)), "bed, table and stove have solid feet");
                    VerifyScene(true);Shot(RoomId);
                    View.PlacePlayer(5,8,RpgDirection.Down);Tap(Key.Down);Next();return;
                case 4:
                    if (View?.Map.ID != Exteriors[index]) return;
                    Check(View.Player.Cell==activeDoor+Vector2I.Down, "real exit returns to the correct building instance");
                    index++;
                    if(index==Buildings.Length) { Finish(); return; }
                    stage=1;return;
            }
        }

        private bool MapIsReady()
        {
            var expected = stage == 3 ? RoomId : stage is 2 or 4 ? Exteriors[index] : null;
            var ready = View?.Map != null && (expected == null || View.Map.ID == expected) && !View.Player.IsMoving;
            var now = Time.GetTicksMsec();
            if (waitStarted == 0) waitStarted = now;
            if (now - waitStarted > 15000)
                throw new TimeoutException($"map readiness at stage {stage}/{index}, expected '{expected}'");
            if (!ready) { readyFrame = 0; return false; }
            if (readyFrame == 0) readyFrame = Engine.GetProcessFrames();
            if (Engine.GetProcessFrames() - readyFrame < 2) return false;
            waitStarted = readyFrame = 0;
            return true;
        }

        private void VerifyCatalog()
        {
            var maps=Entities.Collect<RpgMap>();
            foreach(var id in new[] { "rpg_village","rpg_forest","rpg_desert","rpg_winter","rpg_woodland_shrine" })
                Check(maps.TryGetValue(id,out var map) && map.Width>=48 && map.Height>=36, "outdoor map is expanded: "+id);
            var objects=Entities.Collect<RpgObject>();
            var rooms=new HashSet<string>(Rooms);
            Check(rooms.Count==6,"six distinct generated interiors are used");
            foreach(var room in rooms)
            {
                var id="village_pack/interior/"+room;
                Check(objects.TryGetValue(id,out var obj) && obj.Category=="interior","interior object is registered: "+id);
                if(obj?.Parts==null)continue;
                // Top-down draws the whole room; isometric draws floor and wall quads that together cover the picture.
                Check(obj.Parts.Exists(IsTopDownWhole),"the whole room is one flat top-down part: "+id);
                var covered=new bool[120];int total=0;
                foreach(var part in obj.Parts)
                {
                    if(part.Only!="isometric"||part.Crop==null||part.Crop.Length!=4)continue;
                    Check(part.Geometry!=RpgPartGeometry.Quad||part.Points?.Count==4,"architectural quads have four corners: "+id);
                    for(int y=part.Crop[1]/32;y<(part.Crop[1]+part.Crop[3])/32;y++)
                        for(int x=part.Crop[0]/32;x<(part.Crop[0]+part.Crop[2])/32;x++)
                        { if(!covered[y*12+x])total++;covered[y*12+x]=true; }
                }
                Check(total==120,"all floor, wall and threshold source cells are mapped: "+id);
            }
        }

        private void VerifyScene(bool indoor)
        {
            using var textures=new RpgTextures();
            using var builder=new RpgRenderSceneBuilder(textures.Image,textures.Charset);
            var scene=builder.Build(View.Map,View.Session.Cells,View.Session.Statics);
            bool groupFound=false,planesFound=false,sharedTexture=false;
            string expected=indoor?Rooms[index]:Buildings[index];
            foreach(var visual in scene.Visuals)
            {
                if(visual.Id?.Contains(expected)==true)
                {
                    groupFound=true;
                    if(visual.WorldQuad!=null)planesFound=true;
                    if(visual.Texture!=null)sharedTexture=true;
                }
            }
            Check(groupFound && sharedTexture,"one picture supplies the rendered object: "+expected);
            if(indoor) Check(planesFound,"isometric floor and walls use architectural planes");
            Check(scene.Warnings.Count==0,"every picture of the map resolves: "+string.Join("; ",scene.Warnings));
        }

        private static bool IsTopDownWhole(RpgObjectPart part)
            => part.Only=="topdown" && part.Geometry==RpgPartGeometry.Ground && part.Crop==null;

        private void Tap(Key key)
        {
            Input.ParseInputEvent(new InputEventKey { PhysicalKeycode=key,Keycode=key,Pressed=true });release=key;
        }
        private void Next() { stage++; }
        private void Shot(string name)
        {
            if(string.IsNullOrEmpty(shots)||DisplayServer.GetName()=="headless")return;
            DirAccess.MakeDirRecursiveAbsolute(shots);
            Cthangover.Mods.Interface.View.ViewportCapture.SavePng(GetViewport(),shots.TrimEnd('/')+"/"+name+".png");
        }
        private void Check(bool ok,string message)
        {
            if(ok)passed++;else failed++;
            GameLogger.Log("TEST",(ok?"PASS ":"FAIL ")+message);
        }
        private void Finish()
        {
            stage=-1;
            GameLogger.Log("TEST",$"[TEST_RESULT] name=biome_expansion passed={passed} failed={failed}");
            ApplicationExit.Quit(GetTree(),failed==0?0:1);
        }
    }
}
