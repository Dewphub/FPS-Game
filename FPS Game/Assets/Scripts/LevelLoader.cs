using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    [SerializeField] UnityEngine.UI.Slider loadingSlider;
    [SerializeField] Animator transition;
    [SerializeField] float transitionTime = 1f;

    public void LoadNextScene(int _sceneIndex)
    {
        StartCoroutine(LoadSceneAsynchronously(_sceneIndex));
    }
    IEnumerator LoadSceneAsynchronously(int sceneIndex)
    {
        if (sceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            // Play Transition Animation
            transition.SetTrigger("Start");
            // Wait
            yield return new WaitForSeconds(transitionTime);
        }
        // Load Scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);
        while(!operation.isDone)
        {
            loadingSlider.value = operation.progress;
            yield return null;
        }
    }
}
