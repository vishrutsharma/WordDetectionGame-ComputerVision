using TMPro;
using System;
using UnityEngine;
using System.Collections;
using AR.Models;
using AR.Managers;
using AR.Animations;
using AR.Controllers;
using AR.GlobalStrings;
using System.Collections.Generic;

namespace AR.Views
{
    public class WordView : MonoBehaviour
    {
        #region --------------------- Serialize Fields ----------------------
#pragma warning disable 649
        [SerializeField] private Color hintColor;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color completedColor;
        [SerializeField] private Transform wordHolderPanel;
        [SerializeField] private GameObject wordHolderPrefab;
        [SerializeField] private WordController wordController;
        [SerializeField] private AudioClip popClip;
#pragma warning restore 649
        #endregion -----------------------------------------------------------


        #region ---------------------- Private Fields -------------------------
        private List<GameObject> wordHolders;
        private float holdersDestroyDelay = 3;
        private String currentWord;


        #endregion ------------------------------------------------------------

        #region ----------------------- Public Methods -------------------------
        /// <summary>
        /// Populate word in Slot Tiles and hide it based on provided letter indexes
        /// </summary>
        /// <param name="word"></param>
        /// <param name="letterIndexes"></param>
        /// <param name="wordsInitializationComplete"></param>
        /// <returns></returns>
        public IEnumerator InitWords(string word, List<int> letterIndexes, Action wordsInitializationComplete)
        {
            Word currentWordData = GameManager.Instance.GetCurrentWordData();
            if (currentWordData.isGuided)
                yield return new WaitForSeconds(0.5f);

            currentWord = word;
            wordHolders = new List<GameObject>();
            float delayTime = 0.8f;
            for (int i = 0; i < word.Length; i++)
            {
                GameObject wOb = Instantiate(wordHolderPrefab);
                wOb.transform.SetParent(wordHolderPanel);
                wOb.transform.localScale = Vector2.zero;
                wOb.GetComponent<Oscillation>().oscillationSpeed = 0;
                wordHolders.Add(wOb);
                TextMeshProUGUI letterText = wOb.GetComponentInChildren<TextMeshProUGUI>();
                if (letterIndexes.Contains(i))
                {
                    letterText.color = hintColor;
                    letterText.text = currentWordData.showLetter ? word[i] + "" : "-";
                }
                else
                {
                    letterText.color = defaultColor;
                    letterText.text = word[i] + "";
                }
                SoundManager.Instance.PlayAudio(popClip, 0.5f, false, true);
                wOb.GetComponent<Animator>().Play(GameStrings.scaleInAnimation);
                yield return new WaitForSeconds(delayTime);
            }
            wordsInitializationComplete.Invoke();
        }

        public void IndicateLetter(int index)
        {
            wordHolders[index].GetComponent<Oscillation>().oscillationSpeed = 20;
            wordHolders[index].GetComponent<Oscillation>().scaleFactor = 2;
            wordHolders[index].GetComponentInChildren<TextMeshProUGUI>().color = hintColor;
        }

        public void OnCorrectLetter(int index)
        {
            TextMeshProUGUI letterText = wordHolders[index].GetComponentInChildren<TextMeshProUGUI>();
            letterText.text = currentWord[index].ToString();
            letterText.color = completedColor;
            wordHolders[index].GetComponent<Oscillation>().oscillationSpeed = 0;
            wordHolders[index].GetComponent<Oscillation>().scaleFactor = 0;
        }

        public void CloseWordHolders()
        {
            for (int i = 0; i < wordHolders.Count; i++)
            {
                wordHolders[i].GetComponent<Animator>().Play(GameStrings.scaleOutAnimation);
                Destroy(wordHolders[i].gameObject, holdersDestroyDelay);
            }
            wordHolders.Clear();
        }

        public void ShowIncorrect(int index)
        {
            wordHolders[index].GetComponent<Animator>().Play(GameStrings.wrongAnswer);
        }

        #endregion ------------------------------------------------------------
    }
}
