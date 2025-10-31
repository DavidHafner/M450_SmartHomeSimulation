using System;
using System.IO;
using JetBrains.Annotations;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartHomeSimulation.Tests
{
    [TestClass]
    public class ZimmerMitJalousiesteuerungTests
    {
        [TestMethod]
        public void Jalousie_ShouldClose_WhenHotterThanVorgabe_AndNoPersonsInside()
        {
            // Arrange
            var fakeZimmer = new FakeZimmer("Wohnzimmer")
            {
                Temperaturvorgabe = 22.0,
                PersonenImZimmer = false
            };
            var zimmer = new ZimmerMitJalousiesteuerung(fakeZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 30.0 };

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            zimmer.VerarbeiteWetterdaten(wetter);

            // Assert
            Assert.IsTrue(zimmer.JalousieHeruntergefahren, "Jalousie should close when it's hotter and no one is inside.");
            StringAssert.Contains(writer.ToString(), "Jalousie wird geschlossen");
        }

        [TestMethod]
        public void Jalousie_ShouldNotClose_WhenHotterThanVorgabe_ButPersonsInside()
        {
            // Arrange
            var fakeZimmer = new FakeZimmer("Küche")
            {
                Temperaturvorgabe = 20.0,
                PersonenImZimmer = true
            };
            var zimmer = new ZimmerMitJalousiesteuerung(fakeZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 25.0 };

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            zimmer.VerarbeiteWetterdaten(wetter);

            // Assert
            Assert.IsFalse(zimmer.JalousieHeruntergefahren, "Jalousie should not close if persons are inside.");
            StringAssert.Contains(writer.ToString(), "kann nicht geschlossen werden");
        }

        [TestMethod]
        public void Jalousie_ShouldOpen_WhenCoolerThanVorgabe()
        {
            // Arrange
            var fakeZimmer = new FakeZimmer("Schlafzimmer")
            {
                Temperaturvorgabe = 25.0,
                PersonenImZimmer = false
            };
            var zimmer = new ZimmerMitJalousiesteuerung(fakeZimmer);

            // Simulate closed blinds
            typeof(ZimmerMitJalousiesteuerung)
                .GetProperty("JalousieHeruntergefahren")!
                .SetValue(zimmer, true);

            var wetter = new Wetterdaten { Aussentemperatur = 15.0 };

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            zimmer.VerarbeiteWetterdaten(wetter);

            // Assert
            Assert.IsFalse(zimmer.JalousieHeruntergefahren, "Jalousie should open when it's cooler than the target temperature.");
            StringAssert.Contains(writer.ToString(), "Jalousie wird geöffnet");
        }

        [TestMethod]
        public void VerarbeiteWetterdaten_ShouldCallBaseZimmer()
        {
            // Arrange
            var fakeZimmer = new FakeZimmer("Büro")
            {
                Temperaturvorgabe = 23.0
            };
            var zimmer = new ZimmerMitJalousiesteuerung(fakeZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 28.0 };
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
        public void Jalousie_ShouldNotToggle_WhenStateRemainsSame()
        {
            // Arrange
            var fakeZimmer = new FakeZimmer("Bad")
            {
                Temperaturvorgabe = 22.0,
                PersonenImZimmer = false
            };
            var zimmer = new ZimmerMitJalousiesteuerung(fakeZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 30.0 };

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            zimmer.VerarbeiteWetterdaten(wetter); // first — closes
            string firstOutput = writer.ToString();
            writer.GetStringBuilder().Clear();

            zimmer.VerarbeiteWetterdaten(wetter); // second — should not change
            string secondOutput = writer.ToString();

            // Assert
            Assert.IsTrue(firstOutput.Contains("Jalousie wird geschlossen"));
            Assert.AreEqual(string.Empty, secondOutput.Trim(), "Should not reprint if Jalousie is already closed.");
        }
    }
}