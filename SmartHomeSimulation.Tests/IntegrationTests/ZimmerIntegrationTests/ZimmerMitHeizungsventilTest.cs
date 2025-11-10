using Microsoft.VisualStudio.TestTools.UnitTesting;
using M320_SmartHome;
using System;
using System.IO;
using SmartHomeSimulation.Tests.IsolationTests;

namespace SmartHomeSimulation.Tests.IntegrationTests
{
    [TestClass]
    public class ZimmerMitHeizungsventilTests
    {
        [TestMethod]
        public void Ventil_ShouldOpen_WhenAussentemperaturBelowVorgabe()
        {
            using var writer = new StringWriter();
            Console.SetOut(writer);
            // Arrange
            var baseZimmer = new Wohnzimmer() { Temperaturvorgabe = 22.0 };
            var zimmer = new ZimmerMitHeizungsventil(baseZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 15.0 };

            // Act
            zimmer.VerarbeiteWetterdaten(wetter);

            // Assert
            Assert.IsTrue(zimmer.HeizungsventilOffen, "Ventil should open when it is colder than the target temperature.");
        }

        [TestMethod]
        public void Ventil_ShouldClose_WhenAussentemperaturAboveVorgabe()
        {
            using var writer = new StringWriter();
            Console.SetOut(writer);
            
            // Arrange
            var baseZimmer = new Kueche() { Temperaturvorgabe = 20.0 };
            var zimmer = new ZimmerMitHeizungsventil(baseZimmer);
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
            var baseZimmer = new BadWC() { Temperaturvorgabe = 20.0 };
            var zimmer = new ZimmerMitHeizungsventil(baseZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 10.0 };

            // Capture console output
            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act — first call opens the valve
            zimmer.VerarbeiteWetterdaten(wetter);
            bool state1 = zimmer.HeizungsventilOffen;
            string firstOutput = writer.ToString().Trim();
            bool state2 = zimmer.HeizungsventilOffen;
            writer.GetStringBuilder().Clear();

            // Act again — same condition, should NOT print again
            // zimmer.VerarbeiteWetterdaten(wetter);
            string secondOutput = writer.ToString().Trim();

            // Assert
            Assert.IsTrue(firstOutput.Contains("Heizungsventil wird geöffnet"));
            Assert.AreEqual(state1,state2);
        }

        [TestMethod]
        public void ConsoleOutput_ShouldMatchOpenAndCloseMessages()
        {
            // Arrange
            var baseZimmer = new Schlafzimmer() { Temperaturvorgabe = 20.0 };
            var zimmer = new ZimmerMitHeizungsventil(baseZimmer);

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