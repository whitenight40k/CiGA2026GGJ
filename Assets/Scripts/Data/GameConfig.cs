using UnityEngine;

namespace MaskGame.Data
{
    /// <summary>
    /// 游戏配置数据 - 存储游戏全局参数
    /// </summary>
    [System.Serializable]
    public class GameConfig
    {
        [Header("时间设置 - 每天递减")]
        [Tooltip("每天的决策时间（秒）")]
        public float[] decisionTimePerDay = new float[] { 10f, 7f, 5f }; // Day1:10秒, Day2:7秒, Day3:5秒

        [Header("关卡设置")]
        [Tooltip("总天数/关卡数")]
        public int totalDays = 3;
        
        [Tooltip("每天的对话数量")]
        public int[] encountersPerDay = new int[] { 3, 4, 5 }; // Day1:3人, Day2:4人, Day3:5人

        [Header("生命值设置 - 固定4条血")]
        [Tooltip("所有天数的血量都是4条")]
        public int fixedHealth = 4;
        
        [Tooltip("选错或超时扣除的血量")]
        public int batteryPenalty = 1;
        
        /// <summary>
        /// 获取指定天的决策时间
        /// </summary>
        public float GetDecisionTime(int day)
        {
            int dayIndex = day - 1;
            if (dayIndex >= 0 && dayIndex < decisionTimePerDay.Length)
            {
                return decisionTimePerDay[dayIndex];
            }
            return 10f; // 默认10秒
        }
    }
}