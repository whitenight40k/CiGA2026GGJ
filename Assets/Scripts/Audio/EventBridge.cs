using UnityEngine;
using MaskGame.Managers;

namespace MaskGame.Audio
{
    public class GameAudioEventBridge : MonoBehaviour
    {
        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnAnswerResult.AddListener(OnAnswerResult);
                GameManager.Instance.OnGameOver.AddListener(OnGameOver);
                GameManager.Instance.OnDayComplete.AddListener(OnDayComplete);
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnAnswerResult.RemoveListener(OnAnswerResult);
                GameManager.Instance.OnGameOver.RemoveListener(OnGameOver);
                GameManager.Instance.OnDayComplete.RemoveListener(OnDayComplete);
            }
        }

        private void OnAnswerResult(bool isCorrect, string feedback)
        {
            if (feedback.Contains("超时"))
            {
                AudioManager.Instance?.Play(SoundType.Timeout);
            }
            else if (isCorrect)
            {
                AudioManager.Instance?.Play(SoundType.AnswerCorrect);
            }
            else
            {
                AudioManager.Instance?.Play(SoundType.AnswerWrong);
            }
        }

        private void OnGameOver()
        {
            AudioManager.Instance?.Play(SoundType.GameOver);
        }

        private void OnDayComplete()
        {
            AudioManager.Instance?.Play(SoundType.DayComplete);
        }
    }
}
