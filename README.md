# HoloChess_and_DiscordChessBot

Developed a Discord Chess Bot that moderates a chess game between two Discord members. 
I implemented slash commands and other types of commands available in Discord to make the interaction experience pleasant.
Deployed the Discord Bot on the cloud (https://dash.daki.cc) . 

Developed a Mixed Reality Chess Program, HoloChess which is a chess game against one of the top chess engines (stockfish) or you can also analyze the game by yourself by playing from both ends.Used Unity to develop the Chess Game and also deployed the game on Microsoft hololens2.

## Glossary
| Term      | Description |
| ----------- | ----------- |
| Mixed Reality    | Mixed reality is a blend of physical and digital worlds, unlocking natural and intuitive 3D human, computer, and environmental interactions. This new reality is based on advancements in computer vision, graphical processing, display technologies, input systems, and cloud computing.   |
| MRTK    | MRTK-Unity is a Microsoft-driven project that provides a set of components and features, used to accelerate cross-platform MR app development in Unity.  |
| Pointers | Pointer is the way to interact with object in the Hololens world . The key ones used are Hand ray,Gaze,AirTap . refer https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk2/features/input/pointers?view=mrtkunity-2022-05 for more info|
| UCI    | The Universal Chess Interface (UCI) is an open communication protocol that enables chess engines to communicate with user interfaces. |

##Hololens
<add image of hololens>

##Core Capabilities
1. Concepts of Mesh Renderer to develop the chess tiles for easier calculation for positioning the chess pieces.
2. The right-hand ray of the Hololens as the way of interaction with the chessboard
3. UCI Protocol to communicate with Stockfish Engine 
4. Adding OnGrabbable script and other prebuilt scripts to control the motion of the chess pieces

##Prerequisite
Make sure you have installed all of the following prerequisites on your development machine:
*Unity - [unity](https://unity.com/download).
*MRTK - [mrtk](https://hololabinc.github.io/MixedRealityToolkit-Unity/Documentation/Installation.html).
*Stockfish - [stockfish](https://stockfishchess.org).
*discord - [discord](https://discordpy.readthedocs.io/en/stable/).
*chess - [chess](https://discordpy.readthedocs.io/en/stable/).

##Running
Build the unity project and Deploy it on Hololens.
Run the python code to make the discord chess bot active.

<add image of your hololens proj>

##Reference

https://www.youtube.com/watch?v=qVhG6ZWqD-o&list=PLmcbjnHce7SeAUFouc3X9zqXxiPbCz8Zp


##Add Ons
1) Integrate Holochess to the discord Bot through rest API so that the Holochess player can play through his Hololens while the other player can communicate through Discord.
2) This Project doesn't work with the current stockfish because it isn't supported on Hololens yet . It recieves results from stockfish when run on unity (since it is using my pc spec for the same). Used UCI but it wasn't working on Hololens so have to find an alternative for stockfish. If you build to develop on this you can use stockfish unity asset which is 150$ .






















