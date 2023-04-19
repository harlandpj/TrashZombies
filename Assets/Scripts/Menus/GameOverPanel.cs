using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameOverPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.RenderSettings.skybox = null;
    }

    public void SwitchToMainMenu()
    {
        // can't switch to main menu - scene 0 in build order, due to some sort of
        // scene unloading bug, so going to main game again instead!
        Time.timeScale = 1f;
        
        if (HUDController.Instance != null)
        {
            HUDController.Instance.SetupInitialHUDValues();
        }

        if (TrashSpawner.Instance != null)
        {
            TrashSpawner.Instance.RestartTrashSpawner();
        }
        
        //SceneManager.LoadScene(1);
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
}
