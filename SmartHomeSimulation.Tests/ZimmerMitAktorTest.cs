using JetBrains.Annotations;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartHomeSimulation.Tests;

[TestClass]
[TestSubject(typeof(ZimmerMitAktor))]
public class ZimmerMitAktorTest 
{

    [TestMethod]
    public void METHOD()
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

public class MockZimmerMitAktor : ZimmerMitAktor
{
    public MockZimmerMitAktor(Zimmer zimmer) : base(zimmer) { }
}

public class MockWetterdaten : Wetterdaten
{
    public string Info { get; set; }
}

// --- Actual Test Class ---

public class ZimmerMitAktorTests
{
    [Fact]
    public void Konstruktor_Soll_Zimmer_Setzen()
    {
        // Arrange
        var mockZimmer = new MockZimmer("Wohnzimmer");

        // Act
        var aktorZimmer = new MockZimmerMitAktor(mockZimmer);

        // Assert
        Assert.Equal("Wohnzimmer", aktorZimmer.Name);
        Assert.NotNull(aktorZimmer);
    }

    [Fact]
    public void Temperaturvorgabe_GetUndSet_Soll_DurchgereichtWerden()
    {
        // Arrange
        var mockZimmer = new MockZimmer("Schlafzimmer");
        var aktorZimmer = new MockZimmerMitAktor(mockZimmer);

        // Act
        aktorZimmer.Temperaturvorgabe = 22.5;

        // Assert
        Assert.Equal(22.5, mockZimmer.Temperaturvorgabe);
        Assert.Equal(22.5, aktorZimmer.Temperaturvorgabe);
    }

    [Fact]
    public void PersonenImZimmer_GetUndSet_Soll_DurchgereichtWerden()
    {
        // Arrange
        var mockZimmer = new MockZimmer("Bad");
        var aktorZimmer = new MockZimmerMitAktor(mockZimmer);

        // Act
        aktorZimmer.PersonenImZimmer = true;

        // Assert
        Assert.True(mockZimmer.PersonenImZimmer);
        Assert.True(aktorZimmer.PersonenImZimmer);
    }

    [Fact]
    public void VerarbeiteWetterdaten_Soll_An_Unterliegendes_Zimmer_WeitergeleitetWerden()
    {
        // Arrange
        var mockZimmer = new MockZimmer("Küche");
        var aktorZimmer = new MockZimmerMitAktor(mockZimmer);
        var wetterdaten = new MockWetterdaten { Info = "Sonnig" };

        // Act
        aktorZimmer.VerarbeiteWetterdaten(wetterdaten);

        // Assert
        Assert.True(mockZimmer.VerarbeiteWetterdatenCalled);
        Assert.Equal(wetterdaten, mockZimmer.LetzteWetterdaten);
    }

    [Fact]
    public void GetZimmerMitAktor_Soll_Aktor_Selbst_Zurueckgeben_Wenn_Typ_Passt()
    {
        // Arrange
        var mockZimmer = new MockZimmer("Wohnzimmer");
        var aktorZimmer = new MockZimmerMitAktor(mockZimmer);

        // Act
        var result = aktorZimmer.GetZimmerMitAktor<MockZimmerMitAktor>();

        // Assert
        Assert.Same(aktorZimmer, result);
    }

    [Fact]
    public void GetZimmerMitAktor_Soll_Null_Zurueckgeben_Wenn_Typ_Nicht_Gefunden()
    {
        // Arrange
        var mockZimmer = new MockZimmer("Flur");
        var aktorZimmer = new MockZimmerMitAktor(mockZimmer);

        // Act
        var result = aktorZimmer.GetZimmerMitAktor<MockZimmer>();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetZimmerMitAktor_Soll_Rekursiv_Suchen()
    {
        // Arrange
        var baseZimmer = new MockZimmer("Kinderzimmer");
        var innerAktor = new MockZimmerMitAktor(baseZimmer);
        var outerAktor = new MockZimmerMitAktor(innerAktor);

        // Act
        var result = outerAktor.GetZimmerMitAktor<MockZimmerMitAktor>();

        // Assert
        Assert.NotNull(result);
    }

}
}