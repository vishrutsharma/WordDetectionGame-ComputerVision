using System;
using UnityEngine;
using AR.Views;
using System.Collections;
using AR.Managers;
using AR.Scriptables;
using System.Collections.Generic;

namespace AR.Controllers
{
    public class WorldController : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private NarrationController narrationController;
        [SerializeField] private List<WorldView> worldViews;
        [SerializeField] private UnlockableElementsScriptable unlockElementsData;
#pragma warning restore 649
        private int currentWorldIndex = 0;

        private void Start()
        {
            ShowWorld();
        }

        /// <summary>
        /// Show World Panel in ScrollView
        /// </summary>
        private void ShowWorld()
        {
            worldViews[currentWorldIndex].ToggleInputUI(false);
            worldViews[currentWorldIndex].ShowWorld(
                delegate
                {
                    narrationController.InitNarration();
                }
            );
        }


        /// <summary>
        /// Check whether all items of current scene are unlocked or not, if unlocked game will move to next scene
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        private IEnumerator ShowNextNarrationState(int itemIndex)
        {
            int sceneIndex = GameManager.Instance.GetUnlockSceneIndex();
            yield return StartCoroutine(narrationController.PlayDialogue(unlockElementsData.unlockData[sceneIndex].unlockableItems[itemIndex - 1].dialogue));
            yield return new WaitForSeconds(1);

            bool hasUnlockedNewScene = itemIndex >= unlockElementsData.unlockData[sceneIndex].unlockableItems.Count ? true : false;
            sceneIndex = hasUnlockedNewScene ? sceneIndex + 1 : sceneIndex;
            itemIndex = hasUnlockedNewScene ? 0 : itemIndex;
            GameManager.Instance.SetUnlockSceneIndex(sceneIndex);
            GameManager.Instance.SetItemToUnlockIndex(itemIndex);
            if (hasUnlockedNewScene)
                worldViews[currentWorldIndex].ShowNewScene();
            else
            {
                GameManager.Instance.allowUserInput = true;
                worldViews[currentWorldIndex].ToggleInputUI(true);
            }
        }

        /// <summary>
        /// Setup unlocked item state of current world and play narration between Elle and Pigeon
        /// </summary>
        public void SetWorldItems()
        {
            worldViews[currentWorldIndex].SetupItems();
            if (GameManager.Instance.HasUnlockedItemInLastGame())
            {
                GameManager.Instance.allowUserInput = false;
                GameManager.Instance.SetItemUnlockStatus(false);
                int sceneIndex = GameManager.Instance.GetUnlockSceneIndex();
                int itemIndex = GameManager.Instance.GetItemUnlockIndex();
                itemIndex++;
                worldViews[currentWorldIndex].UnlockItem(
                    itemIndex - 1,
                    delegate
                    {
                        StartCoroutine(ShowNextNarrationState(itemIndex));
                    }
                );
            }
            else
            {
                worldViews[currentWorldIndex].ToggleInputUI(true);
            }
            GameManager.Instance.allowUserInput = true;
        }
    }
}
