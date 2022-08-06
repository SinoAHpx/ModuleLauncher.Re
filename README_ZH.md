![brand-logo](https://user-images.githubusercontent.com/34391004/182008531-99bc3d73-d59c-4a7c-9b3d-5a7a17f586ed.png)

> 像飞一样启动Minecraft。

[![NuGet latest version](https://badgen.net/nuget/v/modulelauncher.re/latest)](https://nuget.org/packages/modulelauncher.re)
[![GitHub license](https://badgen.net/github/license/SinoAHpx/ModuleLauncher.Re)](https://github.com/SinoAHpx/ModuleLauncher.Re/blob/main/LICENSE)
[![GitHub stars](https://badgen.net/github/stars/SinoAHpx/ModuleLauncher.Re)](https://github.com/SinoAHpx/ModuleLauncher.Re/stargazers/)
[![Telegram](https://img.shields.io/endpoint?color=blue&url=https://telegram-badge-4mbpu8e0fit4.runkit.sh/?url=https://t.me/axenhaxor)](https://t.me/axenhaxor)

[English](README.md) / 简体中文

## 一览

- 基于 .NET 6 开发，支持跨平台。
- 从最古老到最新的 Minecraft，都可以启动。当然也可以启动它们的 Forge， OptiFine， Liteload 以及 Fabric。
- 灵活轻快拓展性好。
- 可以自定义 OAuth 的客户端id（client id）和重定向链接（redirect url）。
- 支持BmclApi和Mcbbs源，对中国用户下载友好。
- 注释（相对来说）比较全面。
- 你还可以自己去发现更多好康的。



<details>
<summary>康康这个</summary>

- MoudleLauncher 现在不会、以后也不会支持各类 Mod 加载器的自动安装。
- 虽然实现一个内置下载器很简单，但是这会违背灵活性和可拓展性，所以没有这个库没有任何内置的下载引擎。

</details>

## 开始

这里只是简单的示例，很多基础知识不会详细地说明，如果你一头雾水，请去看 [文档](https://sinoahpx.github.io/ModuleLauncher.NET.Documentation/#/) 来获得更详细的说明。

### 安装

+ 用 Nuget 安装：
  + **IDE 的包管理页面**
  + Nuget Package Manger: ```Install-Package ModuleLauncher.Re```
  + .NET CLI: ```dotnet add package ModuleLauncher.Re```
+ 或者把这个仓库下载下来，然后自己编译

### 启动

这里假设你已经有了一个 `.minecraft` 目录，以及不要觉得下面的代码很多，其实真正有效的代码不到10行。

```cs
// 你想要启动的版本
var version = "<version>";

// 用 .minecraft 的路径来初始化一个 MinecraftResolver 实例
var minecraftResolver = new MinecraftResolver(rootPath);

// 用 Minecraft id (version) 来获取一个 MinecraftEntry 实例
// MinecraftEntry 差不多是这个库里最重要的东西
var minecraft = minecraftResolver.GetMinecraft(version);

// 用方法链的方式启动 Minecraft
// 除了要启动的版本名称之外，启动 Minecraft 只需要两个值
// player: 离线玩家的用户名
// (注意这里是一个 string 对象被隐式转换成了 AuthenticateResult 对象)
// java: java 可执行文件的路径
var process = await minecraft.WithAuthentication("<player>")
    .WithJava(@"<java>")
    .LaunchAsync();
```

启动方法 `LaunchAsync` 返回的是一个 `Process` 实例，可以对它进行一些操作。

```cs
// 读 Minecraft 的启动输出信息
// 这里的 ReadOutputLine 和 IsNullOrEmpty 是 Manganese 这个库提供的拓展方法
while (!process.ReadOutputLine().IsNullOrEmpty())
{
    Console.WriteLine(process.ReadOutputLine());
}
```

## 贡献

### 贡献代码

- 你可以 fork 这个仓库然后提交 Pull Request 到 `new` 分支。注意一定得是 `new` 分支。
- 开一个 [issue](https://github.com/SinoAHpx/ModuleLauncher.Re/issues)。

### 修订文档

觉得文档不太行？

- 给 [文档仓库](https://github.com/SinoAHpx/ModuleLauncher.NET.Documentation) 提交 PR。
- 开一个 [issue](https://github.com/SinoAHpx/ModuleLauncher.NET.Documentation/issues)。
- 提供文档翻译。

## 致谢

- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)： 非常强大的JSON操作库。
- [Flurl.Http](https://github.com/tmenier/Flurl)：这个 HTTP 库是真的丝滑。
- [Manganese](https://github.com/SinoAHpx/Manganese)：我的破玩意。
- [JetBrains](https://www.jetbrains.com/)：提供了免费的 IDE。
- [Google](https://google.com) 和 [Stack Overflow](https://stackoverflow.com).
