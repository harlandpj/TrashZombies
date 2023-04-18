using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        UnityEngine.RenderSettings.skybox = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchToGameScene()
    {
        // switch to main game - scene 1 in build order
        SceneManager.LoadScene(1,LoadSceneMode.Single);
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
