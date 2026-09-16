using UnityEditor;
using UnityEngine;

namespace Blizz606.UnityHackatime
{
    internal sealed class HackatimeSettingsWindow : EditorWindow
    {
        private string status = "Not configured";
        private string apiKey, apiUrl, projectOverride;
        private bool enabled, debug;

        [MenuItem("Window/Hackatime")]
        private static void Open() { GetWindow<HackatimeSettingsWindow>("Hackatime"); }

        private void OnEnable() { Load(); }
        private void Load()
        {
            apiKey = HackatimeSettings.ApiKey; apiUrl = HackatimeSettings.ApiUrl; projectOverride = HackatimeSettings.ProjectOverride;
            enabled = HackatimeSettings.TrackingEnabled; debug = HackatimeSettings.DebugLogging;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Hackatime for Unity", EditorStyles.boldLabel);
            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Status", status);
            EditorGUILayout.Space(4);
            EditorGUI.BeginChangeCheck();
            apiKey = EditorGUILayout.PasswordField("API Key", apiKey);
            projectOverride = EditorGUILayout.TextField("Project Name", projectOverride);
            enabled = EditorGUILayout.Toggle("Enable Tracking", enabled);
            debug = EditorGUILayout.Toggle("Debug Logging", debug);
            apiUrl = EditorGUILayout.TextField("API URL", apiUrl);
            if (EditorGUI.EndChangeCheck()) Save(true);
            EditorGUILayout.Space(8);
            if (GUILayout.Button("Test Connection")) TestConnection();
            EditorGUILayout.LabelField("Last Heartbeat", HackatimeActivityTracker.LastHeartbeatText);
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("Only project-relative activity, project, branch, time, language, editor and normalized OS are sent. The API key is stored in Unity EditorPrefs and is never written to the project.", MessageType.Info);
        }

        private void Save(bool reinitialize)
        {
            HackatimeSettings.ApiKey = apiKey == null ? string.Empty : apiKey.Trim();
            HackatimeSettings.ProjectOverride = projectOverride;
            HackatimeSettings.ApiUrl = apiUrl;
            HackatimeSettings.TrackingEnabled = enabled;
            HackatimeSettings.DebugLogging = debug;
            if (reinitialize) HackatimePlugin.Reinitialize();
            status = enabled && !string.IsNullOrWhiteSpace(apiKey) ? "Configured" : "Not configured";
            Repaint();
        }

        private void TestConnection()
        {
            Save(false);
            if (string.IsNullOrWhiteSpace(HackatimeSettings.ApiKey)) { status = "Not configured"; Repaint(); return; }
            status = "Testing…"; Repaint();
            HackatimeClient.Instance.Send(new HackatimeHeartbeat(HackatimeProject.ActiveEntity(), false), (result, message) => { status = message; Repaint(); });
        }
    }
}
