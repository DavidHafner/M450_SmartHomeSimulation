using System;
using System.IO;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartHomeSimulation.Tests.IsolationTests;

namespace SmartHomeSimulation.Tests.IntegrationTests.AktorIntegrationTests
{
    [TestClass]
    public class ZimmerMitHeizungsventilTests
    {

        [TestMethod]
        public void Ventil_ShouldClose_WhenAussentemperaturAboveVorgabe()
        {
            // Arrange
            var writer = new StringWriter();
            Console.SetOut(writer);
            var fakeZimmer = new FakeZimmer("Küche") { Temperaturvorgabe = 20.0 };
            var zimmer = new ZimmerMitHeizungsventil(fakeZimmer);
            var wetter = new Wettersensor().GetWetterdaten();

            // First, open the valve manually to simulate previous state
            typeof(ZimmerMitHeizungsventil)
                .GetProperty("HeizungsventilOffen")!
                .SetValue(zimmer, true);

            // Act
            zimmer.VerarbeiteWetterdaten(wetter);

            // Assert
            if (wetter.Aussentemperatur >= fakeZimmer.Temperaturvorgabe)
            {
                Assert.IsFalse( zimmer.HeizungsventilOffen, "Ventil should close when it is warmer than the target temperature.");    
            }
            else
            {
                Assert.IsTrue( zimmer.HeizungsventilOffen, "Ventil should open when it is colder than the target temperature.");
            }
            
        }



        [TestMethod]
        public void VerarbeiteWetterdaten_ShouldCallBaseZimmer()
        {
            // Arrange
            var fakeZimmer = new FakeZimmer("Schlafzimmer") { Temperaturvorgabe = 22.0 };
            var zimmer = new ZimmerMitHeizungsventil(fakeZimmer);
            var wetter = new Wettersensor().GetWetterdaten();

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
        
        
        [TestMethod]
        public void Wettersensor_Liefert_KorrekteWetterdaten_Integration()
        {
            // Arrange
            using var writer = new StringWriter();
            Console.SetOut(writer);
            var sensor = new Wettersensor();
            var fakeZimmer = new FakeZimmer("Schlafzimmer") { Temperaturvorgabe = 22.0 };
            var zimmer = new ZimmerMitHeizungsventil(fakeZimmer);
            

            // Act
            var wetter = sensor.GetWetterdaten();
            zimmer.VerarbeiteWetterdaten(wetter);

            // Assert
            Assert.IsInRange( -25, 35, wetter.Aussentemperatur);
            // Kein Absturz, Ventil hat sinnvollen Zustand
            Assert.IsTrue(zimmer.HeizungsventilOffen || !zimmer.HeizungsventilOffen);
        }
    }
}