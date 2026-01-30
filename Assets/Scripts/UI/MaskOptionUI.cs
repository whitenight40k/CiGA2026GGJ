using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace MaskGame.UI
{
    /// <summary>
    /// 面具选项UI - 鼠标悬停显示选项文本
    /// </summary>
    public class MaskOptionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("选项数据")]
        [SerializeField] private string optionText;
        
        [Header("提示UI")]
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private TextMeshProUGUI tooltipText;
        
        [Header("设置")]
        [SerializeField] private Vector2 tooltipOffset = new Vector2(0, 50);

        private RectTransform tooltipRect;

        private void Awake()
        {
            if (tooltipPanel != null)
            {
                tooltipRect = tooltipPanel.GetComponent<RectTransform>();
                tooltipPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 设置选项文本
        /// </summary>
        public void SetOptionText(string text)
        {
            optionText = text;
        }

        /// <summary>
        /// 鼠标进入时显示提示
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (tooltipPanel != null && !string.IsNullOrEmpty(optionText))
            {
                tooltipPanel.SetActive(true);
                
                if (tooltipText != null)
                {
                    tooltipText.text = optionText;
                }

                // 更新提示框位置（在鼠标上方）
                if (tooltipRect != null)
                {
                    Vector2 localPoint;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        tooltipRect.parent as RectTransform,
                        eventData.position,
                        eventData.pressEventCamera,
                        out localPoint
                    );
                    tooltipRect.localPosition = localPoint + tooltipOffset;
                }
            }
        }

        /// <summary>
        /// 鼠标离开时隐藏提示
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
            }
        }
    }
}
