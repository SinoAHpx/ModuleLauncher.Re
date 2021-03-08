# ModuleLauncher.Re 2.8

![GitHub](https://img.shields.io/github/license/AHpxChina/ModuleLauncher.RE?logo=github&style=for-the-badge)![GitHub stars](https://img.shields.io/github/stars/AHpxChina/ModuleLauncher.RE?logo=github&style=for-the-badge)![GitHub repo size](https://img.shields.io/github/repo-size/AHpxChina/ModuleLauncher.RE?logo=github&style=for-the-badge)

ModuleLauncher.Re 使得开发Minecraft启动器变得高效而优雅。
---------------------------------------------------------

### 介绍

如你所见，ModuleLauncher这个项目是[Module-Launcher启动模块](https://www.mcbbs.net/thread-815868-1-1.html)的C#版本，它具有以下特性：

+支持1.7.10及以上的加载器和原版Minecraft
  + 不保证可以启动1.7.10之前的Minecraft
+ 支持补全Minecraft缺失的资源和库文件
+ 支持获取几乎整个[Mcbbs](https://www.mcbbs.net)首页的新闻
+ 支持Yggdrasil和authlib-injector外置的几乎所有接口
+ 正在~~快速~~迭代中

请访问本项目的[wiki](https://github.com/AHpxChina/ModuleLauncher.Re/wiki)来获取更多信息和使用文档，

### 安装

+ 将ModuleLauncher.Re源码克隆到本地并进行编译成dll以导入引用
+ 使用[Nuget](https://www.nuget.org/packages/ModuleLauncher.Re/2.8.0)

  ```
  Install-Package ModuleLauncher.Re -Version 2.8.0
  ```

### 更新日志

+ 2.8
  + 支持了微软登陆方案，请参考[Microsoft Authentication Scheme](https://wiki.vg/Microsoft_Authentication_Scheme)

### 计划

+ 添加对Forge，Optifine，Fabric的下载和自动安装支持
+ 使用多线程，以优化下载速度

### 开源许可

+ #### [ Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)

  > *MIT License*
  >
  > Copyright (c) 2007 James Newton-King
  >
  > Json.NET is a popular high-performance JSON framework for .NET
  >
+ #### [RestSharp](https://github.com/restsharp/RestSharp)

  > *Apache License 2.0*
  >
  > Simple REST and HTTP API Client for .NET
  >
+ #### [Html Agility Pack (HAP)](https://github.com/zzzprojects/html-agility-pack)

  > *MIT Lisence*
  >
  > This is an agile HTML parser that builds a read/write DOM and supports plain XPATH or XSLT (you actually don't HAVE to understand XPATH nor XSLT to use it, don't worry...). It is a .NET code library that allows you to parse "out of the web" HTML files. The parser is very tolerant with "real world" malformed HTML. The object model is very similar to what proposes System.Xml, but for HTML documents (or streams).
  >

### 鸣谢

+ [Jetbrains](https://www.jetbrains.com/?from=ModuleLauncher.Re)

  > 本项目已获得[Jetbrains OS](https://www.jetbrains.com/shop/eform/opensource)许可
  >
  > <img src="https://i.loli.net/2020/11/04/tQDus23pyNWgX57.png" height="100" width=100>
  >
  >
+ #### [Bmclapi](https://bmclapidoc.bangbang93.com/)

  > BMCLAPI是[@bangbang93](http://weibo.com/bangbang93)开发的BMCL的一部分，用于解决国内线路对Forge和Minecraft官方使用的Amazon S3 速度缓慢的问题。BMCLAPI是对外开放的，所有需要Minecraft资源的启动器均可调用。
  >
+ #### [Module-Launcher启动模块](https://www.mcbbs.net/thread-815868-1-1.html)

  > 一个由回忆、Ploer_Shile制作的支持全版本、全拓展启动，支持多渠道服务器验证的易语言启动模块
  >
