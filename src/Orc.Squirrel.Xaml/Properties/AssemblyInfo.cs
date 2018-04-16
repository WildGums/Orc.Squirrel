// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;

// All other assembly info is defined in SolutionAssemblyInfo.cs

[assembly: AssemblyTitle("Orc.Squirrel.Xaml")]
[assembly: AssemblyProduct("Orc.Squirrel.Xaml")]
[assembly: AssemblyDescription("Orc.Squirrel.Xaml library")]
[assembly: NeutralResourcesLanguage("en-US")]

[assembly: InternalsVisibleTo("Orc.Squirrel.Tests")]

[assembly: XmlnsPrefix("http://schemas.wildgums.com/orc/squirrel", "orcsquirrel")]
[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/squirrel", "Orc.Squirrel")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/squirrel", "Orc.Squirrel.Behaviors")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/squirrel", "Orc.Squirrel.Controls")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/squirrel", "Orc.Squirrel.Converters")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/squirrel", "Orc.Squirrel.Fonts")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/squirrel", "Orc.Squirrel.Markup")]
[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/squirrel", "Orc.Squirrel.Views")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/squirrel", "Orc.Squirrel.Windows")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]
