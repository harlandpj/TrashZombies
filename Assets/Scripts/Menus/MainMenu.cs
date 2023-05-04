using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MainMenu : MonoBehaviour
{
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        UnityEngine.RenderSettings.skybox = null;
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    public void SwitchToGameScene()
    {
        if (HUDController.Instance != null)
        {
            HUDController.Instance.SetupInitialHUDValues();
        }

        if (TrashSpawner.Instance != null) 
        { 
            TrashSpawner.Instance.RestartTrashSpawner();   
        }

        if (GameController.Instance != null)
        {
            GameController.Instance.ResetOrOnRestartVariables(true);
        }

        // switch to main game - scene 1 in build order
        SceneManager.LoadScene(1);
    }

    public void SwitchToCredits()
    {
        // switch to credits scene
        SceneManager.LoadScene(3);
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
}
