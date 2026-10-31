namespace Cthangover.Mods.VillagePack.Tests
{
    using System;
    using System.Linq;
    using Cthangover.Core.Battle;
    using Cthangover.Core.Bootstrap;
    using Cthangover.Core.Games;
    using Cthangover.Core.Scenes;
    using Cthangover.Core.Settings;
    using Cthangover.Core.UI;
    using Cthangover.Core.Utils;
    using Cthangover.Mods.Dialog;
    using Cthangover.Mods.Interface.View;
    using Cthangover.Mods.Rpg;
    using Cthangover.FFBattle.UI;
    using Godot;

    /// <summary>
    /// Exercises authored data through real interaction, battle rewards and save/load.
    /// Enemy HP is reduced only after the battle starts, keeping the UI victory/loot path.
    /// </summary>
    public partial class SnowEnemiesTestDriver : Node
    {
        private const string Slot = "slot_9936";
        private readonly string shots;
        private int step, ticks, passed, failed;
        private double elapsed;
        private ulong nextAttack;
        private BattleSceneContext testedBattle;
        private static RpgWorldView View => RpgWorldHost.Current?.View;
        private static DialogBox Box => RpgInput.FindDialogBox();

        public SnowEnemiesTestDriver(string shots)
        {
            this.shots = shots;
            Name = "SnowEnemiesTestDriver";
            var timer = new Timer { WaitTime = 0.1, Autostart = true };
            timer.Timeout += Tick;
            AddChild(timer);
        }

        private void Tick()
        {
            if (step < 0) return;
            elapsed += 0.1;
            if (elapsed > 180) { Check(false, $"timeout at step {step}"); Finish(); return; }
            try { RunStep(); }
            catch (Exception ex) { Check(false, ex.ToString()); Finish(); }
        }

        private static readonly string[] EnemyNames = { "white_wolf", "walrus", "polar_bear", "yeti" };
        private static readonly int[] Health = { 24, 32, 40, 48 };
        private static readonly int[] Defence = { 0, 2, 1, 1 };
        private static readonly int[] Attack = { 5, 5, 7, 8 };
        private int enemyIndex;

        private void RunStep()
        {
            if (!Boot.IsReady || string.IsNullOrEmpty(GetTree().Root.GetNodeOrNull<SceneManager>("SceneManager")?.CurrentSceneName)) return;
            if (Box?.IsRunning == true)
            {
                if (Box.IsWaitingAnswer) Box.TrySelectVariant(0); else Box.NextAction();
                return;
            }
            if (GodotSceneService.IsTransitioning || ++ticks < 3) return;
            switch (step)
            {
                case 0:
                    if (View?.Map.ID != "rpg_village") return;
                    RpgWorldHost.Teleport(new RpgTeleport { Map = "rpg_winter", X = 20, Y = 14, Dir = RpgDirection.Down });
                    Next(); return;
                case 1:
                    if (View?.Map.ID != "rpg_winter") return;
                    VerifyEnemies(); Shot("snow_enemies_overview"); step = 10; ticks = 0; return;
                case 10:
                    if (View?.Map.ID != "rpg_winter") return;
                    var actor = View.Find("rpg_winter/village_pack_winter_" + EnemyNames[enemyIndex]);
                    Check(actor?.Visible == true, "enemy is visible before battle: " + EnemyNames[enemyIndex]);
                    if (actor == null) { Finish(); return; }
                    Check(View.CanEnter(actor.Actor.Cell + Vector2I.Down), "enemy has a walkable interaction cell");
                    View.PlacePlayer(actor.Actor.Cell.X, actor.Actor.Cell.Y + 1, RpgDirection.Up);
                    Shot("snow_enemy_" + EnemyNames[enemyIndex]);
                    Check(View.Interact(), "normal interaction starts " + EnemyNames[enemyIndex]);
                    Next(); return;
                case 11:
                    if (!Win("village_pack/" + EnemyNames[enemyIndex])) return;
                    Next(); return;
                case 12:
                    if (!ContinueVictory()) return;
                    Next(); return;
                case 13:
                    if (View?.Map.ID != "rpg_winter") return;
                    Check(View.Find("rpg_winter/village_pack_winter_" + EnemyNames[enemyIndex])?.Visible == false,
                        "victory removes only the defeated enemy: " + EnemyNames[enemyIndex]);
                    enemyIndex++;
                    if (enemyIndex < EnemyNames.Length) { step = 10; ticks = 0; return; }
                    SaveService.Save(Slot);
                    Check(GameSession.LoadAndEnter(this, Slot), "defeated snow enemies can be saved and loaded");
                    Next(); return;
                case 14:
                    if (View?.Map.ID != "rpg_winter") return;
                    foreach (var name in EnemyNames)
                        Check(View.Find("rpg_winter/village_pack_winter_" + name)?.Visible == false, "defeat persists after reload: " + name);
                    Check(View.Find("rpg_winter/winter_chest")?.Visible == true && View.Find("rpg_winter/winter_barrel")?.Visible == true,
                        "enemy flags leave existing winter containers available");
                    SaveService.DeleteSave(Slot); Finish(); return;
            }
        }

        private void VerifyEnemies()
        {
            var infos = Cthangover.Core.Mods.Resolvers.Entities.Collect<Cthangover.Core.Characters.CharacterInfo>();
            using var textures = new RpgTextures();
            for (int i = 0; i < EnemyNames.Length; i++)
            {
                string name = EnemyNames[i], id = "village_pack/" + name;
                Check(infos.TryGetValue(id, out var info), "pack registers character " + id);
                if (info == null) continue;
                Check(info.Health == Health[i] && info.Defence == Defence[i] && info.Attack == Attack[i] && info.Exp > 0,
                    "authored combat parameters load for " + name);
                var character = Cthangover.Core.Factories.Impls.CharacterFactory.Instance.Get(id);
                Check(character?.Image != null && character.Actions.Count == 2, "portrait and both battle actions resolve for " + name);
                Check(character?.Attributes.Health.Value == Health[i] && character.Attributes.Attack.Value == Attack[i],
                    "runtime factory preserves combat stats for " + name);
                Check(TranslationServer.Translate(info.Name).ToString() != info.Name, "enemy name is localized: " + name);
                var tex = textures.Charset("snow/" + name);
                int frame = name == "white_wolf" ? 48 : 64;
                Check(tex != null && tex.GetWidth() == frame * 3 && tex.GetHeight() == frame * 4,
                    "annotated 3x4 charset resolves for " + name);
                var actor = View.Find("rpg_winter/village_pack_winter_" + name);
                Check(actor?.Visible == true && actor.Current.Route?.Steps.Count == 4 && !View.CanEnter(actor.Actor.Cell),
                    "solid patrolling map enemy resolves for " + name);
            }
            Check(View.CanEnter(new Vector2I(38,14)) && View.CanEnter(new Vector2I(39,14)), "snow enemies preserve the entrance and exit");
        }

        private bool Win(string id)
        {
            if (GetTree().CurrentScene is not BattleSceneContext battle) return false;
            if (battle.IsDestroyed) return true;
            if (battle.ActiveBattleCore is not IBattleAutoPlayable auto || !auto.IsWaitingForPlayerInput || Time.GetTicksMsec() < nextAttack) return false;
            var enemies = battle.FindChild("FFEnemyPanel", true, false) as FFEnemyPanel;
            var players = battle.FindChild("FFPlayerPanel", true, false) as FFPlayerPanel;
            var menu = battle.FindChildren("*", "", true, false).OfType<FFMenuPanel>().FirstOrDefault();
            if (enemies == null || players == null || menu == null) return false;
            if (testedBattle != battle)
            {
                testedBattle = battle;
                Check(enemies.Widgets.Count == 1 && enemies.Widgets[0].Card.ID == id, "battle contains " + id);
                Shot("battle_" + EnemyNames[enemyIndex]);
                foreach (var enemy in enemies.Widgets) { enemy.Card.Attributes.Defence.Value = 0; enemy.Card.Attributes.Health.Value = 1; }
            }
            var target = enemies.Widgets.FirstOrDefault();
            foreach (var candidate in enemies.Widgets)
                if (!candidate.IsDead) { target = candidate; break; }
            var player = players.Widgets.FirstOrDefault();
            foreach (var candidate in players.Widgets)
                if (!candidate.IsDead) { player = candidate; break; }
            if (target == null || player == null) return false;
            player.Card.Attributes.Point.Value = 100;
            Click(player);
            for (int i = 0; i < menu.Entries.Count; i++)
                if (menu.Entries[i].Key == "attack") { menu.SelectedIndex = i; break; }
            menu.ConfirmSelection();
            for (int i = 0; i < menu.Entries.Count; i++)
                if (menu.Entries[i].Data is Cthangover.Core.Characters.ActionCharacter action &&
                    action.Type == Cthangover.Core.Characters.ActionCharacterType.ToEnemy)
                { menu.SelectedIndex = i; break; }
            menu.ConfirmSelection(); Click(target); nextAttack = Time.GetTicksMsec() + 1600;
            return false;
        }

        private bool ContinueVictory()
        {
            if (GetTree().Root.FindChild("ContinueButton", true, false) is not Button button || !button.IsVisibleInTree()) return false;
            button.EmitSignal(BaseButton.SignalName.Pressed); return true;
        }
        private static void Click(Control c) => c.EmitSignal(Control.SignalName.GuiInput, new InputEventMouseButton { ButtonIndex = MouseButton.Left, Pressed = true });
        private void Shot(string name)
        {
            if (string.IsNullOrEmpty(shots) || DisplayServer.GetName() == "headless") return;
            DirAccess.MakeDirRecursiveAbsolute(shots);
            ViewportCapture.SavePng(GetViewport(), shots.TrimEnd('/') + "/" + name + ".png");
        }
        private void Next() { step++; ticks = 0; }
        private void Check(bool ok, string message)
        {
            if (ok) passed++; else failed++;
            GameLogger.Log("TEST", (ok ? "PASS " : "FAIL ") + message);
        }
        private void Finish()
        {
            step = -1;
            GameLogger.Log("TEST", $"[TEST_RESULT] name=snow_enemies passed={passed} failed={failed}");
            Cthangover.Core.Bootstrap.ApplicationExit.Quit(GetTree(), failed == 0 ? 0 : 1);
        }
    }
}
