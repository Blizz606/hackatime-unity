# Hackatime for Unity

Editor-only Hackatime/WakaTime-compatible time tracking for Unity 2022.3 LTS and newer.

## Features

- Automatic heartbeats for scenes, saves, play mode and hierarchy activity.
- 30-second throttling, duplicate suppression, queueing and `429 Retry-After` handling.
- `.wakatime-project` project and branch overrides, then Git branch detection.
- Project-relative entities such as `Assets/Scenes/Forest.unity`.
- No runtime dependencies; no code is included in player builds.

## Installation

In Unity, open **Window → Package Manager**, select **+ → Add package from Git URL**, and enter the GitHub URL for this repository.

## Setup

Open **Window → Hackatime**, enter your personal API key, leave **Enable Tracking** on, and use **Test Connection**. The key is stored locally in Unity `EditorPrefs`; it is not stored in the project or committed to Git.

## Project naming

An optional Project Name override has highest priority. Otherwise the first line of `.wakatime-project`, the Unity project folder name, `Application.productName`, and finally `Unity Project` are used. The second line of `.wakatime-project` overrides the Git branch.

## Privacy

The plugin sends the project-relative active entity, timestamp, project, branch when available, language, category, save flag, editor name and normalized operating system to the configured Hackatime heartbeat endpoint. It does not send absolute local paths, machine names or the API key in the request body. The API key is sent only as a Bearer authorization header.

## Troubleshooting

Check the API key and endpoint in **Window → Hackatime**. Enable **Debug Logging** for non-sensitive diagnostics. `401` means the key was rejected, `403` access is forbidden, `404` indicates a wrong endpoint, `429` is retried after a delay, and `5xx`/network errors are transient service or connection failures.

## License and provenance

MIT. The supplied source file had no explicit copyright or license header; its provenance cannot be independently established from the file alone. Confirm that you have the right to redistribute it before publishing a public repository.
