using System;
using UnityEngine;

public class AudioRecorder : MonoBehaviour
{
    public event Action<AudioClip> OnRecordingStopped; // 録音停止時のイベント

    private AudioClip recordedClip;
    private const int sampleRate = 44100;
    private const float silenceThreshold = 0.01f; // 無音とみなす音量の閾値
    private const float silenceDuration = 1f; // 無音検知時間（秒）
    private int recordingStartPosition;
    private bool isRecording = false;
    private float silenceTimer = 0f;

    public void StartRecording()
    {
        if (isRecording)
        {
            Debug.LogWarning("既に録音中です。");
            return;
        }

        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("マイクが見つかりません。");
            return;
        }

        isRecording = true;
        silenceTimer = 0f;
        recordedClip = Microphone.Start(null, true, 300, sampleRate);
        recordingStartPosition = Microphone.GetPosition(null);
        Debug.Log("録音開始...");

        StartCoroutine(CheckForSilence());
    }

    public void StopRecordingManually()
    {
        if (!isRecording)
        {
            Debug.LogWarning("録音は開始されていません。");
            return;
        }

        StopRecording();
    }

    private void StopRecording()
    {
        if (!isRecording) return;

        int recordingEndPosition = Microphone.GetPosition(null);
        Microphone.End(null);
        isRecording = false;

        AudioClip trimmedClip = TrimAudioClip(recordedClip, recordingStartPosition, recordingEndPosition);
        Debug.Log("録音停止。");
        OnRecordingStopped?.Invoke(trimmedClip); // 録音停止イベントを発行
    }

    private System.Collections.IEnumerator CheckForSilence()
    {
        while (isRecording)
        {
            int position = Microphone.GetPosition(null);
            if (position > 0 && recordedClip.samples >= position)
            {
                float[] samples = new float[128];
                recordedClip.GetData(samples, Mathf.Max(0, position - samples.Length));

                float maxAmplitude = Mathf.Max(samples);
                if (maxAmplitude < silenceThreshold)
                {
                    silenceTimer += Time.deltaTime;
                    if (silenceTimer >= silenceDuration)
                    {
                        Debug.Log("無音検知により録音停止。");
                        StopRecording();
                        yield break;
                    }
                }
                else
                {
                    silenceTimer = 0f;
                }
            }
            yield return null; // 次フレームまで待機
        }
    }

    private AudioClip TrimAudioClip(AudioClip originalClip, int startPosition, int endPosition)
    {
        if (endPosition < startPosition)
        {
            endPosition += originalClip.samples; // マイクバッファがループした場合に対応
        }

        int length = endPosition - startPosition;
        float[] samples = new float[length];
        originalClip.GetData(samples, startPosition);

        AudioClip trimmedClip = AudioClip.Create(
            originalClip.name + "_trimmed",
            length,
            originalClip.channels,
            originalClip.frequency,
            false
        );
        trimmedClip.SetData(samples, 0);

        return trimmedClip;
    }
}
