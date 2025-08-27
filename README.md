# MetaPassthrough-VR-System

A Unity-based VR/MR prototype that integrates **Meta Passthrough Camera API**, **OpenXR**, and **XR Interaction Toolkit**.  
It adds **multi-object detection**, **marker-based interaction**, and **dynamic passthrough windows** so users can reveal and manipulate real-world objects inside a VR scene—ideal for **VR teaching**, **remote experiments**, and **collaborative learning**.

---

## Table of Contents
1. [Overview](#overview)  
2. [Features](#features)  
3. [Tech Stack](#tech-stack)  
4. [Requirements](#requirements)  
5. [Project Structure](#project-structure)  
6. [Setup & Installation](#setup--installation)  
7. [Build & Run (Quest)](#build--run-quest)  
8. [How to Use](#how-to-use)  
9. [Demo](#demo)  
10. [Known Limitations](#known-limitations)  
11. [Roadmap](#roadmap)  
12. [Acknowledgements](#acknowledgements)  
13. [License](#license)

---

## Overview

This project extends Meta’s Passthrough Camera API samples by enabling an **interactive passthrough workflow**:
- The system **detects real-world objects** (e.g., laptop, tools) and shows **labeled bounding boxes** with **markers** in VR.
- Clicking a marker **creates a Passthrough window** (a quad using Meta passthrough) sized and oriented to the object so the **real object appears clearly** in VR.
- Each window **faces the user** initially for better visibility and can be **grabbed/rotated/scaled**.  
- Users can **pause detection**, **clear markers**, **delete one/all windows**, and **hover a marker** to preview its passthrough window before opening.

Use cases include: **classroom demonstrations**, **lab collaborations**, **remote guidance**, and **mixed-reality prototyping**.

---

## Features

- **Automatic Object Recognition**  
  On-device ML recognizes common objects and attaches 2D boxes + VR markers.

- **Marker → Passthrough Window**  
  Click a marker to spawn a **custom-sized Passthrough window** aligned to the object’s real-world extents and orientation.

- **Natural Manipulation**  
  Grab, rotate, and scale windows via **hands** or **controllers** (XR Interaction Toolkit).

- **Flexible Controls**  
  Pause/resume recognition, hover-to-preview, clear markers/boxes, delete one/all windows.

- **Designed for VR Learning & Remote Experimentation**  
  See and operate real tools while discussing in VR.

---

## Tech Stack

- **Unity**: 2022.3 LTS or Unity 6 (6000.x)  
- **XR**: OpenXR + XR Interaction Toolkit  
- **Meta**: Passthrough Camera API, MR Utility Kit (MRUK)  
- **On-device ML (optional)**: Unity Sentis (e.g., YOLO-based object detection)

---

## Requirements

### Software
- **Unity**: 2022.3 LTS or Unity 6 (e.g., 6000.0.38f1)  
- **Packages**:  
  - OpenXR Plugin  
  - XR Interaction Toolkit  
  - Meta OpenXR Feature Group(s) enabled (Passthrough)  
  - Meta MR Utility Kit (MRUK) v74+  
  - Unity Sentis v2.1.1+ (if using the detection sample)

### Hardware
- **Meta Quest 3 / Quest 3S**  
- **Horizon OS v74+**

### Permissions
- `android.permission.CAMERA`  
- `horizonos.permission.HEADSET_CAMERA` (Meta passthrough permission)

> **Note (Unity 6)**: If you upgrade to Unity 6, ensure your **Android Manifest** uses `GameActivity` and that passthrough-related permissions/themes are present. Meta > Tools includes helpers to update the manifest.

---

## Project Structure

```
MetaPassthrough-VR-System/
├── Assets/
│   ├── Brick Project Studio/
│   ├── CompositionLayers/
│   ├── MetaXR/
│   ├── Oculus/
│   ├── PassthroughCameraApiSamples/
│   │   ├── DeleteButton.cs
│   │   ├── MarkerClickToPassthrough.cs
│   │   ├── PassthroughProjectionSurface.cs
│   │   └── ...
│   ├── Plugins/
│   ├── Resources/
│   ├── Settings/
│   ├── XR/
│   ├── XRI/
│   └── ...
│
├── Media/
│   └── demo.gif
│
├── Packages/
├── ProjectSettings/
│
├── .gitignore
├── CODE_OF_CONDUCT.md
├── CONTRIBUTING.md
├── LICENSE
├── README.md
└── TempAssembly.dll

```

> Use a Unity `.gitignore` to exclude `Library/`, `Temp/`, `Logs/`, `Build/`, and large binaries (e.g., `*.apk`).

---

## Setup & Installation

1. **Clone**
   ```bash
   git clone https://github.com/15611257356/MetaPassthrough-VR-System.git
   cd MetaPassthrough-VR-System
   ```

2. **Open in Unity** (2022.3 LTS or Unity 6).

3. **Meta Project Setup Tool**  
   In Unity: **Meta > Tools > Project Setup Tool** → Fix/Apply all recommended settings.

4. **OpenXR Settings**
   - Project Settings → XR Plug-in Management → **OpenXR** as active for Android.
   - Enable Meta feature groups as needed (e.g., **Passthrough**).

5. **Packages**
   - Ensure **XR Interaction Toolkit**, **Meta MRUK**, and **Unity Sentis** (if needed) are installed.

6. **Android Build Support**  
   Install via Unity Hub if not present.

---

## Build & Run (Quest)

1. **Switch Platform**: File → Build Settings → Platform: **Android** → *Switch Platform*  
2. **Scenes**: Add your main scene(s) to *Scenes In Build*.  
3. **Player Settings** (Android):  
   - Identification → Package Name (e.g., `com.example.metapassthrough`)  
   - Minimum API: follow Meta recommendations for your Unity/SDK version  
4. **Build & Run** to a connected Quest device (USB or over Wi‑Fi).

> If Unity 6: verify manifest updates (GameActivity, theme) and passthrough permissions. Use **Meta > Tools** helpers if needed.

---

## How to Use

- Start the app in the headset.  
- The system detects objects and shows **markers** and **2D boxes**.  
- **Click a marker** → creates a **Passthrough window** aligned to the object.  
- **Manipulation**: grab/rotate/scale windows with hands or controllers.  
- **Controls (example mapping)**:  
  - **A**: Confirm/Create window from highlighted marker  
  - **B**: Pause/Resume recognition  
  - **Y**: Delete all windows (hold to confirm)  
  - **Hover marker**: preview a small passthrough window (no creation)  
- **Utilities**: clear markers/boxes, delete single window via its close button.

> Control bindings are configurable via XR Interaction Toolkit—adjust to your project’s input actions if they differ.

---

## Demo

### Full Demo Video
[video1.mp4](./Demo/Video demo1.mp4)
[video2.mp4](./Demo/Video demo2.mp4)

---

## Known Limitations

- **Unity WebCamTexture constraints** (in Meta samples): rectangular capture region, limited resolution, timing offsets.  
- **Single-camera access** at a time in certain flows.  
- **On-device ML performance**: Sentis inference can impact frame time; consider async/layered updates.  
- **Detection accuracy**: may misclassify similar objects; trained on ~80 classes.  
- **Environment Depth + OpenXR**: depth features require Unity 6 with proper OpenXR setup.

---

## Roadmap

- Optimize window interaction & smoothing for hand tracking.  
- Improve object alignment between 2D boxes and 3D anchors.  
- Multi-user collaboration / networking.  
- Editor tooling for faster parameter tuning (window size/orientation presets).

---

## Acknowledgements

- Built on top of concepts from **Meta’s Unity Passthrough Camera API Samples**.  
- Thanks to the Unity and Meta XR communities for guidance and tools.

---

## License

- **Custom code in this repository**: MIT (unless otherwise noted).  
- **Upstream/third-party samples or assets**: retain their original licenses.  
See the `LICENSE` file(s) for details.
