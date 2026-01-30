using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MaskGame.Data;
using MaskGame.Managers;

namespace MaskGame.UI
{
    /// <summary>
    /// UI管理器 - 控制所有UI元素的显示和更新
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("天数显示")]
        [SerializeField] private TextMeshProUGUI dayText;
        
        [Header("生命UI")]
        [SerializeField] private Image healthImage; // 显示健康值的图片
        [SerializeField] private Sprite[] healthSprites; // 0-4条血的sprite数组（长度5）

        [Header("对话区域")]
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI friendGroupText;
        [SerializeField] private GameObject dialoguePanel;

        [Header("时间显示(斜杠)")]
        [SerializeField] private TextMeshProUGUI timeSlashText;
        [SerializeField] private int maxSlashes = 6; // 总斜杠数
        [SerializeField] private Color normalSlashColor = Color.white;
        [SerializeField] private Color warningSlashColor = Color.red;
        [SerializeField] private float warningThreshold = 2f; // 秒

        [Header("面具选项")]
        [SerializeField] private Button[] maskButtons;
        [SerializeField] private MaskOptionUI[] maskOptionUIs;

        private GameManager gameManager;

        private void Awake()
        {
            gameManager = GameManager.Instance;
            if (gameManager == null) return;

            // 订阅游戏事件
            gameManager.OnDayChanged.AddListener(UpdateDay);
            gameManager.OnBatteryChanged.AddListener(UpdateBattery);
            gameManager.OnTimeChanged.AddListener(UpdateTime);
            gameManager.OnNewEncounter.AddListener(DisplayEncounter);
            gameManager.OnAnswerResult.AddListener(ShowAnswerFeedback);

            // 设置面具按钮
            SetupMaskButtons();
        }

        private void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.OnDayChanged.RemoveListener(UpdateDay);
                gameManager.OnBatteryChanged.RemoveListener(UpdateBattery);
                gameManager.OnTimeChanged.RemoveListener(UpdateTime);
                gameManager.OnNewEncounter.RemoveListener(DisplayEncounter);
                gameManager.OnAnswerResult.RemoveListener(ShowAnswerFeedback);
            }
        }

        /// <summary>
        /// 设置面具按钮
        /// </summary>
        private void SetupMaskButtons()
        {
            for (int i = 0; i < maskButtons.Length; i++)
            {
                if (maskButtons[i] != null)
                {
                    MaskType maskType = (MaskType)i;
                    maskButtons[i].onClick.AddListener(() => OnMaskClicked(maskType));
                }
            }
        }

        /// <summary>
        /// 更新天数显示
        /// </summary>
        private void UpdateDay(int day)
        {
            if (dayText != null)
            {
                dayText.text = $"Day {day}";
            }
            
            // 使用固定的最大斜杠数（固定血量）
            maxSlashes = gameManager.Config.fixedHealth;
        }

        /// <summary>
        /// 更新社交电池/健康值
        /// </summary>
        private void UpdateBattery(int battery)
        {
            if (healthImage == null || healthSprites == null) return;
            
            // 限制battery在0-4范围内
            battery = Mathf.Clamp(battery, 0, 4);
            
            // 检查healthSprites数组长度
            if (battery >= 0 && battery < healthSprites.Length && healthSprites[battery] != null)
            {
                healthImage.sprite = healthSprites[battery];
            }
        }

        /// <summary>
        /// 更新倒计时 - 使用斜杠显示
        /// </summary>
        private void UpdateTime(float remainingTime)
        {
            if (timeSlashText == null) return;

            float maxTime = gameManager.Config.GetDecisionTime(gameManager.CurrentDay);
            // 计算当前应该显示多少个斜杠
            // 每个斜杠代表一段时间
            float timePerSlash = maxTime / maxSlashes;
            int currentSlashes = Mathf.CeilToInt(remainingTime / timePerSlash);
            currentSlashes = Mathf.Clamp(currentSlashes, 0, maxSlashes);

            // 生成斜杠字符串
            string slashString = new string('/', currentSlashes);

            // 时间紧急时变红
            Color slashColor = remainingTime <= warningThreshold ? warningSlashColor : normalSlashColor;
            string colorHex = ColorUtility.ToHtmlStringRGB(slashColor);

            timeSlashText.text = $"<color=#{colorHex}>{slashString}</color>";
        }

        /// <summary>
        /// 显示对话
        /// </summary>
        private void DisplayEncounter(EncounterData encounter)
        {
            if (encounter == null) return;

            // 显示朋友分组
            if (friendGroupText != null)
            {
                friendGroupText.text = encounter.friendGroup;
            }

            // 显示对话文本
            if (dialogueText != null)
            {
                dialogueText.text = encounter.dialogueText;
            }

            // 更新面具选项文本
            if (maskOptionUIs != null && encounter.optionTexts != null)
            {
                for (int i = 0; i < maskOptionUIs.Length && i < encounter.optionTexts.Length; i++)
                {
                    if (maskOptionUIs[i] != null)
                    {
                        maskOptionUIs[i].SetOptionText(encounter.optionTexts[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 显示答案反馈
        /// </summary>
        private void ShowAnswerFeedback(bool isCorrect)
        {
            if (!isCorrect)
            {
                // 错误反馈 - 屏幕抖动效果
                StartCoroutine(ScreenShake());
            }
        }

        /// <summary>
        /// 屏幕抖动效果
        /// </summary>
        private System.Collections.IEnumerator ScreenShake()
        {
            if (dialoguePanel == null) yield break;

            Vector3 originalPos = dialoguePanel.transform.localPosition;
            float duration = 0.3f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-10f, 10f);
                float y = Random.Range(-10f, 10f);
                dialoguePanel.transform.localPosition = originalPos + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            dialoguePanel.transform.localPosition = originalPos;
        }

        /// <summary>
        /// 面具按钮点击
        /// </summary>
        private void OnMaskClicked(MaskType maskType)
        {
            gameManager.SelectMask(maskType);
        }
    }
}