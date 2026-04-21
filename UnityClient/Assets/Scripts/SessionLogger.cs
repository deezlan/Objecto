using System;
using System.Collections;
using System.IO;
using System.Text;
using Fusion;
using UnityEngine;

public class SessionLogger : MonoBehaviour
{
    public static SessionLogger Instance { get; private set; }

    private const float SampleInterval = 1f;

    private StringBuilder _log;
    private string _filePath;
    private float _timer;
    private bool _logging;

    private int _sampleCount;
    private float _fpsSum;
    private float _pingSum;
    private float _fpsMin = float.MaxValue;
    private float _fpsMax = float.MinValue;
    private float _pingMin = float.MaxValue;
    private float _pingMax = float.MinValue;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartLogging(string sessionLabel)
    {
        string fileName = $"session_{sessionLabel}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        _filePath = Path.Combine(Application.persistentDataPath, fileName);

        _log = new StringBuilder();
        _log.AppendLine("Timestamp,FPS,Ping_ms");

        _sampleCount = 0;
        _fpsSum = 0f;
        _pingSum = 0f;
        _fpsMin = float.MaxValue;
        _fpsMax = float.MinValue;
        _pingMin = float.MaxValue;
        _pingMax = float.MinValue;

        _logging = true;
        Debug.Log($"SessionLogger: logging started -> {_filePath}");
    }

    public void StopLogging()
    {
        if (!_logging) return;
        _logging = false;

        // Write session averages and ranges
        if (_sampleCount > 0)
        {
            _log.AppendLine();
            _log.AppendLine("--- Session Summary ---");
            _log.AppendLine($"Samples,{_sampleCount}");
            _log.AppendLine($"Avg FPS,{(_fpsSum / _sampleCount):F1}");
            _log.AppendLine($"FPS Range,{_fpsMin:F1} - {_fpsMax:F1}");
            _log.AppendLine($"Avg Ping (ms),{(_pingSum / _sampleCount):F1}");
            _log.AppendLine($"Ping Range (ms),{_pingMin:F1} - {_pingMax:F1}");
        }

        File.WriteAllText(_filePath, _log.ToString());
        Debug.Log($"SessionLogger: log saved -> {_filePath}");
    }

    private void Update()
    {
        if (!_logging) return;

        _timer += Time.deltaTime;
        if (_timer < SampleInterval) return;
        _timer = 0f;

        float fps = 1f / Time.unscaledDeltaTime;
        int ping = GetPing();
        string timestamp = DateTime.Now.ToString("HH:mm:ss");

        _log.AppendLine($"{timestamp},{fps:F1},{ping}");

        // Accumulate for summary
        _fpsSum += fps;
        _pingSum += ping;
        _sampleCount++;
        if (fps < _fpsMin) _fpsMin = fps;
        if (fps > _fpsMax) _fpsMax = fps;
        if (ping < _pingMin) _pingMin = ping;
        if (ping > _pingMax) _pingMax = ping;
    }

    private int GetPing()
    {
        var runner = NetworkManager.Instance?.Runner;
        if (runner != null && runner.IsRunning)
            return Mathf.RoundToInt((float)runner.GetPlayerRtt(runner.LocalPlayer) * 1000f);
        return -1;
    }
}