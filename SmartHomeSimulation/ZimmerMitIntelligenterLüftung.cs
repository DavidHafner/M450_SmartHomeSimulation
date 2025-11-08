namespace M320_SmartHome;

public class ZimmerMitIntelligenterLüftung:ZimmerMitAktor
{
    public ZimmerMitIntelligenterLüftung(Zimmer zimmer) : base(zimmer) {
    }
    public bool LüftungLäuft { get; set; }
    public override void VerarbeiteWetterdaten(Wetterdaten wetterdaten) {
        // if(wetterdaten.Aussentemperatur < this.Zimmer.Temperaturvorgabe&&!wetterdaten.Regen) {
        //     // Lüftung starten
        //     if (this.LüftungLäuft) {
        //         Console.WriteLine($"{this.Name}: Lüftung wird gestartet.");
        //         LüftungLäuft = true;
        //     }
        // } else {
        //     // Lüftung stoppen
        //     if(!this.LüftungLäuft) {
        //         if (this.Zimmer.PersonenImZimmer) {
        //             Console.WriteLine($"{this.Name}: Lüftung kann nicht ausgeschaltet werden weil Personen im Zimmer sind.");
        //         } else {
        //             Console.WriteLine($"{this.Name}: Lüftung wird ausgeschaltet.");
        //             LüftungLäuft = false;
        //         }
        //     }
        // }

        if (Temperaturvorgabe > wetterdaten.Aussentemperatur)
        {
            if (!wetterdaten.Regen)
            {
                if (PersonenImZimmer)
                {
                    Console.WriteLine($"{this.Name}: Lüftung wird eingeschaltet.");
                    LüftungLäuft = true;
                }
                else
                {
                    Console.WriteLine($"{this.Name}: Lüftung wird ausgeschaltet, da niemand im Zimmer ist.");
                    LüftungLäuft = false;
                }
            }
            else
            {
                Console.WriteLine($"{this.Name}: Lüftung wird ausgeschaltet, da es regnet.");
                LüftungLäuft = false;
            }
        }
        else
        {
            Console.WriteLine($"{this.Name}: Lüftung wird ausgeschaltet, da die Aussentemperatur höher ist als die Innentemperatur.");
            LüftungLäuft = false;
        }
        
        base.VerarbeiteWetterdaten(wetterdaten);
    }
}