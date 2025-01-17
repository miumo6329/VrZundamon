using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class AudioTranscriber : MonoBehaviour
{
    private AudioRecorder recorder;
    private string apiUrl;

    // 音声認識結果を返すイベント
    public event Action<string> OnTranscriptionComplete;

    public void Initialize(string apiUrl)
    {
        this.apiUrl = apiUrl;
        recorder = gameObject.AddComponent<AudioRecorder>();
        recorder.OnRecordingStopped += async clip => await OnRecordingStopped(clip);
    }

    public void StartRecording()
    {
        recorder.StartRecording();
    }

    public void StopRecording()
    {
        recorder.StopRecordingManually();
    }

    private async Task OnRecordingStopped(AudioClip clip)
    {
        Debug.Log("録音が完了しました。APIを呼び出します...");
        string result = await SendAudioToApiAsync(clip);
        if (OnTranscriptionComplete != null)
        {
            // 結果をイベントとして通知
            OnTranscriptionComplete.Invoke(result);
        }
    }

    private async Task<string> SendAudioToApiAsync(AudioClip clip)
    {
        byte[] wavData = WavUtility.FromAudioClip(clip);

        using (var client = new HttpClient())
        using (var content = new MultipartFormDataContent())
        {
            content.Add(new ByteArrayContent(wavData), "file", "audio.wav");

            HttpResponseMessage response = await client.PostAsync(apiUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError($"APIエラー: {response.StatusCode}");
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
