/* ==============================================================================
 * CLASS: Signal
 * DESCRIPTION: The main static API for triggering audio events across the game. 
 * Includes advanced overloads for volume/pitch overrides, delayed playback, 
 * and global mixer volume controls.
 * ==============================================================================*/

using Resenha.Signal.Core;
using Resenha.Signal.Communication;
using UnityEngine;

namespace Resenha.Signal
{
    /// <summary>
    /// Global entry point for the Signal Audio Manager. 
    /// Use these static methods to play, pause, stop, and tweak audio dynamically.
    /// </summary>
    public static class Signal
    {
        private static long _nextInstanceId = 1;
        private static SoundManager _cachedManager;
        
        /// <summary>
        /// Checks if the SoundManager exists in the current scene. 
        /// If not, it automatically loads and instantiates it from the Resources folder.
        /// </summary>
        private static void EnsureManagerExists()
        {
            if (_cachedManager) return;

            _cachedManager = UnityEngine.Object.FindFirstObjectByType<SoundManager>();

            if (_cachedManager) return;
            var managerPrefab = Resources.Load<GameObject>("Audio_Managers");
                
            if (managerPrefab)
            {
                GameObject instance = UnityEngine.Object.Instantiate(managerPrefab);
                instance.name = "Audio_Managers (Auto-Loaded)";
                    
                UnityEngine.Object.DontDestroyOnLoad(instance);
                    
                _cachedManager = instance.GetComponent<SoundManager>();
                    
                Debug.Log("[Signal Audio] Audio Manager auto-instantiated successfully!");
            }
            else
            {
                Debug.LogError("[Signal Audio] CRITICAL ERROR: 'Audio_Managers' prefab not found in Resources folder. Please open the Signal Dashboard and run the 1-Click Setup.");
            }
        }

        #region Sound Effects (SFX)
        
        /// <summary>
        /// Plays a 2D sound effect with optional dynamic modifiers.
        /// </summary>
        /// <param name="audioID">The unique ID of the audio entry to play.</param>
        /// <param name="volumeMultiplier">Dynamically scales the volume (1f is default).</param>
        /// <param name="pitchMultiplier">Dynamically scales the pitch (1f is default).</param>
        public static long PlaySFX(string audioID, float volumeMultiplier = 1f, float pitchMultiplier = 1f, bool triggerDucking = false, float duckingDuration = 1f)
        {
            EnsureManagerExists();
            long id = _nextInstanceId++;
            var config = new AudioClipConfig 
            { 
                Category = AudioCategory.SFX, 
                AudioID = audioID,
                VolumeMultiplier = volumeMultiplier,
                PitchMultiplier = pitchMultiplier,
                InstanceID = id,
                TriggerDucking = triggerDucking,
                DuckingDuration = duckingDuration
            };
            AudioEventChannel.RaisePlayAudio(config);
            return id;
        }

        /// <summary>
        /// Plays a 3D sound effect at a specific world position.
        /// </summary>
        public static long PlaySFX(string audioID, Vector3 position, float volumeMultiplier = 1f, float pitchMultiplier = 1f, bool triggerDucking = false, float duckingDuration = 1f)
        {
            EnsureManagerExists();
            long id = _nextInstanceId++;
            var config = new AudioClipConfig 
            { 
                Category = AudioCategory.SFX, 
                AudioID = audioID, 
                Position = position,
                VolumeMultiplier = volumeMultiplier,
                PitchMultiplier = pitchMultiplier,
                InstanceID = id,
                TriggerDucking = triggerDucking,
                DuckingDuration = duckingDuration
            };
            AudioEventChannel.RaisePlayAudio(config);
            return id;
        }

        /// <summary>
        /// Plays a 3D sound effect attached to a specific transform (follows the object).
        /// </summary>
        public static long PlaySFX(string audioID, Transform target, float volumeMultiplier = 1f, float pitchMultiplier = 1f, bool triggerDucking = false, float duckingDuration = 1f)
        {
            EnsureManagerExists();
            long id = _nextInstanceId++;
            var config = new AudioClipConfig 
            { 
                Category = AudioCategory.SFX, 
                AudioID = audioID, 
                TargetTransform = target,
                VolumeMultiplier = volumeMultiplier,
                PitchMultiplier = pitchMultiplier,
                InstanceID = id,
                TriggerDucking = triggerDucking,
                DuckingDuration = duckingDuration
            };
            AudioEventChannel.RaisePlayAudio(config);
            return id;
        }

        /// <summary>
        /// Plays a sound effect after a specific delay. Perfect for syncing with animations or particles.
        /// </summary>
        public static long PlaySFXDelayed(string audioID, float delayInSeconds, float volumeMultiplier = 1f, float pitchMultiplier = 1f)
        {
            EnsureManagerExists();
            long id = _nextInstanceId++;
            var config = new AudioClipConfig 
            { 
                Category = AudioCategory.SFX, 
                AudioID = audioID, 
                Delay = delayInSeconds,
                VolumeMultiplier = volumeMultiplier,
                PitchMultiplier = pitchMultiplier,
                InstanceID = id
            };
            AudioEventChannel.RaisePlayAudio(config);
            return id;
        }

        /// <summary>
        /// Alias for PlaySFXDelayed.
        /// </summary>
        public static long PlayDelayedSFX(string audioID, float delayInSeconds, float volumeMultiplier = 1f, float pitchMultiplier = 1f)
            => PlaySFXDelayed(audioID, delayInSeconds, volumeMultiplier, pitchMultiplier);

        /// <summary>
        /// Plays a looping 3D sound effect attached to a transform.
        /// </summary>
        /// <returns>A unique instance ID used to stop this specific loop later.</returns>
        public static long PlaySFXLoop(string audioID, Transform target, float volumeMultiplier = 1f)
        {
            EnsureManagerExists();
            long id = _nextInstanceId++;
            var config = new AudioClipConfig
            {
                Category = AudioCategory.SFX,
                AudioID = audioID,
                TargetTransform = target,
                Loop = true,
                InstanceID = id,
                VolumeMultiplier = volumeMultiplier
            };
            AudioEventChannel.RaisePlayAudio(config);
            return id;
        }
        
        #endregion

        #region UI Sounds
        
        /// <summary>
        /// Plays a 2D user interface sound effect. Ignores 3D spatialization.
        /// </summary>
        public static long PlayUI(string audioID, float volumeMultiplier = 1f, bool triggerDucking = false, float duckingDuration = 1f)
        {
            EnsureManagerExists();
            long id = _nextInstanceId++;
            var config = new AudioClipConfig 
            { 
                Category = AudioCategory.UI, 
                AudioID = audioID,
                VolumeMultiplier = volumeMultiplier,
                InstanceID = id,
                TriggerDucking = triggerDucking,
                DuckingDuration = duckingDuration
            };
            AudioEventChannel.RaisePlayAudio(config);
            return id;
        }

        /// <summary>
        /// Plays a 2D user interface sound effect with volume and pitch multipliers.
        /// </summary>
        public static long PlayUI(string audioID, float volumeMultiplier, float pitchMultiplier)
        {
            EnsureManagerExists();
            long id = _nextInstanceId++;
            var config = new AudioClipConfig 
            { 
                Category = AudioCategory.UI, 
                AudioID = audioID,
                VolumeMultiplier = volumeMultiplier,
                PitchMultiplier = pitchMultiplier,
                InstanceID = id
            };
            AudioEventChannel.RaisePlayAudio(config);
            return id;
        }

        /// <summary>
        /// Plays a 2D user interface sound effect with volume, pitch, and smart ducking options.
        /// </summary>
        public static long PlayUI(string audioID, float volumeMultiplier, float pitchMultiplier, bool triggerDucking, float duckingDuration = 1f)
        {
            EnsureManagerExists();
            long id = _nextInstanceId++;
            var config = new AudioClipConfig 
            { 
                Category = AudioCategory.UI, 
                AudioID = audioID,
                VolumeMultiplier = volumeMultiplier,
                PitchMultiplier = pitchMultiplier,
                InstanceID = id,
                TriggerDucking = triggerDucking,
                DuckingDuration = duckingDuration
            };
            AudioEventChannel.RaisePlayAudio(config);
            return id;
        }
        
        #endregion

        #region Music
        
        /// <summary>
        /// Plays a music track with an optional crossfade duration and target volume.
        /// </summary>
        /// <param name="audioID">The unique ID of the music entry to play.</param>
        /// <param name="fadeDuration">The duration in seconds to fade in/out. Default is 2.0s.</param>
        /// <param name="targetVolume">The maximum volume to reach after fading in. Default is 1f.</param>
        public static long PlayMusic(string audioID, float fadeDuration = 2.0f, float targetVolume = 1f)
        {
            EnsureManagerExists();
            long id = _nextInstanceId++;
            var config = new AudioClipConfig
            {
                Category = AudioCategory.Music,
                AudioID = audioID,
                Loop = true,
                FadeDuration = fadeDuration,
                VolumeMultiplier = targetVolume,
                InstanceID = id
            };
            AudioEventChannel.RaisePlayAudio(config);
            return id;
        }
        
        #endregion

        #region Global Audio Control
        
        /// <summary>
        /// Stops the currently playing music track smoothly.
        /// </summary>
        public static void StopMusic() => AudioEventChannel.RaiseStopMusic();

        /// <summary>
        /// Stops the currently playing music track smoothly with optional fade duration.
        /// </summary>
        public static void StopMusic(float fadeDuration) => AudioEventChannel.RaiseStopMusic();

        /// <summary>
        /// Pauses the currently playing music track.
        /// </summary>
        public static void PauseMusic() => AudioEventChannel.RaisePauseMusic();

        /// <summary>
        /// Resumes a paused music track.
        /// </summary>
        public static void ResumeMusic() => AudioEventChannel.RaiseResumeMusic();

        public static void StopInstance(long instanceId)
        {
            if (instanceId > 0)
            {
                AudioEventChannel.RaiseStopInstance(instanceId);
            }
        }

        /// <summary>
        /// Pauses a specific instance of a sound by its unique ID.
        /// </summary>
        public static void PauseInstance(long instanceId)
        {
            if (instanceId > 0)
            {
                AudioEventChannel.RaisePauseInstance(instanceId);
            }
        }

        /// <summary>
        /// Resumes a specific instance of a sound by its unique ID.
        /// </summary>
        public static void ResumeInstance(long instanceId)
        {
            if (instanceId > 0)
            {
                AudioEventChannel.RaiseResumeInstance(instanceId);
            }
        }

        // ======================================================================
        // SYSTEM CONTROLS (Mixer Integration for Settings Menus)
        // ======================================================================

        /// <summary>
        /// Adjusts the Master Audio Mixer volume.
        /// </summary>
        /// <param name="normalizedVolume">Volume level from 0.0 (Muted) to 1.0 (Max).</param>
        public static void SetMasterVolume(float normalizedVolume) => AudioEventChannel.RaiseSetGroupVolume(AudioCategory.Master, normalizedVolume);

        /// <summary>
        /// Adjusts the Music Audio Mixer volume.
        /// </summary>
        public static void SetMusicVolume(float normalizedVolume) => AudioEventChannel.RaiseSetGroupVolume(AudioCategory.Music, normalizedVolume);

        /// <summary>
        /// Adjusts the SFX Audio Mixer volume.
        /// </summary>
        public static void SetSFXVolume(float normalizedVolume) => AudioEventChannel.RaiseSetGroupVolume(AudioCategory.SFX, normalizedVolume);
        
        /// <summary>
        /// Adjusts the UI Audio Mixer volume.
        /// </summary>
        public static void SetUIVolume(float normalizedVolume) => AudioEventChannel.RaiseSetGroupVolume(AudioCategory.UI, normalizedVolume);
        
        #endregion
    }
}

namespace Resenha.Signal.Communication
{
    /// <summary>
    /// Alias to Resenha.Signal.Signal for backwards compatibility with communication namespace.
    /// </summary>
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

        public static long PlayMusic(string audioID, float fadeDuration = 2.0f, float targetVolume = 1f)
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
}