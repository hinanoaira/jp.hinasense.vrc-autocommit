using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Hinasense.VRCAutoCommit.Editor
{
    public static class GitAutoCommit
    {
        private const string MenuEnabledKey = "Hinasense_VRCAutoCommit_Enabled";
        private const string LogPrefix = "[GitAutoCommit] ";

        public static bool IsEnabled
        {
            get => EditorPrefs.GetBool(MenuEnabledKey, true);
            set => EditorPrefs.SetBool(MenuEnabledKey, value);
        }

        [MenuItem("Tools/Git Auto Commit/Enabled", false, 1)]
        private static void ToggleEnabled()
        {
            IsEnabled = !IsEnabled;
        }

        [MenuItem("Tools/Git Auto Commit/Enabled", true)]
        private static bool ValidateToggleEnabled()
        {
            Menu.SetChecked("Tools/Git Auto Commit/Enabled", IsEnabled);
            return true;
        }

        [MenuItem("Tools/Git Auto Commit/Commit Now", false, 2)]
        public static void CommitNowMenu()
        {
            ExecuteCommit();
        }

        public static bool ExecuteCommit()
        {
            if (!IsEnabled)
            {
                Debug.Log(LogPrefix + "Skipped commit: Auto-commit is disabled.");
                return false;
            }

            try
            {
                // 未保存のアセットを保存
                AssetDatabase.SaveAssets();

                string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

                // git status 確認
                var statusResult = RunGitCommand(projectRoot, "status --porcelain");
                if (string.IsNullOrWhiteSpace(statusResult.Output) && string.IsNullOrWhiteSpace(statusResult.Error))
                {
                    Debug.Log(LogPrefix + "No changes detected to commit.");
                    return true;
                }

                // git add .
                var addResult = RunGitCommand(projectRoot, "add .");
                if (addResult.ExitCode != 0)
                {
                    Debug.LogError(LogPrefix + $"Failed to git add: {addResult.Error}");
                    return false;
                }

                // コミットメッセージ生成 (YYYYMMDDhhmm)
                string commitMessage = DateTime.Now.ToString("yyyyMMddHHmm");

                // git commit -m "yyyyMMddHHmm"
                var commitResult = RunGitCommand(projectRoot, $"commit -m \"{commitMessage}\"");
                if (commitResult.ExitCode == 0)
                {
                    Debug.Log(LogPrefix + $"Committed successfully with message: \"{commitMessage}\"");
                    return true;
                }
                else
                {
                    Debug.LogWarning(LogPrefix + $"git commit returned code {commitResult.ExitCode}: {commitResult.Output}\n{commitResult.Error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(LogPrefix + $"Exception during auto-commit: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        private static (int ExitCode, string Output, string Error) RunGitCommand(string workingDirectory, string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using var process = new Process { StartInfo = psi };
            var output = new StringBuilder();
            var error = new StringBuilder();

            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data != null) output.AppendLine(e.Data);
            };
            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data != null) error.AppendLine(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            bool exited = process.WaitForExit(10000);
            if (!exited)
            {
                try { process.Kill(); } catch { }
                return (-1, output.ToString(), "Process timed out.");
            }

            return (process.ExitCode, output.ToString().Trim(), error.ToString().Trim());
        }
    }
}

