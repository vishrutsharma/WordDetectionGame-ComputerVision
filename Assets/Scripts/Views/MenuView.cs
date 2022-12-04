using UnityEngine;
using UnityEngine.UI;
using AR.Controllers;
using AR.Animations;
using AR.Managers;
using AR.GlobalStrings;

namespace AR.Views
{
    public class MenuView : MonoBehaviour
    {

        #region ------------------------ Serialize Fields -----------------------------
#pragma warning disable 649
        [SerializeField] private Button playButton;
        [SerializeField] private GameObject canvas;
        [SerializeField] private MenuController menuController;
        [SerializeField] private AudioClip buttonClick;
#pragma warning restore 649
        #endregion ---------------------------------------------------------------------

        #region --------------------------- Private Methods ------------------------------

        private void Start()
        {
            playButton.onClick.AddListener(OnPlayClick);
        }
        private void OnPlayClick()
        {
            SoundManager.Instance.PlayUISound(buttonClick);
            menuController.PlayGame();
        }

        #endregion ---------------------------------------------------------------------
    }
}

