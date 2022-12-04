using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AR.Animations;
using UnityEngine.SceneManagement;

namespace AR.Components
{
    public class SceneTransitionComponent : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// Play Scene Transition fade in fade out between Scenes Menu,Narration,Game
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private IEnumerator PlaySceneTransition(string sceneName)
        {
            Slider progressSlider = GetComponentInChildren<Slider>();
            AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(sceneName);
            progressSlider.value = 0;
            while (sceneAsync.progress < 0.9f)
            {
                progressSlider.value = sceneAsync.progress;
                yield return new WaitForEndOfFrame();
            }
            while (!sceneAsync.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
            sceneAsync.allowSceneActivation = true;
            GetComponent<SceneFade>().FadeIn(delegate { Destroy(gameObject); });
        }

        public void DoSceneTransition(string sceneName)
        {
            GetComponent<SceneFade>().FadeOut(
                delegate
                {
                    StartCoroutine(PlaySceneTransition(sceneName));
                }
            );
        }
    }
}

