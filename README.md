# AgroAI: Edge-Inference AR Mobile Application

An AI-powered mobile assistant built using **Unity Sentis** for real-time agricultural diagnostics. The application runs four distinct Deep Learning models (`.onnx`) directly on your phone, completely offline without requiring an internet connection.

---

## Core Features

* ** Soil Profiler:** Enter 7 parameters (N, P, K, Temperature, Humidity, pH, Rainfall) to get instant crop recommendations.
* ** Disease Scanner:** Point the camera at a plant leaf to detect diseases and get treatment remedies.
* ** Crop Classifier:** Real-time camera recognition for core field crops (Cotton, Jute, Maize, Rice, Wheat).
* ** Tree Identifier:** Instant camera identification for regional tree species.

---

##  Quick Setup Guide for Developers

1. **Clone the Repo:**
   ```bash
   git clone [https://github.com/jasswindersingh/AgroAI-Edge-Inference.git](https://github.com/jasswindersingh/AgroAI-Edge-Inference.git)

Open in Unity: Open the project folder using Unity Editor (2023.2 or later) with Android Build Support installed.

Install Sentis Engine: Go to Window ➔ Package Manager ➔ Add package by name, type com.unity.sentis, and click Add.

Link the Script Components: Select the _MasterARController GameObject in the scene hierarchy and drag your 4 .onnx models, UI panels, sliders, and text boxes into their matching slots in the Inspector.

Build for Android: Go to File ➔ Build Settings, switch the platform to Android, and click Build and Run.


How to Test the Mobile Demo (No Coding Required)
If you just want to try out the app directly on your Android phone:

Download the AgroAI_Prototype.apk file.

Install the APK file on your smartphone (allow installation from unknown sources if your phone asks).

Open the app, grant Camera Permission when prompted, and start scanning!




