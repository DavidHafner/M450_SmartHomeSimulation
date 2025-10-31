using JetBrains.Annotations;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartHomeSimulation.Tests;

[TestClass]
[TestSubject(typeof(ZimmerMitMarkisensteuerung))]
public class ZimmerMitMarkisensteuerungTest
{

    [TestMethod]
    public class MockZimmer : Zimmer
    {
        public MockZimmer(string name) : base(name) { }

        public bool VerarbeiteWetterdatenCalled { get; private set; }
        public Wetterdaten LetzteWetterdaten { get; private set; }

        public override double Temperaturvorgabe { get; set; }
        public override bool PersonenImZimmer { get; set; }

        public override void VerarbeiteWetterdaten(Wetterdaten wetterdaten)
        {
            VerarbeiteWetterdatenCalled = true;
            LetzteWetterdaten = wetterdaten;
        }
    }

    public class MockWetterdaten : Wetterdaten
    {
        public double Aussentemperatur { get; set; }
        public bool Regen { get; set; }
    }

    // --- Actual test class ---
    public class ZimmerMitMarkisensteuerungTests
    {
        [Fact]
        public void Konstruktor_Soll_Zimmer_Setzen()
        {
            // Arrange
            var baseZimmer = new MockZimmer("Terrasse");

            // Act
            var markZimmer = new ZimmerMitMarkisensteuerung(baseZimmer);

            // Assert
            Assert.Equal("Terrasse", markZimmer.Name);
            Assert.False(markZimmer.MarkiseOffen);
        }

        [Fact]
        public void VerarbeiteWetterdaten_Soll_Markise_Oeffnen_Wenn_Kuehl()
        {
            // Arrange
            var baseZimmer = new MockZimmer("Balkon") { Temperaturvorgabe = 25.0 };
            var markZimmer = new ZimmerMitMarkisensteuerung(baseZimmer);
            var wetterdaten = new MockWetterdaten { Aussentemperatur = 15.0, Regen = false };

            // Act
            markZimmer.VerarbeiteWetterdaten(wetterdaten);

            // Assert
            Assert.True(markZimmer.MarkiseOffen);
        }

        [Fact]
        public void VerarbeiteWetterdaten_Soll_Markise_Schliessen_Wenn_Warm_und_Trocken()
        {
            // Arrange
            var baseZimmer = new MockZimmer("Garten") { Temperaturvorgabe = 20.0 };
            var markZimmer = new ZimmerMitMarkisensteuerung(baseZimmer);

            // Start: markise offen
            markZimmer.VerarbeiteWetterdaten(new MockWetterdaten { Aussentemperatur = 15.0, Regen = false });
            Assert.True(markZimmer.MarkiseOffen);

            // Act
            markZimmer.VerarbeiteWetterdaten(new MockWetterdaten { Aussentemperatur = 30.0, Regen = false });

            // Assert
            Assert.False(markZimmer.MarkiseOffen);
        }

        [Fact]
        public void VerarbeiteWetterdaten_Soll_Markise_Nicht_Schliessen_Wenn_Es_Regnet()
        {
            // Arrange
            var baseZimmer = new MockZimmer("Wintergarten") { Temperaturvorgabe = 20.0 };
            var markZimmer = new ZimmerMitMarkisensteuerung(baseZimmer);

            // Start: markise offen
            markZimmer.VerarbeiteWetterdaten(new MockWetterdaten { Aussentemperatur = 15.0, Regen = false });
            Assert.True(markZimmer.MarkiseOffen);

            // Act
            markZimmer.VerarbeiteWetterdaten(new MockWetterdaten { Aussentemperatur = 30.0, Regen = true });

            // Assert
            Assert.True(markZimmer.MarkiseOffen); // stays open because of rain
        }

        [Fact]
        public void VerarbeiteWetterdaten_Soll_Markise_Oeffnen_Wenn_Es_Regnet_und_Sie_Geschlossen_Ist()
        {
            // Arrange
            var baseZimmer = new MockZimmer("Veranda") { Temperaturvorgabe = 20.0 };
            var markZimmer = new ZimmerMitMarkisensteuerung(baseZimmer);

            // Ensure markise is closed
            Assert.False(markZimmer.MarkiseOffen);

            // Act
            markZimmer.VerarbeiteWetterdaten(new MockWetterdaten { Aussentemperatur = 30.0, Regen = true });

            // Assert
            Assert.True(markZimmer.MarkiseOffen);
        }

        [Fact]
        public void VerarbeiteWetterdaten_Soll_Nicht_Erwiederholt_Oeffnen_Wenn_Bereits_Offen()
        {
            // Arrange
            var baseZimmer = new MockZimmer("Terrasse") { Temperaturvorgabe = 20.0 };
            var markZimmer = new ZimmerMitMarkisensteuerung(baseZimmer);
            var wetterdaten = new MockWetterdaten { Aussentemperatur = 10.0, Regen = false };

            // Act
            markZimmer.VerarbeiteWetterdaten(wetterdaten);
            Assert.True(markZimmer.MarkiseOffen);

            // Act again (should remain open)
            markZimmer.VerarbeiteWetterdaten(wetterdaten);

            // Assert
            Assert.True(markZimmer.MarkiseOffen);
        }

        [Fact]
        public void VerarbeiteWetterdaten_Soll_Base_VerarbeiteWetterdaten_Aufrufen()
        {
            // Arrange
            var baseZimmer = new MockZimmer("Loggia") { Temperaturvorgabe = 20.0 };
            var markZimmer = new ZimmerMitMarkisensteuerung(baseZimmer);
            var wetterdaten = new MockWetterdaten { Aussentemperatur = 25.0, Regen = false };

            // Act
            markZimmer.VerarbeiteWetterdaten(wetterdaten);

            // Assert
            Assert.True(baseZimmer.VerarbeiteWetterdatenCalled);
        }
    }