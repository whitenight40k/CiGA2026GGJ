using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace MaskGame.UI
{
    /// <summary>
    /// 游戏结束UI管理器
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("UI元素")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            // 设置按钮事件
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestart);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuit);
            }

            // 显示游戏结束信息
            DisplayGameOverInfo();
        }

        private void DisplayGameOverInfo()
        {
            if (titleText != null)
            {
                titleText.text = "社死！";
            }

            if (scoreText != null)
            {
                // 从PlayerPrefs读取统计数据
                int correctAnswers = PlayerPrefs.GetInt("CorrectAnswers", 0);
                int totalAnswers = PlayerPrefs.GetInt("TotalAnswers", 0);

                scoreText.text = $"答对 {correctAnswers}/{totalAnswers} 题";
            }
        }

        private void OnRestart()
        {
            SceneManager.LoadScene("Main");
        }

        private void OnQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
