using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject highScoreText;
    
    [SerializeField]
    private GameObject highScoreInputBox;
    
    bool bEnableInputs = false;
    bool bEnterHighScore = false;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.RenderSettings.skybox = null;
    }

    private void Awake()
    {
        EnableHighScore(false);
    }

    private void Update()
    {
        if (GameController.Score > GameController.HighScore && !bEnterHighScore) // comment out to test
        {
            bEnterHighScore= true;
            
            // enable high score input  if higher
            EnableHighScore(true);

            if (HUDController.Instance != null)
            {
                HUDController.Instance.TurnOnLifeIcons(false);
            }
        }        
    }

    private void EnableHighScore(bool yesNo = false)
    {
        bEnableInputs = yesNo;
        highScoreText.SetActive(yesNo);
        highScoreInputBox.SetActive(yesNo);
    }

    public void SwitchToMainMenu()
    {
        Time.timeScale = 1f;
        
        if (HUDController.Instance != null)
        {
            HUDController.Instance.SetupInitialHUDValues();
        }

        if (TrashSpawner.Instance != null)
        {
            TrashSpawner.Instance.RestartTrashSpawner();
        }

        bEnterHighScore= false;    
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void EnteredHighScorename()
    {
        string newLength;
        string enteredString = highScoreInputBox.GetComponent<TMP_InputField>().text.ToString();

        if (enteredString.Length > 8)
        {
            newLength = enteredString.Remove(8);
            GameController.HiPlayerName = newLength;
        }
        else
        {
            if (enteredString.Length == 0)
            {
                enteredString = new string("NO-NAME!");
            }
            
            GameController.HiPlayerName = enteredString;
        }
        
        GameController.Instance.SaveUserData();
    }
}
