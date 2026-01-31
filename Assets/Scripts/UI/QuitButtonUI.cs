using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MaskGame.UI
{
    /// <summary>
    /// 退出按钮UI - 返回菜单场景并恢复时间
    /// </summary>
    public class QuitButtonUI : MonoBehaviour, IPointerClickHandler
    {
        [Header("目标场景")]
        [SerializeField]
        private string menuSceneName = "Menu";

        public void OnPointerClick(PointerEventData eventData)
        {
            QuitToMenu();
        }

        /// <summary>
        /// 返回菜单场景
        /// </summary>
        public void QuitToMenu()
        {
            // 恢复时间缩放
            Time.timeScale = 1f;
            
            // 加载菜单场景
            SceneManager.LoadScene(menuSceneName);
        }
    }
}
