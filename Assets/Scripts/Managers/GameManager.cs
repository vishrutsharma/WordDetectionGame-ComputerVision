using UnityEngine;
using ARSingleton;
using UnityEngine.UI;
using Newtonsoft.Json;
using AR.Models;
using System.Collections;
using AR.Animations;
using AR.GlobalStrings;
using UnityEngine.SceneManagement;

namespace AR.Managers
{
    public class GameManager : Singleton<GameManager>
    {

        #region -------------------- Public Fields ------------------------------
        [SerializeField] private bool clearAllPrefs;
        [HideInInspector] public ApplicationState currentApplicationState;
        [HideInInspector] public bool hasSetupARProduct = false;
        [HideInInspector] public bool allowUserInput;
        [SerializeField] private bool enableLogs;

        #endregion -----------------------------------------------------------------

        #region ----------------------- Private Fields ------------------------------
        private UserData currentUserData;
        private Word currentWordData;
        private MechanismType currentMechanismType;
        private AsyncOperation sceneAsync = null;
        private string sceneToLoad = null;
        #endregion ------------------------------------------------------------------


        /// <summary>
        /// Initialize App Session and Load Player Data 
        /// </summary>
        private void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if (clearAllPrefs) PlayerPrefs.DeleteAll();
            Debug.unityLogger.logEnabled = enableLogs;

            if (PlayerPrefs.HasKey(GameStrings.userData))
            {
                currentUserData = JsonConvert.DeserializeObject<UserData>(PlayerPrefs.GetString(GameStrings.userData));
            }
            else
            {
                currentUserData = new UserData();
                SetDefaultData();
                PlayerPrefs.SetString(GameStrings.userData, JsonConvert.SerializeObject(currentUserData));
            }
        }


        private void UpdateUserData()
        {
            string updatedData = JsonConvert.SerializeObject(currentUserData);
            PlayerPrefs.SetString(GameStrings.userData, updatedData);
        }

        private void SetDefaultData()
        {
            currentUserData.lLevelIndex = 0;
            currentUserData.autoHintEnabled = true;
            currentUserData.musicEnabled = true;
            currentUserData.sfxEnabled = true;
            currentUserData.lMechanismIndex = 0;
            currentUserData.lWordDataIndex = 0;
            currentUserData.totalStarsEarned = 0;
            currentUserData.itemToUnlockIndex = 0;
            currentUserData.unlockedSceneIndex = 0;
            currentUserData.hasUnlockedItem = false;
            currentUserData.isNewUser = true;
        }

        #region ------------------------- Public Methods ----------------------------------------

        public void SetCurrentWordData(Word word)
        {
            currentWordData = word;
        }

        public void SetCurrentLevelIndex(int levelIndex)
        {
            currentUserData.lLevelIndex = levelIndex;
            UpdateUserData();
        }

        public void SetCurrentMechanismIndex(int mechanismIndex)
        {
            currentUserData.lMechanismIndex = mechanismIndex;
            UpdateUserData();
        }

        public void SetCurrentWordIndex(int wordDataIndex)
        {
            currentUserData.lWordDataIndex = wordDataIndex;
            UpdateUserData();
        }

        public void SetItemToUnlockIndex(int index)
        {
            currentUserData.itemToUnlockIndex = index;
            UpdateUserData();
        }

        public void SetItemUnlockStatus(bool status)
        {
            currentUserData.hasUnlockedItem = status;
            UpdateUserData();
        }

        public void SetUnlockSceneIndex(int index)
        {
            currentUserData.unlockedSceneIndex = index;
            UpdateUserData();
        }

        public void SetCurrentMechanicType(MechanismType mechanismType)
        {
            currentMechanismType = mechanismType;
        }

        public void AddStars()
        {
            currentUserData.totalStarsEarned++;
            UpdateUserData();
        }

        public void SetIsNewUser(bool status)
        {
            currentUserData.isNewUser = status;
            UpdateUserData();
        }

        public void SetMusicEnabled(bool status)
        {
            currentUserData.musicEnabled = status;
            UpdateUserData();
        }

        public void SetSFXEnabled(bool status)
        {
            currentUserData.sfxEnabled = status;
            UpdateUserData();
        }

        public void SetAutoHintEnabled(bool status)
        {
            currentUserData.autoHintEnabled = status;
            UpdateUserData();
        }

        public MechanismType GetCurrentMechanicType()
        {
            return currentMechanismType;
        }

        public void ResetStars()
        {
            currentUserData.totalStarsEarned = 0;
            UpdateUserData();
        }

        public int GetUserStars()
        {
            return currentUserData.totalStarsEarned;
        }

        public Word GetCurrentWordData()
        {
            return currentWordData;
        }

        public int GetLevelIndex()
        {
            return currentUserData.lLevelIndex;
        }

        public int GetMechanismIndex()
        {
            return currentUserData.lMechanismIndex;
        }


        public int GetWordIndex()
        {
            return currentUserData.lWordDataIndex;
        }

        public int GetItemUnlockIndex()
        {
            return currentUserData.itemToUnlockIndex;
        }

        public int GetUnlockSceneIndex()
        {
            return currentUserData.unlockedSceneIndex;
        }

        public bool HasUnlockedItemInLastGame()
        {
            return currentUserData.hasUnlockedItem;
        }

        public bool IsNewUser()
        {
            return currentUserData.isNewUser;
        }

        public bool CheckIfMusicEnabled()
        {
            return currentUserData.musicEnabled;
        }

        public bool CheckIfSFXEnabled()
        {
            return currentUserData.sfxEnabled;
        }

        public bool CheckIfAutoHintEnabled()
        {
            return currentUserData.autoHintEnabled;
        }

        #endregion -----------------------------------------------------------------------------------
    }
}

