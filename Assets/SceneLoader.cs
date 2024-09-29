using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Image fadePanel; 
    public float fadeDuration = 1f; 

    void Start()
    {
        // Start with the panel fully opaque (black)
        fadePanel.color = new Color(0, 0, 0, 1);
        
        // Determine which scene to load based on the tutorial completion status
        string sceneToLoad = PlayerPrefs.GetInt("HasCompletedTutorial", 0) == 0 ? "Tutorial" : "MainGame";
        
        // Start the fade out and load the next scene asynchronously
        StartCoroutine(FadeOutAndLoadScene(sceneToLoad));
    }

    IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        // Begin asynchronous scene loading in the background
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // Prevent the scene from activating immediately

        // Fade out the black panel over time
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1, 0, t / fadeDuration); // Interpolate the alpha value
            fadePanel.color = new Color(0, 0, 0, alpha); // Apply the alpha to the panel's color
            yield return null;
        }

        // After the fade-out completes, activate the new scene
        asyncLoad.allowSceneActivation = true;
    }
}

