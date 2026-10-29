# VRChat Git Auto Commit

VRChatアバタービルド完了時に自動で変更をステージングし、日時のコミットメッセージ（`YYYYMMDDhhmm`）でGitコミットを作成するUnityエディタ拡張パッケージです。

## パッケージID
`jp.hinasense.vrc-autocommit`

## 機能
- **ビルド完了時の自動Gitコミット**: VRChat SDK の `IVRCSDKPostprocessAvatarCallback` を使用し、アバタービルドが成功・完了したタイミングで自動コミットを実行します（ビルドエラー時にはコミットされません）。
- **日時コミットメッセージ**: 現在日時をもとに `"YYYYMMDDhhmm"`（例: `202610300124`）の形式で自動生成します。
- **未保存アセットの自動保存**: コミット前に `AssetDatabase.SaveAssets()` を実行し、未保存の変更も確実にコミット対象に含めます。
- **変更検知**: コミット対象の差分がない場合は不要な空コミットを行いません。
- **トグル機能**: Unityエディタ上部のメニュー `Tools > Git Auto Commit > Enabled` から自動コミットの有効/無効を切り替えられます。
- **手動コミット**: `Tools > Git Auto Commit > Commit Now` から任意のタイミングで手動コミットを実行できます。

## 動作要件
- Unity 2022.3 以降
- VRCSDK Base (com.vrchat.base)
- Gitがインストールされており、コマンドライン（PATH）から実行可能であること

