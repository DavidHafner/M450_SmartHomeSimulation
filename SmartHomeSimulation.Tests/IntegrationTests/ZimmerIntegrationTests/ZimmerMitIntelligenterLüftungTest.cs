using System;
using System.IO;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartHomeSimulation.Tests.IsolationTests;

namespace SmartHomeSimulation.Tests.IntegrationTests;

[TestClass]
public class ZimmerMitIntelligenterLüftungTest
{
    [TestMethod]
    public void IntelligenteLüftung_ShouldStop_WhenAussentemperaturAboveVorgabe()
    {
        var writer = new StringWriter();
        Console.SetOut(writer);
        var fakeZimmer = new FakeZimmer("Schlafzimmer") { Temperaturvorgabe = 20.0, PersonenImZimmer = false };
        var zimmer = new ZimmerMitIntelligenterLüftung(fakeZimmer);
        Wetterdaten wetter = new() { Aussentemperatur = 25, Regen = false, Windgeschwindigkeit = 30 };

        zimmer.VerarbeiteWetterdaten(wetter);

  
        Assert.IsFalse(zimmer.LüftungLäuft, "Ventil should close when it is warmer than the target temperature.");
        
    }

    [TestMethod]
    public void IntelligenteLüftung_ShouldStart_WhenAussentemperaturBelowVorgabe()
    {
        var writer = new StringWriter();
        Console.SetOut(writer);
        var fakeZimmer = new FakeZimmer("Schlafzimmer") { Temperaturvorgabe = 25.0, PersonenImZimmer = true };
        var zimmer = new ZimmerMitIntelligenterLüftung(fakeZimmer);
        Wetterdaten wetter = new() { Aussentemperatur = 20, Regen = false, Windgeschwindigkeit = 30 };

        zimmer.VerarbeiteWetterdaten(wetter);

        Assert.IsTrue(zimmer.LüftungLäuft, "Ventil should open when it is colder than the target temperature.");
    }
    
    [TestMethod]
    public void IntelligenteLüftung_ShouldStart_WhenAussentemperaturBelowVorgabeAndPersonImZimmer()
    {
        var writer = new StringWriter();
        Console.SetOut(writer);
        var fakeZimmer = new FakeZimmer("Schlafzimmer") { Temperaturvorgabe = 25.0, PersonenImZimmer = true };
        var zimmer = new ZimmerMitIntelligenterLüftung(fakeZimmer);
        Wetterdaten wetter = new() { Aussentemperatur = 20, Regen = false, Windgeschwindigkeit = 30 };

        zimmer.VerarbeiteWetterdaten(wetter);

        Assert.IsTrue(zimmer.LüftungLäuft, "Ventil should open when it is colder than the target temperature.");
    }
    
    [TestMethod]
    public void IntelligenteLüftung_ShouldStop_WhenAussentemperaturBelowVorgabeAndPersonNichtImZimmer()
    {
        var writer = new StringWriter();
        Console.SetOut(writer);
        var fakeZimmer = new FakeZimmer("Schlafzimmer") { Temperaturvorgabe = 25.0, PersonenImZimmer = false };
        var zimmer = new ZimmerMitIntelligenterLüftung(fakeZimmer);
        Wetterdaten wetter = new() { Aussentemperatur = 20, Regen = false, Windgeschwindigkeit = 30 };

        zimmer.VerarbeiteWetterdaten(wetter);

        Assert.IsFalse(zimmer.LüftungLäuft, "Ventil should open when it is colder than the target temperature.");
    }
    
        [TestMethod]
    public void IntelligenteLüftung_ShouldStop_WhenAussentemperaturAboveVorgabeAndEsRegnet()
    {
        var writer = new StringWriter();
        Console.SetOut(writer);
        var fakeZimmer = new FakeZimmer("Schlafzimmer") { Temperaturvorgabe = 20.0, PersonenImZimmer = false };
        var zimmer = new ZimmerMitIntelligenterLüftung(fakeZimmer);
        Wetterdaten wetter = new() { Aussentemperatur = 25, Regen = true, Windgeschwindigkeit = 30 };

        zimmer.VerarbeiteWetterdaten(wetter);

  
        Assert.IsFalse(zimmer.LüftungLäuft, "Ventil should close when it is warmer than the target temperature.");
        
    }

    [TestMethod]
    public void IntelligenteLüftung_ShouldStart_WhenAussentemperaturBelowVorgabeAndEsRegnet()
    {
        var writer = new StringWriter();
        Console.SetOut(writer);
        var fakeZimmer = new FakeZimmer("Schlafzimmer") { Temperaturvorgabe = 25.0, PersonenImZimmer = true };
        var zimmer = new ZimmerMitIntelligenterLüftung(fakeZimmer);
        Wetterdaten wetter = new() { Aussentemperatur = 20, Regen = true, Windgeschwindigkeit = 30 };

        zimmer.VerarbeiteWetterdaten(wetter);

        Assert.IsFalse(zimmer.LüftungLäuft, "Ventil should open when it is colder than the target temperature.");
    }
    
    [TestMethod]
    public void IntelligenteLüftung_ShouldStart_WhenAussentemperaturBelowVorgabeAndPersonImZimmerAndEsRegnet()
    {
        var writer = new StringWriter();
        Console.SetOut(writer);
        var fakeZimmer = new FakeZimmer("Schlafzimmer") { Temperaturvorgabe = 25.0, PersonenImZimmer = true };
        var zimmer = new ZimmerMitIntelligenterLüftung(fakeZimmer);
        Wetterdaten wetter = new() { Aussentemperatur = 20, Regen = true, Windgeschwindigkeit = 30 };

        zimmer.VerarbeiteWetterdaten(wetter);

        Assert.IsTrue(zimmer.LüftungLäuft, "Ventil should open when it is colder than the target temperature.");
    }
    
    [TestMethod]
    public void IntelligenteLüftung_ShouldStop_WhenAussentemperaturBelowVorgabeAndPersonNichtImZimmerAndEsRegnet()
    {
        var writer = new StringWriter();
        Console.SetOut(writer);
        var fakeZimmer = new FakeZimmer("Schlafzimmer") { Temperaturvorgabe = 25.0, PersonenImZimmer = false };
        var zimmer = new ZimmerMitIntelligenterLüftung(fakeZimmer);
        Wetterdaten wetter = new() { Aussentemperatur = 20, Regen = true, Windgeschwindigkeit = 30 };

        zimmer.VerarbeiteWetterdaten(wetter);

        Assert.IsFalse(zimmer.LüftungLäuft, "Ventil should open when it is colder than the target temperature.");
    }
}