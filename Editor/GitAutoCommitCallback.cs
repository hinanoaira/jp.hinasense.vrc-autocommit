using UnityEngine;
using VRC.SDKBase.Editor.BuildPipeline;

namespace Hinasense.VRCAutoCommit.Editor
{
    /// <summary>
    /// VRChatアバタービルド成功完了時に自動でGit Commitを実行するコールバック
    /// </summary>
    public class GitAutoCommitCallback : IVRCSDKPostprocessAvatarCallback
    {
        // ビルド後処理の最後に実行
        public int callbackOrder => 1000;

        public void OnPostprocessAvatar()
        {
            GitAutoCommit.ExecuteCommit();
        }
    }
}
