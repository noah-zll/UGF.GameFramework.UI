# UI设计器新工作流程使用指南

## 概述

新的UI设计器工作流程提供了更加直观和高效的UI开发体验，支持从现有UI脚本还原设计状态，以及一键完成整个UI开发流程。

## 主要特性

### 1. 右键菜单快速启动
- 在Inspector面板的任何UI组件上右键
- 选择"设计"选项
- 自动添加UIDesigner组件并还原现有设计

### 2. 智能脚本解析
- 自动检测现有UI脚本
- 解析脚本中的组件变量
- 还原组件绑定关系
- 自动添加UIComponentBinder标记

### 3. 一键保存工作流程
- 生成UI脚本代码
- 自动挂载脚本组件
- 绑定所有组件变量
- 清理设计脚本
- 保存为Prefab或覆盖现有Prefab

## 使用流程

### 方式一：从零开始创建UI

1. **创建UI GameObject**
   ```
   - 在Hierarchy中创建UI -> Canvas
   - 添加所需的UI组件（Button、Image、Text等）
   - 为需要绑定的组件添加UIComponentBinder
   ```

2. **启动设计模式**
   ```
   - 选中根GameObject
   - 在任意UI组件上右键选择"设计"
   - 或使用菜单：GameObject -> UI -> Add UIDesigner
   ```

3. **配置UI设计**
   ```
   - 设置UI界面名称（如：MainMenuUI）
   - 设置命名空间（如：Game.UI）
   - 自动扫描或手动添加组件绑定
   - 设置变量名称和描述
   ```

4. **保存UI设计**
   ```
   - 点击"保存"按钮
   - 系统自动完成所有步骤
   - 查看Console日志确认执行结果
   ```

### 方式二：从现有UI脚本还原设计

1. **准备现有UI**
   ```
   - 确保GameObject已挂载UI脚本
   - 脚本应继承自UIFormBase或符合命名规范
   - 脚本中包含UI组件字段
   ```

2. **启动设计模式**
   ```
   - 在任意UI组件上右键选择"设计"
   - 系统自动检测现有脚本
   - 解析脚本变量并还原绑定关系
   ```

3. **调整和完善**
   ```
   - 检查自动还原的组件绑定
   - 添加新的组件绑定
   - 修改变量名称或描述
   ```

4. **重新保存**
   ```
   - 点击"保存"按钮
   - 更新脚本和Prefab
   ```

## 操作按钮说明

### 主要操作
- **保存**：执行完整的保存工作流程（推荐使用）
- **验证设置**：检查当前配置是否有效

### 辅助操作
- **仅生成代码**：只生成脚本代码，不执行其他步骤
- **清理设计脚本**：手动清理UIDesigner和UIComponentBinder组件

## 自动生成的文件

### 脚本文件
```
Assets/Scripts/UI/
├── MainMenuUI.Generated.cs    # 自动生成的组件绑定类（不可编辑）
└── MainMenuUI.cs               # 业务逻辑类（可编辑）
```

### Prefab文件
```
Assets/UI/Prefabs/
└── MainMenuUI.prefab           # UI预制体
```

## 生成的代码结构

### 组件绑定类（自动生成）
```csharp
// MainMenuUI.Generated.cs
using UnityEngine;
using UnityEngine.UI;
using UGF.GameFramework.UI;

namespace Game.UI
{
    public partial class MainMenuUI : UIFormBase
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Text titleText;
        
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            OnInitCustom(userData);
        }
        
        partial void OnInitCustom(object userData);
    }
}
```

### 业务逻辑类（可编辑）
```csharp
// MainMenuUI.cs
using UnityEngine;
using UGF.GameFramework.UI;

namespace Game.UI
{
    public partial class MainMenuUI
    {
        partial void OnInitCustom(object userData)
        {
            // 在这里编写业务逻辑
            startButton.onClick.AddListener(OnStartButtonClick);
            settingsButton.onClick.AddListener(OnSettingsButtonClick);
        }
        
        private void OnStartButtonClick()
        {
            // 开始游戏逻辑
        }
        
        private void OnSettingsButtonClick()
        {
            // 设置界面逻辑
        }
    }
}
```

## 最佳实践

### 1. 命名规范
- UI界面名称使用PascalCase，以UI结尾（如：MainMenuUI）
- 变量名称使用camelCase（如：startButton）
- 组件类型作为后缀（如：Button -> xxxButton）

### 2. 组件组织
- 将相关UI组件放在同一个父对象下
- 使用有意义的GameObject名称
- 避免过深的层级结构

### 3. 脚本编写
- 只在业务逻辑类中编写代码
- 不要修改自动生成的组件绑定类
- 使用OnInitCustom方法进行初始化

### 4. 版本控制
- 将.Generated.cs文件排除在版本控制之外
- 提交业务逻辑类和Prefab文件
- 团队成员可以重新生成组件绑定类

## 故障排除

### 常见问题

1. **脚本类型未找到**
   - 等待Unity编译完成
   - 检查命名空间和类名是否正确
   - 查看Console是否有编译错误

2. **组件绑定失败**
   - 确认GameObject层级结构正确
   - 检查组件类型是否匹配
   - 验证变量名称是否有效

3. **Prefab保存失败**
   - 确保目标目录存在写权限
   - 检查文件是否被其他程序占用
   - 查看Unity Console的错误信息

### 调试技巧

1. **查看详细日志**
   ```
   - 打开Console窗口
   - 启用"Collapse"和"Clear on Play"
   - 观察每个步骤的执行日志
   ```

2. **验证生成结果**
   ```
   - 检查生成的脚本文件
   - 确认Prefab中的组件绑定
   - 测试UI功能是否正常
   ```

3. **手动清理**
   ```
   - 使用"清理设计脚本"按钮
   - 手动删除UIDesigner和UIComponentBinder
   - 重新开始设计流程
   ```

## 总结

新的UI设计器工作流程大大简化了UI开发过程，提供了从设计到部署的一站式解决方案。通过智能的脚本解析和自动化的保存流程，开发者可以专注于UI逻辑的实现，而不需要关心繁琐的组件绑定和代码生成细节。

建议在实际项目中逐步采用这个新工作流程，并根据团队的具体需求进行适当的定制和优化。