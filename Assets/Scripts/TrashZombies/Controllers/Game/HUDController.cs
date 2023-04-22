using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HUDController : MonoBehaviour
{
    // HUD display
    [Header("Player Statistics")]
    [SerializeField]
    private TMP_Text PlayerScore; // players score
    [SerializeField]
    private TMP_Text PlayerHiScore; // players score
    [SerializeField]
    private TMP_Text HiPlayerName; // high player name
    [SerializeField]
    private TMP_Text PlayerHealth; // players health
    [SerializeField]
    private TMP_Text CityHealth; // city health (starts off low)
    [SerializeField]
    private TMP_Text CityHealthWarningDisplay; // comes on when health is zero / timer started

    // for turning on/off later
    [SerializeField]
    private TMP_Text highScoreText;
    [SerializeField]
    private TMP_Text highPlayerNameText;
    [SerializeField]
    private TMP_Text playerScoreText;
    [SerializeField]
    private TMP_Text PlayerHealthText;
    [SerializeField]
    private TMP_Text cityHealthText;

    // HUD Lives left, the "square" round lives box and underscores elsewhere
    [Tooltip("This is all RED objects used to underline Player attributes on HUD display")]
    [SerializeField]
    private GameObject[] HUDGraphicElements = new GameObject[8];

    int cityHealthCountdown = 180; // 3 minute warning
    bool cityHealthCountdownStarted = false;

    [SerializeField]
    private AudioClip loseALife;

    private AudioSource audioSource;

    // accessor
    public static HUDController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            ShowPlayerInfo(true);
            InvokeRepeating("UpdatePlayerStats", 0, 1);
            return;
        }
        else
        {
            Instance = this;
        }

        // ensure not destroyed on any subsequent scene load
        //DontDestroyOnLoad(gameObject);

        audioSource = gameObject.GetComponent<AudioSource>();

        // needed for restart/reload situation
        ShowPlayerInfo(true);

        InvokeRepeating("UpdatePlayerStats", 0, 1);
    }

    public void ShowPlayerInfo( bool onOff = true)
    {
        // turn on/off player info on end game
        PlayerScore.enabled = onOff;
        PlayerHiScore.enabled = onOff;
        PlayerHealth.enabled = onOff;
        HiPlayerName.enabled = onOff;   
        PlayerHiScore.enabled = onOff;
        CityHealth.enabled = onOff; 
        playerScoreText.enabled= onOff;
        PlayerHealthText.enabled= onOff;
        cityHealthText.enabled= onOff;  
        highScoreText.enabled= onOff;
        CityHealthWarningDisplay.enabled= onOff;

        // turn on graphics icons
        TurnOnLifeIcons(true);

        for (int i =0; i < HUDGraphicElements.Length; i++)
        {
            if (HUDGraphicElements[i] != null)
            {
                HUDGraphicElements[i].SetActive(onOff);
            }
        }

        if (GameController.Score > GameController.HighScore)
        {
            PlayerHiScore.SetText(GameController.Score.ToString());
        }
        else
        {
            PlayerHiScore.SetText(GameController.HighScore.ToString());

            if (GameController.Instance != null)
            {
                if (GameController.HiPlayerName != null)
                {
                    if (GameController.HiPlayerName.Length > 0)
                    {
                        HiPlayerName.SetText(GameController.HiPlayerName.ToString());
                    }
                    else
                    {
                        HiPlayerName.SetText("NO NAME!");
                    }
                }
                else
                {
                    HiPlayerName.SetText("NO NAME!");
                }
            }
        }

        // clear old player info
        ClearPreviousInfo();    
    }

    public void ClearPreviousInfo()
    {
        PlayerScore.SetText("0".ToString());
        PlayerHealth.SetText("      ".ToString());
        CityHealth.SetText("       ".ToString());
        CityHealthWarningDisplay.SetText("                                                                     ".ToString());
    }

    public void OnGameQuit()
    {
        // quit button action
        GameController.Instance.SaveUserData();

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    private void UpdatePlayerStats()
    {
        // updates player stats, done every second - may use events later!
        Debug.Log("Updating Player Stats");
    
        PlayerScore.SetText(GameController.Score.ToString());
        
        // turn off life icon
        if (GameController.Health <= 0)
        {
            GameController.Lives -= 1;
            audioSource.PlayOneShot(loseALife, 1f);
            
            switch (GameController.Lives)
            {
                case 0:
                    TurnOnLifeIcons(false); 
                    break;
                case 1: 
                    HideLifeIcon(1);
                    HideLifeIcon(2);
                    break;
                case 2:
                    HideLifeIcon(2);
                    break;
            }

            GameController.Health = GameController.MaxHealth;
        }
        else if (GameController.Health == GameController.MaxHealth && 
            GameController.Lives == 3)
        {
            // have come here from new game or a restart
            TurnOnLifeIcons(true);
            ShowPlayerInfo(true);
            GameController.Instance.ResetOrOnRestartVariables(true);
            Time.timeScale = 1f;
        }

        PlayerHealth.SetText(GameController.Health.ToString("0.00") + " %".ToString());
        
        string CityHealthString = GameController.CityHealth.ToString("0.00"); // 2dp
        CityHealthString += " %";
        CityHealth.SetText(CityHealthString);

        if (GameController.CityHealth <= 0f)
        {
            if (!cityHealthCountdownStarted)
            {
                // start warning countdown to game end
                cityHealthCountdownStarted = true;
                InvokeRepeating("CityWarning", 0, 1);
            }
        }
        else
        {
            CityHealthWarningDisplay.SetText("                                                                 ");
        }

        if (cityHealthCountdown <=0)
        {
            StartGameOverRoutine();
        }

        if (GameController.Score > GameController.HighScore)
        {
            PlayerHiScore.SetText(GameController.Score.ToString());
        }
    }

    public void StartGameOverRoutine()
    {
        CancelInvoke("CityWarning");
        
        GameController.Instance.m_bGameOver = true;
        TurnOnLifeIcons(false);
        CityHealthWarningDisplay.SetText("****************  GAME OVER!  *************".ToString());

        Time.timeScale = 0f;
        cityHealthCountdown = 180;
        cityHealthCountdownStarted = false;
        
        GameController.Instance.SaveUserData();

        // load end game scene overlay
        audioSource.Stop();
        SceneManager.LoadScene(2); // end game menu
    }

    void CityWarning()
    {
        cityHealthCountdown -= 1;
        CityHealthWarningDisplay.SetText("COLLECT MORE RUBBISH WITHIN: ".ToString() + cityHealthCountdown.ToString() + " SECONDS!   ".ToString());
    }

    // Initialise values
    public void SetupInitialHUDValues()
    {
        GameController.CityHealth = 100f;
        GameController.Health = 100f;
        GameController.Score = 0;

        PlayerHealth.SetText("100".ToString());
        PlayerHiScore.SetText(GameController.HighScore.ToString());
        PlayerScore.SetText("0".ToString());
        
        string CityHealthString = GameController.CityHealth.ToString("0.00"); // 2dp
        CityHealthString += " %";
        CityHealth.SetText(CityHealthString);   
        
        TurnOnLifeIcons(true);
    }

    public void TurnOnLifeIcons(bool onOff = true)
    {
        // reset icons
        for (int i=0; i < lifeIcons.Length-1; i++)
        {
            if (lifeIcons[i] != null)
            {
                lifeIcons[i].SetActive(onOff);
            }
        }
    }

    [SerializeField]
    private GameObject[] lifeIcons = new GameObject[3];

    // updates player lives icons, switching off one per life
    public void HideLifeIcon(int iconNumber)
    {
        // simply switch off the relevant icon
        if (iconNumber <= lifeIcons.Length)
        {
            // switch it off
            lifeIcons[iconNumber-1].SetActive(false);
        }
    }
}
