using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private const string LoadingSceneName = "LoadingScene";
    private static string _targetSceneName;
    private static float _currentProgress;

    public static float Progress => _currentProgress;

    public static void LoadScene(string sceneName)
    {
        _targetSceneName = sceneName;
        _currentProgress = 0f;
        Time.timeScale = 1f;

        GameObject runnerObj = new GameObject("SceneLoaderRunner");
        Object.DontDestroyOnLoad(runnerObj);
        runnerObj.AddComponent<LoaderRunner>().StartLoading();
    }

    private class LoaderRunner : MonoBehaviour
    {
        public void StartLoading()
        {
            StartCoroutine(LoadSequence());
        }

        private IEnumerator LoadSequence()
        {
            yield return SceneManager.LoadSceneAsync(LoadingSceneName, LoadSceneMode.Additive);

            AsyncOperation operation = SceneManager.LoadSceneAsync(_targetSceneName, LoadSceneMode.Single);
            operation.allowSceneActivation = false;

            float visualProgress = 0f;

            while (!operation.isDone)
            {
                float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
                visualProgress = Mathf.MoveTowards(visualProgress, targetProgress, Time.unscaledDeltaTime * 1.5f);
                _currentProgress = visualProgress;

                if (visualProgress >= 1f && operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            if (SceneManager.GetSceneByName(LoadingSceneName).isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(LoadingSceneName);
            }

            Destroy(gameObject);
        }
    }
}
