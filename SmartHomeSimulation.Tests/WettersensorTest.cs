using System;
using JetBrains.Annotations;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartHomeSimulation.Tests;

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
    public void GetWetterdaten_ShouldReturnValuesWithinExpectedRanges()
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

        // Regen should be a boolean, but we assert its validity anyway
        Assert.IsInstanceOfType(wetterdaten.Regen, typeof(bool));
    }

    [TestMethod]
    public void GetWetterdaten_ShouldChangeTemperatureOverTime()
    {
        // Arrange
        var sensor = new Wettersensor();

        // Act
        var first = sensor.GetWetterdaten();
        var second = sensor.GetWetterdaten();

        // Assert (temperature should usually change)
        Assert.AreNotEqual(first.Aussentemperatur, second.Aussentemperatur,
            "Temperature did not change between readings (possible but unlikely).");
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