using UnityEngine;
using UnityEngine.UI;

namespace MaskGame.UI
{
    using MaskGame.Managers;

    /// <summary>
    /// 跳过技能选择按钮
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class SkipSkillButton : MonoBehaviour
    {
        private Button button;
        private AwardPanelUI awardPanel;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnSkipButtonClicked);
        }

        private void Start()
        {
            // 查找AwardPanelUI组件
            awardPanel = FindObjectOfType<AwardPanelUI>();
        }

        /// <summary>
        /// 点击跳过按钮
        /// </summary>
        private void OnSkipButtonClicked()
        {
            if (awardPanel == null)
            {
                awardPanel = FindObjectOfType<AwardPanelUI>();
            }

            // 调用跳过方法
            if (awardPanel != null)
            {
                awardPanel.SkipSkillSelection();
            }
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnSkipButtonClicked);
            }
        }
    }
}
