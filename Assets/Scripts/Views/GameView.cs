using TMPro;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using AR.Models;
using System.Collections;
using AR.Managers;
using AR.Components;
using AR.Animations;
using AR.Scriptables;
using AR.Controllers;
using AR.GlobalStrings;
using System.Collections.Generic;

namespace AR.Views
{
    public class GameView : MonoBehaviour
    {
        #region ------------------------ Serialize Fields -----------------------
#pragma warning disable 649
        [SerializeField] private Canvas canvas;
        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private GameObject eleObject;
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject dummyInputPanel;

        [Header("AR Product Setup")]
        [SerializeField] private GameObject arProductPanel;
        [SerializeField] private GameObject arCatalogPanel;
        [SerializeField] private GameObject arQuestionPanel;
        [SerializeField] private Button leftScrollButton;
        [SerializeField] private Button rightScrollButton;
        [SerializeField] private List<Sprite> arManualPagesSprites;
        [SerializeField] private Image arManualPageImage;
        [SerializeField] private TextMeshProUGUI aText;
        [SerializeField] private TextMeshProUGUI productSetupText;
        [Space]
        [Space]
        [SerializeField] private TextMeshProUGUI eleDialogueText;
        [SerializeField] private GameObject speechBubble;
        [SerializeField] private GameObject starsParentPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject musicToggleButtonObject;
        [SerializeField] private GameObject soundToggleButtonObject;
        [SerializeField] private GameObject autoHintToggleButtonObject;
        [SerializeField] private List<Sprite> settingToggleSprites;
        [SerializeField] private GameObject transitionObjectPrefab;
        [Header("Unlockable Item")]
        [SerializeField] private GameObject bigStar;
        [SerializeField] private TextMeshProUGUI starStatusText;
        [SerializeField] private GameObject itemProgressPanel;
        [SerializeField] private Image itemUnlockOutlineImage;
        [SerializeField] private Image itemUnlockProgressImage;
        [SerializeField] private GameObject itemRevealPanel;
        [SerializeField] private GameObject keypadObject;
        [SerializeField] private Image itemRevealImage;
        [SerializeField] private TextMeshProUGUI itemRevealLabel;
        [SerializeField] private GameObject unlockStarObject;
        [SerializeField] private ParticleSystem winParticleSystem;
        [SerializeField] private ParticleSystem collectStarParticle;
        // [SerializeField] private ParticleSystem coinsParticle;
        // [SerializeField] private ParticleSystem coinsParticle1;
        // [SerializeField] private ParticleSystem coinsParticle2;

        [Header("Debug")]
        [SerializeField] private Text debugLevelText;
        [SerializeField] private Text debugWordText;
        [SerializeField] private Text debugMechanismText;


        [Header("Hint System")]
        [SerializeField] private GameObject hintButtonObject;
        [SerializeField] private GameObject handCursorObject;
        [SerializeField] private GameController gameController;
        [SerializeField] private AudioClip buttonClick;
        [SerializeField] private AudioClip characterSlideClip;
        [SerializeField] private AudioClip itemRevealClip;
        [SerializeField] private AudioClip gameBackgroundClip;
#pragma warning restore 649
        #endregion ---------------------------------------------------------------------


        #region ------------------------ Private Fields ---------------------------------
        private float eleDialogueDelay = 4;
        private float incorrectWordDelay = 4;
        private float wordCompleteAnimDelay = 3;
        private int currentARManualPageIndex = 0;
        private bool isReadingManual = false;
        private ItemData currentItemData;
        private SoundManager soundManager;

        #endregion ----------------------------------------------------------------------
        public bool IsReadingManual
        {
            get { return isReadingManual; }
        }
        #region ------------------------ Private Methods -----------------------------

        private void Start()
        {
            bool isMusicEnabled = GameManager.Instance.CheckIfMusicEnabled();
            bool isSFXEnabled = GameManager.Instance.CheckIfSFXEnabled();
            bool isAutoHintEnabled = GameManager.Instance.CheckIfAutoHintEnabled();
            musicToggleButtonObject.GetComponent<Image>().sprite = isMusicEnabled ? settingToggleSprites[0] : settingToggleSprites[1];
            soundToggleButtonObject.GetComponent<Image>().sprite = isSFXEnabled ? settingToggleSprites[0] : settingToggleSprites[1];
            autoHintToggleButtonObject.GetComponent<Image>().sprite = isAutoHintEnabled ? settingToggleSprites[0] : settingToggleSprites[1];
        }


        /// <summary>
        /// Show Unlock Star Animation
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowStarsAnimation()
        {
            overlayCanvas.GetComponent<CanvasGroup>().alpha = 1;
            unlockStarObject.SetActive(true);
            unlockStarObject.GetComponent<UnlockStarComponent>().UnlockStar(
                bigStar.transform.position,
                delegate
                {
                    collectStarParticle.gameObject.transform.position = bigStar.transform.position;
                    bigStar.GetComponent<Animator>().Play(GameStrings.collectAnimation);
                    collectStarParticle.Play();
                    unlockStarObject.SetActive(false);
                    SetItemToUnlockStatus(currentItemData);
                }
            );
            yield return new WaitForSeconds(wordCompleteAnimDelay);
        }

        private IEnumerator ShowItemUnlockAnimation()
        {
            GameManager.Instance.ResetStars();
            starsParentPanel.SetActive(false);
            //coinsParticle.Play();// newly added
            Animator anim = itemRevealPanel.GetComponent<Animator>();
            // Added for different particle effects
            // if (currentItemData.totalPointsToUnlock == 5)
            // {
            //     coinsParticle.Play();
            // }
            // if (currentItemData.totalPointsToUnlock == 10)
            // {
            //     coinsParticle1.Play();
            // }
            // if (currentItemData.totalPointsToUnlock == 15)
            // {
            //     coinsParticle2.Play();
            // }

            SoundManager.Instance.PlayAudio(itemRevealClip, 0.5f, false, true);
            itemRevealPanel.SetActive(true);
            //coinsParticle.Play();
            itemRevealImage.sprite = currentItemData.itemSprite;
            itemRevealLabel.text = GameStrings.itemUnlocked + currentItemData.itemName;
            while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                yield return null;
            }
            itemRevealPanel.SetActive(false);
            ToggleGameScene(false);
        }

        // MOVE THIS CODE TO GAME CONTROLLER
        /// <summary>
        /// Handles entire game State after letter inserted is correct and Item Unlock State is checked
        /// </summary>
        /// <param name="hasUnlockedStar"></param>
        /// <param name="hasUnlockedItem"></param>
        /// <returns></returns>
        private IEnumerator ShowEleDialogue(bool hasUnlockedStar, bool hasUnlockedItem)
        {
            ToggleSettingsPanel(false);

            if (hasUnlockedStar)
                yield return StartCoroutine(ShowStarsAnimation());

            overlayCanvas.GetComponent<CanvasGroup>().alpha = 0;
            gameController.ProcessOutroStage();
            speechBubble.GetComponent<SpeechBubbleComponent>().ToggleSpeechBubble(false);

            if (hasUnlockedStar)
            {
                yield return new WaitForSeconds(0.2f);
                itemUnlockProgressImage.sprite = currentItemData.itemSprite;
                itemUnlockOutlineImage.sprite = currentItemData.itemSprite;
                itemProgressPanel.SetActive(true);
                int starsEarned = GameManager.Instance.GetUserStars();
                float res = (float)starsEarned - 1;
                float startFill = starsEarned == 0 ? 0 : (float)res / currentItemData.totalPointsToUnlock;
                itemUnlockProgressImage.fillAmount = startFill;
                overlayCanvas.GetComponent<SceneFade>().FadeOut(null);
                yield return new WaitForSeconds(0.6f);
                float timeQuant = 0;
                float progressFillTime = 2;
                itemUnlockOutlineImage.GetComponent<PulsatingEffect>().BeginPulsate();

                float endFill = (float)starsEarned / currentItemData.totalPointsToUnlock;
                float currentFill = 0;
                while (timeQuant < 1)
                {
                    currentFill = Mathf.Lerp(startFill, endFill, timeQuant);
                    itemUnlockProgressImage.fillAmount = currentFill;
                    timeQuant += Time.deltaTime / progressFillTime;
                    yield return new WaitForEndOfFrame();
                }
                itemUnlockProgressImage.fillAmount = endFill;
                itemUnlockOutlineImage.GetComponent<PulsatingEffect>().EndPulsate();
                if (hasUnlockedItem)
                {

                    itemProgressPanel.SetActive(false);
                    yield return StartCoroutine(ShowItemUnlockAnimation());
                }
                overlayCanvas.GetComponent<SceneFade>().FadeIn(null);
                yield return new WaitForSeconds(0.6f);
                itemProgressPanel.SetActive(false);

            }
            yield return new WaitForSeconds(0.5f);

            string[] wordCompleteStrings = GameStrings.wordCompletedStrings;
            string dialogueString = wordCompleteStrings[Random.Range(0, wordCompleteStrings.Length)];
            yield return StartCoroutine(ElleDialogueCoroutine(dialogueString));

            while (!gameController.IsBoardClear())
            {
                yield return new WaitForSeconds(0.5f);
            }
            SoundManager.Instance.PlayAudio(characterSlideClip, 0.5f, false, true);
            dialoguePanel.GetComponent<Slide>().DoSlideOut(false);
            eleObject.GetComponent<Slide>().DoSlideOut(false);
            dialoguePanel.SetActive(false);
            gameController.OnWordClosureComplete();
        }

        private IEnumerator ShowIncorrectWordState()
        {
            yield return new WaitForSeconds(incorrectWordDelay);
            speechBubble.GetComponent<SpeechBubbleComponent>().ToggleSpeechBubble(false);
        }

        #endregion ---------------------------------------------------------------------


        #region ------------------------ Public Methods -----------------------------

        /// <summary>
        /// Show Elle Dialogue for indicating Player 
        /// </summary>
        /// <param name="dialogueText"></param>
        /// <returns></returns>
        public IEnumerator ElleDialogueCoroutine(string dialogueText)
        {
            SoundManager.Instance.PlayAudio(characterSlideClip, 0.5f, false, true);
            eleObject.GetComponent<Slide>().DoSlideIn(false);
            dialoguePanel.SetActive(true);
            dialoguePanel.GetComponent<Slide>().DoSlideIn(false);

            float letterAnimationDelay = 1 / dialogueText.Length;
            char[] dialogueArray = dialogueText.ToCharArray();
            StringBuilder currentDialogueWorld = new StringBuilder();
            for (int i = 0; i < dialogueArray.Length; i++)
            {
                currentDialogueWorld.Append(dialogueArray[i]);
                eleDialogueText.text = currentDialogueWorld.ToString();
                yield return new WaitForSeconds(letterAnimationDelay);
            }
        }

        public void SetNewWordData()
        {
            // debugLevelText.text = "Level : " + (GameManager.Instance.GetLevelIndex() + 1);
            // debugMechanismText.text = "Mechanism : " + GameManager.Instance.GetCurrentMechanicType();
            // debugWordText.text = "Word : " + GameManager.Instance.GetCurrentWordData().word;
            ToggleSettingsPanel(false);
            keypadObject.SetActive(true);
            if (GameManager.Instance.GetLevelIndex() == 0)
            {
                if (GameManager.Instance.GetMechanismIndex() == 0 && GameManager.Instance.GetWordIndex() <= 2)
                {
                    hintButtonObject.SetActive(false);
                }
                else
                {
                    hintButtonObject.SetActive(true);
                }
            }
            else
            {
                hintButtonObject.SetActive(true);
            }
        }

        public void ToggleDummyInput()
        {
            SoundManager.Instance.PlayUISound(buttonClick);
            bool status = dummyInputPanel.activeInHierarchy;
            dummyInputPanel.SetActive(!status);
        }

        public void SetItemToUnlockStatus(ItemData itemData)
        {
            SoundManager.Instance.PlayAudio(characterSlideClip, 0.5f, false, true);
            dialoguePanel.GetComponent<Slide>().DoSlideOut(false);
            eleObject.GetComponent<Slide>().DoSlideOut(false);
            dialoguePanel.SetActive(false);
            currentItemData = itemData;
            int starsEarned = GameManager.Instance.GetUserStars();
            starStatusText.text = $"{starsEarned}/{currentItemData.totalPointsToUnlock}";
        }

        public void ToggleGameScene(bool status)
        {
            gameController.CloseBridge();
            GameObject transitionObject = Instantiate(transitionObjectPrefab);
            transitionObject.GetComponent<SceneTransitionComponent>().DoSceneTransition(GameStrings.narrationScene);
        }

        public void ShowWordComplete(Answer wordResult, bool hasUnlockedStar = false, bool hasUnlockedItem = false)
        {
            dummyInputPanel.SetActive(false);

            if (wordResult is Answer.CORRECT)
            {
                keypadObject.SetActive(false);
                hintButtonObject.SetActive(false);
                winParticleSystem.Play();
                StopCoroutine("ShowEleDialogue");
                StartCoroutine(ShowEleDialogue(hasUnlockedStar, hasUnlockedItem));
            }
            else
            {
                StopCoroutine("ShowIncorrectWordState");
                StartCoroutine(ShowIncorrectWordState());
            }
        }

        public void ShowSpeechBubble(Answer wordResult)
        {
            dummyInputPanel.SetActive(false);
            speechBubble.SetActive(true);
            speechBubble.GetComponent<SpeechBubbleComponent>().ToggleSpeechBubble(true, wordResult);
        }

        public void ShowHintUI()
        {
            hintButtonObject.SetActive(true);
            handCursorObject.SetActive(true);
            handCursorObject.GetComponent<WaveEffect>().BeginWave();
        }

        public void OnHintButtonClick()
        {
            SoundManager.Instance.PlayUISound(buttonClick);
            if (GameManager.Instance.allowUserInput)
            {
                handCursorObject.SetActive(false);
                handCursorObject.GetComponent<WaveEffect>().EndWave();
                gameController.ShowHint();
            }
        }

        public void ToggleSettingsPanel(bool status)
        {
            overlayCanvas.GetComponent<CanvasGroup>().alpha = status ? 1 : 0;
            settingsPanel.SetActive(status);
        }

        public void OnCloseClick()
        {
            SoundManager.Instance.PlayUISound(buttonClick);
            gameController.OnGameClose();
        }

        public void ToggleMusic()
        {
            bool status = !GameManager.Instance.CheckIfMusicEnabled();
            musicToggleButtonObject.GetComponent<Image>().sprite = status ? settingToggleSprites[0] : settingToggleSprites[1];
            GameManager.Instance.SetMusicEnabled(status);
            SoundManager.Instance.ToggleMusic();
        }

        public void ToggleSFX()
        {
            bool status = !GameManager.Instance.CheckIfSFXEnabled();
            soundToggleButtonObject.GetComponent<Image>().sprite = status ? settingToggleSprites[0] : settingToggleSprites[1];
            GameManager.Instance.SetSFXEnabled(status);
            SoundManager.Instance.ToggleSFX();
        }

        public void ToggleHint()
        {
            bool status = !GameManager.Instance.CheckIfAutoHintEnabled();
            autoHintToggleButtonObject.GetComponent<Image>().sprite = status ? settingToggleSprites[0] : settingToggleSprites[1];
            GameManager.Instance.SetAutoHintEnabled(status);
        }

        public void OnARScrollButtonClick(bool isLeft)
        {
            currentARManualPageIndex = isLeft ? currentARManualPageIndex - 1 : currentARManualPageIndex + 1;
            if (currentARManualPageIndex == 0)
            {
                leftScrollButton.gameObject.SetActive(false);
                if (arManualPagesSprites[currentARManualPageIndex] != null)
                    arManualPageImage.sprite = arManualPagesSprites[currentARManualPageIndex];

                productSetupText.text = GameStrings.productStepsStrings[currentARManualPageIndex];
            }
            else if (currentARManualPageIndex == arManualPagesSprites.Count)
            {
                arCatalogPanel.SetActive(false);
                isReadingManual = false;
            }
            else
            {
                if (arManualPagesSprites[currentARManualPageIndex] != null)
                    arManualPageImage.sprite = arManualPagesSprites[currentARManualPageIndex];

                productSetupText.text = GameStrings.productStepsStrings[currentARManualPageIndex];
                leftScrollButton.gameObject.SetActive(true);
                rightScrollButton.gameObject.SetActive(true);
            }
        }

        public void CloseARTutorial()
        {
            arProductPanel.SetActive(false);
        }

        public void OnProductSetupButtonClick()
        {
            isReadingManual = true;
            arCatalogPanel.SetActive(true);
            currentARManualPageIndex = 0;
            arManualPageImage.sprite = arManualPagesSprites[currentARManualPageIndex];
            productSetupText.text = GameStrings.productStepsStrings[currentARManualPageIndex];
            leftScrollButton.gameObject.SetActive(false);
        }

        public void ShowARProductSetup()
        {
            arProductPanel.SetActive(true);
            arQuestionPanel.SetActive(true);
            StartCoroutine(gameController.BeginBoardCalibration());
        }

        #endregion ---------------------------------------------------------------------
    }
}
