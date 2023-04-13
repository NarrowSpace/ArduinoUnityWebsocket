using WebSocketSharp;
using UnityEngine;
using System;
using UnityEngine.SocialPlatforms.Impl;

public class WebSocketClient : MonoBehaviour
{
    WebSocket ws;

    void Start()
    {
        ws = new WebSocket("ws://myrobotcoach.glitch.me");

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message Received from " + ((WebSocket)sender).Url + ", Data : " + e.Data);
        };

        ws.Connect();
        ws.Send("Hello from Unity");
    }


    private void Update()
    {
        if (ws == null)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            ws.Close();
            Debug.Log("Websoket Closed");
        }
    }


}
