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
    [Header("Player Stats")]
    [SerializeField]
    private TMP_Text PlayerScore; // players score
    [SerializeField]
    private TMP_Text PlayerHiScore; // players score
    [SerializeField]
    private TMP_Text PlayerLives; // players lives
    [SerializeField]
    private TMP_Text PlayerHealth; // players health
    [SerializeField]
    private TMP_Text CityHealth; // city health (starts off low)
    [SerializeField]
    private TMP_Text HiPlayerName; // high score players name
    [SerializeField]
    private TMP_Text EnemiesRemaining; // enemies left to kill this level
    [SerializeField]
    private TMP_Text InfoDisplay; // 'general' status display message box
    
    [SerializeField]
    private TMP_Text CityHealthWarninngDisplay; // comes on when health is zero / timer started

    int cityHealthCountdown = 180; // 3 minute warning!
    bool cityHealthCountdownStarted = false;

    bool bStartedBefore = false; // has HUD just started

    [SerializeField]
    private AudioClip loseALife;
    private AudioSource audioSource;

    public static HUDController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
        }

        // ensure not destroyed on any subsequent scene load
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update (singleton, so only called once it seems)
    void Start()
    {
        InvokeRepeating("UpdatePlayerStats", 0, 1);
        SetupInitialHUDValues();
    }

    public void OnGameQuit()
    {
        // quit button action
        //GameController.Instance.SaveUserData();

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

        if (GameController.Score > GameController.HighScore)
        {
            PlayerHiScore.SetText(GameController.Score.ToString());
        }
        else
        {
            PlayerHiScore.SetText(GameController.HighScore.ToString());
        }
        
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
        else if (GameController.Health == GameController.MaxHealth && GameController.Lives == 3)
        {
            TurnOnLifeIcons();
        }

        PlayerHealth.SetText(GameController.Health.ToString());
        
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
            CityHealthWarninngDisplay.SetText("                                                                 ");
        }

        if (cityHealthCountdown <=0)
        {
            StartGameOverRoutine();
        }
    }

    public void StartGameOverRoutine()
    {
        CancelInvoke("CityWarning");
        
        GameController.Instance.m_bGameOver = true;
        TurnOnLifeIcons(false);
        CityHealthWarninngDisplay.SetText("****************  GAME OVER!  *************".ToString());

        Time.timeScale = 0f;

        cityHealthCountdown = 180;
        cityHealthCountdownStarted = false;
        
        GameController.Instance.SaveUserData();

        // load end game scene overlay
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
    }

    void CityWarning()
    {
        cityHealthCountdown -= 1;
        CityHealthWarninngDisplay.SetText("COLLECT MORE RUBBISH WITHIN: ".ToString() + cityHealthCountdown.ToString() + " SECONDS!   ".ToString());
    }

    // Initialise values
    public void SetupInitialHUDValues()
    {
        GameController.CityHealth = 100f;
        GameController.Health = 100;
        GameController.Score = 0;

        PlayerHealth.SetText("100".ToString());
        PlayerHiScore.SetText(GameController.HighScore.ToString());
        PlayerScore.SetText("0".ToString());
        
        string CityHealthString = GameController.CityHealth.ToString("0.00"); // 2dp
        CityHealthString += " %";
        CityHealth.SetText(CityHealthString);   
        
        TurnOnLifeIcons();
    }

    public void TurnOnLifeIcons(bool onOff = true)
    {
        // reset icons
        for (int i=0; i < lifeIcons.Length-1; i++)
        {
            lifeIcons[i].SetActive(onOff);
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
