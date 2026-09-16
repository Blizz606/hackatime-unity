using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Blizz606.UnityHackatime
{
    internal enum HackatimeConnectionStatus { Connected, NotConfigured, Unauthorized, Forbidden, NotFound, RateLimited, ServerError, NetworkError, Error }

    internal sealed class HackatimeClient
    {
        private const int TimeoutSeconds = 15;
        internal static HackatimeClient Instance { get; } = new HackatimeClient();
        internal bool RequestInFlight { get; private set; }
        internal double RetryAt { get; private set; }

        internal void Send(HackatimeHeartbeat heartbeat, Action<HackatimeConnectionStatus, string> completed = null)
        {
            if (RequestInFlight) { completed?.Invoke(HackatimeConnectionStatus.Error, "A request is already in progress."); return; }
            string key = HackatimeSettings.ApiKey.Trim();
            if (string.IsNullOrEmpty(key)) { completed?.Invoke(HackatimeConnectionStatus.NotConfigured, "API key is missing."); return; }
            RequestInFlight = true;
            string json = "[" + JsonUtility.ToJson(heartbeat) + "]";
            UnityWebRequest request = new UnityWebRequest(HackatimeSettings.NormalizedApiUrl + "/users/current/heartbeats", UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = TimeoutSeconds;
            request.redirectLimit = 3;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + key);
            request.SetRequestHeader("User-Agent", "unity-hackatime/1.0.0 unity/" + Application.unityVersion);
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            operation.completed += _ =>
            {
                HackatimeConnectionStatus status = GetStatus(request);
                string message = status == HackatimeConnectionStatus.Connected ? "Connected" : Describe(status, request);
                if (status == HackatimeConnectionStatus.RateLimited)
                {
                    int retry; if (!int.TryParse(request.GetResponseHeader("Retry-After"), out retry)) retry = 60;
                    RetryAt = EditorTime.Now + Math.Max(1, retry);
                }
                RequestInFlight = false;
                request.Dispose();
                completed?.Invoke(status, message);
            };
        }

        private static HackatimeConnectionStatus GetStatus(UnityWebRequest request)
        {
            long code = request.responseCode;
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError && code == 0) return HackatimeConnectionStatus.NetworkError;
            if (code >= 200 && code < 300) return HackatimeConnectionStatus.Connected;
            if (code == 401) return HackatimeConnectionStatus.Unauthorized;
            if (code == 403) return HackatimeConnectionStatus.Forbidden;
            if (code == 404) return HackatimeConnectionStatus.NotFound;
            if (code == 429) return HackatimeConnectionStatus.RateLimited;
            if (code >= 500) return HackatimeConnectionStatus.ServerError;
            return HackatimeConnectionStatus.Error;
        }

        private static string Describe(HackatimeConnectionStatus status, UnityWebRequest request)
        {
            switch (status)
            {
                case HackatimeConnectionStatus.Unauthorized: return "API key rejected.";
                case HackatimeConnectionStatus.Forbidden: return "Access forbidden.";
                case HackatimeConnectionStatus.NotFound: return "Endpoint not found.";
                case HackatimeConnectionStatus.RateLimited: return "Rate limited; retry delayed.";
                case HackatimeConnectionStatus.ServerError: return "Hackatime server error.";
                case HackatimeConnectionStatus.NetworkError: return "Network or timeout error.";
                default: return "Hackatime request failed (HTTP " + request.responseCode + ").";
            }
        }
    }

    internal static class EditorTime
    {
        internal static double Now { get { return (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds; } }
    }
}
