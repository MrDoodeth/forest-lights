# Forest Lights

A third-person isometric survival game built with Unity. Explore a procedurally generated forest, gather resources, defend yourself from bears, light the altars, and reach the exit.

This branch contains the Android-oriented version of the project.

## 📦 Features

- Procedurally generated 40×40 forest map with trees, berry bushes, grass, flowers, altars, and bear spawns.
- Resource gathering: chop trees for wood and collect berries to restore health.
- Player health, stamina, sprinting, combat, respawning, pause, win, and lose flows.
- Bear AI using Unity NavMesh for patrol, chase, and attack behavior.
- Day/night lighting, sound effects and music, adjustable quality/render scale, and saved audio settings.
- English and Russian localization.
- Mobile controls and Yandex Mobile Ads: banner, interstitial, rewarded, and app-open ads.

## 📋 Requirements

- Unity `2021.3.25f1`.
- Android Build Support for Unity when building an Android package.

## 🚀 Build and run

```bash
git clone https://github.com/MrDoodeth/forest-lights.git
cd forest-lights
git switch Android-build
```

Open the `Ramake survavium` folder in Unity Hub with Unity `2021.3.25f1`. The playable scenes are:

- `Assets/Scenes/Menu/Menu.unity`
- `Assets/Scenes/Survavium/Survavium.unity`

To build an Android package, open **File → Build Settings**, select **Android**, add both scenes if they are not already listed, then choose **Build** or **Build And Run**.

## 📜 License

MIT, see [LICENSE](LICENSE).
