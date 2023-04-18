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

    // Update is called once per frame
    void Update()
    {
       if (!audioSource.isPlaying)
       {
            audioSource.Play(); 
            HUDController.Instance.TurnOnLifeIcons(false);
       }
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
