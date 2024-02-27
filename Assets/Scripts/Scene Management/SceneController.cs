using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    public GameObject LoadingScreen;
    private static GameObject LoadingScreenInstance;

    private void Start()
    {
        LoadingScreenInstance = Instantiate(LoadingScreen);
        LoadingScreenInstance.SetActive(false);
    }
    public static void GoToScene(string scene)
    {
        LoadingScreenInstance.SetActive(true);
        GameController.ResetObservers();

        try
        {
            // Loads the given scene
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }
        catch
        {
            // Loads the scene at index 0
            SceneManager.LoadScene(SceneManager.GetSceneAt(0).ToString(), LoadSceneMode.Single);
        }
    }
    public static string GetScene()
    {
        return SceneManager.GetActiveScene().ToString();
    }
    public static void QuitGame()
    {
        Application.Quit();
    }
    public static bool IsPlayableScene()
    {
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main Menu"))
        {
            return false;
        }

        return true;
    }
}
