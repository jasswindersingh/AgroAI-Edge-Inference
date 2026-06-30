[![Unity Version](https://camo.githubusercontent.com/5c47c410f05a5e9a57b8014b6ea2a76a3d514610a944934d6614ae107765eddf/68747470733a2f2f696d672e736869656c64732e696f2f62616467652f556e6974792d323032322e332532422d626c75652e7376673f7374796c653d666c61742d737175617265266c6f676f3d756e697479)](https://unity.com/) [![Platform](https://img.shields.io/badge/Platform-Android-brightgreen.svg)](https://developer.android.com/) [![License](https://camo.githubusercontent.com/458425f8985b0b0c8a736cffe75e05a098e3d77906acddbcad2bfc54492a4e02/68747470733a2f2f696d672e736869656c64732e696f2f62616467652f4c6963656e73652d4d49542d677265656e2e7376673f7374796c653d666c61742d737175617265)](https://github.com/jasswindersingh/Robots-Dungeon_3D/blob/main/LICENSE)

# AgroAI: Edge-Inference AR Mobile Application

Real-time offline agricultural diagnostics powered by Unity Sentis and edge AI. AgroAI is a mobile AR application engineered for remote field use, delivering instant crop, disease, soil, and tree intelligence without cloud dependency.

> A polished Unity AR solution for edge inference, ecological diagnostics, and precision farming analytics in disconnected environments.

---

## Overview

AgroAI combines high-performance Unity Sentis inference with a sleek glassmorphism interface to bring four edge AI pipelines directly to Android devices. The application processes camera feeds and environmental inputs locally, enabling fast, reliable agricultural insights in areas without internet access.

---

## Core Subsystems

### 1. Soil Profiler Module

- Input: 1×7 tensor vector
- Parameters: Nitrogen, Phosphorus, Potassium, Temperature, Humidity, pH, Rainfall
- Output: Crop recommendation pairing from a 22-class optimization matrix
- UI: Guided slider arrays with dynamic circular ring indicators

### 2. Disease Scanner Engine

- Input: 1×3×224×224 camera buffer viewport
- Functionality: On-device convolutional inference for foliage pathology detection
- Output: Copper fungicide or pruning treatment recommendations

### 3. Crop Classifier Core

- Input: 1×3×224×224 camera buffer viewport
- Functionality: Real-time crop classification for Cotton, Jute, Maize, Rice, and Wheat
- Output: Field crop identification with localized metric translation

### 4. Tree Identifier Matrix

- Input: 1×3×224×224 AR viewport camera stream
- Functionality: 48-class regional flora classification from raw canopy image data
- Output: Instant tree species identification

---

## Technical Architecture

- Inference Engine: `com.unity.sentis`
- Model Format: ONNX (`.onnx`)
- Runtime: On-device CPU/GPU edge execution
- Image Resolution: 224×224 normalized tensors
- UI Framework: Unity UI with CanvasGroup alpha blending and radial 360 gauge visuals

---

## Workspace Layout

```text
📂 AgroAI-Edge-Inference
 ├── 📂 Assets
 │    ├── 📂 Models
 │    ├── 📂 Scripts
 │    └── 📂 UI
 ├── 📂 AgroAI_Workspace
 │    └── 📂 Data
 │         └── 📂 src
 └── README.md
```

---

## Developer Setup

### Prerequisites

- Unity Editor 2023.2 or later
- Android Build Support installed
- TextMeshPro package enabled
- Android device or emulator for testing

### Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/jasswindersingh/AgroAI-Edge-Inference.git
   ```
2. Open the project in Unity Hub.
3. Install the Sentis package:
   - Window ➜ Package Manager
   - Add package by name...
   - Enter `com.unity.sentis`
4. Configure the primary scene and controllers.

### Model Integration

1. Open the main scene and locate the `_MasterARController` GameObject.
2. Select the `NewBehaviourScript` component in the Inspector.
3. Drag each `.onnx` model asset into its corresponding model slot.
4. Assign UI panels, `CanvasGroup` references, and `RectTransform` targets for smooth transitions.
5. Set radial gauge `Image` components to `Filled` / `Radial 360` mode.

---

## Build & Deployment

1. Open **File ➜ Build Settings**.
2. Choose `Android` and click **Switch Platform**.
3. Confirm Android SDK/NDK settings and minimum API levels.
4. Click **Build and Run** to deploy the APK to a connected device.

---

## Run the Prototype (No Coding Required)

1. Copy `AgroAI_Prototype.apk` to an Android device.
2. Enable installation from unknown sources if prompted.
3. Install and launch the app.
4. Grant camera permissions and begin using the AR diagnostics features.

---

## Recommended Testing Flow

- Validate the soil profiler by entering known environmental values.
- Scan healthy and diseased leaves to confirm detector accuracy.
- Run the crop classifier on sample plants for all five crop categories.
- Test the tree identifier on local canopy specimens.

---

## Author

- **Jasswinder Singh** — Lead Core Architecture, Machine Learning Pipelines & Model Serialization Engineering

## Mentor

- **Anshuman Nayak** — Project Mentor
  - GitHub: https://github.com/anshumanfs

---

## Contact

For implementation details, model updates, or deployment support, reach out to the project maintainer through the repository contact information.




