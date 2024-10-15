# Glowbat

A 2D exploration arcade game where players control a bat navigating through dark, expansive cave systems. The primary objective is to keep the cave illuminated by eating fireflies. The cave darkens as the light gradually dims, creating tension between exploration and light management. The game ends if the bat collides with obstacles or if darkness engulfs the screen.

## Authors
**Matthew Fleischman**<br>
*University of Cape Town* <br>
*FLSMAT002@myuct.ac.za* 
<br>

**Mahomed Aadil Ally**<br>
*University of Cape Town* <br>
*ALLMAH002@myuct.ac.za* 
<br>

**Ariel J. Levy**<br>
*University of Cape Town* <br>
*LVYARI002@myuct.ac.za*
___

## **Table of Contents**

1. [Features](#features)
2. [Installation](#installation)
3. [How to Play](#how-to-play)
4. [Controls](#controls)
5. [Coming Soon](#coming-soon)
6. [Credits](#credits)

---

## **Features**

- Procedurally generated cave terrain.
- Dynamic light decay system.
- Randomly spawning consumables that move (fireflies).
- Score and time tracking.
- Detailed sound effects and music.
- Power-ups (e.g. shield, speed-up).
- Interactive tutorial.

---

## **Installation**

### Prerequisites:
- Unity (version 6.0 or higher recommended)

### Steps:

1. **Clone the repository:**
   ```bash
   git clone https://github.com/AJ-Levy/Glowbat.git
   ```

2. **Open the project in Unity:**
   - Launch Unity Hub and click on the `Open` button.
   - Browse to the folder where you cloned the repository and select it.

3. **Install necessary dependencies (if any):**

   - If the project uses any specific Unity packages, ensure that they are       installed. Unity will prompt you to download any missing packages when the project is loaded.

4. **Build and run the game:**
   - Go to `File` > `Build Settings`, choose your platform, and click `Build and Run` to test the game.
  
5. *(Optional)* **Playing the Game in Unity**:
   - To play the game within Unity, navigate to `Assets` > `Scenes`, and double-click `Startup` before pressing the `Play` button.

---

## How To Play

In *Glowbat*, you control a bat exploring dark, mysterious caves. The goal is to survive as long as possible by keeping the cave illuminated and avoiding obstacles.

### Score:
Your score reflects the depth of your exploration, increasing as you venture deeper into the cave. 

### Game Mechanics:
- **Fireflies**: Collect randomly spawning fireflies to increase your light radius. The more fireflies you consume, the brighter your surroundings. However, the light will decay over time, so keep collecting fireflies to stay alive.
- **Obstacles**: Watch out for cave walls and obstacles. Colliding with them will end the game.

### Power-Ups:
- **Shield**: Grants temporary protection from one collision.
- **Speed Boost**: Temporarily increases your speed for some time.

### Game Over:
The game ends if your light completely fades or if you collide with obstacles.

---

## Controls

| Key            | Action                      |
|----------------|-----------------------------|
| `W` / `↑`      | Move Up                     |
| `A` / `←`      | Move Left                   |
| `S` / `↓`      | Move Down                   |
| `D` / `→`      | Move Right                  |
| `Esc` / `Space`| Pause / Open Menu           |

---

## Coming Soon
- Cave Biomes
- More Powerups  
  ...

## Credits

### Development Team

- **Game Design & Programming**: Ariel Levy
- **Artwork & Playtesting**: Matthew Fleischman
- **Sound & Music**: Mahomed Aadil Ally

### Other Sources

- **Font**
   - **Name:** Pixel UniCode
   - **Designer:** anonymous-1000937
   - **License:** Creative Commons v3.0 
   - **Source:** [FontStruct](https://fontstruct.com/fontstructions/show/908795/pixel_unicode)
 
- **Background** 
  - **Name:** A Parallax Cave Background
   - **Designer:** Nrin
   - **License:** Creative Commons
   - **Source:** [Reddit](https://www.reddit.com/r/PixelArt/comments/61xvdq/ocwipcc_a_parallax_cave_background_i_made/)
   - **Adaptation**: The image was modified from its original form.
