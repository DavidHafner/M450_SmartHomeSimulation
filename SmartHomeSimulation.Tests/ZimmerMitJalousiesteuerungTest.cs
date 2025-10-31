using JetBrains.Annotations;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartHomeSimulation.Tests;

[TestClass]
[TestSubject(typeof(ZimmerMitJalousiesteuerung))]
public class ZimmerMitJalousiesteuerungTest
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

// --- Tests for ZimmerMitJalousiesteuerung ---
public class ZimmerMitJalousiesteuerungTests
{
    [Fact]
    public void Konstruktor_Soll_Zimmer_Setzen()
    {
        // Arrange
        var baseZimmer = new MockZimmer("Wohnzimmer");

        // Act
        var jalZimmer = new ZimmerMitJalousiesteuerung(baseZimmer);

        // Assert
        Assert.Equal("Wohnzimmer", jalZimmer.Name);
        Assert.False(jalZimmer.JalousieHeruntergefahren);
    }

    [Fact]
    public void VerarbeiteWetterdaten_Soll_Jalousie_Schliessen_Wenn_Warm_und_Keine_Personen()
    {
        // Arrange
        var baseZimmer = new MockZimmer("Schlafzimmer")
        {
            Temperaturvorgabe = 22.0,
            PersonenImZimmer = false
        };
        var jalZimmer = new ZimmerMitJalousiesteuerung(baseZimmer);
        var wetterdaten = new MockWetterdaten { Aussentemperatur = 30.0 };

        // Act
        jalZimmer.VerarbeiteWetterdaten(wetterdaten);

        // Assert
        Assert.True(jalZimmer.JalousieHeruntergefahren);
        Assert.True(baseZimmer.VerarbeiteWetterdatenCalled);
    }

    [Fact]
    public void VerarbeiteWetterdaten_Soll_Jalousie_Nicht_Schliessen_Wenn_Personen_Im_Zimmer()
    {
        // Arrange
        var baseZimmer = new MockZimmer("Büro")
        {
            Temperaturvorgabe = 22.0,
            PersonenImZimmer = true
        };
        var jalZimmer = new ZimmerMitJalousiesteuerung(baseZimmer);
        var wetterdaten = new MockWetterdaten { Aussentemperatur = 30.0 };

        // Act
        jalZimmer.VerarbeiteWetterdaten(wetterdaten);

        // Assert
        Assert.False(jalZimmer.JalousieHeruntergefahren);
    }

    [Fact]
    public void VerarbeiteWetterdaten_Soll_Jalousie_Oeffnen_Wenn_Kuehler_Wird()
    {
        // Arrange
        var baseZimmer = new MockZimmer("Küche")
        {
            Temperaturvorgabe = 22.0,
            PersonenImZimmer = false
        };
        var jalZimmer = new ZimmerMitJalousiesteuerung(baseZimmer);

        // Simulate: closed blinds first
        jalZimmer.VerarbeiteWetterdaten(new MockWetterdaten { Aussentemperatur = 30.0 });
        Assert.True(jalZimmer.JalousieHeruntergefahren);

        // Act: now it’s cooler
        jalZimmer.VerarbeiteWetterdaten(new MockWetterdaten { Aussentemperatur = 15.0 });

        // Assert
        Assert.False(jalZimmer.JalousieHeruntergefahren);
    }

    [Fact]
    public void VerarbeiteWetterdaten_Soll_Nicht_Erwiederholt_Schliessen_Wenn_Bereits_Geschlossen()
    {
        // Arrange
        var baseZimmer = new MockZimmer("Bad")
        {
            Temperaturvorgabe = 22.0,
            PersonenImZimmer = false
        };
        var jalZimmer = new ZimmerMitJalousiesteuerung(baseZimmer);
        var wetterdaten = new MockWetterdaten { Aussentemperatur = 30.0 };

        // Act: close once
        jalZimmer.VerarbeiteWetterdaten(wetterdaten);
        Assert.True(jalZimmer.JalousieHeruntergefahren);

        // Act again (should remain closed)
        jalZimmer.VerarbeiteWetterdaten(wetterdaten);

        // Assert
        Assert.True(jalZimmer.JalousieHeruntergefahren);
    }

    [Fact]
    public void VerarbeiteWetterdaten_Soll_Nicht_Erwiederholt_Oeffnen_Wenn_Bereits_Offen()
    {
        // Arrange
        var baseZimmer = new MockZimmer("Flur")
        {
            Temperaturvorgabe = 22.0,
            PersonenImZimmer = false
        };
        var jalZimmer = new ZimmerMitJalousiesteuerung(baseZimmer);
        var wetterdaten = new MockWetterdaten { Aussentemperatur = 15.0 };

        // Act: already open (default)
        jalZimmer.VerarbeiteWetterdaten(wetterdaten);

        // Assert
        Assert.False(jalZimmer.JalousieHeruntergefahren);
    }
}