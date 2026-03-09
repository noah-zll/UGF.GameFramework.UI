# UI 功能扩展实现方案

本文档详细说明如何实现代码生成器的事件扩展（Point 1）和 UI 组的自动化管理（Point 3），并对动画系统（Point 2）提出建议。

## 1. 代码生成器事件扩展实现

当前 `UICodeGenerator.cs` 仅支持 `Button` 组件。我们需要修改它以支持 `Toggle`, `Slider`, `InputField`, `Dropdown` 等组件。

### 1.1 修改 `GenerateBindingClass` 方法

在 `UICodeGenerator.cs` 中找到 `GenerateBindingClass` 方法内的 `InitializeEvents` 部分，扩展 `foreach` 循环以处理不同类型的组件。

```csharp
// 原有代码：
// if (binding.IsValid() && binding.ComponentType == "Button") { ... }

// 修改后的代码：
foreach (var binding in bindings)
{
    if (!binding.IsValid()) continue;

    string methodName = char.ToUpper(binding.ComponentName[0]) + binding.ComponentName.Substring(1);
    
    switch (binding.ComponentType)
    {
        case "Button":
            sb.AppendLine($"{indent}        // 绑定按钮事件: {binding.ComponentName}");
            sb.AppendLine($"{indent}        {binding.ComponentName}.onClick.AddListener(On{methodName}Click);");
            break;

        case "Toggle":
            sb.AppendLine($"{indent}        // 绑定Toggle事件: {binding.ComponentName}");
            sb.AppendLine($"{indent}        {binding.ComponentName}.onValueChanged.AddListener(On{methodName}ValueChanged);");
            break;

        case "Slider":
            sb.AppendLine($"{indent}        // 绑定Slider事件: {binding.ComponentName}");
            sb.AppendLine($"{indent}        {binding.ComponentName}.onValueChanged.AddListener(On{methodName}ValueChanged);");
            break;

        case "InputField":
        case "TMP_InputField":
            sb.AppendLine($"{indent}        // 绑定输入框事件: {binding.ComponentName}");
            sb.AppendLine($"{indent}        {binding.ComponentName}.onValueChanged.AddListener(On{methodName}ValueChanged);");
            sb.AppendLine($"{indent}        {binding.ComponentName}.onEndEdit.AddListener(On{methodName}EndEdit);");
            break;

        case "Dropdown":
        case "TMP_Dropdown":
            sb.AppendLine($"{indent}        // 绑定下拉框事件: {binding.ComponentName}");
            sb.AppendLine($"{indent}        {binding.ComponentName}.onValueChanged.AddListener(On{methodName}ValueChanged);");
            break;
            
        case "ScrollRect":
             sb.AppendLine($"{indent}        // 绑定滚动视图事件: {binding.ComponentName}");
             sb.AppendLine($"{indent}        {binding.ComponentName}.onValueChanged.AddListener(On{methodName}ValueChanged);");
             break;
    }
}
```

### 1.2 修改 `GenerateLogicClass` 方法

同样地，在 `GenerateLogicClass` 中生成对应的回调方法存根。

```csharp
// 在生成按钮事件处理方法循环中扩展：

foreach (var binding in bindings)
{
    if (!binding.IsValid()) continue;
    
    string methodName = char.ToUpper(binding.ComponentName[0]) + binding.ComponentName.Substring(1);
    string comment = $"{indent}    /// <summary>\n{indent}    /// {binding.ComponentName} {binding.ComponentType} 事件\n{indent}    /// </summary>";

    switch (binding.ComponentType)
    {
        case "Button":
            sb.AppendLine(comment);
            sb.AppendLine($"{indent}    private void On{methodName}Click()");
            sb.AppendLine($"{indent}    {{");
            sb.AppendLine($"{indent}        // TODO: Button Click Logic");
            sb.AppendLine($"{indent}    }}");
            break;

        case "Toggle":
            sb.AppendLine(comment);
            sb.AppendLine($"{indent}    private void On{methodName}ValueChanged(bool isOn)");
            sb.AppendLine($"{indent}    {{");
            sb.AppendLine($"{indent}        // TODO: Toggle ValueChanged Logic");
            sb.AppendLine($"{indent}    }}");
            break;

        case "Slider":
            sb.AppendLine(comment);
            sb.AppendLine($"{indent}    private void On{methodName}ValueChanged(float value)");
            sb.AppendLine($"{indent}    {{");
            sb.AppendLine($"{indent}        // TODO: Slider ValueChanged Logic");
            sb.AppendLine($"{indent}    }}");
            break;

        case "InputField":
        case "TMP_InputField":
            sb.AppendLine(comment);
            sb.AppendLine($"{indent}    private void On{methodName}ValueChanged(string text)");
            sb.AppendLine($"{indent}    {{");
            sb.AppendLine($"{indent}    }}");
            sb.AppendLine();
            sb.AppendLine($"{indent}    private void On{methodName}EndEdit(string text)");
            sb.AppendLine($"{indent}    {{");
            sb.AppendLine($"{indent}    }}");
            break;

        case "Dropdown":
        case "TMP_Dropdown":
            sb.AppendLine(comment);
            sb.AppendLine($"{indent}    private void On{methodName}ValueChanged(int index)");
            sb.AppendLine($"{indent}    {{");
            sb.AppendLine($"{indent}    }}");
            break;
            
        case "ScrollRect":
            sb.AppendLine(comment);
            sb.AppendLine($"{indent}    private void On{methodName}ValueChanged(Vector2 position)");
            sb.AppendLine($"{indent}    {{");
            sb.AppendLine($"{indent}    }}");
            break;
    }
    sb.AppendLine();
}
```

> **注意**：还需要同步修改 `UpdateLogicClassEvents` 和 `AddMissingEventHandlers` 方法，逻辑与上述类似，通过正则表达式或字符串分析检查方法是否存在，不存在则追加。

---

## 2. 动画系统建议 (针对问题 2)

**结论：不需要单独做一个类似 DOTween 的动画库包。**

GameFramework 的设计理念是轻量和模块化。对于 UI 扩展包：

1.  **基础需求（淡入淡出/缩放）**：可以直接在 `UIFormBase` 中集成基于 Coroutine 或简单的 Update 插值的逻辑。这不需要引入庞大的第三方库。
2.  **高级需求（复杂序列动画）**：如果项目已经使用了 DOTween，直接在生成的 `OnOpen` / `OnClose` 中调用 DOTween 即可。

**建议实现方案：**

在 `UIFormBase` 中增加虚方法，供子类重写或统一处理。

```csharp
// UIFormBase.cs 增加部分

protected virtual float FadeTime => Config != null ? Config.FadeInDuration : 0.3f;

protected override void OnOpen(object userData)
{
    base.OnOpen(userData);
    
    // 自动播放进场动画
    StopAllCoroutines();
    StartCoroutine(PlayFadeIn(FadeTime));
}

protected override void OnClose(bool isShutdown, object userData)
{
    // 在这里拦截关闭事件可能比较复杂，因为 GF 的 CloseUIForm 是立即销毁/回收。
    // 通常做法是：手动调用 CloseWithAnimation() -> 播放动画 -> 动画结束回调 CloseUIForm()
    base.OnClose(isShutdown, userData);
}

// 简单的原生淡入实现
private IEnumerator PlayFadeIn(float duration)
{
    CanvasGroup group = GetComponent<CanvasGroup>();
    if (group == null) group = gameObject.AddComponent<CanvasGroup>();
    
    group.alpha = 0;
    float timer = 0;
    while (timer < duration)
    {
        timer += Time.deltaTime;
        group.alpha = timer / duration;
        yield return null;
    }
    group.alpha = 1;
}
```

如果需要支持 DOTween，可以使用宏定义 `ENABLE_DOTWEEN` 来切换实现。

---

## 3. UI 组自动管理实现

当前 `UIConfigManager` 存储了组配置，但没有自动注册到 GameFramework 的 UI 模块中。我们需要一个初始化入口。

### 3.1 扩展 `UIConfigManager`

在 `UIConfigManager` 中添加一个 `InitializeUIGroups` 方法。

```csharp
// UIConfigManager.cs

public void InitializeUIGroups()
{
    UIComponent uiComponent = GameEntry.GetComponent<UIComponent>();
    if (uiComponent == null)
    {
        Log.Error("UIComponent is invalid.");
        return;
    }

    foreach (var groupConfig in m_UIGroupConfigs.Values)
    {
        if (!uiComponent.HasUIGroup(groupConfig.GroupName))
        {
            uiComponent.AddUIGroup(groupConfig.GroupName, groupConfig.Depth);
            Log.Info($"Auto create UI Group: {groupConfig.GroupName}, Depth: {groupConfig.Depth}");
        }
    }
}
```

### 3.2 调用时机

在项目的流程入口（例如 `ProcedurePreload` 或 `GameEntry.Custom` 初始化阶段），加载完 UI 配置表后，立即调用：

```csharp
// 示例调用代码
GameEntry.GetComponent<UIConfigManager>().InitializeUIGroups();
```

这样就不需要手动硬编码 `AddUIGroup("Main", 100)` 这样的代码了，完全由 `UIFormConfig` 或 `UIGroupConfig` 资源决定。

---

## 4. 资源加载与打开接口 (针对问题 4)

**确认**：`UIExtensions.cs` 中已经存在 `OpenUIFormWithConfig` 方法。

```csharp
public static int OpenUIFormWithConfig(this UIComponent uiComponent, string formName, object userData = null)
```

**现状**：已实现。
**建议**：确保 `UIFormConfig` 资源文件被正确加载到 `UIConfigManager` 中，否则该方法会报 "config not found"。这通常结合 Point 3 的初始化流程一起做（先加载所有 Config 资源，再初始化 Group，然后就可以使用此接口了）。
