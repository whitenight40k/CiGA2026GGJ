# MASK 游戏开发架构文档

## 📋 项目概述

**项目名称:** MASK - 社交面具选择游戏  
**类型:** 对话选择类游戏  
**Unity版本:** 2021.3+  
**开发日期:** 2026年1月

### 游戏玩法

玩家需要在不同社交场景中选择正确的"社交面具"（人格）来应对朋友的对话。选择正确进入下一段对话，选择错误扣血。血量归零游戏失败。

---

## 🏗️ 架构设计

### 核心设计模式

1. **单例模式** - GameManager
2. **观察者模式** - UIManager 订阅 GameManager 的 UnityEvent
3. **数据驱动设计** - ScriptableObject 存储对话数据
4. **MVC概念** - GameManager(Controller) + UIManager(View) + EncounterData(Model)

### 项目结构

```
Assets/
├── Scripts/
│   ├── Data/                    # 数据层
│   │   ├── MaskType.cs         # 面具类型枚举
│   │   ├── GameConfig.cs       # 游戏配置类
│   │   └── EncounterData.cs    # 对话数据 ScriptableObject
│   │
│   ├── Managers/               # 管理层
│   │   └── GameManager.cs      # 核心游戏逻辑管理器
│   │
│   └── UI/                     # UI层
│       ├── UIManager.cs        # UI更新管理器
│       ├── MaskOptionUI.cs     # 面具选项悬停提示组件
│       └── GameOverUI.cs       # 游戏结束界面管理器
│
├── Data/Encounters/            # 对话数据资源
│   └── *.asset                 # EncounterData 实例
│
├── Scenes/
│   ├── Main.unity             # 主游戏场景
│   └── GameOver.unity         # 游戏结束场景
│
└── Art/                       # 美术资源
    └── Health/                # 生命值图标
```

---

## 📦 核心组件说明

### 1. GameManager (游戏管理器)

**职责：** 控制游戏流程、天数、难度、对话切换

**关键字段：**
```csharp
- currentDay: int                    // 当前天数 (1-3)
- currentEncounterIndex: int         // 当前对话索引
- socialBattery: int                 // 剩余血量
- remainingTime: float               // 决策倒计时
- encounterPool: List<EncounterData> // 对话数据池
```

**核心方法：**
```csharp
- InitializeGame()              // 初始化游戏
- LoadNextEncounter()           // 加载下一个对话
- SelectMask(MaskType)          // 处理玩家选择
- ProcessAnswer()               // 验证答案并更新状态
- CompleteDay()                 // 完成当天，进入下一天
- GameOver() / GameWin()        // 游戏结束/胜利
```

**事件系统：**
```csharp
- OnDayChanged(int)             // 天数变化
- OnBatteryChanged(int)         // 血量变化
- OnTimeChanged(float)          // 倒计时更新
- OnNewEncounter(EncounterData) // 新对话加载
- OnAnswerResult(bool)          // 答案反馈
- OnGameOver()                  // 游戏结束
- OnDayComplete()               // 完成一天
```

### 2. UIManager (UI管理器)

**职责：** 监听 GameManager 事件，更新所有UI元素

**UI组件引用：**
```csharp
- dayText: TextMeshProUGUI           // 天数显示 "Day 1"
- batteryIcons: Image[]              // 生命值图标数组
- dialogueText: TextMeshProUGUI      // 对话文本
- friendGroupText: TextMeshProUGUI   // 朋友分组标签
- timeSlashText: TextMeshProUGUI     // 时间斜杠显示 "//////"
- maskButtons: Button[]              // 面具按钮数组
- maskOptionUIs: MaskOptionUI[]      // 面具选项UI组件
```

**核心方法：**
```csharp
- UpdateDay(int)                    // 更新天数显示
- UpdateBattery(int)                // 更新血量图标
- UpdateTime(float)                 // 更新时间斜杠
- DisplayEncounter(EncounterData)   // 显示对话和选项
- ShowAnswerFeedback(bool)          // 显示答案反馈（屏幕抖动）
```

### 3. MaskOptionUI (面具选项UI)

**职责：** 实现鼠标悬停显示选项文本提示

**接口实现：**
- `IPointerEnterHandler` - 鼠标进入事件
- `IPointerExitHandler` - 鼠标离开事件

**关键字段：**
```csharp
- optionText: string               // 选项描述文本
- tooltipPanel: GameObject         // 提示框面板
- tooltipText: TextMeshProUGUI     // 提示框文本
- tooltipOffset: Vector2           // 提示框偏移量
```

**使用方法：**
1. 在每个 Image_mask 上添加此组件
2. 绑定提示框UI引用
3. 通过 `SetOptionText()` 设置选项文本

---

## 📊 数据结构

### EncounterData (对话数据)

**类型：** ScriptableObject  
**创建路径：** `Assets/Create/Mask Game/Encounter Data`

**字段说明：**
```csharp
[Header("对话内容")]
public string dialogueText;          // 朋友说的话（10-20字）

[Header("选项文本")]
public string[] optionTexts;         // 4个选项的文本描述
                                     // optionTexts[0] = Mask1 的描述
                                     // optionTexts[1] = Mask2 的描述
                                     // optionTexts[2] = Mask3 的描述
                                     // optionTexts[3] = Mask4 的描述

[Header("朋友信息")]
public string friendGroup;           // 朋友分组（如：亲密朋友、同事、长辈）

[Header("正确答案")]
public MaskType correctMask;         // 正确的面具类型（Mask1-4）

[Header("反馈文本（可选）")]
public string successFeedback;       // 选对后的短反馈
public string failureFeedback;       // 选错后的短反馈
```

### GameConfig (游戏配置)

**类型：** Serializable Class

**字段说明：**
```csharp
[Header("时间设置")]
public float initialDecisionTime = 5f;  // 决策时间（秒）

[Header("关卡设置")]
public int totalDays = 3;                        // 总天数/关卡数
public int[] encountersPerDay = {3, 4, 5};      // 每天对话数
                                                  // Day1: 3人
                                                  // Day2: 4人
                                                  // Day3: 5人

[Header("生命值设置")]
public int[] healthPerDay = {6, 5, 4};          // 每天血量（斜杠数）
                                                  // Day1: 6条血
                                                  // Day2: 5条血
                                                  // Day3: 4条血
public int batteryPenalty = 1;                   // 选错/超时扣血量
```

### MaskType (面具类型枚举)

```csharp
public enum MaskType
{
    Mask1 = 0,  // 面具①
    Mask2 = 1,  // 面具②
    Mask3 = 2,  // 面具③
    Mask4 = 3   // 面具④
}
```

---

## 🎮 游戏流程图

```
游戏开始
    ↓
初始化 Day 1 (3人, 6条血)
    ↓
加载对话 → 显示选项文本（悬停）→ 玩家选择
    ↓                                    ↓
倒计时 (5秒)                        选择正确？
    ↓                                 ↙      ↘
超时 → 扣1血                        是        否 → 扣1血
    ↓                                ↓          ↓
血量>0？                           下一对话    血量>0？
  ↙    ↘                              ↓          ↙    ↘
否      是                        完成3人？      否    是
 ↓      ↓                          ↙    ↘       ↓    ↓
失败  继续                        是      否    失败  继续
       ↓                          ↓      ↓
       ↓                      进入Day 2  继续
       ↓                       (4人,5血)
       ↓                           ↓
       ↓                       重复流程
       ↓                           ↓
       ↓                      进入Day 3
       ↓                       (5人,4血)
       ↓                           ↓
       ↓                       重复流程
       ↓                           ↓
       └───────────────────→ 完成Day 3 → 胜利
```

---

## 🔧 开发指南

### 创建新对话数据

1. 在 Unity 中右键 → `Create/Mask Game/Encounter Data`
2. 填写以下字段：
   - `Dialogue Text`: 朋友说的话
   - `Option Texts`: 4个选项的描述（按 Mask1-4 顺序）
   - `Friend Group`: 朋友分组标签
   - `Correct Mask`: 选择正确答案
3. 保存到 `Assets/Data/Encounters/` 文件夹
4. 在 GameManager 的 Encounter Pool 中添加此数据

### 修改游戏难度

在 GameManager 的 GameConfig 中调整：
- `initialDecisionTime`: 改变决策时间
- `encountersPerDay[]`: 修改每天对话数量
- `healthPerDay[]`: 调整每天初始血量
- `batteryPenalty`: 改变扣血惩罚

### 添加新功能

**扩展 GameManager:**
1. 添加新的私有字段存储状态
2. 创建新的 UnityEvent 通知 UI
3. 在适当的方法中触发事件

**扩展 UIManager:**
1. 添加新的 UI 组件引用
2. 订阅 GameManager 的新事件
3. 创建更新方法处理 UI 显示

---

## 🐛 常见问题排查

### 问题：对话不显示
- 检查 GameManager 的 Encounter Pool 是否为空
- 检查 EncounterData 是否正确配置
- 检查 UIManager 的 dialogueText 是否绑定

### 问题：时间斜杠不更新
- 检查 UIManager 的 timeSlashText 是否绑定
- 检查 maxSlashes 是否与 healthPerDay 匹配
- 确认 OnTimeChanged 事件已订阅

### 问题：选项提示不显示
- 检查 Image_mask 上是否添加 MaskOptionUI 组件
- 检查 tooltipPanel 和 tooltipText 是否绑定
- 确认 UIManager 已调用 SetOptionText()

### 问题：天数不递增
- 检查 encountersPerDay 数组配置
- 确认 currentEncounterIndex 正确计数
- 检查 OnDayChanged 事件是否触发

---

## 📝 命名规范

### C# 脚本
- 类名: PascalCase (GameManager, UIManager)
- 方法: PascalCase (LoadNextEncounter, ProcessAnswer)
- 私有字段: camelCase (currentDay, socialBattery)
- 公开属性: PascalCase (CurrentDay, SocialBattery)
- 事件: OnXxxYyy (OnDayChanged, OnBatteryChanged)

### Unity 资源
- ScriptableObject: PascalCase (EncounterData)
- 场景: PascalCase (Main, GameOver)
- UI对象: PascalCase_description (Image_mask, Text_dialogue)

---

## 🚀 未来扩展建议

### 功能扩展
1. **多样化反馈**
   - 添加音效系统
   - 丰富屏幕特效（粒子、闪烁）
   - 角色立绘/头像

2. **关卡设计**
   - 更多天数/关卡
   - 难度递增（时间缩短）
   - 特殊事件对话

3. **数据统计**
   - 详细的答题统计
   - 排行榜系统
   - 成就系统

4. **本地化**
   - 多语言支持
   - 文本外部化管理

### 技术优化
1. **对象池**
   - 复用 UI 元素
   - 减少 GC 压力

2. **数据管理**
   - JSON/CSV 外部数据
   - 热更新支持

3. **测试框架**
   - 单元测试
   - 自动化测试场景

---

## 📖 参考资源

### Unity 文档
- UnityEvents: https://docs.unity3d.com/Manual/UnityEvents.html
- ScriptableObjects: https://docs.unity3d.com/Manual/class-ScriptableObject.html
- TextMeshPro: https://docs.unity3d.com/Packages/com.unity.textmeshpro@latest

### 设计模式
- 单例模式: 确保 GameManager 唯一实例
- 观察者模式: 事件驱动的 UI 更新
- 策略模式: 可用于不同难度配置

---

**文档版本:** 1.0  
**最后更新:** 2026年1月30日  
**维护者:** MASK 开发团队
