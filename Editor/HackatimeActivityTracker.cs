using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blizz606.UnityHackatime
{
    internal static class HackatimeActivityTracker
    {
        private const double IntervalSeconds = 30.0;
        private static bool initialized, queued, queuedWrite, sending;
        private static double nextAllowed;
        private static string lastEntity;
        private static double lastSuccess;

        internal static string LastHeartbeatText
        {
            get { return lastSuccess <= 0 ? "Never" : FormatAge(EditorTime.Now - lastSuccess); }
        }

        internal static void Initialize()
        {
            Unregister();
            initialized = true;
            if (!HackatimeSettings.TrackingEnabled) return;
            Register();
            Request(false);
        }

        internal static void Shutdown() { Unregister(); initialized = false; }

        internal static void Request(bool write)
        {
            if (!initialized || !HackatimeSettings.TrackingEnabled || string.IsNullOrWhiteSpace(HackatimeSettings.ApiKey)) return;
            if (sending || EditorTime.Now < nextAllowed) { queued = true; queuedWrite |= write; return; }
            HackatimeHeartbeat heartbeat = new HackatimeHeartbeat(HackatimeProject.ActiveEntity(), write);
            if (!write && heartbeat.entity == lastEntity && EditorTime.Now - lastSuccess < IntervalSeconds) return;
            sending = true;
            HackatimeClient.Instance.Send(heartbeat, (status, message) =>
            {
                sending = false;
                nextAllowed = status == HackatimeConnectionStatus.RateLimited
                    ? Math.Max(EditorTime.Now + IntervalSeconds, HackatimeClient.Instance.RetryAt)
                    : EditorTime.Now + IntervalSeconds;
                if (status == HackatimeConnectionStatus.Connected)
                {
                    lastEntity = heartbeat.entity;
                    lastSuccess = EditorTime.Now;
                }
                if (HackatimeSettings.DebugLogging) Debug.Log("[Hackatime] " + message);
                if (queued && EditorTime.Now >= nextAllowed) { bool save = queuedWrite; queued = queuedWrite = false; Request(save); }
            });
        }

        private static void Register()
        {
            EditorApplication.update += OnUpdate;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorSceneManager.sceneSaved += OnSceneSaved;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneClosing += OnSceneClosing;
            EditorSceneManager.newSceneCreated += OnSceneCreated;
            EditorApplication.quitting += Shutdown;
        }

        private static void Unregister()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorSceneManager.sceneSaved -= OnSceneSaved;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneClosing -= OnSceneClosing;
            EditorSceneManager.newSceneCreated -= OnSceneCreated;
            EditorApplication.quitting -= Shutdown;
        }

        private static void OnUpdate() { if (queued && !sending && EditorTime.Now >= nextAllowed) { bool save = queuedWrite; queued = queuedWrite = false; Request(save); } }
        private static void OnPlayModeChanged(PlayModeStateChange _) { Request(false); }
        private static void OnHierarchyChanged() { Request(false); }
        private static void OnSceneSaved(Scene _) { Request(true); }
        private static void OnSceneOpened(Scene _, OpenSceneMode __) { Request(false); }
        private static void OnSceneClosing(Scene _, bool __) { Request(false); }
        private static void OnSceneCreated(Scene _, NewSceneSetup __, NewSceneMode ___) { Request(false); }

        private static string FormatAge(double seconds)
        {
            if (seconds < 2) return "just now";
            if (seconds < 60) return Math.Floor(seconds) + " seconds ago";
            if (seconds < 3600) return Math.Floor(seconds / 60) + " minutes ago";
            return Math.Floor(seconds / 3600) + " hours ago";
        }
    }
}
