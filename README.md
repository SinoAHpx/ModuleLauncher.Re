![brand-logo](https://user-images.githubusercontent.com/34391004/182008531-99bc3d73-d59c-4a7c-9b3d-5a7a17f586ed.png)

> Your on-the-fly Minecraft launcher core.

[![NuGet latest version](https://badgen.net/nuget/v/modulelauncher.re/latest)](https://nuget.org/packages/modulelauncher.re)
[![GitHub license](https://badgen.net/github/license/SinoAHpx/ModuleLauncher.Re)](https://github.com/SinoAHpx/ModuleLauncher.Re/blob/main/LICENSE)
[![GitHub stars](https://badgen.net/github/stars/SinoAHpx/ModuleLauncher.Re)](https://github.com/SinoAHpx/ModuleLauncher.Re/stargazers/)
[![Axen.Haxor](https://badgen.net/discord/members/tY8Y5HtvuK)](https://discord.gg/tY8Y5HtvuK)

## Glance

- Based on .NET 6.
- Multi-platform supported.
- Launch every version of Minecraft, from very old to very new, plus Forge, OptiFine, Liteload and Fabric.
- Flexible, lightweight and extensible.
- Customizable OAuth client id & redirect url for Microsoft authorization shceme.
- Multiple resources download source (especially for Chinese users).
- Fully documented (almost).
- Waiting for you to explore more.



<details>
<summary>But you should know</summary>

- MoudleLauncher don't and won't support automatically install Forge, OptiFine, Liteload, Fabric or other loaders.
- It's not hard to implement an in-built downloader, but we didn't do it, because it will literally against the flexibility and extensiblity we promised.

</details>

## Getting started

We don't talk about very basic facts here, if you have got confused, go to [documentation](https://sinoahpx.github.io/ModuleLauncher.NET.Documentation/#/).

### Install

+ Install ModuleLauncher.Re via Nuget:
  + **IDE interface**
  + Nuget Package Manger: ```Install-Package ModuleLauncher.Re```
  + .NET CLI: ```dotnet add package ModuleLauncher.Re```
+ Or clone the repo and compile it to dynamic link library

### Launch

Assuming you must have an existing `.minecraft`. By the way, don't be afraid of these lines of codes, actually real codes are within 10 lines.

```cs
// version you want to launch
var version = "<version>";

// initialize a MinecraftResolver instance by path of .minecraft
var minecraftResolver = new MinecraftResolver(rootPath);

// get MinecraftEntry by Minecraft id (version)
// it's very IMPORTANT
var minecraft = minecraftResolver.GetMinecraft(version);

// launch in method-chain way
// essentially only two value needed to launch Minecraft besides Minecraft version
// player: username of an offline player 
// (notice that here string has been implicitly converted to AuthenticateResult)
// java: path of java executable file
var process = await minecraft.WithAuthentication("<player>")
    .WithJava(@"<java>")
    .LaunchAsync();
```

The launch method `LaunchAsync` returns an instance of `Process`, you can manipulate it as you wish.

```cs
// read and write Minecraft output
// ReadOutputLine and IsNullOrEmpty here is extension methods provided by library Manganese
while (!process.ReadOutputLine().IsNullOrEmpty())
{
    Console.WriteLine(process.ReadOutputLine());
}
```

## Contribute

### Code contribution

- You can fork this repo and open a pull request to `new` branch. Noticibly you can't open a pull request directly to the `main` branch.
- Open an [issue](https://github.com/SinoAHpx/ModuleLauncher.Re/issues).

### Documentation contribution

If you didn't satified with the current document, you can:

- Open PR to [documentation repo](https://github.com/SinoAHpx/ModuleLauncher.NET.Documentation).
- Open an [issue](https://github.com/SinoAHpx/ModuleLauncher.NET.Documentation/issues) in documentation repo.

## Credits

- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json): very powerful JSON manipulation.
- [Flurl.Http](https://github.com/tmenier/Flurl): such a fluent HTTP library.
- [Manganese](https://github.com/SinoAHpx/Manganese): good one, useful.
- [JetBrains](https://www.jetbrains.com/): provided free IDEs.
- [Google](https://google.com) and [Stack Overflow](https://stackoverflow.com).