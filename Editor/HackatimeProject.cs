using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Blizz606.UnityHackatime
{
    internal static class HackatimeProject
    {
        internal static string Root { get { return Path.GetFullPath(Path.Combine(Application.dataPath, "..")); } }

        internal static string ProjectName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(HackatimeSettings.ProjectOverride)) return HackatimeSettings.ProjectOverride.Trim();
                string[] config = ReadProjectFile();
                if (config.Length > 0 && !string.IsNullOrWhiteSpace(config[0])) return config[0].Trim();
                try { string name = new DirectoryInfo(Root).Name; if (!string.IsNullOrEmpty(name)) return name; } catch { }
                if (!string.IsNullOrWhiteSpace(Application.productName)) return Application.productName;
                return "Unity Project";
            }
        }

        internal static string Branch
        {
            get
            {
                string[] config = ReadProjectFile();
                if (config.Length > 1 && !string.IsNullOrWhiteSpace(config[1])) return config[1].Trim();
                try
                {
                    string headPath = Path.Combine(Root, ".git", "HEAD");
                    if (!File.Exists(headPath)) return null;
                    string head = File.ReadAllText(headPath).Trim();
                    const string prefix = "ref: refs/heads/";
                    return head.StartsWith(prefix, StringComparison.Ordinal) ? head.Substring(prefix.Length).Trim() : "detached";
                }
                catch { return null; }
            }
        }

        internal static string ActiveEntity()
        {
            string scene = EditorSceneManager.GetActiveScene().path;
            return string.IsNullOrEmpty(scene) ? "Unsaved Scene" : scene.Replace('\\', '/');
        }

        internal static string[] ReadProjectFile()
        {
            try
            {
                string path = Path.Combine(Root, HackatimeSettings.ProjectFileName);
                return File.Exists(path) ? File.ReadAllLines(path) : new string[0];
            }
            catch { return new string[0]; }
        }
    }
}
