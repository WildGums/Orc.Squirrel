Orc.Squirrel
===============

Is a library that adds a few extra features on top of [Squirrel](https://github.com/Squirrel/Squirrel.Windows) to allow updates to come from different channels.

You can setup "Alpha", "Beta" and "Stable" channels to release your software through.

Downloads
-----------

- **[Orc.Squirrel](http://www.nuget.org/packages/Orc.Squirrel/)** => contains an **IUpdateService** and an **UpdateChannel** class
- **[Orc.Squirrel.Xaml](http://www.nuget.org/packages/Orc.Squirrel.Xaml/)** => contains an installation notification window

Quick start
------------

- Use [Squirrel](https://github.com/Squirrel/Squirrel.Windows) to deploy your software
- Use **UpdateChannel** for setting up your update channels
- Use **IUpdateService** for handling updates (for your software)
