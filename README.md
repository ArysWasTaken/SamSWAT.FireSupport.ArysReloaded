# Disclaimer

[![CC BY-NC 4.0][cc-by-shield]][cc-by]

[cc-by]: https://creativecommons.org/licenses/by-nc/4.0/
[cc-by-shield]: https://img.shields.io/badge/License-CC%20BY--NC%204.0-lightgrey.svg

This mod was originally made by <a href="https://github.com/SamSWAT911">SamSWAT911</a>.

In their absence, I have decided to maintain the beloved FireSupport mod.

<a href="#"><img src="https://media.discordapp.net/attachments/417281262085210112/1013842879715749999/140818-F-PO994-258-scaled.jpg?width=1440&height=450"></a>

# Fire Support

BepInEx mod for SPT that will add insurgency-style fire support options into Escape From Tarkov.

This mod is tied to the in-game [rangefinder](https://escapefromtarkov.fandom.com/wiki/Vortex_Ranger_1500_rangefinder), so make sure it's in your inventory before you go into a raid. After you enter location, you will hear a radio message saying something like support is now available. Currently only A-10 autocannon strafe is implemented, but I have plans to extend it later.

Double tap `Y` key (by default) to open gestures menu where you should notice new radial menu with available requests, their respective amounts and timer appearing after request in the center circle.

<details> 
  <summary>Radial Menu</summary>
   <a href="#"><img src="https://media.discordapp.net/attachments/417281262085210112/1013870628366987334/radialmenu.png?width=256&height=256"></a>
</details>

After you select support option, by clicking on it with `LMB`, vertical spotter mark will appear. Move your mouse around to choose position and then press `LMB` again. If you want to cancel request, you can do that by hitting `Left ALT` and `RMB`. Also, if your spotter can't hit any surface (e.g. you pointing into the sky) notice message will appear.

<details> 
  <summary>Vertical Spotter</summary>
   <a href="#"><img src="https://media.discordapp.net/attachments/417281262085210112/1013916622681022526/spotterVertical.gif?width=700&height=256"></a>
</details>

Autocannon strafe implies that there is a heading of where to shoot, so after you confirmed position, horizontal spotter will appear allowing you to choose direction by moving your mouse left or right. At this point you still can cancel request by pressing `Left ALT` and `RMB`, or, if you wish to confirm, press `LMB` once again.

<details> 
  <summary>Horizontal Spotter</summary>
   <a href="#"><img src="https://media.discordapp.net/attachments/417281262085210112/1013916623188529162/spotterHorizontal.gif?width=700&height=256"></a>
</details>

After confirmation, you will hear a radio message from station about your support request and a short time later the A-10 will strike at the selected position and direction in a rectangular pattern. The kill zone lies somewhere between 20 to 40 metres in each direction from the horizontal spotter. Some settings like request amount and its cooldown along with radio messages volume are configurable in BepInEx Configuration Manager that opens with `F12` key.

<details> 
  <summary>Available settings</summary>
  <a href="#"><img src="https://media.discordapp.net/attachments/417281262085210112/1013924835543494758/unknown.png"></a>
</details>

Full video demonstration of this mod on YouTube:

<details> 
  <summary>YouTube video</summary>
   <a href="https://www.youtube.com/watch?v=el2CoSHbSK4"><img src="https://media.discordapp.net/attachments/417281262085210112/1013944435077296238/unknown.png?width=560&height=315"></a>
</details>

### How to install

1. Download the latest release here: [link](https://github.com/Nympfonic/SamSWAT.FireSupport.ArysReloaded/releases) -OR- build from source (instructions below)
2. Extract the 7z file and drop the folders `BepInEx` and `SPT` into your game directory.

### Requirements

- Visual Studio 2022 (.NET desktop workload) or Jetbrains Rider 2025
- .NET Standard 2.1

### How to build from source

1. Download/clone this repository
2. VisualStudio > File > Open solution > `SamSWAT.FireSupport.ArysReloaded.sln`
3. Open the project .csproj and `Directory.Build.props` in a text editor and change the paths to your liking
4. Change assembly references to point to where you keep your assembly refs
5. Extract `assets.7z` with `here` option
6. Delete archive after extraction
7. VisualStudio > Build > Rebuild solution
