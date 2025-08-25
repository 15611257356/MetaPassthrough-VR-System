MetaPassthrough-VR-System

This project implements a Meta Passthrough VR System in Unity (OpenXR + XR Interaction Toolkit).
It extends Meta’s Passthrough Camera API samples by integrating object detection, marker-based interaction, and dynamic passthrough windows, enabling users to naturally interact with both virtual content and real-world objects.

📖 Table of Contents

Project Overview

Key Features

Requirements

Installation & Setup

How to Use

Demo

Limitations & Future Work

License

🚀 Project Overview

This system leverages the Meta Passthrough Camera API to build a hybrid VR/AR prototype for teaching, remote collaboration, and lab scenarios.
It enables users to:

Recognize real-world objects via on-device ML (Unity Sentis).

Dynamically create passthrough windows aligned with detected objects.

Interact with passthrough windows (move, rotate, scale, preview, delete).

Seamlessly blend the VR world with real-world objects for immersive learning and collaboration.

✨ Key Features

Automatic Object Detection: Recognizes common objects (laptops, tools, etc.) and attaches bounding boxes + markers in VR.

Marker-based Interaction: Users can click markers to open passthrough windows.

Dynamic Passthrough Windows:

Custom-sized and oriented to match the real object.

Always face the user initially.

Can be grabbed, rotated, scaled, and repositioned.

Flexible Controls:

Pause/resume recognition.

Delete single or all windows.

Hover preview before opening windows.

Use Cases: VR teaching, collaborative lab experiments, remote assistance.

⚙️ Requirements
Software

Unity 2022.3 LTS or Unity 6 (6000.x)

Meta OpenXR Plugin (2.1.0 or newer)

Meta MR Utility Kit (MRUK) v74.0.0+

Unity Sentis v2.1.1+

Hardware

Meta Quest 3 / Quest 3S

Horizon OS v74+

Permissions

android.permission.CAMERA

horizonos.permission.HEADSET_CAMERA

🛠 Installation & Setup

Clone this repository:

git clone https://github.com/15611257356/MetaPassthrough-VR-System.git


Open the project in Unity 2022.3 LTS.

Go to Meta > Tools > Project Setup Tool and apply recommended settings.

Ensure Android Build Support is enabled.

Connect your Meta Quest 3 headset and build to device.

🎮 How to Use

Run the application inside Quest 3.

Look around your environment — detected objects will show bounding boxes and markers.

Click on a marker to open a passthrough window of the object.

Use controllers to:

Grab & Move the window.

Rotate & Scale the window.

Hover on marker to preview before creating a window.

Pause recognition or clear markers/windows via menu controls.

📹 Demo

You can showcase your results with short demo clips.
👉 Just upload your demo videos (.mp4, .gif) to a Media/ folder in your repo, then reference them here:

Example (replace with your files):

### Object Detection & Marker Placement
![Object Detection](./Media/object_detection.gif)

### Dynamic Passthrough Window
![Passthrough Window Demo](./Media/passthrough_window.gif)

### Full Demo Video
[![Watch the video](./Media/demo_thumbnail.png)](https://user-images.githubusercontent.com/your-demo-link.mp4)


⚠️ 注意：GitHub 不支持直接存储超过 100MB 的视频。如果视频较大，建议上传到 GitHub Releases / Google Drive / YouTube / Bilibili，然后把链接贴在这里。

⚠️ Limitations & Future Work

Performance: Running ML (Sentis YOLO) on-device may affect frame rate.

Detection Accuracy: Misclassification may happen (e.g., phone → remote).

Unity 6 Compatibility: Requires updating Android Manifest manually.

Future Improvements:

Support multiple concurrent windows with shared interaction logic.

Optimize Sentis inference for lower latency.

Enable remote collaboration features.

📜 License

This project builds upon Meta’s Unity Passthrough Camera API Samples and follows their license terms.
Custom code for passthrough window interaction is released under the MIT License.
