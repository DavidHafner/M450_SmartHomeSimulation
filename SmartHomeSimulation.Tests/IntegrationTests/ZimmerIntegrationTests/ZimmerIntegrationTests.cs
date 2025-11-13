using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using M320_SmartHome;
namespace SmartHomeSimulation.Tests.IntegrationTests;

[TestClass]
    public class ZimmerIntegrationTests {

        [TestMethod]
        public void Wohnung_Actors_React_Correctly_To_Wettersensor() {
            var writer = new StringWriter();
            Console.SetOut(writer);
            var sensor = new Wettersensor();
            var wohnung = new Wohnung();

            // Set temperature targets (handle name variant for Küche)
            wohnung.SetTemperaturvorgabe("BadWC", 23);
            wohnung.SetTemperaturvorgabe("Küche", 22);
            wohnung.SetTemperaturvorgabe("Kueche", 22);
            wohnung.SetTemperaturvorgabe("Schlafen", 19);
            wohnung.SetTemperaturvorgabe("Wohnen", 22);
            wohnung.SetTemperaturvorgabe("Wohnzimmer", 22);
            wohnung.SetTemperaturvorgabe("Wintergarten", 20);

            wohnung.SetPersonenImZimmer("Küche", true);
            wohnung.SetPersonenImZimmer("Schlafen", false);
            wohnung.SetPersonenImZimmer("Wohnzimmer", true);
            wohnung.SetPersonenImZimmer("Wintergarten", true);
            wohnung.SetPersonenImZimmer("BadWC", true);

            // Retrieve actor wrappers
            var badHeizung = wohnung.GetZimmer<ZimmerMitHeizungsventil>("BadWC");
            var badLueftung = wohnung.GetZimmer<ZimmerMitIntelligenterLüftung>("BadWC");

            var kuecheHeizung = wohnung.GetZimmer<ZimmerMitHeizungsventil>("Küche");
            var kuecheJalousie = wohnung.GetZimmer<ZimmerMitJalousiesteuerung>("Küche");

            var schlafenHeizung = wohnung.GetZimmer<ZimmerMitHeizungsventil>("Schlafen");
            var schlafenLueftung = wohnung.GetZimmer<ZimmerMitIntelligenterLüftung>("Schlafen");
            var schlafenJalousie = wohnung.GetZimmer<ZimmerMitJalousiesteuerung>("Schlafen");

            var winterMarkise = wohnung.GetZimmer<ZimmerMitMarkisensteuerung>("Wintergarten");
            var winterJalousie = wohnung.GetZimmer<ZimmerMitJalousiesteuerung>("Wintergarten");

            var wohnHeizung = wohnung.GetZimmer<ZimmerMitHeizungsventil>("Wohnzimmer");
            var wohnLueftung = wohnung.GetZimmer<ZimmerMitIntelligenterLüftung>("Wohnzimmer");
            var wohnJalousie = wohnung.GetZimmer<ZimmerMitJalousiesteuerung>("Wohnzimmer");

            bool expectedWinterMarkiseOffen = false;
            bool expectedWinterJalousieHerunter = false;
            bool expectedKuecheJalousieHerunter = false;
            bool expectedSchlafenJalousieHerunter = false;
            bool expectedWohnJalousieHerunter = false;

            for (int i = 0; i < 50; i++) {
                var wetter = sensor.GetWetterdaten();
                wohnung.HandleWetterdaten(i + 1, wetter);

                // Heizungsventile expectations
                Assert.AreEqual(wetter.Aussentemperatur < badHeizung.Temperaturvorgabe, badHeizung.HeizungsventilOffen);
                Assert.AreEqual(wetter.Aussentemperatur < kuecheHeizung.Temperaturvorgabe, kuecheHeizung.HeizungsventilOffen);
                Assert.AreEqual(wetter.Aussentemperatur < schlafenHeizung.Temperaturvorgabe, schlafenHeizung.HeizungsventilOffen);
                Assert.AreEqual(wetter.Aussentemperatur < wohnHeizung.Temperaturvorgabe, wohnHeizung.HeizungsventilOffen);

                // Lüftung expectations
                bool LüftungBad = badLueftung.Temperaturvorgabe > wetter.Aussentemperatur && !wetter.Regen && badLueftung.PersonenImZimmer;
                Assert.AreEqual(LüftungBad, badLueftung.LüftungLäuft);

                bool LüftungSchlafen = schlafenLueftung.Temperaturvorgabe > wetter.Aussentemperatur && !wetter.Regen && schlafenLueftung.PersonenImZimmer;
                Assert.AreEqual(LüftungSchlafen, schlafenLueftung.LüftungLäuft);

                bool LüftungWohn = wohnLueftung.Temperaturvorgabe > wetter.Aussentemperatur && !wetter.Regen && wohnLueftung.PersonenImZimmer;
                Assert.AreEqual(LüftungWohn, wohnLueftung.LüftungLäuft);

                // Jalousie state machines (persons may prevent closure)
                UpdateJalousie(ref expectedKuecheJalousieHerunter, kuecheJalousie, wetter);
                UpdateJalousie(ref expectedSchlafenJalousieHerunter, schlafenJalousie, wetter);
                UpdateJalousie(ref expectedWohnJalousieHerunter, wohnJalousie, wetter);
                UpdateJalousie(ref expectedWinterJalousieHerunter, winterJalousie, wetter);

                Assert.AreEqual(expectedKuecheJalousieHerunter, kuecheJalousie.JalousieHeruntergefahren);
                Assert.AreEqual(expectedSchlafenJalousieHerunter, schlafenJalousie.JalousieHeruntergefahren);
                Assert.AreEqual(expectedWohnJalousieHerunter, wohnJalousie.JalousieHeruntergefahren);
                Assert.AreEqual(expectedWinterJalousieHerunter, winterJalousie.JalousieHeruntergefahren);

                // Markise state machine
                if (wetter.Aussentemperatur > winterMarkise.Temperaturvorgabe) {
                    if (expectedWinterMarkiseOffen) {
                        if (!wetter.Regen)
                            expectedWinterMarkiseOffen = false;
                    } else {
                        if (wetter.Regen)
                            expectedWinterMarkiseOffen = true;
                    }
                } else {
                    if (!expectedWinterMarkiseOffen)
                        expectedWinterMarkiseOffen = true;
                }
                Assert.AreEqual(expectedWinterMarkiseOffen, winterMarkise.MarkiseOffen);
            }
        }

        private void UpdateJalousie(ref bool expectedState, ZimmerMitJalousiesteuerung jal, Wetterdaten wetter) {
            var writer = new StringWriter();
            Console.SetOut(writer);
            if (wetter.Aussentemperatur > jal.Temperaturvorgabe) {
                if (!expectedState && !jal.PersonenImZimmer)
                    expectedState = true;
            } else {
                if (expectedState)
                    expectedState = false;
            }
        }
    }