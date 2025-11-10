using System;
using System.IO;
using JetBrains.Annotations;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartHomeSimulation.Tests.IsolationTests;

namespace SmartHomeSimulation.Tests.IntegrationTests
{
    [TestClass]
    public class ZimmerMitJalousiesteuerungTests
    {
        [TestMethod]
        public void Jalousie_ShouldClose_WhenHotterThanVorgabe_AndNoPersonsInside()
        {
            // Arrange
            var wohnzimmer = new Wohnzimmer()
            {
                Temperaturvorgabe = 22.0,
                PersonenImZimmer = false
            };
            var zimmer = new ZimmerMitJalousiesteuerung(wohnzimmer);
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
            var kueche = new Kueche()
            {
                Temperaturvorgabe = 20.0,
                PersonenImZimmer = true
            };
            var zimmer = new ZimmerMitJalousiesteuerung(kueche);
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
            var schlafzimmer = new Schlafzimmer()
            {
                Temperaturvorgabe = 25.0,
                PersonenImZimmer = false
            };
            var zimmer = new ZimmerMitJalousiesteuerung(schlafzimmer);

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
        public void Jalousie_ShouldNotToggle_WhenStateRemainsSame()
        {
            // Arrange
            var badWc = new BadWC()
            {
                Temperaturvorgabe = 22.0,
                PersonenImZimmer = false
            };
            var zimmer = new ZimmerMitJalousiesteuerung(badWc);
            var wetter = new Wetterdaten { Aussentemperatur = 30.0 };

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            zimmer.VerarbeiteWetterdaten(wetter); // first — closes
            bool state1=zimmer.JalousieHeruntergefahren;
            string firstOutput = writer.ToString();
            writer.GetStringBuilder().Clear();
            bool state2=zimmer.JalousieHeruntergefahren;

            zimmer.VerarbeiteWetterdaten(wetter); // second — should not change
            string secondOutput = writer.ToString();

            // Assert
            Assert.IsTrue(firstOutput.Contains("Jalousie wird geschlossen"));
            Assert.AreEqual(state1,state2);
        }
    }
}