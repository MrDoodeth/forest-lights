# Forest Lights

A third-person isometric survival game built with Unity. Explore a procedurally generated forest, gather resources, defend yourself from bears, light the altars, and reach the exit.

This branch contains the WebGL-oriented version of the project.

## 📦 Features

- Procedurally generated 40×40 forest map with trees, berry bushes, grass, flowers, altars, and bear spawns.
- Resource gathering: chop trees for wood and collect berries to restore health.
- Player health, stamina, sprinting, combat, respawning, pause, win, and lose flows.
- Bear AI using Unity NavMesh for patrol, chase, and attack behavior.
- Day/night lighting, sound effects and music, adjustable quality/render scale, and saved audio settings.
- English and Russian localization.
- Desktop and mobile controls with Yandex Games integration.

## 📋 Requirements

- Unity `2021.3.25f1`.
- WebGL Build Support for Unity when building a browser package.

## 🚀 Build and run

```bash
git clone https://github.com/MrDoodeth/forest-lights.git
cd forest-lights
git switch HTML-Build
```

Open the `Ramake survavium` folder in Unity Hub with Unity `2021.3.25f1`. The playable scenes are:

- `Assets/Scenes/Menu/Menu.unity`
- `Assets/Scenes/Survavium/Survavium.unity`

To create a WebGL build, open **File → Build Settings**, select **WebGL**, add both scenes if they are not already listed, then choose **Build** or **Build And Run**.

## 📜 License

MIT, see [LICENSE](LICENSE).
