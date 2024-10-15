using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// A peristent singleton class that loads scenes asychronously and smoothly (with an animation).
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance; // Singleton instance

    private Animator crossFadeAnimator;
    private bool isLoadingScene = false;

    /// <summary>
    /// Make sure only one instance of this class exists (singleton).
    /// </summary>
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

    /// <summary>
    /// Load the first scene based on whether the player has completed the tutorial or not.
    /// </summary>
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

    /// <summary>
    /// Loads a scene if no other scene is currently being loaded.
    /// </summary>
    /// <param name="sceneName">The name of the scene to be loaded.</param>
    public void LoadScene(string sceneName)
    {
        if (!isLoadingScene)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }

    /// <summary>
    /// This corountine asynchronously loads a scene whilst executing a fade in and fade out animation.
    /// </summary>
    /// <param name="sceneName"The name of the scene to be loaded.></param>
    /// <returns>An IEnumerator that can be used to yield execution in a coroutine.</returns>
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
