using WebSocketSharp;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SocialPlatforms.Impl;


// UnityMainThreadDispatcher library: https://github.com/PimDeWitte/UnityMainThreadDispatcher
// WebSocketSharp Library: https://github.com/sta/websocket-sharp

public class WebSocketClient : MonoBehaviour
{
    WebSocket ws;
    private int myScore = 0;
    private int prevScore = -1; // set to -1 initially to ensure animation triggers on first score update
    public Text myScoreText;
    public Animator manAnimator;
    private bool isAnimationTriggered = false;
    public Animator monsterAnimator;

    public AudioSource src;
    public AudioClip Hurt;

    private void Awake()
    {
        manAnimator = GameObject.Find("GunMan").GetComponent<Animator>();
        monsterAnimator = GameObject.Find("Monster").GetComponent<Animator>();
    }

    void Start()
    {
        ws = new WebSocket("ws://myrobotcoach.glitch.me");

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message Received from " + ((WebSocket)sender).Url + ", Data : " + e.Data);
            UpdateScore(e.Data);
        };

        ws.Connect();
    }

    private void UpdateScore(string jsonData)
    {
        // Use the Try and Catch function to avoid situations where there may be JSON data that cannot be processed or issues with conversion.
        try
        {
            // Converts a JSON string into an object of the specified type
            ScoreData scoreData = JsonUtility.FromJson<ScoreData>(jsonData);
            myScore = scoreData.Counter;
            Debug.Log("Score: " + myScore);

            // Update in the Main Thread
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                myScoreText.text = "Score: " + myScore.ToString();

                if (myScore != prevScore) //Trigger GunMan Animation
                {
                    if (prevScore < myScore && prevScore != -1 && (myScore - prevScore) == 1)
                    {
                        manAnimator.SetTrigger("Shoot");

                        monsterAnimator.SetTrigger("GetHit");

                       src.clip = Hurt;

                        src.Play();

                        isAnimationTriggered = true;

                        Debug.Log("Shoot!");

                    }

                    else if (prevScore > myScore || prevScore == myScore || prevScore == 0)
                    {
                        isAnimationTriggered = false;
                    }
                }

               prevScore = myScore; //Update the score

            });
        }


        catch (Exception e)
        {
            Debug.LogError("Error parsing score data: " + e.Message);
        }
    }

    public void Update()
    {
        if (ws == null) return;
    }

    private void OnDestroy()
    {
        ws.Close();
        ws = null;
    }

    [Serializable]
    private class ScoreData
    {
        // public string id;
        public int Counter;
    }


}
