using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace M320_SmartHome {
    public class Wohnung {
        public List<Zimmer> zimmerList { get; set; }
        private IWettersensor wettersensor;

        public Wohnung(IWettersensor wettersensor) {
            // Wettersensor ggf. in einem ersten Schritt hier generieren. DAnn kann das später beim Testing für den WettersensorMock nach IoC umgebaut werden.
            this.wettersensor = wettersensor;

            zimmerList = new List<Zimmer>();
            this.zimmerList.Add(new ZimmerMitHeizungsventil(new ZimmerMitIntelligenterLüftung(new BadWC())));
            this.zimmerList.Add(new ZimmerMitJalousiesteuerung(new ZimmerMitHeizungsventil(new Kueche())));
            this.zimmerList.Add(new ZimmerMitJalousiesteuerung(new ZimmerMitHeizungsventil(new ZimmerMitIntelligenterLüftung(new Schlafzimmer()))));
            this.zimmerList.Add(new ZimmerMitJalousiesteuerung(new ZimmerMitMarkisensteuerung(new Wintergarten())));
            this.zimmerList.Add(new ZimmerMitJalousiesteuerung(new ZimmerMitHeizungsventil(new ZimmerMitIntelligenterLüftung(new Wohnzimmer()))));
        }

        public void SetTemperaturvorgabe(string zimmername, double temperaturvorgabe) {
            var zimmer = this.zimmerList.FirstOrDefault(x => x.Name == zimmername);
            if(zimmer != null) {
                zimmer.Temperaturvorgabe = temperaturvorgabe;
            }
        }

        public void SetPersonenImZimmer(string zimmername, bool personenImZimmer) {
            var zimmer = this.zimmerList.FirstOrDefault(x => x.Name == zimmername);
            if (zimmer != null) {
                zimmer.PersonenImZimmer = personenImZimmer;
            }
        }

        public void GenerateWetterdaten(int minute = 1) {
            var wetterdaten = this.wettersensor.GetWetterdaten();

            Console.WriteLine($"\n*** Minute {minute}, Verarbeite Wetterdaten:\n    Aussentemperatur: {wetterdaten.Aussentemperatur}°C\n    Regen: {(wetterdaten.Regen ? "ja" : "nein")}\n    Windgeschwindigkeit: {wetterdaten.Windgeschwindigkeit}km/h");
            foreach(var zimmer in this.zimmerList) {
                zimmer.VerarbeiteWetterdaten(wetterdaten);
            }
        }

        public T GetZimmer<T>(string zimmername) where T : Zimmer
        {
            var zimmer = this.zimmerList.FirstOrDefault(x => x.Name == zimmername);
            if (zimmer is ZimmerMitAktor)
            {
                return ((ZimmerMitAktor)zimmer).GetZimmerMitAktor<T>();
            }
            return zimmer as T;
        }
    }
}
