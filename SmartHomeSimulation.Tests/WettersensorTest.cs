using JetBrains.Annotations;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartHomeSimulation.Tests;

[TestClass]
[TestSubject(typeof(Wettersensor))]
public class WettersensorTest
{

    [TestMethod]
    public void METHOD()
    {
        [Fact]
        public void Constructor_ShouldInitializeTemperatureWithinRange()
        {
            // Arrange & Act
            var sensor = new Wettersensor();

            var wetterdaten = sensor.GetWetterdaten();

            // Assert
            Assert.InRange(wetterdaten.Aussentemperatur, -25, 35);
        }

        [Fact]
        public void GetWetterdaten_ShouldReturnValidWindgeschwindigkeit()
        {
            // Arrange
            var sensor = new Wettersensor();

            // Act
            var wetterdaten = sensor.GetWetterdaten();

            // Assert
            Assert.InRange(wetterdaten.Windgeschwindigkeit, 0, 35);
        }

        [Fact]
        public void GetWetterdaten_ShouldReturnBooleanRegenValue()
        {
            // Arrange
            var sensor = new Wettersensor();

            // Act
            var wetterdaten = sensor.GetWetterdaten();

            // Assert
            Assert.IsType<bool>(wetterdaten.Regen);
        }

        [Fact]
        public void GetWetterdaten_ShouldReturnTemperatureWithinLimitsOverMultipleCalls()
        {
            // Arrange
            var sensor = new Wettersensor();

            // Act
            for (int i = 0; i < 100; i++)
            {
                var wetterdaten = sensor.GetWetterdaten();

                // Assert
                Assert.InRange(wetterdaten.Aussentemperatur, -25, 35);
            }
        }

        [Fact]
        public void GetWetterdaten_ShouldChangeTemperatureOverTime()
        {
            // Arrange
            var sensor = new Wettersensor();

            var first = sensor.GetWetterdaten();
            bool changed = false;

            // Act
            for (int i = 0; i < 10; i++)
            {
                var next = sensor.GetWetterdaten();
                if (next.Aussentemperatur != first.Aussentemperatur)
                {
                    changed = true;
                    break;
                }
            }

            // Assert
            Assert.True(changed, "Temperature should eventually change after several readings.");
        }
    }
}