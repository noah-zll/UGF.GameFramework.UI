# UGF GameFramework UI Extension Package

## 概述

UGF.GameFramework.UI 是基于 Unity GameFramework 的 UI 扩展包，提供了组件绑定、数据集成和代码生成等功能，旨在简化 Unity UI 开发流程。

## 特性

- **UIFormBase 基类**: 扩展 GameFramework 的 UIFormLogic，提供更丰富的功能
- **组件自动绑定**: 通过 UIComponentBinder 实现组件的自动绑定和管理
- **配置管理**: 提供 UIConfigManager 统一管理 UI 相关配置
- **扩展方法**: 丰富的 UI 操作扩展方法，简化常用操作
- **代码生成**: 自动生成 UI 窗体代码，提高开发效率

## 安装

1. 将此包放置在项目的 `Packages` 目录下
2. 或通过 Unity Package Manager 添加本地包

## 依赖

- Unity 2022.3 或更高版本
- UGF.GameFramework
- UGF.GameFramework.Data

## 快速开始

### 1. 创建 UI 窗体

```csharp
using UGF.GameFramework.UI;

public class MainMenuForm : UIFormBase
{
    protected override void OnUIFormInit(object userData)
    {
        // 初始化逻辑
    }
    
    protected override void OnUIFormOpen(object userData)
    {
        // 打开逻辑
    }
}
```

### 2. 使用组件绑定

在 UI 预制体中添加 `UIComponentBinder` 组件到需要绑定的 UI 元素上：

```csharp
// 在 UIFormBase 子类中使用绑定的组件
var startButton = GetComponent<Button>("startButton");
startButton.SetOnClick(OnStartButtonClick);
```

### 3. 配置管理

```csharp
// 添加 UI 窗体配置
var configManager = UIExtensions.GetUIConfigManager();
var config = new UIFormConfig("MainMenu", "UI/MainMenuForm");
configManager.AddUIFormConfig(config);

// 使用配置打开窗体
GameEntry.UI.OpenUIFormWithConfig("MainMenu");
```

## 核心组件

### UIFormBase

扩展的 UI 窗体基类，提供：
- 自动组件绑定
- 配置管理
- 生命周期方法
- 便捷的组件访问

### UIComponentBinder

组件绑定器，用于：
- 标记需要绑定的 UI 组件
- 自动检测组件类型
- 生成变量名和路径
- 支持事件绑定配置

### UIConfigManager

配置管理器，提供：
- UI 窗体配置管理
- UI 组配置管理
- 全局配置管理
- 配置的加载和保存

### UIExtensions

扩展方法集合，包含：
- UI 组件的便捷操作方法
- 事件绑定的简化方法
- 安全的组件访问方法

## 开发阶段

当前包采用分阶段开发模式：

- **阶段1**: 核心基础架构 ✅
- **阶段2**: 组件绑定系统
- **阶段3**: 数据集成
- **阶段4**: 代码生成工具
- **阶段5**: 编辑器工具
- **阶段6**: 测试与示例

## 许可证

本项目采用 MIT 许可证。

## 贡献

欢迎提交 Issue 和 Pull Request 来改进此包。

## 支持

如有问题，请在项目的 Issue 页面提交。