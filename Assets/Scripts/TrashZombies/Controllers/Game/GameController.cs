using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro; // text mesh pro
using Unity.VisualScripting;

/// <summary>
/// GameController (Singleton) class handles access to all other controllers
/// note: The controller gameObject in scene MUST be called "GameController"
/// </summary>
///
// do a [RequireComponent(typeof(GameController))] for all here to to ensure incorrect setup in editor/runtime?
public class GameController : MonoBehaviour
{
    /// <summary>
    /// Instance function for access to instance
    /// </summary>
    public static GameController Instance { get; private set; }

    // Private Variables (these exist here for now - possibly implement a "GameState" Manager later?)
    public bool m_bGameStarted = false;
    public bool m_bGamePaused = false;
    public bool m_bGameOver = false;

    [SerializeField]
    private Material skyBox;

    // player and other statistics
    public static string PlayerName;
    public static int HighScore;
    public static int Score;

    [Header("Player Statistics")]
    [SerializeField]
    private static float maxHealth = 100;
    private static float m_Health = 100;

    [Header("City Statistics")]
    [SerializeField]
    private static float m_CityHealth = 100f;

    public static float MaxHealth
    {
        get => maxHealth;
    }

    public static float Health
    {
        get => m_Health;
        set
        {
            if (value >= 0)
            {
                if (value <= maxHealth)
                {
                    m_Health = value; // increase health
                }
                else
                {
                    m_Health = maxHealth;
                }
            }
            else
            {
                if (value <= 0)
                {
                    m_Health = 0;
                }
                else
                {
                    m_Health = value;
                }
            }
        }
    }

    [Header("Maximum Lives")]
    [SerializeField]
    private static readonly int maxLives = 3;  // maximum lives
    private static int m_Lives = maxLives;

    public static int Lives
    {
        get => m_Lives;
        set
        {
            if (value >= 0)
            {
                if (value <= maxLives)
                {
                    m_Lives = value;
                }
                else
                {
                    m_Lives = maxLives;
                }
            }
            else
            {
                if (value <= 0)
                {
                    m_Lives = 0;
                }
                else
                {
                    m_Lives = value;
                }
            }
        }
    }

    public static float CityHealth
    {
        get => m_CityHealth;

        set 
        { 
            if (value >= 100)
            {
                m_CityHealth = 100f;
            }
            else 
            {
                if (value < 0)
                {
                    m_CityHealth = 0;
                }
                else
                {
                    m_CityHealth = value;
                }
            }
        } 
    }    


    /// <summary>
    /// setup static Instance object (if not there) and retain data on (possible other) scene loading
    /// </summary>
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

        ResetOrOnRestartVariables(true);
    }

    // reset variables to initial state
    public void ResetOrOnRestartVariables(bool newGame =true)
    {
        // reset player score
        Score = 0;
        Health = 100f;
        CityHealth = 100f;
        m_bGameOver = false;
        LoadUserData();

        HUDController.Instance.TurnOnLifeIcons(true);
        Lives = 3;
    }

    bool bStartGameOver = false; // started game over routine

    private void Update()
    {
        if (Lives == 0 && !bStartGameOver)
        {
            bStartGameOver= true;
            HUDController.Instance.ShowPlayerInfo(false);   
            HUDController.Instance.StartGameOverRoutine();
        }    
    }

    [System.Serializable]
    class SaveData
    {
        public string PlayName; // players name
        public int Score; // players high score
    }

    public void SaveUserData()
    {
        SaveData data = new SaveData();

        if (Score > HighScore)
        {
            // new high score
            data.Score = Score;

            if (PlayerName != null)
            {
                if (PlayerName.Length != 0)
                {
                    data.PlayName = PlayerName;
                }
            }
            else
            {
                data.PlayName = "No Name";
            }
        }
        else
        {
            data.Score = HighScore;
            data.PlayName = PlayerName;
        }

        // convert to JSON format and save to file
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log($"Saving Data to: {Application.persistentDataPath} in savefile.json");
    }

    public void LoadUserData()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            PlayerName = data.PlayName;
            HighScore = data.Score;
            Debug.Log($"Loading Data from: {Application.persistentDataPath} in savefile.json");
        }
    }

}
