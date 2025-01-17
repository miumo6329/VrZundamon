using UnityEngine;

public class Manager : MonoBehaviour
{
    private SystemConfig systemConfig;

    [SerializeField] TalkModel talkModel;

    private void Start()
    {
        systemConfig = new SystemConfig();

        // 設定内容をデバッグログに表示
        Debug.Log($"WhisperLocalAPI IP: {systemConfig.data.WhisperLocalAPI.IP}, Port: {systemConfig.data.WhisperLocalAPI.Port}");
        Debug.Log($"GeminiAPI Model: {systemConfig.data.GeminiAPI.Model}, AccessToken: {systemConfig.data.GeminiAPI.AccessToken}");

        talkModel.Initialize(ref systemConfig);
    }

    // 設定の更新
    public void UpdateConfig(string whisperIP, int whisperPort, string geminiModel, string geminiAccessToken)
    {
        systemConfig.UpdateConfig(whisperIP, whisperPort, geminiModel, geminiAccessToken);
    }
}