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
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnAnswerResult.RemoveListener(OnAnswerResult);
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
    }
}
