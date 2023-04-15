using WebSocketSharp;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SocialPlatforms.Impl;
using System.Threading;


// UnityMainThreadDispatcher library: https://github.com/PimDeWitte/UnityMainThreadDispatcher
// WebSocketSharp Library: https://github.com/sta/websocket-sharp

public class WebSocketClient : MonoBehaviour
{
    WebSocket ws;

    //Score
    private int myScore = 0;
    private int prevScore = -1; // set to -1 initially to ensure animation triggers on first score update
    public Text myScoreText;

    //Animator
    public Animator manAnimator;
    public Animator monsterAnimator;

    //Audio
    public AudioSource src;
    public AudioClip Hurt;

    //Health System
    private int maxHealth = 10;
    private bool isMonsterDead = false;
    public Image[] hearts; 


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
            Debug.Log("My score is " + myScore);

            //Monster Health System
            int currentHealth = maxHealth - myScore;

            // Update in the Main Thread
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                myScoreText.text = "x " + myScore.ToString();

                if (myScore != prevScore) //Trigger GunMan Animation
                {
                    if (prevScore < myScore && prevScore != -1 && (myScore - prevScore) == 1)
                    {
                        manAnimator.SetTrigger("Shoot");
                        monsterAnimator.SetTrigger("GetHit");
                        src.clip = Hurt;
                        src.Play();

                        //Monster Heart Sprite
                        if (currentHealth < 10 && currentHealth > 0)
                        {
                          hearts[currentHealth].enabled = false;
                          isMonsterDead = false;
                        }

                        if (currentHealth == 0)
                        {
                            hearts[currentHealth].enabled = false;
                            isMonsterDead= true;
                            monsterAnimator.SetTrigger("Dead");
                            Debug.Log("I'm Deeeeeeeeeeaaaaaddddd!!");
                        }

                        Debug.Log("CurrentHealth" + currentHealth);
                    }

                    else if (myScore == 0 && prevScore != myScore)
                    {
                        Debug.Log ("Restart!");
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
