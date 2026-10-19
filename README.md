# 🎵 Signal

> Lightweight, zero-setup audio architecture and sound manager for Unity 6+.  
> An open-source production toolkit by **[Resenha Studios](https://github.com/Resenha-Studios)**.

[![Version](https://img.shields.io/badge/version-2.0.0-blue.svg)](https://github.com/Resenha-Studios/signal)
[![Unity](https://img.shields.io/badge/unity-6000.0%2B-black.svg)](https://unity.com)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](./LICENSE.md)
[![UI Toolkit](https://img.shields.io/badge/UI%20Toolkit-Standard-purple.svg)](https://docs.unity3d.com/Manual/UIElements.html)
[![Localization](https://img.shields.io/badge/Languages-EN%20%7C%20PT%20%7C%20ES%20%7C%20JA%20%7C%20ZH-orange.svg)](#localization)

Built on a modern **UI Toolkit Dashboard**, automated **lazy loading**, smart **object pooling**, **3D spatial audio**, **surface footstep raycasting**, and **type-safe audio keys**.

---

## ✨ Features

| Feature | Description |
|---|---|
| **Zero-Setup Runtime** | Never drag manager prefabs into scenes. Signal auto-spawns dynamically on first audio call. |
| **Tools > Resenha > Signal** | Unified Resenha Studio tooling menu with keyboard shortcut (`Ctrl/Cmd + Shift + S`). |
| **Clean UI Toolkit Dashboard** | AAA minimal dark interface with pagination, instant search, and real-time audio previews. |
| **5-Language Localization** | Full editor interface localized in English, Portuguese, Spanish, Japanese, and Chinese. |
| **Surface Footstep Engine** | Detects Physics Materials and Terrain textures dynamically via Raycast for character footsteps. |
| **Ambient Triggers (3D & 2D)** | Crossfading ambient zones with customizable fade in/out curves and exit behaviors. |
| **3D Spatial Audio** | Fine-grained Doppler level, speaker spread angle, and min/max rolloff distances per entry. |
| **Smart Audio Ducking** | Automatically dip background music when dialog or priority sound effects trigger. |
| **Smart Audio Pooling** | Zero-allocation object pooling for high-intensity SFX and UI sounds. |
| **AudioKeys Code Generator** | Generates strongly-typed C# constants (`AudioKeys.SFX.jump`) for IDE autocomplete and zero typos. |
| **Backward Compatible** | Seamless migration path from legacy `SignalAudioManagerUnity` with zero breaking changes. |

---

## 📦 Installation

### Option A: Unity Package Manager via Git URL (Recommended)

1. In Unity, open `Window > Package Manager`.
2. Click `+` and choose **Add package from git URL...**
3. Enter the repository URL:
```
https://github.com/chspDEV/signal-audio-manager-unity.git
```

### Option B: OpenUPM / Manifest

Add the dependency to your `Packages/manifest.json`:
```json
{
  "dependencies": {
    "com.resenha.signal": "https://github.com/chspDEV/signal-audio-manager-unity.git"
  }
}
```

---

## 🚀 Quick Start

### 1. Initial Setup
1. Open the dashboard via **`Tools > Resenha > Signal`**.
2. Click **Initialize Signal** to automatically generate your Audio Mixers, SFX Prefabs, and Configuration assets in your desired folder.

### 2. Playing Audio in Code

```csharp
using Resenha.Signal;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private void Start()
    {
        // 1. Play background music with 2.0s crossfade
        Signal.PlayMusic("exploration_theme", 2.0f);
    }

    public void OnJump()
    {
        // 2. Play 2D sound effect
        Signal.PlaySFX("jump");

        // Or using generated type-safe keys:
        // Signal.PlaySFX(AudioKeys.SFX.jump);
    }

    public void OnExplosion(Vector3 worldPos)
    {
        // 3. Play 3D sound effect at position with ducking
        Signal.PlaySFX("explosion", worldPos, triggerDucking: true, duckingDuration: 1.5f);
    }

    public void OnButtonClick()
    {
        // 4. Play UI sound (never paused by Time.timeScale)
        Signal.PlayUI("btn_click");
    }
}
```

### 3. Volume Sliders in Settings Menus

```csharp
// Simple normalized values (0.0f to 1.0f)
Signal.SetMasterVolume(1.0f);
Signal.SetMusicVolume(0.8f);
Signal.SetSFXVolume(0.9f);
Signal.SetUIVolume(1.0f);
```

---

## 🔄 Migrating from v1.x (Signal Audio Manager)

Signal v2.0 introduces clean namespaces under `Resenha.Signal.*`. To ensure existing projects do not break:

1. **API Calls**: Calling `Signal.PlaySFX(...)` or `Signal.PlayMusic(...)` works out of the box.
2. **Old Namespaces**: `using SignalAudioManagerUnity.Communication;`, `SignalAudioManagerUnity.Core;`, `SignalAudioManagerUnity.Data;`, and `SignalAudioManagerUnity.Components;` are fully supported via backward-compatible proxy types with `[Obsolete]` guides.
3. **Scenes & Prefabs**: All component types (`SoundManager`, `PlaySFX`, `FootstepController`, etc.) utilize Unity's `[MovedFrom]` serialization mapping, preserving all scene and prefab references without data loss.

---

## 🌐 Localization

The Signal Dashboard supports 5 languages out of the box:
- 🇺🇸 English
- 🇧🇷 Português
- 🇪🇸 Español
- 🇯🇵 日本語
- 🇨🇳 简体中文

Switch languages anytime using the dropdown selector in the top right corner of the dashboard.

---

## 📄 License

Distributed under the **MIT License**. Created with ❤️ by **[Resenha Studios](https://github.com/Resenha-Studios)**.
