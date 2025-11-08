using System;
using System.IO;
using JetBrains.Annotations;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartHomeSimulation.Tests.IsolationTests;

namespace SmartHomeSimulation.Tests.IsolationTests;

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
            var fakeZimmer = new FakeZimmer("Wohnzimmer") { Temperaturvorgabe = 25.0 };
            var zimmer = new ZimmerMitMarkisensteuerung(fakeZimmer);
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
            var fakeZimmer = new FakeZimmer("Küche") { Temperaturvorgabe = 20.0 };
            var zimmer = new ZimmerMitMarkisensteuerung(fakeZimmer);
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
            var fakeZimmer = new FakeZimmer("Büro") { Temperaturvorgabe = 20.0 };
            var zimmer = new ZimmerMitMarkisensteuerung(fakeZimmer);
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
            var fakeZimmer = new FakeZimmer("Terrasse") { Temperaturvorgabe = 22.0 };
            var zimmer = new ZimmerMitMarkisensteuerung(fakeZimmer);
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
        public void VerarbeiteWetterdaten_ShouldCallBaseZimmer()
        {
            // Arrange
            var fakeZimmer = new FakeZimmer("Schlafzimmer") { Temperaturvorgabe = 20.0 };
            var zimmer = new ZimmerMitMarkisensteuerung(fakeZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 18.0, Regen = false };

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
            Assert.IsTrue(fakeZimmer.VerarbeiteWetterdatenCalled, "Base VerarbeiteWetterdaten should be called.");
            Assert.AreEqual(wetter, fakeZimmer.LetzteWetterdaten);
        }

        [TestMethod]
        public void Markise_ShouldNotToggle_Unnecessarily()
        {
            // Arrange
            var fakeZimmer = new FakeZimmer("Balkon") { Temperaturvorgabe = 25.0 };
            var zimmer = new ZimmerMitMarkisensteuerung(fakeZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 20.0, Regen = false };

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act — first call opens
            zimmer.VerarbeiteWetterdaten(wetter);
            string firstOutput = writer.ToString();
            writer.GetStringBuilder().Clear();

            // Act again — same conditions, should not reprint
            zimmer.VerarbeiteWetterdaten(wetter);
            string secondOutput = writer.ToString();

            // Assert
            Assert.IsTrue(firstOutput.Contains("Markise wird geöffnet"));
            Assert.AreEqual(string.Empty, secondOutput.Trim(), "Should not output message if state unchanged.");
        }
    }
}