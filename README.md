![ModuleLauncher.png](https://i.loli.net/2020/10/15/Pv5sotRSDkIGOwT.png)

![GitHub](https://img.shields.io/github/license/AHpxChina/ModuleLauncher.RE?logo=github&style=for-the-badge)![GitHub stars](https://img.shields.io/github/stars/AHpxChina/ModuleLauncher.RE?logo=github&style=for-the-badge)![GitHub repo size](https://img.shields.io/github/repo-size/AHpxChina/ModuleLauncher.RE?logo=github&style=for-the-badge)

ModuleLauncher.Re 使得开发Minecraft启动器变得高效而优雅。
---
### 关于

+ 基于.NetFramework 4.7.2和C#
+ 主要开发者是AHpx
+ 支持几乎所有Minecraft版本和加载器
  + 至少Minecraft 1.6.4的加载器到1.16.2的加载器测试通过

### 安装

+ 将ModuleLauncher.Re源码克隆到本地并进行编译成dll以导入引用

+ 使用[Nuget][https://www.nuget.org/packages/ModuleLauncher.Re/2.6.0]

  ```
  Install-Package ModuleLauncher.Re -Version 2.6.0
  ```

### 功能

|        Function        |         Description         |
| :--------------------: | :-------------------------: |
| YggdrasilAuthenticator |     认证Mojang账户相关      |
|  OfflineAuthenticator  |        离线认证模块         |
|                        |                             |
|    LibrariesLocator    |    获取运行库和Natives库    |
|     AssetsLocator      |       获取所有Assets        |
|  MinecraftDownloader   |  获取Minecraft下载相关信息  |
|    ForgeDownloader     |    获取Forge下载相关信息    |
|   OptifineDownloader   |  获取Optifine下载相关信息   |
|     JreDownloader      |     获取Jre下载相关信息     |
|    MinecraftLocator    |    操作本地Minecraft对象    |
|       McbbsNews        |   获取来自mcbbs首页的新闻   |
|       MojangApi        |      MojangApi相关操作      |
|                        |                             |
|   LauncherArguments    | 获取Minecraft的启动相关参数 |
|      LauncherCore      |        启动Minecraft        |
|                        |                             |
|       HttpHelper       |        网络相关方法         |
|      StringHelper      |       字符串相关方法        |

### 关于示例

在ModuleLauncher.Re 2.1~~(当然这在2.5版本并不存在)~~~~(而且2.6版本也不存在)~~版本的解决方案中包含了一个很棒的示例程序，你可以通过这个示例了解本库的用法。以下是一些预览图片。

### 简单用法

##### 获取所有本地Minecraft版本

```c#
new MinecraftLocator("${.minecraft}").GetMinecraftFileEntities();
```

##### 启动Minecraft

```c#
var core = new LauncherCore
{
	JavaPath = "${JavaPath}",
	LauncherArgument = new LauncherArgument
	{
		MinecraftLocator = new MinecraftLocator("${.minecraft}"),
		AuthenticateResult = ${OfflineAuthenticate result}/${YggdrasilAuthenticate result},
		MaxMemorySize = "${Max memory size G or M",
		MinMemorySize = "${Max memory size G or M",
		LauncherName = "${Your launcher's name}", //暂时没啥用
		JvmArgument = "${Java virtual machine arguments}",
		//Optional
		ConnectionConfig = new AutoConnectConfig
		{
		    IpAddress = "${Auto connect server's ip}",
		    Port = "${Server post default 25565}"
		},
		ResolutionConfig = new WindowResolution
		{
		    WindowWidth = "${Window width}",
		    WindowHeight = "${Window height}",
		    FullScreen = false//default
		}
	}
}

var result = core.Launch("${Launch version}");//return Process

while (result.StandardOutput.ReadLine() != null)
{
	Console.WriteLine(result.StandardOutput.ReadLine());
}
```

##### 获取所有运行库

```c#
new LibrariesLocator(new MinecraftLocator("${.minecraft}")).GetLibraries("${version name}");
```

##### 获取所有Assets

```c#
new AssetsLocator(new MinecraftLocator("${.minecraft}")).GetAssets("${version name}");
```

##### 验证器

+ 正版验证

```c#
new YggdrasilAuthenticator("${email}","${password}","${client token (optional)").Authenticate();
```

+ 离线验证

```c#
new OfflineAuthenticator("${name}").Authenticate();
```

### 更新日志

+ 添加了对MojangApi的支持

### 开源许可

+ #### [ Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)

  > *MIT License*
  >
  > Copyright (c) 2007 James Newton-King
  >
  > Json.NET is a popular high-performance JSON framework for .NET

+ #### [RestSharp](https://github.com/restsharp/RestSharp)

  > *Apache License 2.0*
  >
  > Simple REST and HTTP API Client for .NET

+ #### [Masuit.Tools](https://github.com/ldqk/Masuit.Tools)

  > *MIT Lisence*
  >
  > https://masuit.com/55
  >
  > 一旦使用本开源项目以及引用了本项目或包含本项目代码的公司因为违反劳动法（包括但不限定非法裁员、超时用工、雇佣童工等）在任何法律诉讼中败诉的，项目作者有权利追讨本项目的使用费，或者直接不允许使用任何包含本项目的源代码！任何性质的`外包公司`或`996公司`需要使用本类库，请联系作者进行商业授权！其他企业或个人可随意使用不受限。
  
+ #### [Html Agility Pack (HAP)](https://github.com/zzzprojects/html-agility-pack)

  > *MIT Lisence*
  >
  > This is an agile HTML parser that builds a read/write DOM and supports plain XPATH or XSLT (you actually don't HAVE to understand XPATH nor XSLT to use it, don't worry...). It is a .NET code library that allows you to parse "out of the web" HTML files. The parser is very tolerant with "real world" malformed HTML. The object model is very similar to what proposes System.Xml, but for HTML documents (or streams).

### 鸣谢

+ #### [Bmclapi](https://bmclapidoc.bangbang93.com/)

  > BMCLAPI是[@bangbang93](http://weibo.com/bangbang93)开发的BMCL的一部分，用于解决国内线路对Forge和Minecraft官方使用的Amazon S3 速度缓慢的问题。BMCLAPI是对外开放的，所有需要Minecraft资源的启动器均可调用。
  
+ #### [Module-Launcher启动模块](https://www.mcbbs.net/thread-815868-1-1.html)

  > 一个由回忆、Ploer_Shile制作的支持全版本、全拓展启动，支持多渠道服务器验证的易语言启动模块
