namespace Cthangover.Mods.VillagePack.Tests
{
    using System.Linq;
    using Cthangover.Core.Mods;
    using Godot;

    /// <summary>Activates the opt-in winter journey; production content contains no test driver.</summary>
    public class SnowEnemiesTestInitializer : IModInitializer
    {
        private static bool queued;
        public void OnModLoaded(string id) { }
        public void OnModResourcesReady()
        {
            var args = OS.GetCmdlineUserArgs();
            if (queued || !args.Contains("--itest=snow_enemies")) return;
            queued = true;
            string shots = null;
            foreach (var arg in args)
                if (arg.StartsWith("--shots=")) { shots = arg.Substring(8); break; }
            ModRoot.AddDeferred(new SnowEnemiesTestDriver(shots));
        }
    }
}
