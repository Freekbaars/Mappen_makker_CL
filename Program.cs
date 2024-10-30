using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Mappen_makker_CL
{
    class Program
    {
        static void Main(string[] args)
        {
            // Vraag om de locatie waar de hoofdmap moet worden aangemaakt
            Console.WriteLine("Voer de locatie in waar de hoofdmap moet worden aangemaakt (bijv. C:\\Projecten):");
            string locatie = Console.ReadLine() ?? string.Empty;

            // Controleer of de locatie bestaat
            if (!Directory.Exists(locatie))
            {
                Console.WriteLine("De opgegeven locatie bestaat niet. Controleer het pad en probeer opnieuw.");
                return;
            }

            Console.WriteLine("Voer het scheepsnummer (BNxx) in:");
            string scheepsNummer = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Voer de naam van het schip in:");
            string scheepsNaam = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(scheepsNaam))
            {
                scheepsNaam = char.ToUpper(scheepsNaam[0]) + scheepsNaam.Substring(1).ToLower();
            }

            // Zoek het XML-bestand in de programmamap
            string xmlPad = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "naam_mappen.xml");

            // Controleer of het XML-bestand bestaat
            if (!File.Exists(xmlPad))
            {
                Console.WriteLine("XML-bestand 'naam_mappen.xml' niet gevonden in de programmamap.");
                return;
            }

            XElement xml = XElement.Load(xmlPad);
            var schepen = xml.Elements("Schip").ToList();

            // Selecteer een schipstype
            Console.WriteLine("Kies een type schip:");
            for (int i = 0; i < schepen.Count; i++)
            {
                string typeSchip = schepen[i].Element("Type")?.Value;
                Console.WriteLine($"{i + 1}: {typeSchip}");
            }

            int keuze;
            do
            {
                Console.WriteLine("Voer het nummer in van het gewenste type schip:");
            } while (!int.TryParse(Console.ReadLine(), out keuze) || keuze < 1 || keuze > schepen.Count);

            var gekozenSchip = schepen[keuze - 1];
            string geselecteerdTypeSchip = gekozenSchip.Element("Type")?.Value;
            string hoofdMapPad = Path.Combine(locatie, $"{scheepsNaam} BN{scheepsNummer} ({geselecteerdTypeSchip})");

            // Maak de hoofdmap aan als deze nog niet bestaat
            if (!Directory.Exists(hoofdMapPad))
            {
                Directory.CreateDirectory(hoofdMapPad);
                Console.WriteLine($"Hoofdmap '{hoofdMapPad}' aangemaakt.");
            }

            // Maak submappen aan en controleer op submappen binnen mappen
            var mappen = gekozenSchip.Element("Mappen")?.Elements("Map");
            if (mappen != null)
            {
                foreach (var map in mappen)
                {
                    // Lees de naam van de hoofdmap uit het <Naam>-element
                    string mapNaam = $"{map.Element("Naam")?.Value} BN{scheepsNummer}";
                    string volledigeMapPad = Path.Combine(hoofdMapPad, mapNaam);

                    if (!Directory.Exists(volledigeMapPad))
                    {
                        Directory.CreateDirectory(volledigeMapPad);
                        Console.WriteLine($"Submap '{mapNaam}' aangemaakt in '{hoofdMapPad}'.");
                    }

                    // Controleer en maak submappen aan indien aanwezig
                    var subMappen = map.Elements("subMap");
                    if (subMappen != null)
                    {
                        foreach (var subMap in subMappen)
                        {
                            string subMapNaam = subMap.Value;
                            string volledigeSubMapPad = Path.Combine(volledigeMapPad, subMapNaam);

                            if (!Directory.Exists(volledigeSubMapPad))
                            {
                                Directory.CreateDirectory(volledigeSubMapPad);
                                Console.WriteLine($"Sub-submap '{subMapNaam}' aangemaakt in '{volledigeMapPad}'.");
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Geen mappen gevonden in XML voor het gekozen schip.");
            }

            Console.WriteLine("Mappenstructuur aangemaakt.");
        }
    }
}
