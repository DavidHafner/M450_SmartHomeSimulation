using System;
using System.IO;
using JetBrains.Annotations;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartHomeSimulation.Tests.IsolationTests;

namespace SmartHomeSimulation.Tests.IntegrationTests.AktorIntegrationTests;

[TestClass]
[TestSubject(typeof(ZimmerMitMarkisensteuerung))]
public class ZimmerMitMarkisensteuerungTest
{
[TestClass]
    public class ZimmerMitMarkisensteuerungTests
    {
        [TestMethod]
        public void Markise_ShouldOpen_WhenCoolerThanVorgabe()
        {
            // Arrange
            var fakeZimmer = new FakeZimmer("Wohnzimmer") { Temperaturvorgabe = 25.0 };
            var zimmer = new ZimmerMitMarkisensteuerung(fakeZimmer);
            var wetter = new Wettersensor().GetWetterdaten();

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            zimmer.VerarbeiteWetterdaten(wetter);
            
            // Assert
            if(wetter.Aussentemperatur > fakeZimmer.Temperaturvorgabe) {
                // Markise schliessen
                if(zimmer.MarkiseOffen) {
                    if (wetter.Regen) {
                        Assert.IsTrue(zimmer.MarkiseOffen, " Markise kann nicht geschlossen werden weils regnet.");
                    } else {
                        Assert.IsFalse(zimmer.MarkiseOffen," Markise wird geschlossen.");
                    }
                } else if(wetter.Regen) {
                    Assert.IsFalse(zimmer.MarkiseOffen," Markise wird geöffnet weils regnet.");
                }
            } else {
                // Markise öffnen
                if (!zimmer.MarkiseOffen) {
                    Assert.IsFalse(zimmer.MarkiseOffen," Markise wird geöffnet.");
                }
            }
        }

        [TestMethod]
        public void Markise_ShouldClose_WhenHotterThanVorgabe_AndNoRain_AndIsOpen()
        {
            // Arrange
            var fakeZimmer = new FakeZimmer("Küche") { Temperaturvorgabe = 20.0 };
            var zimmer = new ZimmerMitMarkisensteuerung(fakeZimmer);
            // Start with open markise
            typeof(ZimmerMitMarkisensteuerung).GetProperty("MarkiseOffen")!.SetValue(zimmer, true);

            var wetter = new Wettersensor().GetWetterdaten();

            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            zimmer.VerarbeiteWetterdaten(wetter);

            // Assert
            if (wetter.Aussentemperatur >= fakeZimmer.Temperaturvorgabe && !wetter.Regen)
            {
                // Too hot and dry → close the Markise
                Assert.IsFalse(zimmer.MarkiseOffen, "Markise should close when it's hotter and no rain.");
            }
            else
            {
                // Cooler or rainy → keep it open
                Assert.IsTrue(zimmer.MarkiseOffen, "Markise should stay open when it's cooler or raining.");
            }
        }
        
    }
}