using JetBrains.Annotations;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartHomeSimulation.Tests;

[TestClass]
[TestSubject(typeof(ZimmerMitHeizungsventil))]
public class ZimmerMitHeizungsventilTest
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
}

// --- Actual test class ---
public class ZimmerMitHeizungsventilTests
{
    [Fact]
    public void Konstruktor_Soll_Zimmer_Setzen()
    {
        // Arrange
        var baseZimmer = new MockZimmer("Wohnzimmer");

        // Act
        var heizZimmer = new ZimmerMitHeizungsventil(baseZimmer);

        // Assert
        Assert.Equal("Wohnzimmer", heizZimmer.Name);
        Assert.False(heizZimmer.HeizungsventilOffen);
    }

    [Fact]
    public void VerarbeiteWetterdaten_Soll_Heizungsventil_Oeffnen_Wenn_Kalt()
    {
        // Arrange
        var baseZimmer = new MockZimmer("Schlafzimmer") { Temperaturvorgabe = 21.0 };
        var heizZimmer = new ZimmerMitHeizungsventil(baseZimmer);
        var wetterdaten = new MockWetterdaten { Aussentemperatur = 10.0 };

        // Act
        heizZimmer.VerarbeiteWetterdaten(wetterdaten);

        // Assert
        Assert.True(heizZimmer.HeizungsventilOffen);
        Assert.True(baseZimmer.VerarbeiteWetterdatenCalled);
    }

    [Fact]
    public void VerarbeiteWetterdaten_Soll_Heizungsventil_Schliessen_Wenn_Warm()
    {
        // Arrange
        var baseZimmer = new MockZimmer("Büro") { Temperaturvorgabe = 20.0 };
        var heizZimmer = new ZimmerMitHeizungsventil(baseZimmer);

        // Simulate open valve first
        heizZimmer.VerarbeiteWetterdaten(new MockWetterdaten { Aussentemperatur = 10.0 });
        Assert.True(heizZimmer.HeizungsventilOffen);

        // Act
        heizZimmer.VerarbeiteWetterdaten(new MockWetterdaten { Aussentemperatur = 25.0 });

        // Assert
        Assert.False(heizZimmer.HeizungsventilOffen);
    }

    [Fact]
    public void VerarbeiteWetterdaten_Soll_Ventil_Nicht_Erwiederholt_Oeffnen_Wenn_Bereits_Offen()
    {
        // Arrange
        var baseZimmer = new MockZimmer("Küche") { Temperaturvorgabe = 20.0 };
        var heizZimmer = new ZimmerMitHeizungsventil(baseZimmer);
        var wetterdaten = new MockWetterdaten { Aussentemperatur = 10.0 };

        // Act
        heizZimmer.VerarbeiteWetterdaten(wetterdaten);
        Assert.True(heizZimmer.HeizungsventilOffen);

        // Act again (should remain open)
        heizZimmer.VerarbeiteWetterdaten(wetterdaten);

        // Assert
        Assert.True(heizZimmer.HeizungsventilOffen);
    }

    [Fact]
    public void VerarbeiteWetterdaten_Soll_Ventil_Nicht_Erwiederholt_Schliessen_Wenn_Bereits_Zu()
    {
        // Arrange
        var baseZimmer = new MockZimmer("Bad") { Temperaturvorgabe = 20.0 };
        var heizZimmer = new ZimmerMitHeizungsventil(baseZimmer);
        var wetterdaten = new MockWetterdaten { Aussentemperatur = 25.0 };

        // Act (valve initially closed)
        heizZimmer.VerarbeiteWetterdaten(wetterdaten);

        // Assert
        Assert.False(heizZimmer.HeizungsventilOffen);

        // Act again (should remain closed)
        heizZimmer.VerarbeiteWetterdaten(wetterdaten);

        // Assert
        Assert.False(heizZimmer.HeizungsventilOffen);
    }
}