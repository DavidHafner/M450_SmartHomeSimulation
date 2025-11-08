using Microsoft.VisualStudio.TestTools.UnitTesting;
using M320_SmartHome;
using System;
using System.IO;

namespace SmartHomeSimulation.Tests
{
    [TestClass]
    public class ZimmerMitHeizungsventilTests
    {
        [TestMethod]
        public void Ventil_ShouldOpen_WhenAussentemperaturBelowVorgabe()
        {
            // Arrange
            var fakeZimmer = new FakeZimmer("Wohnzimmer") { Temperaturvorgabe = 22.0 };
            var zimmer = new ZimmerMitHeizungsventil(fakeZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 15.0 };

            // Act
            zimmer.VerarbeiteWetterdaten(wetter);

            // Assert
            Assert.IsTrue(zimmer.HeizungsventilOffen, "Ventil should open when it is colder than the target temperature.");
        }

        [TestMethod]
        public void Ventil_ShouldClose_WhenAussentemperaturAboveVorgabe()
        {
            // Arrange
            var fakeZimmer = new FakeZimmer("Küche") { Temperaturvorgabe = 20.0 };
            var zimmer = new ZimmerMitHeizungsventil(fakeZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 25.0 };

            // First, open the valve manually to simulate previous state
            typeof(ZimmerMitHeizungsventil)
                .GetProperty("HeizungsventilOffen")!
                .SetValue(zimmer, true);

            // Act
            zimmer.VerarbeiteWetterdaten(wetter);

            // Assert
            Assert.IsFalse(zimmer.HeizungsventilOffen, "Ventil should close when it is warmer than the target temperature.");
        }

        [TestMethod]
        public void Ventil_ShouldNotToggle_Unnecessarily()
        {
            // Arrange
            var fakeZimmer = new FakeZimmer("Bad") { Temperaturvorgabe = 20.0 };
            var zimmer = new ZimmerMitHeizungsventil(fakeZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 10.0 };

            // Capture console output
            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act — first call opens the valve
            zimmer.VerarbeiteWetterdaten(wetter);
            string firstOutput = writer.ToString().Trim();
            writer.GetStringBuilder().Clear();

            // Act again — same condition, should NOT print again
            zimmer.VerarbeiteWetterdaten(wetter);
            string secondOutput = writer.ToString().Trim();

            // Assert
            Assert.IsTrue(firstOutput.Contains("Heizungsventil wird geöffnet"));
            Assert.AreEqual(string.Empty, secondOutput, "Should not print again if valve state stays the same.");
        }

        [TestMethod]
        public void VerarbeiteWetterdaten_ShouldCallBaseZimmer()
        {
            // Arrange
            var fakeZimmer = new FakeZimmer("Schlafzimmer") { Temperaturvorgabe = 22.0 };
            var zimmer = new ZimmerMitHeizungsventil(fakeZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 15.0 };

            var originalOut = Console.Out;
            var writer = new StringWriter();
            Console.SetOut(writer);
            
            try
            {
                // Act
                zimmer.VerarbeiteWetterdaten(wetter);
            }
            finally
            {
                Console.SetOut(originalOut); // ✅ restore safely
                writer.Dispose();
            }
            // Assert
            Assert.IsTrue(fakeZimmer.VerarbeiteWetterdatenCalled);
            Assert.AreEqual(wetter, fakeZimmer.LetzteWetterdaten);
        }

        [TestMethod]
        public void ConsoleOutput_ShouldMatchOpenAndCloseMessages()
        {
            // Arrange
            var fakeZimmer = new FakeZimmer("Arbeitszimmer") { Temperaturvorgabe = 20.0 };
            var zimmer = new ZimmerMitHeizungsventil(fakeZimmer);

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            zimmer.VerarbeiteWetterdaten(new Wetterdaten { Aussentemperatur = 10.0 }); // open
            zimmer.VerarbeiteWetterdaten(new Wetterdaten { Aussentemperatur = 25.0 }); // close

            string output = writer.ToString();

            // Assert
            StringAssert.Contains(output, "Heizungsventil wird geöffnet");
            StringAssert.Contains(output, "Heizungsventil wird geschlossen");
        }
    }
}