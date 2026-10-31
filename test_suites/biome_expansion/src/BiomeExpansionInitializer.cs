namespace Cthangover.Mods.VillagePack.Tests
{
    using System;
    using Cthangover.Core.Mods;
    using Godot;

    /// <summary>Mounts the architecture journey only through its opt-in suite flag.</summary>
    public class BiomeExpansionInitializer : IModInitializer
    {
        private static bool queued;
        public void OnModLoaded(string id) { }
        public void OnModResourcesReady()
        {
            var args = OS.GetCmdlineUserArgs();
            if (queued || Array.IndexOf(args, "--itest=biome_expansion") < 0) return;
            queued = true;
            string shots = null;
            foreach (var arg in args)
                if (arg.StartsWith("--shots=")) shots = arg.Substring(8);
            ModRoot.AddDeferred(new BiomeExpansionDriver(shots));
        }
    }
}
