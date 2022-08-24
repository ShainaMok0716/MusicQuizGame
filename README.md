# MusicQuizGame
A coding test by Shaina Mok
Develop in Unity version 2020.3.30f1

## To Play
Run the [Music Quiz Game.exe] under MusicQuiz\Build
or
Press play in Unity

## Reading Guide
Classic MVC architecture
Everthing starts in GameManager.Start()

Model
	- Struct (all data struct)
	- GameData (runtime data)

View
	- WelcomeView (ui of welcome screen)
	- GameView (ui of game screen)
	- ResultView (ui of result screen)
	- PlaylistCellView (ui of prefab PlaylistCell)

Controller
	- GameManager (store GameData and controll the game flow)
	- AssesManager (store and download assets such as song and texture)

Prefab Used
	- PlaylistCell

Super Class
	- Singleton

## Use of Multithreading
download song and image using System.Threading.Tasks with async & await

## Package Used
used Newtonsoft Json v3.0.2 for converting json into struct
you can found the ref doc here: https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@3.0/manual/index.html

## Remarks
Please let me know if you have any question by email: shainamok@gmail.com

Thanks for your time.