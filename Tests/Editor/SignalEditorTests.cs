using NUnit.Framework;
using Resenha.Signal.Editor.Localization;

namespace Resenha.Signal.Editor.Tests
{
    public class SignalEditorTests
    {
        [Test]
        public void Localization_ReturnsValidStrings()
        {
            SignalStrings.CurrentLanguage = SignalLanguage.English;
            string titleEn = SignalStrings.Get("window_title");
            Assert.AreEqual("Signal", titleEn);

            SignalStrings.CurrentLanguage = SignalLanguage.Portuguese;
            string subtitlePt = SignalStrings.Get("window_subtitle");
            Assert.AreEqual("Arquitetura de Áudio e Gerenciador de Som", subtitlePt);
        }
    }
}
