using NUnit.Framework;
using Resenha.Signal.Communication;

namespace Resenha.Signal.Tests
{
    public class SignalRuntimeTests
    {
        [Test]
        public void AudioEventChannel_CanSubscribeAndRaise()
        {
            bool eventFired = false;
            AudioEventChannel.PlayAudioAction handler = config => { eventFired = true; };

            AudioEventChannel.OnAudioPlayRequested += handler;
            AudioEventChannel.RaisePlayAudio(new AudioClipConfig { AudioID = "test" });
            AudioEventChannel.OnAudioPlayRequested -= handler;

            Assert.IsTrue(eventFired);
        }

        [Test]
        public void LegacyCompatibility_TypesExist()
        {
#pragma warning disable CS0618
            Assert.IsNotNull(typeof(SignalAudioManagerUnity.Communication.Signal));
            Assert.IsNotNull(typeof(SignalAudioManagerUnity.Core.SoundManager));
            Assert.IsNotNull(typeof(SignalAudioManagerUnity.Data.AudioEntry));
#pragma warning restore CS0618
        }
    }
}
