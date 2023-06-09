using WebSocketSharp;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SocialPlatforms.Impl;
using System.Threading;
using System.Collections;


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
    public Animator monsterCtrl;

    //Audio
    public AudioSource src;
    public AudioClip gunFire;

    public AudioSource winUI;
    public AudioClip winSound;

    //Health System
    private int maxHealth = 10;
    private bool isMonsterDead = false;
    //public Image[] hearts;

    //Canvas
    public CanvasGroup introPanel;
    public CanvasGroup winPanel;

    [SerializeField]
    private WeaponParticle gunParticle;

    [SerializeField]
    private string monsterTag;

    private bool gunShoot = false;

    private void Awake()
    {
        manAnimator = GameObject.Find("GunMan").GetComponent<Animator>();
       // monsterAnimator = GameObject.Find("Monster").GetComponent<Animator>();
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

        // Set initial states of panels
        introPanel.gameObject.SetActive(true);
        winPanel.gameObject.SetActive(false);

        // Particle system
        gunParticle.SetEnemyTag(monsterTag);
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

                StartCoroutine(ShowVsUI());

                if (myScore != prevScore) //Trigger GunMan Animation
                {
                    if (prevScore < myScore && prevScore != -1 && (myScore - prevScore) == 1)
                    {
                        gunShoot = true;
                        gunParticle.Fire();

                        manAnimator.SetTrigger("Shoot");
                        PlaySoundHurt();
 
                        //Monster Heart Sprite
                        if (currentHealth < 10 && currentHealth > 0)
                        {
                          isMonsterDead = false;
                        }

                        if (currentHealth == 0)
                        {
                            isMonsterDead= true;

                            monsterCtrl.SetTrigger("Dead");
                            //monsterAnimator.SetTrigger("Dead");
                            Debug.Log("I'm Deeeeeeeeeeaaaaaddddd!!");
                            //PlaySoundWin();
                            StartCoroutine(ShowWinUI());

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


    public void PlaySoundHurt()
    {
        src.clip = gunFire;
        src.Play();
    }

    public void PlaySoundWin()
    {
        winUI.clip = winSound;
        winUI.Play();
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

    public IEnumerator ShowVsUI()
    {
        float timer = 0f;

        while (timer < 1f) // fade the intro panel Alpha
        {
            timer += Time.deltaTime / 3.3f; //devide by 3.3 means that the loop runs for 3.3s in total

            introPanel.alpha = Mathf.Lerp (1f, 0f, timer);

            yield return null;
        }

        introPanel.gameObject.SetActive(false);
    }

    public IEnumerator ShowWinUI()
    {
        float timer = 0f;

        while (timer < 1f) // fade the intro panel Alpha
        {
            timer += Time.deltaTime / 1f; //devide by 5 means that the loop runs for 5s in total

            winPanel.alpha = Mathf.Lerp (0f, 1f, timer);

            Debug.Log("Title Ends! Game Begin!");

            yield return null;
        }

        winPanel.gameObject.SetActive(true);
        PlaySoundWin();
        
    }


}
