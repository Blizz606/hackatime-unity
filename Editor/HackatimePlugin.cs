using UnityEditor;

namespace Blizz606.UnityHackatime
{
    [InitializeOnLoad]
    internal static class HackatimePlugin
    {
        static HackatimePlugin() { HackatimeActivityTracker.Initialize(); }
        internal static void Reinitialize() { HackatimeActivityTracker.Initialize(); }
    }
}
