# CrabTemplate
A project template for modding Crab Game with BepInEx.

## Getting Started
Firstly, after [downloading](https://github.com/lammas321/CrabTemplate/archive/refs/heads/main.zip) this template, you may want to name this something else, so whenever you see 'CrabTemplate', replace that with whatever you want the name of your mod to be. To do so, you'll want to rename and check the contents of the following via notepad or something equivalent, without opening the project in Visual Studio:
- `CrabTemplate.sln`
- `CrabTemplate.csproj`
- `Plugin.cs`
- and `Patches.cs`

Then, you may open the project in Visual Studio and open the `.csproj`. In here, if you didn't earlier, you can edit the properties to fit your needs. You should edit/verify:
- AssemblyName
- Product
- Version
- SteamDir, ProfileDir, UnhollowedDir, and DepsDir
- Configurations
- and Dependencies

Now you may start poking around the other `.cs` files of the template and making your mod!
