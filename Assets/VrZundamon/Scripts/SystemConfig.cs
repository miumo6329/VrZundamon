using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SystemConfig
{
    // データクラスの定義
    [System.Serializable]
    public class WhisperLocalAPIConfig
    {
        public string IP;
        public int Port;
    }

    [System.Serializable]
    public class GeminiAPIConfig
    {
        public string Model;
        public string AccessToken;
    }

    [System.Serializable]
    public class Data
    {
        public WhisperLocalAPIConfig WhisperLocalAPI;
        public GeminiAPIConfig GeminiAPI;
    }

    public Data data;
    private string filePath;

    public SystemConfig()
    {
        // StreamingAssetsフォルダ内の設定ファイルのパス
        filePath = Path.Combine(Application.streamingAssetsPath, "SystemConfig.json");
        LoadConfig();
    }

    // 設定を読み込むメソッド
    public void LoadConfig()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            data = JsonConvert.DeserializeObject<Data>(json);
        }
        else
        {
            Debug.LogError("config.json not found in StreamingAssets!");
        }
    }

    // パラメータを更新してJSONファイルに保存するメソッド
    public void UpdateConfig(string whisperIP, int whisperPort, string geminiModel, string geminiAccessToken)
    {
        // 新しいパラメータで更新
        data.WhisperLocalAPI.IP = whisperIP;
        data.WhisperLocalAPI.Port = whisperPort;
        data.GeminiAPI.Model = geminiModel;
        data.GeminiAPI.AccessToken = geminiAccessToken;

        // 更新後の設定をJSONにシリアライズ
        string updatedJson = JsonConvert.SerializeObject(data, Formatting.Indented);

        // ファイルに保存
        File.WriteAllText(filePath, updatedJson);
        Debug.Log("Config updated and saved!");
    }
}