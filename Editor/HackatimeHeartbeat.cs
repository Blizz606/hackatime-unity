using System;

namespace Blizz606.UnityHackatime
{
    [Serializable]
    internal struct HackatimeHeartbeat
    {
        public string entity, type, project, branch, plugin, language, editor, operating_system, category;
        public double time;
        public bool is_write;

        internal HackatimeHeartbeat(string entity, bool isWrite)
        {
            this.entity = entity;
            type = "file";
            time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;
            project = HackatimeProject.ProjectName;
            branch = HackatimeProject.Branch;
            plugin = "unity-hackatime/1.0.0";
            language = HackatimeLanguageDetector.FromPath(entity);
            editor = "Unity";
            operating_system = HackatimePlatform.Name;
            category = "coding";
            is_write = isWrite;
        }
    }

    internal static class HackatimePlatform
    {
        internal static string Name
        {
            get
            {
#if UNITY_EDITOR_WIN
                return "Windows";
#elif UNITY_EDITOR_OSX
                return "macOS";
#else
                return "Linux";
#endif
            }
        }
    }
}
