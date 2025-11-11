using System;
using System.IO;
using JetBrains.Annotations;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartHomeSimulation.Tests.IsolationTests
{
    [TestClass]
    public class ZimmerMitIntelligenterLüftungTest
    {
        [TestMethod]
        public void VerarbeiteWetterdaten_LueftungSollEinschalten_WennKuehlerUndPersonenImZimmer()
        {
            using var writer = new StringWriter();
            Console.SetOut(writer);
            var fakeZimmer = new FakeZimmer("Wohnzimmer") { Temperaturvorgabe = 22.0 };
            var zimmer = new ZimmerMitIntelligenterLüftung(fakeZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 18.0 };
            zimmer.PersonenImZimmer = true;
            
            
            zimmer.VerarbeiteWetterdaten(wetter);
            Assert.IsTrue(zimmer.LüftungLäuft);
            Assert.Contains("Lüftung wird eingeschaltet", writer.ToString());
        }

        [TestMethod]
        public void VerarbeiteWetterdaten_SollAusschalten_WennNiemandImZimmer()
        {
            using var writer = new StringWriter();
            Console.SetOut(writer);
            var fakeZimmer = new FakeZimmer("Wohnzimmer") {Temperaturvorgabe = 22.0};
            var zimmer = new ZimmerMitIntelligenterLüftung(fakeZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 18.0 , Regen = false};
            zimmer.PersonenImZimmer = false;
            
            zimmer.VerarbeiteWetterdaten(wetter);
            
            Assert.IsFalse(zimmer.LüftungLäuft);
        }

        [TestMethod]
        public void VerarbeiteWetterdaten_SollAusschalten_WennAussenWaermer()
        {
            using var writer = new StringWriter();
            Console.SetOut(writer);
            var fakeZimmer = new FakeZimmer("Wohnzimmer") {Temperaturvorgabe = 20.0};
            var zimmer = new ZimmerMitIntelligenterLüftung(fakeZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 25.0 , Regen = false};
            zimmer.PersonenImZimmer = true;
            
            zimmer.VerarbeiteWetterdaten(wetter);
            
            Assert.IsFalse(zimmer.LüftungLäuft);
            
        }

        [TestMethod]
        public void VerarbeiteWetterdaten_SollAusschalten_WennEsRegnet()
        {
            using var writer = new StringWriter();
            Console.SetOut(writer);
            var fakeZimmer = new FakeZimmer("Wohnzimmer") {Temperaturvorgabe = 22.0};
            var zimmer = new ZimmerMitIntelligenterLüftung(fakeZimmer);
            var wetter = new Wetterdaten { Aussentemperatur = 18.0 , Regen = true};
            zimmer.PersonenImZimmer = true;
            
            zimmer.VerarbeiteWetterdaten(wetter);
            
            Assert.IsFalse(zimmer.LüftungLäuft);

        }
    }
    
}
