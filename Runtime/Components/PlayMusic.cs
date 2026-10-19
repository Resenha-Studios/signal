using Resenha.Signal.Communication;
using UnityEngine;

namespace Resenha.Signal.Components
{
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, "SignalAudioManagerUnity.Components", "SignalAudioManagerUnity")]
    public class PlayMusic : MonoBehaviour
    {
        [SerializeField] private bool playOnStart = false;
        [SerializeField] private string musicID = "DefaultMusic";
        [SerializeField] private float fadeDuration = 2f;

        private void Start()
        {
            if (playOnStart)
            {
                Play();
            }
        }

        public void Play()
        {
            Signal.PlayMusic(musicID, fadeDuration);
        }
    }
}