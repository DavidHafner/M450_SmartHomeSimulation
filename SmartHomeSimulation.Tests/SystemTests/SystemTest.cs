using System;
using M320_SmartHome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartHomeSimulation.Tests.SystemTests;

[TestClass]
public class SystemTest
{
    private const int MAX_TEMP = 35;
    private const int MIN_TEMP = -25;
    [TestMethod]
    public void SysTest_ShouldPass()
    {
        Random random = new Random(Guid.NewGuid().GetHashCode());
        Wohnung wohnung = new Wohnung();
        Wettersensor wettersensor = new Wettersensor();
        for (int second = 0; second < 24 * 60 * 60; second++)
        {
            //Wettersensor tick
            Wetterdaten wetterdaten = wettersensor.GetWetterdaten();
            wohnung.SetTemperaturvorgabe("BadWC", random.Next(MIN_TEMP,MAX_TEMP));
            wohnung.SetTemperaturvorgabe("Kueche", random.Next(MIN_TEMP,MAX_TEMP));
            wohnung.SetTemperaturvorgabe("Schlafen", random.Next(MIN_TEMP,MAX_TEMP));
            wohnung.SetTemperaturvorgabe("Wohnen", random.Next(MIN_TEMP,MAX_TEMP));
            wohnung.SetTemperaturvorgabe("Wintergarten", random.Next(MIN_TEMP,MAX_TEMP));

            wohnung.SetPersonenImZimmer("Kueche", random.NextDouble()>0.5);
            wohnung.SetPersonenImZimmer("Schlafen", random.NextDouble()>0.5);
            wohnung.SetPersonenImZimmer("Wohnen", random.NextDouble()>0.5);
            wohnung.SetPersonenImZimmer("Wintergarten", random.NextDouble()>0.5);


            // Wohnung tick
            wohnung.HandleWetterdaten(second / 60, wetterdaten);
        }
    }
}