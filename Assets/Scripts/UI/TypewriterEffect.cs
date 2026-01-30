using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MaskGame.UI
{
    /// <summary>
    /// 打字机效果 - 让文本逐字显示，支持音效和富文本漂浮效果
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TypewriterEffect : MonoBehaviour
    {
        [Header("打字机设置")]
        [Tooltip("每个字的显示间隔（秒）")]
        [SerializeField] private float charDelay = 0.05f;
        
        [Tooltip("标点符号的额外延迟（秒）")]
        [SerializeField] private float punctuationDelay = 0.2f;
        
        [Tooltip("是否在开始时自动播放")]
        [SerializeField] private bool playOnEnable = false;

        [Header("音效设置")]
        [Tooltip("打字音效（每个字播放一次）")]
        [SerializeField] private AudioClip typingSound;
        
        [Tooltip("音效音量")]
        [SerializeField][Range(0f, 1f)] private float soundVolume = 0.5f;
        
        [Tooltip("每隔几个字播放一次音效（1=每个字，2=每两个字）")]
        [SerializeField] private int soundFrequency = 1;
        
        [Header("富文本漂浮效果")]
        [Tooltip("是否启用富文本标记内容的漂浮效果")]
        [SerializeField] private bool enableFloatingEffect = true;
        
        [Tooltip("漂浮幅度")]
        [SerializeField] private float floatAmplitude = 5f;
        
        [Tooltip("漂浮速度")]
        [SerializeField] private float floatSpeed = 2f;

        private TextMeshProUGUI textComponent;
        private AudioSource audioSource;
        private Coroutine typewriterCoroutine;
        private Coroutine floatingCoroutine;
        private string fullText;
        private bool isTyping = false;
        private List<int> richTextIndices = new List<int>(); // 存储富文本标记字符的索引

        private void Awake()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            
            // 创建AudioSource组件用于播放音效
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = soundVolume;
        }

        private void OnEnable()
        {
            if (playOnEnable && !string.IsNullOrEmpty(textComponent.text))
            {
                PlayTypewriter(textComponent.text);
            }
        }

        /// <summary>
        /// 播放打字机效果
        /// </summary>
        public void PlayTypewriter(string text)
        {
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
            }
            
            fullText = text;
            DetectRichTextTags(); // 检测富文本标记
            typewriterCoroutine = StartCoroutine(TypewriterCoroutine());
            
            // 启动漂浮效果
            if (enableFloatingEffect && richTextIndices.Count > 0)
            {
                if (floatingCoroutine != null)
                {
                    StopCoroutine(floatingCoroutine);
                }
                floatingCoroutine = StartCoroutine(FloatingEffectCoroutine());
            }
        }

        /// <summary>
        /// 检测富文本标记的字符范围
        /// </summary>
        private void DetectRichTextTags()
        {
            richTextIndices.Clear();
            
            // 检测常见的富文本标签：<b>、<color>、<mark>等
            string[] startTags = { "<b>", "<color=", "<mark=", "<i>", "<u>", "<s>" };
            string[] endTags = { "</b>", "</color>", "</mark>", "</i>", "</u>", "</s>" };
            
            for (int i = 0; i < startTags.Length; i++)
            {
                int searchStart = 0;
                while (searchStart < fullText.Length)
                {
                    int startIndex = fullText.IndexOf(startTags[i], searchStart);
                    if (startIndex == -1) break;
                    
                    // 找到开始标签后，查找结束标签
                    int tagEndIndex = fullText.IndexOf(">", startIndex) + 1;
                    int endIndex = fullText.IndexOf(endTags[i], tagEndIndex);
                    
                    if (endIndex != -1)
                    {
                        // 记录标记内容的实际字符位置（不包括标签本身）
                        for (int j = tagEndIndex; j < endIndex; j++)
                        {
                            // 只记录可见字符
                            if (fullText[j] != '<' && fullText[j] != '>')
                            {
                                richTextIndices.Add(j);
                            }
                        }
                        searchStart = endIndex + endTags[i].Length;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 立即显示全部文本（跳过动画）
        /// </summary>
        public void SkipToEnd()
        {
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }
            
            if (floatingCoroutine != null)
            {
                StopCoroutine(floatingCoroutine);
                floatingCoroutine = null;
            }
            
            textComponent.text = fullText;
            isTyping = false;
            
            // 重新启动漂浮效果
            if (enableFloatingEffect && richTextIndices.Count > 0)
            {
                floatingCoroutine = StartCoroutine(FloatingEffectCoroutine());
            }
        }

        /// <summary>
        /// 打字机协程
        /// </summary>
        private IEnumerator TypewriterCoroutine()
        {
            isTyping = true;
            textComponent.text = "";
            
            int charIndex = 0;
            
            while (charIndex < fullText.Length)
            {
                // 添加下一个字符
                textComponent.text += fullText[charIndex];
                
                // 播放打字音效
                if (typingSound != null && charIndex % soundFrequency == 0)
                {
                    audioSource.PlayOneShot(typingSound, soundVolume);
                }
                
                // 计算延迟时间
                float delay = charDelay;
                
                // 标点符号增加延迟
                char currentChar = fullText[charIndex];
                if (IsPunctuation(currentChar))
                {
                    delay += punctuationDelay;
                }
                
                yield return new WaitForSeconds(delay);
                charIndex++;
            }
            
            isTyping = false;
            typewriterCoroutine = null;
        }

        /// <summary>
        /// 漂浮效果协程 - 对富文本标记的字符应用波浪动画
        /// </summary>
        private IEnumerator FloatingEffectCoroutine()
        {
            while (true)
            {
                // 强制更新文本网格
                textComponent.ForceMeshUpdate();
                
                TMP_TextInfo textInfo = textComponent.textInfo;
                
                // 遍历所有可见字符
                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                    
                    // 跳过不可见字符
                    if (!charInfo.isVisible) continue;
                    
                    // 检查当前字符是否在富文本标记范围内
                    if (IsCharacterInRichText(i, textInfo))
                    {
                        int materialIndex = charInfo.materialReferenceIndex;
                        int vertexIndex = charInfo.vertexIndex;
                        
                        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
                        
                        // 计算波浪偏移
                        float offset = Mathf.Sin((Time.time * floatSpeed) + (i * 0.5f)) * floatAmplitude;
                        
                        // 应用偏移到所有4个顶点（每个字符有4个顶点）
                        vertices[vertexIndex + 0].y += offset;
                        vertices[vertexIndex + 1].y += offset;
                        vertices[vertexIndex + 2].y += offset;
                        vertices[vertexIndex + 3].y += offset;
                    }
                }
                
                // 更新顶点数据
                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }
                
                yield return null;
            }
        }
        
        /// <summary>
        /// 检查字符是否在富文本标记范围内
        /// </summary>
        private bool IsCharacterInRichText(int characterIndex, TMP_TextInfo textInfo)
        {
            // 由于富文本标签会被TMP处理，我们需要检查原始文本位置
            // 这里简化处理：检查字符是否被标记（可以通过样式检测）
            if (characterIndex >= textInfo.characterCount) return false;
            
            TMP_CharacterInfo charInfo = textInfo.characterInfo[characterIndex];
            
            // 检测是否有特殊样式（粗体、斜体等）
            bool hasBold = (charInfo.style & FontStyles.Bold) == FontStyles.Bold;
            bool hasItalic = (charInfo.style & FontStyles.Italic) == FontStyles.Italic;
            bool hasUnderline = (charInfo.style & FontStyles.Underline) == FontStyles.Underline;
            
            return hasBold || hasItalic || hasUnderline;
        }

        /// <summary>
        /// 判断是否为标点符号
        /// </summary>
        private bool IsPunctuation(char c)
        {
            return c == '。' || c == '！' || c == '？' || c == '，' || c == '、' || 
                   c == '.' || c == '!' || c == '?' || c == ',' || c == ';' || c == ':';
        }

        /// <summary>
        /// 检查是否正在播放动画
        /// </summary>
        public bool IsTyping => isTyping;

        private void OnDisable()
        {
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }
            
            if (floatingCoroutine != null)
            {
                StopCoroutine(floatingCoroutine);
                floatingCoroutine = null;
            }
            
            isTyping = false;
        }
    }
}
