# UIDesigner 使用示例

这个示例展示了如何使用新的UIDesigner流程来开发UI界面。

## 示例内容

### 文件结构

```
UIDesignerExample/
├── Scripts/
│   ├── MainMenuUI.cs                    # 业务逻辑类（可编辑）
│   └── Generated/
│       └── MainMenuUIBinding.Generated.cs # 组件绑定类（自动生成，不可编辑）
├── Prefabs/
│   └── MainMenuUI.prefab                # UI预制体（包含UIDesigner组件）
└── README.md                            # 本文档
```

### 核心概念

#### 1. UIDesigner 组件
- 添加到UI预制体的根节点
- 配置UI界面名称、命名空间等基本信息
- 管理所有需要绑定的UI组件

#### 2. 组件绑定类（自动生成）
- 文件名：`{UI界面名称}Binding.Generated.cs`
- 包含所有UI组件的属性声明
- 包含自动绑定逻辑
- **重要：此文件由系统自动生成，请勿手动修改**

#### 3. 业务逻辑类（可编辑）
- 文件名：`{UI界面名称}.cs`
- 继承自UIFormBase（通过partial类机制）
- 包含所有业务逻辑代码
- **开发者在此文件中编写具体的UI逻辑**

## 使用步骤

### 第一步：创建UI预制体

1. 在Unity中创建一个新的Canvas
2. 设计你的UI界面布局
3. 在根节点添加`UIDesigner`组件
4. 配置基本信息：
   - UI界面名称：`MainMenuUI`
   - 命名空间：`Game.UI.Examples`

### 第二步：配置组件绑定

在UIDesigner的Inspector中添加需要绑定的组件：

| 组件名称 | 变量名 | 组件类型 | 描述 |
|---------|--------|----------|------|
| 主面板 | mainPanel | RectTransform | 主要的UI容器 |
| 开始游戏按钮 | startGameButton | Button | 开始新游戏 |
| 继续游戏按钮 | continueGameButton | Button | 继续已有游戏 |
| 设置按钮 | settingsButton | Button | 打开设置界面 |
| 退出按钮 | exitGameButton | Button | 退出游戏 |
| 音量滑块 | volumeSlider | Slider | 调节音量 |
| 音量文本 | volumeValueText | Text | 显示音量数值 |
| 版本文本 | versionText | Text | 显示游戏版本 |
| 玩家名称 | playerNameText | Text | 显示玩家名称 |
| 成就图标 | achievementIcons | Image[] | 成就显示 |

### 第三步：生成代码

1. 在UIDesigner的Inspector中点击"生成代码"按钮
2. 系统会自动生成两个文件：
   - `MainMenuUIBinding.Generated.cs`（组件绑定类）
   - `MainMenuUI.cs`（业务逻辑类模板）

### 第四步：编写业务逻辑

在生成的`MainMenuUI.cs`文件中编写具体的业务逻辑：

```csharp
namespace Game.UI.Examples
{
    public partial class MainMenuUI
    {
        /// <summary>
        /// 自定义初始化逻辑
        /// </summary>
        partial void OnInitCustom(object userData)
        {
            // 绑定按钮事件
            startGameButton.onClick.AddListener(OnStartGameClick);
            settingsButton.onClick.AddListener(OnSettingsClick);
            // ... 其他初始化逻辑
        }
        
        private void OnStartGameClick()
        {
            // 开始游戏逻辑
            GameEntry.GetComponent<UIComponent>().OpenUIForm("GameplayUI");
            Close();
        }
        
        // ... 其他业务逻辑方法
    }
}
```

## 代码结构说明

### 组件绑定类的结构

```csharp
// 自动生成的组件绑定类
public partial class MainMenuUI : UIFormBase
{
    // 1. 组件属性声明
    public Button startGameButton { get; private set; }
    public Button settingsButton { get; private set; }
    // ...
    
    // 2. 组件绑定方法
    private void BindComponents()
    {
        startGameButton = transform.Find("StartButton").GetComponent<Button>();
        settingsButton = transform.Find("SettingsButton").GetComponent<Button>();
        // ...
    }
    
    // 3. 重写OnInit方法，自动调用绑定
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        BindComponents();
        OnInitCustom(userData); // 调用自定义初始化
    }
    
    // 4. 声明自定义初始化方法
    partial void OnInitCustom(object userData);
}
```

### 业务逻辑类的结构

```csharp
// 开发者编写的业务逻辑类
public partial class MainMenuUI
{
    // 1. 实现自定义初始化方法
    partial void OnInitCustom(object userData)
    {
        // 绑定事件、初始化状态等
    }
    
    // 2. 重写生命周期方法
    protected override void OnOpen(object userData) { }
    protected override void OnClose(bool isShutdown, object userData) { }
    
    // 3. 事件处理方法
    private void OnStartGameClick() { }
    private void OnSettingsClick() { }
    
    // 4. 其他业务逻辑方法
    private void RefreshUI() { }
    private void PlayAnimation() { }
}
```

## 最佳实践

### 1. 命名规范

- **UI界面名称**：使用PascalCase，如`MainMenuUI`、`InventoryPanel`
- **变量名称**：使用camelCase，如`startButton`、`healthBar`
- **方法名称**：使用PascalCase，如`OnStartGameClick`、`RefreshUI`

### 2. 组件组织

- 使用有意义的GameObject名称
- 保持合理的层级结构
- 将相关组件放在同一个容器下

### 3. 代码分离

- **组件绑定类**：只包含组件声明和绑定逻辑
- **业务逻辑类**：只包含业务逻辑和事件处理
- 避免在组件绑定类中添加自定义代码

### 4. 事件处理

```csharp
// 推荐的事件绑定方式
partial void OnInitCustom(object userData)
{
    // 在初始化时绑定事件
    startButton.onClick.AddListener(OnStartButtonClick);
    volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
}

// 在OnClose中解绑事件（如果需要）
protected override void OnClose(bool isShutdown, object userData)
{
    startButton.onClick.RemoveListener(OnStartButtonClick);
    base.OnClose(isShutdown, userData);
}
```

### 5. 数据传递

```csharp
// 通过userData传递数据
protected override void OnOpen(object userData)
{
    base.OnOpen(userData);
    
    if (userData is MainMenuData data)
    {
        playerNameText.text = data.PlayerName;
        // 使用传入的数据初始化UI
    }
}
```

## 常见问题

### Q: 为什么要使用partial类？
A: partial类允许将一个类的定义分散到多个文件中，这样可以将自动生成的代码和手写的代码完全分离，避免代码冲突。

### Q: 如果UI结构改变了怎么办？
A: 只需要在UIDesigner中重新配置组件绑定，然后重新生成代码即可。业务逻辑类不会被覆盖。

### Q: 可以手动修改生成的绑定类吗？
A: 不建议，因为每次重新生成代码时都会覆盖修改。所有自定义逻辑都应该写在业务逻辑类中。

### Q: 如何处理动态创建的UI元素？
A: 对于动态创建的UI元素，可以在业务逻辑类中手动获取和管理，不需要通过UIDesigner绑定。

## 扩展示例

### 动画处理

```csharp
private void PlayOpenAnimation()
{
    // 使用DOTween播放动画
    mainPanel.transform.localScale = Vector3.zero;
    mainPanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
}
```

### 数据绑定

```csharp
private void RefreshPlayerInfo()
{
    var playerData = GameEntry.GetComponent<DataComponent>().GetPlayerData();
    playerNameText.text = playerData.Name;
    levelText.text = $"Level {playerData.Level}";
}
```

### 本地化支持

```csharp
private void UpdateLocalization()
{
    titleText.text = GameEntry.GetComponent<LocalizationComponent>().GetString("UI.MainMenu.Title");
    startButton.GetComponentInChildren<Text>().text = GameEntry.GetComponent<LocalizationComponent>().GetString("UI.MainMenu.StartGame");
}
```

这个示例展示了UIDesigner的完整使用流程，通过这种方式可以大大提高UI开发的效率和代码质量。