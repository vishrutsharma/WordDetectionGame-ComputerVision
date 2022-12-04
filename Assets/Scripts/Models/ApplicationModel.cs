﻿
namespace AR.Models
{
    public enum ApplicationState
    {
        MENU_STATE,
        NARRATION_STATE,
        GAMEPLAY_STATE,
        GAMEOVER_STATE
    }

    [System.Serializable]
    public class UserData
    {
        public bool isNewUser;
        public bool autoHintEnabled;
        public bool sfxEnabled;
        public bool musicEnabled;
        public string userName;
        public int userAge;
        public int lLevelIndex;
        public int lMechanismIndex;
        public int lWordDataIndex;
        public int totalStarsEarned;
        public int unlockedSceneIndex;
        public int itemToUnlockIndex;
        public bool hasUnlockedItem;

    }
}


