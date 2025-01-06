# Feature

This is a working document for describing and working out feature design for the Shellguard project.

Each feature must be described fully, meaning that the feature SHOULD NOT be responsible for anything that is not mentioned here.

## App

The `app` feature handles the application lifecycle and instantiating the game. It also handles

## Game

The `game` feature handles the game lifecycle, including saving/loading the game. It is also responsible for determining when the game is over or won.

The game is over when the Darkspore enters your hollow stump home, reaching the Rooted Heart - the natural object holding your life force and soul as a forest gnome.

## Player

The `player` feature handles the character that is controlled by the player. It is responsible for taking the player input and translating it into actions the player character perform in the game affecting the game world.

Player actions include:

- Swinging a weapon...?
- Placing and repairing base structures `StructureRepo`
- Requesting terrain manipulation `TerrainRepo`

## Structures

The `structures` feature handles the all structures in the game, both default and player-placed.

## Terrain

The `terrain` feature handles the tilemap layers that make up the world ground layers. It supports:

- Getting tile data about a game coordinate.
- 

## WorldGrid



## 

The `object_placement` feature handles requests to place objects within the game world.

The placement is snapped to 
