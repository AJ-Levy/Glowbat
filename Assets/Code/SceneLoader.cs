using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance; // Singleton instance

    private Animator crossFadeAnimator;
    private bool isLoadingScene = false;

    void Awake()
    {
        // Check if there is already an instance of SceneLoader
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

        GameObject crossFadePanel = GameObject.FindWithTag("CrossFadePanel");
        if (crossFadePanel != null)
        {
            crossFadeAnimator = crossFadePanel.GetComponent<Animator>();
        }

        string sceneToLoad = PlayerPrefs.GetInt("HasCompletedTutorial", 0) == 0 ? "Tutorial" : "MainGame";

        StartCoroutine(LoadSceneAsync(sceneToLoad));
    }

    public void LoadScene(string sceneName)
    {
        if (!isLoadingScene)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        isLoadingScene = true;

        // Load the next scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Find and assign the CrossFade panel in the current scene
        GameObject crossFadePanel = GameObject.FindWithTag("CrossFadePanel");
        if (crossFadePanel != null)
        {
            crossFadeAnimator = crossFadePanel.GetComponent<Animator>();
            crossFadeAnimator.SetTrigger("Start"); // Trigger the fade-out animation
        }

        // Wait for the fade-out animation to finish
        yield return new WaitForSeconds(0.95f);

         while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Activate the new scene
        asyncLoad.allowSceneActivation = true; 

        isLoadingScene = false;
    }
}
