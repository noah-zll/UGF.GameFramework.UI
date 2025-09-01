# UGF UI设计器系统

## 概述

UGF UI设计器系统是一个强大的Unity编辑器工具，用于自动化UI开发流程。它提供了组件绑定、代码生成和Prefab生成等功能，采用简化的工作流程，大大提高了UI开发效率。

## 主要功能

### 1. 核心组件

- **UIDesigner**: 主要的UI设计组件，用于配置UI信息和管理组件绑定
- **UIComponentBinder**: 组件标记脚本，用于标识需要绑定的UI组件
- **SimpleUIWorkflow**: 简化的保存工作流程，一键完成代码生成和Prefab创建

### 2. 用户界面增强

- **UIDesignerSettings**: 设置面板，允许用户自定义各种参数
- **UIDesignerContextMenu**: 右键菜单支持，快速启动设计模式

## 使用指南

### 基本使用流程

1. **创建UI对象**
   ```csharp
   // 在场景中创建一个GameObject
   GameObject uiObject = new GameObject("MyUI");
   
   // 添加UIDesigner组件
   UIDesigner designer = uiObject.AddComponent<UIDesigner>();
   ```

2. **配置UI设计器**
   ```csharp
   designer.uiName = "MyUI";
   designer.namespaceName = "MyProject.UI";
   designer.outputPath = "Assets/Scripts/UI";
   ```

3. **添加UI组件并标记绑定**
   ```csharp
   // 创建Button
   GameObject buttonGO = new GameObject("ConfirmButton");
   buttonGO.transform.SetParent(uiObject.transform);
   buttonGO.AddComponent<Button>();
   buttonGO.AddComponent<UIComponentBinder>(); // 标记为需要绑定
   
   // 创建Text
   GameObject textGO = new GameObject("TitleText");
   textGO.transform.SetParent(uiObject.transform);
   textGO.AddComponent<Text>();
   textGO.AddComponent<UIComponentBinder>(); // 标记为需要绑定
   ```

4. **保存UI设计**
   - 在Inspector中点击"保存"按钮
   - 或者通过代码调用：
   ```csharp
   SimpleUIWorkflow.SaveAsPrefab(designer, "Assets/Prefabs/MyUI.prefab");
   ```

### 生成的代码结构

系统会生成两个分部类文件：

1. **组件绑定类** (`MyUI.Binding.cs`)
   ```csharp
   public partial class MyUI : UIFormBase
   {
       [SerializeField] private Button m_ConfirmButton;
       [SerializeField] private Text m_TitleText;
       
       protected override void BindComponents()
       {
           m_ConfirmButton = GetComponent<Button>("ConfirmButton");
           m_TitleText = GetComponent<Text>("TitleText");
       }
   }
   ```

2. **业务逻辑类** (`MyUI.cs`)
   ```csharp
   public partial class MyUI : UIFormBase
   {
       protected override void OnInit()
       {
           base.OnInit();
           // 在这里添加初始化逻辑
       }
       
       protected override void OnOpen()
       {
           base.OnOpen();
           // 在这里添加打开逻辑
       }
   }
   ```

## 配置选项

### 通过Project Settings配置

1. 打开 `Edit > Project Settings`
2. 找到 `UGF UI Designer` 面板
3. 配置以下选项：

#### 代码生成设置
- **启用代码生成**: 是否生成组件绑定代码
- **代码模板路径**: 自定义代码模板文件路径
- **使用分部类**: 是否使用分部类结构

#### Prefab生成设置
- **Prefab输出路径**: Prefab文件的默认保存路径
- **自动刷新资源**: 生成后是否自动刷新AssetDatabase
- **自动选中Prefab**: 生成后是否自动选中Prefab

#### 用户界面设置
- **显示详细日志**: 是否在Console中显示详细的执行日志
- **显示完成通知**: 是否显示完成对话框

## 测试系统

### 运行测试

1. **通过菜单运行**
   - 选择 `UGF > UI Designer > Run Tests`
   - 在测试运行器窗口中选择要运行的测试

2. **测试类型**
   - **单元测试**: 测试各个组件的基本功能
   - **集成测试**: 测试整个系统的端到端功能
   - **性能测试**: 测试系统在不同负载下的性能表现

### 创建自定义测试

```csharp
[Test]
public void TestCustomFeature()
{
    // 创建测试对象
    var testUI = CreateTestUI("CustomTestUI", 10);
    var designer = testUI.GetComponent<UIDesigner>();
    
    // 执行测试逻辑
    try
    {
        SimpleUIWorkflow.SaveAsPrefab(designer, "Assets/Test/CustomTestUI.prefab");
        
        // 验证结果
        Assert.IsTrue(File.Exists("Assets/Test/CustomTestUI.prefab"), "Prefab应该被创建");
    }
    catch (System.Exception ex)
    {
        Assert.Fail($"工作流程执行失败: {ex.Message}");
    }
}
```

## 故障排除

### 常见问题

1. **代码生成失败**
   - 检查输出路径是否存在
   - 确认UI名称和命名空间是否有效
   - 查看Console中的错误信息

2. **编译超时**
   - 增加编译超时时间设置
   - 检查是否有编译错误
   - 确认项目没有其他编译问题

3. **Prefab生成失败**
   - 检查Prefab输出路径是否存在
   - 确认GameObject结构是否正确
   - 查看是否有组件绑定错误

4. **组件绑定问题**
   - 确认UIComponentBinder已添加到需要绑定的对象上
   - 检查组件名称是否符合命名规范
   - 验证组件类型是否受支持

### 调试技巧

1. **启用详细日志**
   ```csharp
   var settings = UIDesignerSettings.Instance;
   settings.enableVerboseLogging = true;
   ```

2. **使用进度窗口**
   - 启用进度窗口可以看到详细的执行步骤
   - 观察每个阶段的状态和消息

3. **检查生成的代码**
   - 查看生成的绑定代码是否正确
   - 确认组件引用是否有效

## 扩展开发

### 自定义代码模板

1. 创建自定义模板文件
2. 在设置中指定模板路径
3. 使用占位符定义动态内容：
   - `{UI_NAME}`: UI类名
   - `{NAMESPACE}`: 命名空间
   - `{COMPONENTS}`: 组件声明
   - `{BINDINGS}`: 组件绑定代码

### 添加自定义组件支持

```csharp
public class CustomComponentBinder : UIComponentBinder
{
    public override bool IsValidComponent(Component component)
    {
        // 自定义组件验证逻辑
        return component is MyCustomComponent;
    }
    
    public override string GetBindingCode(Component component)
    {
        // 自定义绑定代码生成
        return $"m_{component.name} = GetComponent<MyCustomComponent>(\"{component.name}\");";
    }
}
```

## 版本历史

### v2.0.0 (当前版本)
- 重新设计的保存工作流程
- 新的进度显示窗口
- 完整的设置系统
- 全面的测试覆盖
- 性能优化和稳定性改进

### v1.0.0
- 基础的UI设计器功能
- 简单的代码生成
- 基本的Prefab生成

## 许可证

本项目遵循MIT许可证。详情请参阅LICENSE文件。

## 贡献

欢迎提交Issue和Pull Request来改进这个项目。在贡献代码之前，请确保：

1. 运行所有测试并确保通过
2. 遵循现有的代码风格
3. 添加适当的测试覆盖
4. 更新相关文档

## 支持

如果您在使用过程中遇到问题，请：

1. 查看本文档的故障排除部分
2. 运行测试以确认系统状态
3. 在GitHub上提交Issue
4. 提供详细的错误信息和重现步骤