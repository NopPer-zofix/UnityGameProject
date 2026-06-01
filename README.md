# 👾 UnityGameProject
## Name of the game: <span style = "color:red">Still Standing</span>
This is our group's Unity game project. It's the Top-Down zoombie shooter, made with a use of Unity Free assets and our imagination.

# Starting
Here is gonna be a list of requirements, following which you will be able to easily try the game and adjust it however you want.

- Unity version 2021.3.45f2
- Git LFS is a must for handling game assets and textures

# Installation
1. CLone the Repository<br>
`git clone https://github.com/NopPer-zofix/UnityGameProject.git`
2. Make sure to install the Git LFS<br>
`git lfs install`
3. Pull with the LFS<br>
`git lfs pull`

4. Open Unity Hub -> Click `Add` > `Add project from disk` -> Select root folder of the repository

# Game
## Controlls
In the game player has movement binds on WASD, Shooting on LMB, Pickup for weapons and keys on F, Pump for a shotgun on R.

## Game Story
Our game has it's story held in a world damaged by a zombie virus, our protagonist is a one of a few survived humans in this tragic world. Gameplay mainly rolls around player scouting through streets while being attacked by zombies and he has no choice other than fightig for life.
His final goal is to defeath the Boss, therefore saving the world.

# Structure of the project

## Our project has distinct diversion of the Assets folders:

1. Folder for every single sombie with it's own controller and animations are under the `Assets/animations_nameofmodel`
2. Folder for the Player and player's weapons and bullets for them
3. Folder for the Canvas and other UI objects and in `Assets/UI` folder, while the UI which is part of scenes is under the `Prefab/CanvasUI`
4. Scripts are strictly only under the Scripts folder
5. Audio files for the zombies and player interactions are under in the `Assets/Audio`
6. Tiles are under the Tilemaps folder in Assets
