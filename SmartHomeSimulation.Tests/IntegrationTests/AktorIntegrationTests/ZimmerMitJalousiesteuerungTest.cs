using System;
using System.IO;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartHomeSimulation.Tests.IsolationTests;

namespace SmartHomeSimulation.Tests.IntegrationTests.AktorIntegrationTests
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
            var wetter = new Wettersensor().GetWetterdaten();

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            zimmer.VerarbeiteWetterdaten(wetter);

            // Assert
            if (wetter.Aussentemperatur >= fakeZimmer.Temperaturvorgabe)
            {
                 Assert.IsTrue(zimmer.JalousieHeruntergefahren, "Jalousie should close when it's hotter and no one is inside.");
                 StringAssert.Contains(writer.ToString(), "Jalousie wird geschlossen");
            }
            else
            {
                Assert.IsFalse(zimmer.JalousieHeruntergefahren, "Jalousie should not close when it's cooler and no one is inside.");
            }
            
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

            var wetter = new Wettersensor().GetWetterdaten();

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            zimmer.VerarbeiteWetterdaten(wetter);

            // Assert
            if (wetter.Aussentemperatur <= fakeZimmer.Temperaturvorgabe)
            {
                Assert.IsFalse(zimmer.JalousieHeruntergefahren, "Jalousie should open when it's cooler than the target temperature.");
                StringAssert.Contains(writer.ToString(), "Jalousie wird geöffnet");
            }
            else
            {
                Assert.IsFalse(zimmer.JalousieHeruntergefahren, "Jalousie should not open when it's hotter than the target temperature.");
            }
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
            var wetter = new Wettersensor().GetWetterdaten();

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