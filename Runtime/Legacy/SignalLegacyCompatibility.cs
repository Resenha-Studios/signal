using System;
using UnityEngine;

namespace SignalAudioManagerUnity.Communication
{
    [Obsolete("SignalAudioManagerUnity.Communication.Signal is deprecated. Please use Resenha.Signal.Signal instead.")]
    public static class Signal
    {
        public static long PlaySFX(string audioID, float volumeMultiplier = 1f, float pitchMultiplier = 1f, bool triggerDucking = false, float duckingDuration = 1f)
            => Resenha.Signal.Signal.PlaySFX(audioID, volumeMultiplier, pitchMultiplier, triggerDucking, duckingDuration);

        public static long PlaySFX(string audioID, Vector3 position, float volumeMultiplier = 1f, float pitchMultiplier = 1f, bool triggerDucking = false, float duckingDuration = 1f)
            => Resenha.Signal.Signal.PlaySFX(audioID, position, volumeMultiplier, pitchMultiplier, triggerDucking, duckingDuration);

        public static long PlaySFX(string audioID, Transform targetTransform, float volumeMultiplier = 1f, float pitchMultiplier = 1f, bool triggerDucking = false, float duckingDuration = 1f)
            => Resenha.Signal.Signal.PlaySFX(audioID, targetTransform, volumeMultiplier, pitchMultiplier, triggerDucking, duckingDuration);

        public static long PlaySFXDelayed(string audioID, float delayInSeconds, float volumeMultiplier = 1f, float pitchMultiplier = 1f)
            => Resenha.Signal.Signal.PlaySFXDelayed(audioID, delayInSeconds, volumeMultiplier, pitchMultiplier);

        public static long PlayDelayedSFX(string audioID, float delayInSeconds, float volumeMultiplier = 1f, float pitchMultiplier = 1f)
            => Resenha.Signal.Signal.PlayDelayedSFX(audioID, delayInSeconds, volumeMultiplier, pitchMultiplier);

        public static long PlaySFXLoop(string audioID, Transform target, float volumeMultiplier = 1f)
            => Resenha.Signal.Signal.PlaySFXLoop(audioID, target, volumeMultiplier);

        public static long PlayUI(string audioID, float volumeMultiplier = 1f, bool triggerDucking = false, float duckingDuration = 1f)
            => Resenha.Signal.Signal.PlayUI(audioID, volumeMultiplier, triggerDucking, duckingDuration);

        public static long PlayUI(string audioID, float volumeMultiplier, float pitchMultiplier)
            => Resenha.Signal.Signal.PlayUI(audioID, volumeMultiplier, pitchMultiplier);

        public static long PlayUI(string audioID, float volumeMultiplier, float pitchMultiplier, bool triggerDucking, float duckingDuration = 1f)
            => Resenha.Signal.Signal.PlayUI(audioID, volumeMultiplier, pitchMultiplier, triggerDucking, duckingDuration);

        public static void PlayMusic(string audioID, float fadeDuration = 2.0f, float targetVolume = 1f)
            => Resenha.Signal.Signal.PlayMusic(audioID, fadeDuration, targetVolume);

        public static void StopMusic()
            => Resenha.Signal.Signal.StopMusic();

        public static void StopMusic(float fadeDuration = 1f)
            => Resenha.Signal.Signal.StopMusic(fadeDuration);

        public static void PauseMusic()
            => Resenha.Signal.Signal.PauseMusic();

        public static void ResumeMusic()
            => Resenha.Signal.Signal.ResumeMusic();

        public static void StopInstance(long instanceId)
            => Resenha.Signal.Signal.StopInstance(instanceId);

        public static void PauseInstance(long instanceId)
            => Resenha.Signal.Signal.PauseInstance(instanceId);

        public static void ResumeInstance(long instanceId)
            => Resenha.Signal.Signal.ResumeInstance(instanceId);

        public static void SetMasterVolume(float normalizedVolume)
            => Resenha.Signal.Signal.SetMasterVolume(normalizedVolume);

        public static void SetMusicVolume(float normalizedVolume)
            => Resenha.Signal.Signal.SetMusicVolume(normalizedVolume);

        public static void SetSFXVolume(float normalizedVolume)
            => Resenha.Signal.Signal.SetSFXVolume(normalizedVolume);

        public static void SetUIVolume(float normalizedVolume)
            => Resenha.Signal.Signal.SetUIVolume(normalizedVolume);
    }

    [Obsolete("SignalAudioManagerUnity.Communication.AudioCategory is deprecated. Please use Resenha.Signal.Communication.AudioCategory instead.")]
    public enum AudioCategory
    {
        Master = Resenha.Signal.Communication.AudioCategory.Master,
        Music = Resenha.Signal.Communication.AudioCategory.Music,
        SFX = Resenha.Signal.Communication.AudioCategory.SFX,
        UI = Resenha.Signal.Communication.AudioCategory.UI
    }

    [Obsolete("SignalAudioManagerUnity.Communication.AudioClipConfig is deprecated. Please use Resenha.Signal.Communication.AudioClipConfig instead.")]
    public class AudioClipConfig : Resenha.Signal.Communication.AudioClipConfig { }

    [Obsolete("SignalAudioManagerUnity.Communication.AudioEventChannel is deprecated. Please use Resenha.Signal.Communication.AudioEventChannel instead.")]
    public static class AudioEventChannel
    {
        public static void RaisePlayAudio(Resenha.Signal.Communication.AudioClipConfig config) => Resenha.Signal.Communication.AudioEventChannel.RaisePlayAudio(config);
        public static void RaiseStopMusic() => Resenha.Signal.Communication.AudioEventChannel.RaiseStopMusic();
        public static void RaisePauseMusic() => Resenha.Signal.Communication.AudioEventChannel.RaisePauseMusic();
        public static void RaiseResumeMusic() => Resenha.Signal.Communication.AudioEventChannel.RaiseResumeMusic();
        public static void RaiseStopInstance(long instanceId) => Resenha.Signal.Communication.AudioEventChannel.RaiseStopInstance(instanceId);
        public static void RaisePauseInstance(long instanceId) => Resenha.Signal.Communication.AudioEventChannel.RaisePauseInstance(instanceId);
        public static void RaiseResumeInstance(long instanceId) => Resenha.Signal.Communication.AudioEventChannel.RaiseResumeInstance(instanceId);
        public static void RaiseSetGroupVolume(Resenha.Signal.Communication.AudioCategory category, float normalizedVolume) => Resenha.Signal.Communication.AudioEventChannel.RaiseSetGroupVolume(category, normalizedVolume);
    }
}

namespace SignalAudioManagerUnity.Core
{
    [Obsolete("SignalAudioManagerUnity.Core.SoundManager is deprecated. Please use Resenha.Signal.Core.SoundManager instead.")]
    public class SoundManager : Resenha.Signal.Core.SoundManager { }

    [Obsolete("SignalAudioManagerUnity.Core.PooledAudioSource is deprecated. Please use Resenha.Signal.Core.PooledAudioSource instead.")]
    public class PooledAudioSource : Resenha.Signal.Core.PooledAudioSource { }

    [Obsolete("SignalAudioManagerUnity.Core.AmbientSoundManager is deprecated. Please use Resenha.Signal.Core.AmbientSoundManager instead.")]
    public class AmbientSoundManager : Resenha.Signal.Core.AmbientSoundManager { }
}

namespace SignalAudioManagerUnity.Data
{
    [Obsolete("SignalAudioManagerUnity.Data.SoundManagerSO is deprecated. Please use Resenha.Signal.Data.SoundManagerSO instead.")]
    public class SoundManagerSO : Resenha.Signal.Data.SoundManagerSO { }

    [Obsolete("SignalAudioManagerUnity.Data.AudioEntry is deprecated. Please use Resenha.Signal.Data.AudioEntry instead.")]
    public class AudioEntry : Resenha.Signal.Data.AudioEntry { }

    [Obsolete("SignalAudioManagerUnity.Data.AmbientTypeSO is deprecated. Please use Resenha.Signal.Data.AmbientTypeSO instead.")]
    public class AmbientTypeSO : Resenha.Signal.Data.AmbientTypeSO { }

    [Obsolete("SignalAudioManagerUnity.Data.AmbientSoundConfig is deprecated. Please use Resenha.Signal.Data.AmbientSoundConfig instead.")]
    public class AmbientSoundConfig : Resenha.Signal.Data.AmbientSoundConfig { }

    [Obsolete("SignalAudioManagerUnity.Data.SurfaceSoundMapSO is deprecated. Please use Resenha.Signal.Data.SurfaceSoundMapSO instead.")]
    public class SurfaceSoundMapSO : Resenha.Signal.Data.SurfaceSoundMapSO { }
}

namespace SignalAudioManagerUnity.Components
{
    [Obsolete("SignalAudioManagerUnity.Components.PlaySFX is deprecated. Please use Resenha.Signal.Components.PlaySFX instead.")]
    public class PlaySFX : Resenha.Signal.Components.PlaySFX { }

    [Obsolete("SignalAudioManagerUnity.Components.PlayMusic is deprecated. Please use Resenha.Signal.Components.PlayMusic instead.")]
    public class PlayMusic : Resenha.Signal.Components.PlayMusic { }

    [Obsolete("SignalAudioManagerUnity.Components.FootstepController is deprecated. Please use Resenha.Signal.Components.FootstepController instead.")]
    public class FootstepController : Resenha.Signal.Components.FootstepController { }

    [Obsolete("SignalAudioManagerUnity.Components.AmbientTrigger is deprecated. Please use Resenha.Signal.Components.AmbientTrigger instead.")]
    public class AmbientTrigger : Resenha.Signal.Components.AmbientTrigger { }

    [Obsolete("SignalAudioManagerUnity.Components.AmbientTrigger2D is deprecated. Please use Resenha.Signal.Components.AmbientTrigger2D instead.")]
    public class AmbientTrigger2D : Resenha.Signal.Components.AmbientTrigger2D { }
}
