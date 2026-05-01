# Spyro Editor

----------------------------------------
### ⚠️ Work in Progress ⚠️
----------------------------------------

WAD resource viewer and editor for the Spyro trilogy on PlayStation 1.

This is meant to be a spiritual successor to "Spyro World Viewer," and is heavily based on its source code as well as my own work from adding the Spyro games to the [noclip.website](https://github.com/magcius/noclip.website/tree/main/src/Spyro) project. It can be used for ROM hacks and will also serve to document the contents and format of the WAD subfiles.

Only Windows 11 and newer versions of 10 are supported. Linux and macOS will not be supported. No AI is used.

Initial features will include:

- 3D view of the levels
- Basic level editing
  - Custom vertex colors and swappable textures
  - Custom textures (maybe)
  - Limited custom geometry
    - Right now, only the visible geometry is parsed
    - Collision data is separate and will require more work to parse
- Raw hex view of subfiles and export functionality
- Support for NSTC and PAL versions of each game

In the future, some things that would be nice are:

- Support for viewing and editing mobys
- More game-specific features, like changing orbs in 2 or eggs in 3
- Custom dialogue
- Editing of code overlays?
  - Might not be feasible, but this will depend on the ongoing decompile and community PSYQ tools
