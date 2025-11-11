using System;
using System.IO;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartHomeSimulation.Tests.IsolationTests;

namespace SmartHomeSimulation.Tests.IntegrationTests.AktorIntegrationTests;

[TestClass]
public class ZimmerMitIntelligenterLüftungTest
{
    [TestMethod]
    public void IntelligenteLüftung_ShouldClose_WhenAussentemperaturAboveVorgabe()
    {
        var writer = new StringWriter();
        Console.SetOut(writer);
        var fakeZimmer = new FakeZimmer("Schlafzimmer") { Temperaturvorgabe = 20.0 };
        var zimmer = new ZimmerMitIntelligenterLüftung(fakeZimmer);
        var wetter = new Wettersensor().GetWetterdaten();

        typeof(ZimmerMitIntelligenterLüftung)
            .GetProperty("LüftungLäuft")!
            .SetValue(zimmer, true);
        
        zimmer.VerarbeiteWetterdaten(wetter);
        
        if (zimmer.Temperaturvorgabe > wetter.Aussentemperatur)
        {
            if (!wetter.Regen)
            {
                if (zimmer.PersonenImZimmer)
                {
                    Assert.IsTrue( zimmer.LüftungLäuft, "Ventil should open when it is colder than the target temperature.");
                }
                else
                {
                    Assert.IsFalse( zimmer.LüftungLäuft, "Ventil should close when it is warmer than the target temperature.");    
                }
            }
            else
            {
                Assert.IsFalse( zimmer.LüftungLäuft, "Ventil should close when it is warmer than the target temperature.");    
            }
        }
        else
        {
            Assert.IsFalse( zimmer.LüftungLäuft, "Ventil should close when it is warmer than the target temperature.");    
        }
    }

}