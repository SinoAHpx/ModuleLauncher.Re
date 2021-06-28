<p align="center">
  <a href="https://github.com/AHpxChina/ModuleLauncher.Re">
    <img src="https://i.loli.net/2021/03/09/NdwvUPWLljSC6rh.png" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">ModuleLauncher.Re</h3>

  <p align="center">
    Makes it possible to develop Minecraft launchers with efficiency and elegance.
    <br />
    <a href="https://ahpxchina.github.io/ModuleLauncher.Re.Document/"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/AHpxChina/ModuleLauncher.Re/tree/master/ModuleLauncher.Example">View Demo</a>
    ·
    <a href="https://github.com/AHpxChina/ModuleLauncher.Re/issues">Report Bug</a>
    ·
    <a href="https://github.com/AHpxChina/ModuleLauncher.Re/issues">Request Feature</a>
  </p>

## About The Project

This is an easy-to-use, trainee-friendly Minecraft launcher library. It's enough for you to develop a high-performance, elegant Minecraft launcher application.

Here's why:
* We chose .Net Standard 2.1 as the development framework.
* Simple but not spartan, focusing on core features.
* Complete documentation and examples, and almost every method with xml-doc.
* Cross-platform, same code, different os.

This library still has many shortcomings and defects, we welcome anyone to submit [issues](https://github.com/AHpxChina/ModuleLauncher.Re/issues).

### Built With

ModuleLauncher.Re is possible because of the support of the following great projects:

* [.Net](https://dotnet.microsoft.com/)
* [Newtonsoft.Json](https://json.net)
* [Downloader](https://github.com/bezzad/Downloader)

## Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

### Installation

+ Install **ModuleLauncher.Re** via Nuget:
  + IDE interface
  + Nuget Package Manger: ```Install-Package ModuleLauncher.Re -Version 3.0.0```
  + .Net CLI: ```dotnet add package ModuleLauncher.Re --version 3.0.0```

+ Or clone the repo and compile it to dynamic link library

### Usage

Create a simple ```Launcher``` instance:

```csharp
var launcher = new Launcher("<.minecraft>")
{
    Authentication = "<player name>",
    Java = @"<javaw.exe> or <java.exe>"
};
```

After constructing the Launcher instance, we can launch Minecraft:

```csharp
await launcher.Launch("<the version to launch>");
```

_For more examples, please refer to the [Documentation](https://example.com)_

## Roadmap

See the [open issues](https://github.com/AHpxChina/ModuleLauncher.Re/issues) for a list of proposed features (and known issues).

## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

Distributed under the MIT License. See `LICENSE` for more information.

## Contact

破晓 - [Telegram](https://t.me/AhpxChina) - ahpx@yandex.com

Project Link: https://github.com/AHpxChina/ModuleLauncher.Re

## Acknowledgements

In no particular order.

* [Jetbrains Rider](https://www.jetbrains.com/rider)
* [.Net](https://dotnet.microsoft.com/)
* [Newtonsoft.Json](https://json.net)
* [Downloader](https://github.com/bezzad/Downloader)
* [MoreLINQ](https://github.com/morelinq/MoreLINQ)
* [Best-README-Template](https://github.com/othneildrew/Best-README-Template)
* [BMCLAPI](https://bmclapidoc.bangbang93.com/#api-_)
* [ModuleLauncher](https://www.mcbbs.net/thread-815868-1-1.html)
