using System;
using JetBrains.Annotations;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartHomeSimulation.Tests.IsolationTests;

[TestClass]
[TestSubject(typeof(Wettersensor))]
public class WettersensorTest
{

    [TestMethod]
    public void Constructor_ShouldInitializeCurrentTempWithinRange()
    {
        // Arrange & Act
        var sensor = new Wettersensor();

        var wetterdaten = sensor.GetWetterdaten();

        // Assert
        Assert.IsTrue(
            wetterdaten.Aussentemperatur >= -25 && wetterdaten.Aussentemperatur <= 35,
            $"Temperature out of range: {wetterdaten.Aussentemperatur}"
        );
    }

    [TestMethod]
    public void GetWetterdaten_SolltenInnerhalbDesAngegebenenWertebereichsSein()
    {
        // Arrange
        var sensor = new Wettersensor();

        // Act
        var wetterdaten = sensor.GetWetterdaten();

        // Assert
        Assert.IsTrue(wetterdaten.Aussentemperatur >= -25 && wetterdaten.Aussentemperatur <= 35,
            $"Temperature out of range: {wetterdaten.Aussentemperatur}");

        Assert.IsTrue(wetterdaten.Windgeschwindigkeit >= 0 && wetterdaten.Windgeschwindigkeit <= 35,
            $"Wind speed out of range: {wetterdaten.Windgeschwindigkeit}");

        // Checks if Regen is the correct datatype
        Assert.IsInstanceOfType(wetterdaten.Regen, typeof(bool));
    }
    

    [TestMethod]
    public void GetWetterdaten_ShouldRoundTemperatureToOneDecimal()
    {
        // Arrange
        var sensor = new Wettersensor();

        // Act
        var wetterdaten = sensor.GetWetterdaten();

        // Assert
        double rounded = Math.Round(wetterdaten.Aussentemperatur, 1);
        Assert.AreEqual(rounded, wetterdaten.Aussentemperatur,
            "Temperature is not rounded to one decimal place.");
    }

}