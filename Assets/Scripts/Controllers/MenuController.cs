using UnityEngine;
using AR.Views;
using AR.Models;
using AR.Managers;
using AR.Components;
using AR.GlobalStrings;

namespace AR.Controllers
{
    public class MenuController : MonoBehaviour
    {

        #region ------------------- Serialize Fields --------------------------
#pragma warning disable 649
        [SerializeField] private CameraPermission cameraPermission;
        [SerializeField] private MenuView menuView;
        [SerializeField] private AudioClip menuBackgroundClip;
        [SerializeField] private GameObject transitionObjectPrefab;

#pragma warning restore 649
        #endregion --------------------------------------------------------------

        #region -------------------- Private Methods ----------------------------

        private void Start()
        {
            GameManager.Instance.currentApplicationState = ApplicationState.MENU_STATE;
            SoundManager.Instance.InitManager();
            SoundManager.Instance.PlayBGM(menuBackgroundClip, 1, true);
        }

        #endregion ----------------------------------------------------------------

        #region ------------------- Public Methods ------------------------------

        /// <summary>
        /// Check for Devices Camera Permission, will proceeed if granted
        /// </summary>
        public void PlayGame()
        {
            cameraPermission.ProcessCameraAccess(
                delegate (bool isPermissionGranted)
                {
                    if (isPermissionGranted)
                    {
                        Debug.Log("Loading Narration Scene because permission granted");
                        GameObject transitionObject = Instantiate(transitionObjectPrefab);
                        transitionObject.GetComponent<SceneTransitionComponent>().DoSceneTransition(GameStrings.narrationScene);
                    }
                    else
                    {
                        Debug.Log("Show popup indicating permission required");
                    }
                }
            );
        }

        #endregion ----------------------------------------------------------------
    }

}
