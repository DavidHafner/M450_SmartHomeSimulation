using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using M320_SmartHome;

namespace SmartHomeSimulation.Tests.IntegrationTests.AktorIntegrationTests;


[TestClass]
    public class AktorenIntegrationTests {

        [TestMethod]
        public void HeatingValve_Responds_To_Wettersensor() {
            var writer = new StringWriter();
            Console.SetOut(writer);
            var sensor = new Wettersensor();
            var baseZimmer = new Wohnzimmer(); // any room
            baseZimmer.Temperaturvorgabe = 22;
            var ventilZimmer = new ZimmerMitHeizungsventil(baseZimmer);

            for (int i = 0; i < 40; i++) {
                var wetter = sensor.GetWetterdaten();
                ventilZimmer.VerarbeiteWetterdaten(wetter);
                bool expectedOpen = wetter.Aussentemperatur < baseZimmer.Temperaturvorgabe;
                Assert.AreEqual(expectedOpen, ventilZimmer.HeizungsventilOffen, $"Iteration {i}: Erwarteter Ventil-Zustand falsch.");
            }
        }

        [TestMethod]
        public void IntelligenteLueftung_Responds_To_Wettersensor() {
            var writer = new StringWriter();
            Console.SetOut(writer);
            var sensor = new Wettersensor();
            var baseZimmer = new Schlafzimmer { Temperaturvorgabe = 21, PersonenImZimmer = true };
            var lueftungZimmer = new ZimmerMitIntelligenterLüftung(baseZimmer);

            for (int i = 0; i < 40; i++) {
                var wetter = sensor.GetWetterdaten();
                lueftungZimmer.VerarbeiteWetterdaten(wetter);
                bool expected =
                    baseZimmer.Temperaturvorgabe > wetter.Aussentemperatur
                    && !wetter.Regen
                    && baseZimmer.PersonenImZimmer;
                Assert.AreEqual(expected, lueftungZimmer.LüftungLäuft, $"Iteration {i}: Erwarteter Lüftungs-Zustand falsch.");
            }
        }

        [TestMethod]
        public void Jalousie_Closes_And_Opens_Correctly_NoPersons() {
            var writer = new StringWriter();
            Console.SetOut(writer);
            var sensor = new Wettersensor();
            var baseZimmer = new Wohnzimmer { Temperaturvorgabe = 22, PersonenImZimmer = false };
            var jalZimmer = new ZimmerMitJalousiesteuerung(baseZimmer);

            bool expectedClosed = false;
            for (int i = 0; i < 50; i++) {
                var wetter = sensor.GetWetterdaten();
                // State machine expectation
                if (wetter.Aussentemperatur > baseZimmer.Temperaturvorgabe && !expectedClosed)
                    expectedClosed = true;
                else if (wetter.Aussentemperatur <= baseZimmer.Temperaturvorgabe && expectedClosed)
                    expectedClosed = false;

                jalZimmer.VerarbeiteWetterdaten(wetter);
                Assert.AreEqual(expectedClosed, jalZimmer.JalousieHeruntergefahren, $"Iteration {i}: Erwarteter Jalousie-Zustand falsch.");
            }
        }

        [TestMethod]
        public void Jalousie_Stays_Open_When_PersonsPresent() {
            var writer = new StringWriter();
            Console.SetOut(writer);
            var sensor = new Wettersensor();
            var baseZimmer = new Wohnzimmer { Temperaturvorgabe = 22, PersonenImZimmer = true };
            var jalZimmer = new ZimmerMitJalousiesteuerung(baseZimmer);

            for (int i = 0; i < 30; i++) {
                var wetter = sensor.GetWetterdaten();
                jalZimmer.VerarbeiteWetterdaten(wetter);
                // With persons present it never closes
                Assert.IsFalse(jalZimmer.JalousieHeruntergefahren, $"Iteration {i}: Jalousie sollte offen bleiben (Personen vorhanden).");
            }
        }

        [TestMethod]
        public void Markise_StateMachine_Works() {
            var writer = new StringWriter();
            Console.SetOut(writer);
            var sensor = new Wettersensor();
            var baseZimmer = new Wintergarten { Temperaturvorgabe = 20 };
            var markisenZimmer = new ZimmerMitMarkisensteuerung(baseZimmer);

            bool expectedOffen = false;
            for (int i = 0; i < 60; i++) {
                var wetter = sensor.GetWetterdaten();

                if (wetter.Aussentemperatur > baseZimmer.Temperaturvorgabe) {
                    if (expectedOffen) {
                        if (!wetter.Regen)
                            expectedOffen = false; // schliessen
                        // bei Regen bleibt offen
                    } else {
                        if (wetter.Regen)
                            expectedOffen = true; // öffnen wegen Regen
                    }
                } else { // Aussentemperatur <= Vorgabe
                    if (!expectedOffen)
                        expectedOffen = true; // öffnen
                }

                markisenZimmer.VerarbeiteWetterdaten(wetter);
                Assert.AreEqual(expectedOffen, markisenZimmer.MarkiseOffen, $"Iteration {i}: Erwarteter Markisen-Zustand falsch.");
            }
        }
    }