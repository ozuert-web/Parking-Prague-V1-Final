using System;

namespace ParkingPragueV1Final
{
    class Program
    {
        static void Main()
        {
            string[] parkering = new string[100];
            for (int i = 0; i < parkering.Length; i++)
                parkering[i] = ""; // empty string = ledig plats

            bool körProgrammet = true;
            while (körProgrammet)
            {
                Console.WriteLine("\n---Prague Parking V1 ---");
                Console.WriteLine("1. Parkera fordon");
                Console.WriteLine("2. Sök Fordon");
                Console.WriteLine("3. Hämta ut fordon");
                Console.WriteLine("4. Flytta fordon");
                Console.WriteLine("0. Avsluta");
                Console.Write("Ditt Val: ");

                string val = Console.ReadLine() ?? "";
                switch (val)
                {
                    case "1":
                        ParkeraFordon(parkering);
                        break;
                    case "2":
                        SökFordon(parkering);
                        break;
                    case "3":
                        HämtaUtFordon(parkering);
                        break;
                    case "4":
                        FlyttaFordon(parkering);
                        break;
                    case "0":
                        körProgrammet = false;
                        break;
                    default:
                        Console.WriteLine("Ogiltigt val. Försök igen.");
                        break;
                }
            }
            Console.WriteLine("Program avslutas.");
        }

        // Läs registreringsnummer med validering
        static string LäsRegistreringsnummer()
        {
            while (true)
            {
                Console.Write("Ange registreringsnummer: ");
                string? input = Console.ReadLine();
                string regnummer = (input ?? "").Trim().ToUpperInvariant();

                if (regnummer.Length > 0 && regnummer.Length <= 10)
                {
                    bool giltigt = true;
                    foreach (char tecken in regnummer)
                    {
                        if (!char.IsLetterOrDigit(tecken))
                        {
                            giltigt = false;
                            break;
                        }
                    }
                    if (giltigt) return regnummer;
                }
                Console.WriteLine("Ange 1-10 tecken, endast bokstäver och siffror.");
            }
        }

        static string LäsFordonstyp()
        {
            while (true)
            {
                Console.Write("Fordonstyp (1=BIL, 2=MC): ");
                string val = Console.ReadLine() ?? "";
                if (val == "1") return "BIL";
                if (val == "2") return "MC";
                Console.WriteLine("Välj 1 eller 2");
            }
        }

        static int LäsPlatsnummer()
        {
            while (true)
            {
                Console.Write("Platsnummer (1-100): ");
                string inmatning = Console.ReadLine() ?? "";
                if (int.TryParse(inmatning, out int platsnummer))
                {
                    if (platsnummer >= 1 && platsnummer <= 100) return platsnummer - 1;
                }
                Console.WriteLine("Ange platsnummer mellan 1 till 100.");
            }
        }

        static int HittaFordon(string[] parkering, string regnummer)
        {
            for (int i = 0; i < parkering.Length; i++)
            {
                if (!string.IsNullOrEmpty(parkering[i]))
                {
                    string[] fordonPåPlatsen = parkering[i].Split('|');
                    foreach (string fordon in fordonPåPlatsen)
                    {
                        string[] uppgifter = fordon.Split('#');
                        if (uppgifter.Length >= 2 && uppgifter[1] == regnummer)
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        static bool KanParkera(string plats, string fordontyp)
        {
            if (string.IsNullOrEmpty(plats)) return true;

            if (fordontyp == "MC")
            {
                string[] fordonPåPlats = plats.Split('|');
                if (fordonPåPlats.Length == 1)
                {
                    string[] uppgifter = fordonPåPlats[0].Split('#');
                    if (uppgifter.Length >= 1 && uppgifter[0] == "MC") return true;
                }
            }

            return false;
        }

        static int HittaLedigPlats(string[] parkering, string fordontyp)
        {
            // If MC, prefer to add to an existing single MC spot
            if (fordontyp == "MC")
            {
                for (int i = 0; i < parkering.Length; i++)
                {
                    if (!string.IsNullOrEmpty(parkering[i]) && CanAddMCToSlot(parkering[i]))
                    {
                        return i;
                    }
                }
            }

            // Find empty slot
            for (int i = 0; i < parkering.Length; i++)
            {
                if (string.IsNullOrEmpty(parkering[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        // helper used above
        static bool CanAddMCToSlot(string plats)
        {
            if (string.IsNullOrEmpty(plats)) return false;
            string[] fordonPåPlats = plats.Split('|');
            if (fordonPåPlats.Length != 1) return false;
            string[] uppgifter = fordonPåPlats[0].Split('#');
            return uppgifter.Length >= 1 && uppgifter[0] == "MC";
        }

        static void LäggTillFordon(string[] parkering, int index, string fordon)
        {
            if (string.IsNullOrEmpty(parkering[index]))
                parkering[index] = fordon;
            else
                parkering[index] += "|" + fordon;
        }

        static string HämtaFordonsträng(string plats, string regnummer)
        {
            if (string.IsNullOrEmpty(plats)) return "";
            string[] fordonPåPlatsen = plats.Split('|');
            foreach (string fordon in fordonPåPlatsen)
            {
                string[] uppgifter = fordon.Split('#');
                if (uppgifter.Length >= 2 && uppgifter[1] == regnummer) return fordon;
            }
            return "";
        }

        static void TaBortFordon(string[] parkering, int index, string regnummer)
        {
            if (string.IsNullOrEmpty(parkering[index])) return;
            string[] fordonPåPlats = parkering[index].Split('|');
            string[] kvar = Array.FindAll(fordonPåPlats, f =>
            {
                string[] u = f.Split('#');
                return !(u.Length >= 2 && u[1] == regnummer);
            });

            parkering[index] = string.Join("|", kvar);
        }

        static void ParkeraFordon(string[] parkering)
        {
            string regnummer = LäsRegistreringsnummer();
            if (HittaFordon(parkering, regnummer) != -1)
            {
                Console.WriteLine("Fordonet är redan parkerat.");
                return;
            }

            string fordontyp = LäsFordonstyp();
            int index = HittaLedigPlats(parkering, fordontyp);
            if (index == -1)
            {
                Console.WriteLine("Ingen ledig plats för fordonet.");
                return;
            }

            string fordon = fordontyp + "#" + regnummer;
            LäggTillFordon(parkering, index, fordon);
            Console.WriteLine($"Fordonet {regnummer} parkerat på plats {index + 1}.");
        }

        static void HämtaUtFordon(string[] parkering)
        {
            string regnummer = LäsRegistreringsnummer();
            int index = HittaFordon(parkering, regnummer);
            if (index == -1)
            {
                Console.WriteLine("Fordonet hittades inte.");
                return;
            }
            TaBortFordon(parkering, index, regnummer);
            Console.WriteLine($"Fordonet {regnummer} har hämtats ut från plats {index + 1}.");
        }

        static void SökFordon(string[] parkering)
        {
            string regnummer = LäsRegistreringsnummer();
            int index = HittaFordon(parkering, regnummer);
            if (index == -1)
            {
                Console.WriteLine("Fordonet hittades inte.");
                return;
            }
            Console.WriteLine($"Fordonet {regnummer} finns på plats {index + 1}.");
        }

        static void FlyttaFordon(string[] parkering)
        {
            string regnummer = LäsRegistreringsnummer();
            int gammaltIndex = HittaFordon(parkering, regnummer);
            if (gammaltIndex == -1)
            {
                Console.WriteLine("Fordonet hittades inte.");
                return;
            }

            int nyttIndex = LäsPlatsnummer();
            if (nyttIndex == gammaltIndex)
            {
                Console.WriteLine("Fordonet står redan på den platsen");
                return;
            }

            string fordon = HämtaFordonsträng(parkering[gammaltIndex], regnummer);
            if (string.IsNullOrEmpty(fordon))
            {
                Console.WriteLine("Fel vid hämtning av fordonsträng.");
                return;
            }

            string fordonstyp = fordon.Split('#')[0];
            if (!KanParkera(parkering[nyttIndex], fordonstyp))
            {
                Console.WriteLine("Fordonet kan inte flyttas till den platsen.");
                return;
            }

            TaBortFordon(parkering, gammaltIndex, regnummer);
            LäggTillFordon(parkering, nyttIndex, fordon);
            Console.WriteLine($"Flyttade fordon {regnummer} från plats {gammaltIndex + 1} till plats {nyttIndex + 1}.");
        }
    }
}

