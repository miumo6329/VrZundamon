using UnityEngine;
using UnityEngine.Events;


public class TalkModel : MonoBehaviour
{
    private AudioTranscriber transcriber;

    [SerializeField] private UnityEvent<string> OnHandleTranscriptionComplete;

    public void Initialize(ref SystemConfig config)
    {
        transcriber = gameObject.AddComponent<AudioTranscriber>();
        string url = "http://" + config.data.WhisperLocalAPI.IP + ":" + config.data.WhisperLocalAPI.Port + "/transcribe/";
        Debug.Log($"WhisperLocalAPI URL: {url}");
        transcriber.Initialize(url);

        // イベントの購読
        transcriber.OnTranscriptionComplete += HandleTranscriptionComplete;
    }

    public void OnStartSpeaking()
    {
        transcriber.StartRecording();
    }

    public void OnStopSpeaking()
    {
        transcriber.StopRecording();
    }

    // 音声認識結果を処理する
    private void HandleTranscriptionComplete(string transcriptionResult)
    {
        if (!string.IsNullOrEmpty(transcriptionResult))
        {
            OnHandleTranscriptionComplete.Invoke(transcriptionResult);
            Debug.Log($"音声認識結果: {transcriptionResult}");
        }
        else
        {
            Debug.LogError("音声認識に失敗しました。");
        }
    }
}
