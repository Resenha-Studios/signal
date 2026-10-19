<p align="center">
  <h1 align="center">🎵 Signal</h1>
  <p align="center">
    <strong>Manage and play audio in Unity effortlessly — Zero setup, high performance, and modern UI Toolkit!</strong><br>
    <em>Gerencie e toque áudios, músicas e efeitos sonoros na Unity de forma facilitada!</em>
  </p>
  <p align="center">
    <a href="https://github.com/Resenha-Studios/signal/blob/main/LICENSE.md"><img src="https://img.shields.io/badge/license-MIT-blue.svg" alt="License MIT"></a>
    <img src="https://img.shields.io/badge/Unity-6000.0%2B-black.svg?logo=unity" alt="Unity Version"></a>
    <img src="https://img.shields.io/badge/UI-UI%20Toolkit-informational.svg" alt="UI Toolkit"></a>
    <img src="https://img.shields.io/badge/PRs-Welcome-brightgreen.svg" alt="PRs Welcome"></a>
    <img src="https://img.shields.io/badge/Localization-5%20Languages-orange.svg" alt="Localization"></a>
  </p>
</p>

---

## ⚡ What is Signal? / O que é o Signal?

> ### 🎯 In a Nutshell
> **Signal is an open-source tool to manage, organize, and play audio in Unity effortlessly!**
>
> Say goodbye to dragging manager prefabs, AudioSources, and audio clips into every single scene. With Signal, you trigger any sound with a single clean line of code:
> ```csharp
> Signal.PlaySFX("jump"); // Plays instantly anywhere!
> ```
> Signal takes care of everything behind the scenes:
> - 🚀 **Zero-Setup Runtime**: Automatically instantiates audio hosts and pools on first call. Never pollutes your scenes.
> - 🎛️ **Modern UI Toolkit Dashboard**: Manage SFX, Music, and UI sounds from **`Tools > Resenha > Signal`** with instant search, pagination, and real-time audio preview.
> - 🔊 **Surface-Aware Footsteps & 3D Audio**: Raycast-based footstep engine detecting terrain and physics materials, plus 3D falloff and smart ducking.
> - 🛡️ **Typo-Proof Autocomplete**: Auto-generates type-safe constants (`AudioKeys.SFX.jump`).
>
> ---
>
> *🇧🇷 **Em poucas palavras:** O Signal é uma ferramenta para **gerenciar e tocar áudios na Unity de forma facilitada!** Em vez de configurar AudioSources manuais e managers em cada cena, basta chamar `Signal.PlaySFX("jump")` em qualquer script. Ele cuida do pooling, mixagem, crossfade de músicas e oferece um dashboard moderno em UI Toolkit para organizar todos os seus sons com facilidade.*

---

## 🚀 How to Open

Open the Signal Dashboard directly from the top menu:  
👉 **`Tools > Resenha > Signal`** (or press **`Ctrl + Shift + S`** / **`Cmd + Shift + S`**).

---

## 📦 Key Features

- **Zero-Setup Architecture**:
  - Automatically loads and instantiates the audio host on-demand from Resources.
  - Scene-independent: persist seamlessly across scene loads with `DontDestroyOnLoad`.
- **Clean UI Toolkit Dashboard**:
  - Built 100% with Unity UI Toolkit (UXML + USS) adhering to AAA minimal design principles.
  - Instant library search, pagination, bulk imports, and real-time audio playback previews.
- **Multilingual Editor Support**:
  - Full native localization in 5 languages: **English**, **Português**, **Español**, **日本語**, and **简体中文**.
- **High-Performance Object Pooling**:
  - Zero-allocation audio pools for high-frequency SFX and UI feedback to prevent GC spikes.
- **Surface-Aware Footstep Engine**:
  - Raycast-driven footstep system dynamically detecting Physics Materials and Terrain Texture Layers.
- **Ambient Trigger Zones (3D & 2D)**:
  - Trigger volume areas with customizable crossfade curves and distance falloff.
- **3D Spatial Audio & Smart Ducking**:
  - Fine-grained control over Doppler levels, speaker spread, and 3D rolloff curves.
  - Automatic music ducking when priority voice lines or important sound cues trigger.
- **Strongly-Typed Code Generator (`AudioKeys`)**:
  - Automatically generates C# constant key classes (`AudioKeys.SFX.*`, `AudioKeys.Music.*`) to eliminate runtime typos.
- **100% Backward Compatibility**:
  - Full compatibility proxy layer and Unity `[MovedFrom]` serialization attributes to seamlessly upgrade existing projects from `SignalAudioManagerUnity` v1.x with zero data loss.

---

## 🚀 Installation

### Via Unity Package Manager (Git URL)

1. Open your Unity project.
2. Go to **Window > Package Manager**.
3. Click the **`+`** button in the top-left corner and choose **Add package from git URL...**.
4. Enter the repository URL:
   ```text
   https://github.com/Resenha-Studios/signal.git
   ```
5. Click **Add**.

Alternatively, add the dependency directly to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.resenha.signal": "https://github.com/Resenha-Studios/signal.git"
  }
}
```

---

## 🛠️ Quick Start

### 1. Initialize Signal Assets

1. Open the dashboard via **`Tools > Resenha > Signal`**.
2. Click **Initialize Signal** to automatically generate your Audio Mixers, SFX Prefabs, and Configuration assets in your desired folder.

### 2. Playing Audio in Code

```csharp
using Resenha.Signal;
using UnityEngine;

public class AudioExample : MonoBehaviour
{
    private void Start()
    {
        // Play music with smooth crossfading
        Signal.PlayMusic("exploration_theme", fadeDuration: 2.0f);
    }

    public void OnPlayerJump()
    {
        // Play a 2D sound effect
        Signal.PlaySFX("jump");

        // Or using generated type-safe keys:
        // Signal.PlaySFX(AudioKeys.SFX.jump);
    }

    public void OnExplosion(Vector3 worldPosition)
    {
        // Play 3D spatial sound effect at position with smart ducking
        Signal.PlaySFX("explosion", worldPosition, triggerDucking: true, duckingDuration: 1.5f);
    }

    public void OnButtonHover()
    {
        // Play UI sound effect (ignores Time.timeScale)
        Signal.PlayUI("button_hover");
    }

    public void OnDelayedAction()
    {
        // Play sound after a delay (e.g. syncing with an animation)
        Signal.PlaySFXDelayed("spell_cast", delayInSeconds: 0.75f);
    }
}
```

### 3. Settings Menu Integration (Mixer Control)

Easily link your volume sliders to the Master Audio Mixer with normalized values (`0.0f` to `1.0f`):

```csharp
using Resenha.Signal;

public class SettingsMenu
{
    public void OnMasterVolumeChanged(float value) => Signal.SetMasterVolume(value);
    public void OnMusicVolumeChanged(float value) => Signal.SetMusicVolume(value);
    public void OnSFXVolumeChanged(float value) => Signal.SetSFXVolume(value);
    public void OnUIVolumeChanged(float value) => Signal.SetUIVolume(value);
}
```

---

## 🔄 Migrating from v1.x (Backward Compatibility)

Signal v2.0 introduces clean namespaces under `Resenha.Signal.*`. To ensure existing projects using v1.x do not break:

1. **API Calls**: Static calls to `Signal.PlaySFX(...)` or `Signal.PlayMusic(...)` remain fully compatible.
2. **Old Namespaces**: `using SignalAudioManagerUnity.Communication;`, `SignalAudioManagerUnity.Core;`, `SignalAudioManagerUnity.Data;`, and `SignalAudioManagerUnity.Components;` are supported via backward-compatible proxy classes with `[Obsolete]` guides.
3. **Scenes & Prefabs**: All component types (`SoundManager`, `PlaySFX`, `PlayMusic`, `FootstepController`, `AmbientTrigger`, etc.) utilize Unity's `[MovedFrom]` serialization mapping, preserving all existing scene and prefab references without missing scripts.

---

## 🌐 Localization

The Signal Dashboard supports 5 languages out of the box:
- 🇺🇸 **English**
- 🇧🇷 **Português**
- 🇪🇸 **Español**
- 🇯🇵 **日本語**
- 🇨🇳 **简体中文**

Switch languages anytime using the dropdown selector in the top-right corner of the dashboard header.

---

## 🤝 Contributing

Contributions, bug reports, and suggestions are warmly welcome!
1. Fork the Project.
2. Create your Feature Branch (`git checkout -b feat/amazing-feature`).
3. Commit your Changes (`git commit -m "feat: add amazing feature"`).
4. Push to the Branch (`git push origin feat/amazing-feature`).
5. Open a Pull Request.

---

## 📄 License

Distributed under the **MIT License**. See [`LICENSE.md`](./LICENSE.md) for more details.

Developed with ❤️ by **[Resenha Studios](https://github.com/Resenha-Studios)**.
