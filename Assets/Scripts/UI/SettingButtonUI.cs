using UnityEngine;
using UnityEngine.EventSystems;

namespace MaskGame.UI
{
    /// <summary>
    /// 设置按钮UI - 打开设置面板并暂停游戏
    /// </summary>
    public class SettingButtonUI : MonoBehaviour, IPointerClickHandler
    {
        [Header("设置面板")]
        [SerializeField]
        private GameObject settingCanvas;

        public void OnPointerClick(PointerEventData eventData)
        {
            OpenSettingPanel();
        }

        /// <summary>
        /// 打开设置面板并暂停时间
        /// </summary>
        public void OpenSettingPanel()
        {
            if (settingCanvas != null)
            {
                settingCanvas.SetActive(true);
                Time.timeScale = 0f; // 暂停游戏时间
            }
            else
            {
                Debug.LogWarning("Canvas_setting 未设置！请在Inspector中分配。");
            }
        }
    }
}
