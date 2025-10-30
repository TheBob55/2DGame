# Dodge the Creeps - Godot First 2D Game Tutorial

This is an implementation of the official Godot "Your First 2D Game" tutorial, also known as "Dodge the Creeps".

## Game Description

In this game, your character must move around and avoid the enemies for as long as possible. The game is over when you hit an enemy. The longer you survive, the higher your score!

## Project Structure

- `player.gd` / `player.tscn` - Player character with movement controls
- `mob.gd` / `mob.tscn` - Enemy mobs that spawn randomly
- `main.gd` / `main.tscn` - Main game scene with spawning logic
- `hud.gd` / `hud.tscn` - Heads-up display for score and messages
- `project.godot` - Project configuration file

## Required Assets

To complete this game, you need to download the art and audio assets:

### Art Assets (PNG files)

Download from the official Godot tutorial or create your own. Place these files in an `art/` directory:

**Player sprites:**
- `playerGrey_up1.png`
- `playerGrey_up2.png`
- `playerGrey_walk1.png`
- `playerGrey_walk2.png`

**Mob sprites:**
- `enemyFlyingAlt_1.png`
- `enemyFlyingAlt_2.png`
- `enemySwimming_1.png`
- `enemySwimming_2.png`
- `enemyWalking_1.png`
- `enemyWalking_2.png`

### Audio Assets

Place these files in the `art/` directory:

- `House In a Forest Loop.ogg` - Background music
- `gameover.wav` - Death sound effect

### Font

Place this file in a `fonts/` directory:

- `Xolonium-Regular.ttf` - Font for the HUD

## Getting the Assets

### Option 1: Official Tutorial Assets

Visit the official Godot documentation and download the asset pack:
https://docs.godotengine.org/en/stable/getting_started/first_2d_game/index.html

The tutorial page provides a download link for all required assets.

### Option 2: Use Your Own Assets

You can create or source your own assets with the following specifications:

**Player sprites:**
- Size: approximately 100x100 pixels
- Two animations: "up" (moving vertically) and "walk" (moving horizontally)
- Each animation needs 2 frames

**Mob sprites:**
- Size: approximately 100x100 pixels
- Three types: fly, swim, walk
- Each type needs 2 frames for animation

**Audio:**
- Background music: Looping OGG file
- Death sound: WAV file

**Font:**
- Any TTF font will work for the HUD text

## How to Play

1. Open the project in Godot Engine (4.3 or later recommended)
2. Make sure all assets are in place (see above)
3. Press F5 or click the "Play" button to run the game
4. Use WASD or Arrow Keys to move
5. Press Space to start the game
6. Avoid the enemies!

## Controls

- **W / Up Arrow** - Move up
- **S / Down Arrow** - Move down
- **A / Left Arrow** - Move left
- **D / Right Arrow** - Move right
- **Space** - Start game

## Features

- Player movement in 8 directions
- Animated player and enemy sprites
- Random enemy spawning from screen edges
- Collision detection
- Score tracking
- Start screen and game over screen
- Background music and sound effects

## Tutorial Reference

This game follows the official Godot tutorial:
https://docs.godotengine.org/en/stable/getting_started/first_2d_game/index.html

## License

This is an educational project based on the Godot Engine tutorial. Use and modify as you wish for learning purposes.
