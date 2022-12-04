using UnityEngine;
using UnityEngine.UI;
using AR.Managers;
using AR.Components;
using AR.Animations;
using AR.Controllers;
using AR.GlobalStrings;

namespace AR.Views
{
    public class NarrationView : MonoBehaviour
    {

        #region ------------------------ Serialize Fields ------------------------
#pragma warning disable 649
        [SerializeField] private GameObject elleObject;
        [SerializeField] private GameObject motherObject;
        [SerializeField] private Canvas narrationCanvas;
        [SerializeField] private GameObject registrationPanel;
        [SerializeField] private GameObject transitionObjectPrefab;
        [SerializeField] private Slider ageSlider;
        [SerializeField] private Text ageBubbleText;
        [SerializeField] private GameObject debugLevelPanel;
        [SerializeField] private NarrationController narrationController;
        [SerializeField] private AudioClip characterSlideClip;
        [SerializeField] private AudioClip buttonClickClip;

        //public GameObject Skip_Btn;
#pragma warning restore 649
        #endregion ----------------------------------------------------------------

        #region  --------------------- Private Methods -----------------------------
        public void LoadScene(string sceneName)
        {
            GameObject transitionObject = Instantiate(transitionObjectPrefab);
            transitionObject.GetComponent<SceneTransitionComponent>().DoSceneTransition(sceneName);
        }

        #endregion ----------------------------------------------------------------


        #region ----------------------- Public Methods ----------------------------

        public void EnableNarration()
        {
            narrationCanvas.gameObject.SetActive(true);
            narrationCanvas.GetComponent<SceneFade>().FadeOut(
              delegate
              {
                  Debug.Log("");
              }
            );

            SlideCharacters(false, true);
        }

        public void ShowUserRegistrationPanel()
        {
            registrationPanel.SetActive(true);
            //Skip_Btn.SetActive(false);
        }

        public void SlideCharacters(bool status, bool isSlidIn)
        {
            SoundManager.Instance.PlayAudio(characterSlideClip, 0.5f, false, true);
            if (isSlidIn)
            {
                elleObject.GetComponent<Slide>().DoSlideIn(false);
                motherObject.GetComponent<Slide>().DoSlideIn(false);
            }
            else
            {
                elleObject.GetComponent<Slide>().DoSlideOut(false);
                motherObject.GetComponent<Slide>().DoSlideOut(false);
            }
        }

        public void DisableNarration()
        {
            // narrationCanvas.GetComponent<SceneFade>().FadeIn(
            //     delegate
            //     {
            //         Debug.Log("");
            //     }
            // );
        }

        public void SubmitRegistration()
        {
            SoundManager.Instance.PlayUISound(buttonClickClip);
            registrationPanel.SetActive(false);
            narrationController.OnRegistrationComplete((int)ageSlider.value);
            //Skip_Btn.SetActive(true);
        }

        public void OnSliderValueChanged()
        {
            ageBubbleText.text = ageSlider.value.ToString();
        }

        public void ToggleDebugLevelPanel()
        {
            SoundManager.Instance.PlayUISound(buttonClickClip);
            debugLevelPanel.SetActive(!debugLevelPanel.activeInHierarchy);
        }

        public void ResetGame()
        {
            SoundManager.Instance.PlayUISound(buttonClickClip);
            PlayerPrefs.DeleteAll();
            LoadScene(GameStrings.menuScene);
        }

        public void OnDebugLevelClick(int levelIndex)
        {
            SoundManager.Instance.PlayUISound(buttonClickClip);
            if (GameManager.Instance.allowUserInput)
            {
                GameManager.Instance.SetCurrentLevelIndex(levelIndex);
                GameManager.Instance.SetCurrentMechanismIndex(0);
                GameManager.Instance.SetCurrentWordIndex(0);
                LoadScene(GameStrings.gameScene);
            }
        }

        #endregion ----------------------------------------------------------------
    }
}
