using System.Collections;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    [SerializeField] UnityEngine.UI.Slider loadingSlider;

    public void LoadNextScene(int _sceneIndex)
    {
        StartCoroutine(LoadSceneAsynchronously(_sceneIndex));
    }
    IEnumerator LoadSceneAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);
        while(!operation.isDone)
        {
            loadingSlider.value = operation.progress;
            yield return null;
        }
    }
}
