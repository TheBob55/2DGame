# Asset Download Instructions

## Quick Setup

The easiest way to get all required assets is to download them from the official Godot tutorial repository.

### Step 1: Download Assets

You can download the asset pack from the Godot documentation or use these direct sources:

**Official Tutorial Assets:**
The Godot documentation provides a complete asset package. Visit:
https://docs.godotengine.org/en/stable/getting_started/first_2d_game/index.html

Look for the "Project setup" section which contains a download link for the assets.

### Step 2: Extract Files

After downloading, extract the files into the appropriate directories:

```
2DGame/
├── art/
│   ├── playerGrey_up1.png
│   ├── playerGrey_up2.png
│   ├── playerGrey_walk1.png
│   ├── playerGrey_walk2.png
│   ├── enemyFlyingAlt_1.png
│   ├── enemyFlyingAlt_2.png
│   ├── enemySwimming_1.png
│   ├── enemySwimming_2.png
│   ├── enemyWalking_1.png
│   ├── enemyWalking_2.png
│   ├── House In a Forest Loop.ogg
│   └── gameover.wav
└── fonts/
    └── Xolonium-Regular.ttf
```

## Alternative: Manual Asset Creation

If you want to create your own assets, here are the specifications:

### Player Images
- Format: PNG
- Size: ~100x100 pixels
- Transparent background
- 2 frames for "up" animation (moving vertically)
- 2 frames for "walk" animation (moving horizontally)

### Enemy Images
- Format: PNG
- Size: ~100x100 pixels
- Transparent background
- 3 enemy types (flying, swimming, walking)
- 2 frames per enemy type

### Audio Files
- Background music: OGG format, looping
- Death sound: WAV format, short (1-2 seconds)

### Font
- Format: TTF
- Any readable font works
- Recommended: Xolonium (free font)
- Download Xolonium: https://fontlibrary.org/en/font/xolonium

## Verification

To verify all assets are in place, run:

```bash
ls -R art/ fonts/
```

You should see all the files listed above.

## Import Settings

When you first open the project in Godot, it will automatically import all assets.
The import process may take a few moments. Once complete, you can run the game!

## Troubleshooting

**Missing Resource Errors:**
If you see errors about missing resources when opening scenes:
1. Make sure all files are in the correct directories
2. Check that file names match exactly (case-sensitive)
3. Re-import the project (Project → Reload Current Project)

**Audio Not Playing:**
- Ensure audio files are in OGG (music) and WAV (effects) formats
- Check volume settings in Godot's Audio settings

**Sprites Not Showing:**
- Verify PNG files have transparent backgrounds
- Check that import settings are correct (should auto-import as Texture2D)
