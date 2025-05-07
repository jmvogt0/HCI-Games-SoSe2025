using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using NativeWebSocket;

public class HyperateSocket : MonoBehaviour
{
    [Header("Hyperate Settings")]
    [Tooltip("Token von https://www.hyperate.io/api")]
    public string websocketToken = "xQ1Ekhd0tJOxUAWFIYQ5lqXCOzClnuG7iIMMtX8gudtAaJ6GCAYAtu2HWAqQH0V1";
    public string hyperateID = "79E0";


    [Header("References")]
    public HeartRateBaselineRecorder recorder;  // Im Inspector belegen!

    private WebSocket websocket;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    async void Start()
    {
        // Fallback, falls du die Referenz nicht im Inspector gesetzt hast
        if (recorder == null)
            recorder = FindObjectOfType<HeartRateBaselineRecorder>();

        if (recorder == null)
        {
            Debug.LogError("Kein HeartRateBaselineRecorder gefunden! Bitte im Inspector zuweisen.");
            return;
        }

        websocket = new WebSocket($"wss://app.hyperate.io/socket/websocket?token={websocketToken}");

        websocket.OnOpen += () =>
        {
            Debug.Log("WebSocket geöffnet");
            SendWebSocketMessage();
        };
        websocket.OnError += (e) =>
        {
            Debug.LogError("WebSocket-Fehler: " + e);
        };
        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket geschlossen");
        };
        websocket.OnMessage += OnWebSocketMessage;

        InvokeRepeating(nameof(SendHeartbeat), 1f, 25f);

        await websocket.Connect();
    }

    private void OnWebSocketMessage(byte[] bytes)
    {
        var json = System.Text.Encoding.UTF8.GetString(bytes);
        var msg = JObject.Parse(json);

        if (msg["event"]?.ToString() == "hr_update")
        {
            int bpm = System.Convert.ToInt32(msg["payload"]?["hr"] ?? 0);
            Debug.Log($"HR Update: {bpm} BPM");

            // Aktuelle HR global speichern
            if (HeartRateManager.Instance != null)
                HeartRateManager.Instance.UpdateHR(bpm);

            // Wenn BaselineRecorder aktiv ist
            if (recorder != null)
                recorder.OnHeartRateReceived(bpm);
        }
    }

    async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open)
            await websocket.SendText($"{{\"topic\":\"hr:{hyperateID}\",\"event\":\"phx_join\",\"payload\":{{}},\"ref\":0}}");
    }

    async void SendHeartbeat()
    {
        if (websocket.State == WebSocketState.Open)
            await websocket.SendText("{\"topic\":\"phoenix\",\"event\":\"heartbeat\",\"payload\":{},\"ref\":0}");
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (websocket != null)
        {
            websocket.DispatchMessageQueue();
        }
#endif
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}