using System.Collections;
using UnityEngine;

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
        // Play Transition Animation
        transition.SetTrigger("Start");
        // Wait
        yield return new WaitForSeconds(transitionTime);
        // Load Scene
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);
        while(!operation.isDone)
        {
            loadingSlider.value = operation.progress;
            yield return null;
        }
    }
}
