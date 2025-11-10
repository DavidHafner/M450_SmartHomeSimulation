using System;
using System.IO;
using JetBrains.Annotations;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartHomeSimulation.Tests.IsolationTests;

namespace SmartHomeSimulation.Tests.IntegrationTests;

[TestClass]
[TestSubject(typeof(ZimmerMitMarkisensteuerung))]
public class ZimmerMitMarkisensteuerungTest
{
[TestClass]
    public class ZimmerMitMarkisensteuerungTests
    {
        [TestMethod]
        public void Markise_ShouldOpen_WhenCoolerThanVorgabe()
        {
            // Arrange
            var wohnzimmer = new Wohnzimmer() { Temperaturvorgabe = 25.0 };
            var zimmer = new ZimmerMitMarkisensteuerung(wohnzimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 20.0, Regen = false };

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            zimmer.VerarbeiteWetterdaten(wetter);

            // Assert
            Assert.IsTrue(zimmer.MarkiseOffen, "Markise should open when it's cooler than target temperature.");
            StringAssert.Contains(writer.ToString(), "Markise wird geöffnet");
        }

        [TestMethod]
        public void Markise_ShouldClose_WhenHotterThanVorgabe_AndNoRain_AndIsOpen()
        {
            // Arrange
            var kueche = new Kueche() { Temperaturvorgabe = 20.0 };
            var zimmer = new ZimmerMitMarkisensteuerung(kueche);
            // Start with open markise
            typeof(ZimmerMitMarkisensteuerung).GetProperty("MarkiseOffen")!.SetValue(zimmer, true);

            var wetter = new Wetterdaten { Aussentemperatur = 30.0, Regen = false };

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            zimmer.VerarbeiteWetterdaten(wetter);

            // Assert
            Assert.IsFalse(zimmer.MarkiseOffen, "Markise should close when it's hotter and no rain.");
            StringAssert.Contains(writer.ToString(), "Markise wird geschlossen");
        }

        [TestMethod]
        public void Markise_ShouldNotClose_WhenHotterThanVorgabe_AndRaining()
        {
            // Arrange
            var wintergarten = new Wintergarten() { Temperaturvorgabe = 20.0 };
            var zimmer = new ZimmerMitMarkisensteuerung(wintergarten);
            // Start with open markise
            typeof(ZimmerMitMarkisensteuerung).GetProperty("MarkiseOffen")!.SetValue(zimmer, true);

            var wetter = new Wetterdaten { Aussentemperatur = 30.0, Regen = true };

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            zimmer.VerarbeiteWetterdaten(wetter);

            // Assert
            Assert.IsTrue(zimmer.MarkiseOffen, "Markise should remain open because it is raining.");
            StringAssert.Contains(writer.ToString(), "kann nicht geschlossen werden");
        }

        [TestMethod]
        public void Markise_ShouldOpen_WhenHotterThanVorgabe_AndRaining_AndCurrentlyClosed()
        {
            // Arrange
            var badWc = new BadWC() { Temperaturvorgabe = 22.0 };
            var zimmer = new ZimmerMitMarkisensteuerung(badWc);
            // Markise closed
            typeof(ZimmerMitMarkisensteuerung).GetProperty("MarkiseOffen")!.SetValue(zimmer, false);

            var wetter = new Wetterdaten { Aussentemperatur = 30.0, Regen = true };

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            zimmer.VerarbeiteWetterdaten(wetter);

            // Assert
            Assert.IsTrue(zimmer.MarkiseOffen, "Markise should open when it's raining even if hot.");
            StringAssert.Contains(writer.ToString(), "Markise wird geöffnet weils regnet");
        }

        [TestMethod]
        public void Markise_ShouldNotToggle_Unnecessarily()
        {
            // Arrange
            var fakeZimmer = new Schlafzimmer() { Temperaturvorgabe = 25.0 };
            var zimmer = new ZimmerMitMarkisensteuerung(fakeZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 20.0, Regen = false };

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act — first call opens
            zimmer.VerarbeiteWetterdaten(wetter);
            bool state1 = zimmer.MarkiseOffen;
            string firstOutput = writer.ToString();
            // Act again — same conditions, should not reprint
            zimmer.VerarbeiteWetterdaten(wetter);
            bool state2 = zimmer.MarkiseOffen;

            writer.GetStringBuilder().Clear();

            // Assert
            Assert.IsTrue(firstOutput.Contains("Markise wird geöffnet"));
            Assert.AreEqual(state1,state2);
        }
    }
}