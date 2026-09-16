using UnityEditor;

namespace Blizz606.UnityHackatime
{
    internal static class HackatimeSettings
    {
        internal const string DefaultApiUrl = "https://hackatime.hackclub.com/api/hackatime/v1";
        internal const string ProjectFileName = ".wakatime-project";
        private const string Prefix = "Blizz606.UnityHackatime/";
        private const string ApiKeyKey = Prefix + "ApiKey";
        private const string ApiUrlKey = Prefix + "ApiUrl";
        private const string ProjectOverrideKey = Prefix + "ProjectOverride";
        private const string EnabledKey = Prefix + "Enabled";
        private const string DebugKey = Prefix + "Debug";

        internal static string ApiKey { get { return EditorPrefs.GetString(ApiKeyKey, string.Empty); } set { EditorPrefs.SetString(ApiKeyKey, value ?? string.Empty); } }
        internal static string ApiUrl { get { return EditorPrefs.GetString(ApiUrlKey, DefaultApiUrl); } set { EditorPrefs.SetString(ApiUrlKey, value ?? DefaultApiUrl); } }
        internal static string ProjectOverride { get { return EditorPrefs.GetString(ProjectOverrideKey, string.Empty); } set { EditorPrefs.SetString(ProjectOverrideKey, value ?? string.Empty); } }
        internal static bool TrackingEnabled { get { return EditorPrefs.GetBool(EnabledKey, true); } set { EditorPrefs.SetBool(EnabledKey, value); } }
        internal static bool DebugLogging { get { return EditorPrefs.GetBool(DebugKey, false); } set { EditorPrefs.SetBool(DebugKey, value); } }

        internal static string NormalizedApiUrl
        {
            get
            {
                string value = (ApiUrl ?? string.Empty).Trim().TrimEnd('/');
                if (value.Equals("https://hackatime.hackclub.com", System.StringComparison.OrdinalIgnoreCase))
                    return DefaultApiUrl;
                return string.IsNullOrEmpty(value) ? DefaultApiUrl : value;
            }
        }
    }
}
