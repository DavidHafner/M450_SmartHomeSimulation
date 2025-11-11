namespace M320_SmartHome {
    internal class Program {
        static void Main(string[] args) {
            int zeitdauerMinuten = 30;

            var wettersensor = new Wettersensor();
            var wohnung = new Wohnung();

            wohnung.SetTemperaturvorgabe("BadWC", 23);
            wohnung.SetTemperaturvorgabe("Kueche", 22);
            wohnung.SetTemperaturvorgabe("Schlafen", 19);
            wohnung.SetTemperaturvorgabe("Wohnen", 22);
            wohnung.SetTemperaturvorgabe("Wintergarten", 20);

            wohnung.SetPersonenImZimmer("Kueche", true);
            wohnung.SetPersonenImZimmer("Schlafen", false);
            wohnung.SetPersonenImZimmer("Wohnen", true);
            wohnung.SetPersonenImZimmer("Wintergarten", true);

            for(var i = 0; i<zeitdauerMinuten; i++)
            {
                Wetterdaten wetterdaten = wettersensor.GetWetterdaten();
                wohnung.HandleWetterdaten(i+1,wetterdaten);
            }
        }
    }
}