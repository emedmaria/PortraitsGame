### Portraits game

This project is a plain  memory game. 
The goal is simple, it consists in matching the total amount of pairs shown by level in order to progress. 

#### Scenes. 
The App has a single scene PortraitsGame.scn placed under the /scene folder. 

#### App

The loading is handled by a Loader.cs script attached to the Camera. 
It has the references needed to make it work (and sound;)), more specifically the GameManager and SoundManager (singleton access for every script component) 

#### Controllers
- GameManager: Manages the global state of the app. 
- PortraitsController: Rules the state of the current level.

#### Views
Placed within the hierarchy. The suitable script added as a component to each view. 

#### Models
Are implemented as ScriptableObjects and saved in an asset. They are located under /ScriptableObjects folder. 
It can be created as assets in the Project View (Create> Portraits Game>XX).
For handiness, all can be created from the inspector but the GameModel and PlayerModel by reason must be unique!

#### Levels
As mentioned above levels are Scriptable objects configurable. 
The display of the customizable parameters needs some retouches (meaning, conditional showing based on selection, slides...). 
It is thought to give flexibility creating the layout distribution to have more diversity in the levels. Currently, only Square layouts are possible (4x4 or 5x5 depending on the number of pairs to score). 
Same with the play mode, it is prepared to have two modes but for now, Count Down is the one usable. 

Currently, three levels are created (/ScriptableObjects/LevelData). 

#### Todos
- Change the multidimesional array used in the grid to jagged arrays for faster access. 
- Hide/Show properties in the inspector for CustomEditor created for the the PortraitsLevel SO. 
- Random Grid distribution (display).
- More visual effects,animations and fade in/out transitions. 
- New game mode.
