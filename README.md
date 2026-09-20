# ECoffe Sustainability VR Game

**A Unity VR café game built for Meta Quest, combining food preparation, café customization, and sustainability themed gameplay.**

Players work through a café shift, prepare customer orders, and make choices about the cups and plates they use. After the shift, a waste sorting minigame challenges them to throw items into the correct bins. Earned coins can be spent on recipes, furniture, and decorations for the café.

The project also includes a short sandwich making demo for a focused introduction to the gameplay.

## Gameplay

- **Café shifts:** prepare food and drinks, complete customer orders, and manage customer patience.
- **Sustainability choices:** order scoring considers both the prepared item and the use of sustainable cups or plates.
- **Waste sorting:** complete a VR minigame after the shift by throwing rubbish into the appropriate bins.
- **Café customization:** use the shop to unlock recipes and add furniture and decorative items.
- **Saved progression:** local JSON saves store progress, coins, and purchased items.
- **Short demo:** a separate scene focuses on making a sandwich.

The source also includes an AI assisted NPC dialogue feature in which the player explains the use of sustainable utensils.

## Scene guide

The main scenes are in **[`Assets/Scenes/scenes/`](Assets/Scenes/scenes/)**. Scene names and capitalization below match the project files.

| Scene | Purpose |
| --- | --- |
| [`Title.unity`](Assets/Scenes/scenes/Title.unity) | Title screen and entry point for the full game. |
| [`Game.unity`](Assets/Scenes/scenes/Game.unity) | Main café gameplay, including customer orders and food preparation. |
| [`Shop.unity`](Assets/Scenes/scenes/Shop.unity) | Shop for recipes, furniture, and café decorations. |
| [`TrashSortVR.unity`](Assets/Scenes/scenes/TrashSortVR.unity) | Post shift minigame: throw each item into the correct bin. |
| [`short.unity`](Assets/Scenes/scenes/short.unity) | Short demo version focused on making a sandwich. |

Start with **`Title`** to explore the full game, or open **`short`** to explore the sandwich demo.

## Technology

| Component | Technology |
| --- | --- |
| Engine | Unity 6 — **6000.0.33f1** |
| Language | C# |
| Target platform | Meta Quest / Android |
| VR interaction | XR Interaction Toolkit 3.0.8 and Meta XR Interaction SDK OVR 74.0.1 |
| Rendering | Universal Render Pipeline 17.0.3 |
| Input | Unity Input System 1.11.2 |
| Local persistence | JSON files through Unity's `Application.persistentDataPath` |

The project also contains OpenXR/Oculus XR packages, an OpenAI dialogue integration, and PlayFab integration scripts. Package versions are recorded in [`Packages/manifest.json`](Packages/manifest.json).

## Open the project

1. Install **Unity Hub** and **Unity 6000.0.33f1**. The original editor version is recorded in [`ProjectSettings/ProjectVersion.txt`](ProjectSettings/ProjectVersion.txt).
2. For a standalone Quest build, install the editor's **Android Build Support**, including **Android SDK & NDK Tools** and **OpenJDK**.
3. Clone this repository:

   ```bash
   git clone https://github.com/HindAlz/ECoffe-Sustainability-VR-Game.git
   ```

4. In Unity Hub, add the cloned project folder containing `Assets`, `Packages`, and `ProjectSettings`.
5. Open it with the matching editor version and allow Unity to import assets and resolve packages.
6. Open `Assets/Scenes/scenes/Title.unity` for the full game, or `Assets/Scenes/scenes/short.unity` for the sandwich demo.

## Build for Meta Quest

1. Set up the Quest headset for development, connect it to the computer, and authorize USB debugging.
2. In Unity, open **File → Build Profiles** and create or select an **Android** profile. Switch to that profile.
3. In the active profile's scene list, include and enable `Title`, `Game`, `Shop`, and `TrashSortVR`. Put `Title` first for the full game.
4. To create a demo that launches directly into sandwich preparation, include `short` and place it first instead. Retain any other scenes needed by its navigation.
5. Select the connected headset as the run device and choose **Build And Run**.

**Scene setup note:** the build settings currently enable `Title`, `Game`, and `short`, while `Shop` and `TrashSortVR` are unchecked. Enable the latter two before building the full game so its scene transitions can load them.

See Unity's [Android build instructions](https://docs.unity.com/en-us/engine/6000.0/manual/platform-specific/android/building-and-delivering/build-process) for the build-profile workflow.

## Code navigation

| Location | What to explore |
| --- | --- |
| [`Assets/OurScripts/`](Assets/OurScripts/) | Food and drink interactions, customer systems, shop logic, and local save data. |
| [`CustomerSystem.cs`](Assets/OurScripts/CustomerSystem.cs) | Orders, customer patience, scoring, and the transition to waste sorting. |
| [`shop.cs`](Assets/OurScripts/shop.cs) | Recipe and décor purchases. |
| [`PlayerInventoryManager.cs`](Assets/PlayerInventoryManager.cs) | Activating unlocked preparation tools and café decorations. |
| [`trashmanager.cs`](Assets/trashmanager.cs) | Waste spawning, sorting bonuses, and the end of day review. |
| [`SaveSystem.cs`](Assets/OurScripts/SaveSystem.cs) | Reading and writing local JSON save files. |
| [`GPTDialogue.cs`](Assets/GPTDialogue.cs) | The sustainability themed NPC dialogue integration. |
| [`Assets/OurPrefabs/`](Assets/OurPrefabs/) | Project prefabs. |

