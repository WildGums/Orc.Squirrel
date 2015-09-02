Orc.Squirrel
===============

[![Join the chat at https://gitter.im/WildGums/Orc.Squirrel](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/WildGums/Orc.Squirrel?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

![License](https://img.shields.io/github/license/wildgums/orc.squirrel.svg)
![NuGet downloads](https://img.shields.io/nuget/dt/orc.squirrel.svg)
![Version](https://img.shields.io/nuget/v/orc.squirrel.svg)
![Pre-release version](https://img.shields.io/nuget/vpre/orc.squirrel.svg)

Is a library that adds a few extra features on top of [Squirrel](https://github.com/Squirrel/Squirrel.Windows) to allow updates to come from different channels.

You can setup "Alpha", "Beta" and "Stable" channels to release your software through.

NuGet Packages
----------------

- **[Orc.Squirrel](http://www.nuget.org/packages/Orc.Squirrel/)** => contains an **IUpdateService** and an **UpdateChannel** class
- **[Orc.Squirrel.Xaml](http://www.nuget.org/packages/Orc.Squirrel.Xaml/)** => contains an installation notification window

Quick start
------------

- Use [Squirrel](https://github.com/Squirrel/Squirrel.Windows) to deploy your software
- Use **UpdateChannel** for setting up your update channels
- Use **IUpdateService** for handling updates (for your software)
