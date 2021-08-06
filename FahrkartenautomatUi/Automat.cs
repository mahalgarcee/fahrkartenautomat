using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FahrkartenautomatUi
{
    class Automat
    {   
        private Dictionary<decimal, int> bestand = new Dictionary<decimal, int>();
        private decimal[] keys = new decimal[] { 2, 1, 0.5M, 0.2M, 0.1M };
        private decimal gesamtBestand = 0;
        private List<decimal> moeglicheFahrpreisen = new List<decimal>();
        Dictionary<int, Buchung> buchungen = new Dictionary<int, Buchung>();
        private string errorMessage;
        static Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();

        public Automat(string eingabeBestand, Dictionary<int, Buchung> buchungen)
        {
            try
            {
                int[] eingabe = Array.ConvertAll(eingabeBestand.Split(';'), int.Parse);

                bestand.Add(10, 0);
                bestand.Add(5, 0);

                for (int i = 0; i < 5; i++)
                {
                    bestand.Add(keys[i], eingabe[i]);
                }
                form1.richTextBox2.AppendText("Anfangsbestand " + gesamtBestandBerechnung() + " Euro(" + string.Join(";", bestand.Select(kv => kv.Value).ToArray()) + ") \n");
            }
            catch (Exception e)
            {
                errorMessage = errorMessage + " Anfangsbestand falsch angegeben";
            }
            this.buchungen = buchungen;

            decimal anfang = 0.90M;
            decimal ende = 19.90M;
            while (anfang < ende)
            {
                anfang = anfang + 0.1M;
                moeglicheFahrpreisen.Add(anfang);
            }
            moeglicheFahrpreisen.Add(ende);

        }
        public Dictionary<decimal, int> Bestand { get => bestand; set => bestand = value; }

        /*Wechselberechnung ist eine Methode, die nach Berechung des Wechselgelds 
         *  Wechselgeld als Dictionary<double, int> zuruekgibt.
           */
        public Dictionary<decimal, int> Wechselberechnung(Buchung buchung)
        {
            decimal engeworfenesGeld = 0;

            //Rückgabe initialisieren
            Dictionary<decimal, int> ruekgabe = new Dictionary<decimal, int>();
            for (int i = 0; i < keys.Length; i++)
            {
                ruekgabe.Add(keys[i], 0);
            }
            decimal wechselGeld = 50000;
            if (buchung.FahrPreis > moeglicheFahrpreisen[0] && buchung.FahrPreis < moeglicheFahrpreisen[moeglicheFahrpreisen.Count - 1])
            {
                if (moeglicheFahrpreisen.Contains(buchung.FahrPreis))
                {
                    // Die Summe des eingeworfenenen Gelds berechnen
                    foreach (KeyValuePair<decimal, int> entry in buchung.GeschaeftFall)
                    {
                        if (!(entry.Value == 0))
                        {
                            engeworfenesGeld += (entry.Value * entry.Key);
                        }
                    }

                    wechselGeld = engeworfenesGeld - buchung.FahrPreis;
                    decimal summeMin = 0;
                    List<decimal> keysBestand = new List<decimal>(bestand.Keys);
                    //Prüfen ob das eingeworfene Geld ist mehr als der Buchungspreis
                    if (wechselGeld >= 0)

                    {

                        //Das eingeworfene Geld zu Münzbestand hinzufügen
                        foreach (KeyValuePair<decimal, int> entry in buchung.GeschaeftFall)
                        {
                            if (!(entry.Value == 0))
                            {
                                bestand[entry.Key] += entry.Value;
                            }
                        }
                        if (wechselGeld > 0)

                        {
                            foreach (decimal key in keysBestand)
                            {
                                if (key <= wechselGeld)
                                {
                                    summeMin = summeMin + bestand[key] * key;
                                }

                            }
                            if (summeMin >= wechselGeld)

                            {
                                foreach (decimal key in keysBestand)
                                {

                                    if (key <= wechselGeld && bestand[key] > 0)
                                    {
                                        if (key != 10 && key != 5) // das Wechselgeld soll nur von Münzen sein
                                        {
                                            int div = Convert.ToInt32(wechselGeld / key);
                                            //int mod = Convert.ToInt32(wechselGeld % key);
                                            int min = Math.Min(div, bestand[key]);
                                            wechselGeld = wechselGeld - min * key;
                                            bestand[key] -= min;
                                            ruekgabe[key] += min;
                                            if (wechselGeld == 0)
                                            {
                                                break;

                                            }
                                        }

                                    }


                                }
                                if (wechselGeld != 0)
                                {

                                    buchung.Errormessage = buchung.Errormessage + "(WechselGeld kann nicht passend ausgezahlt werden) ";
                                }

                            }
                            else
                            {
                                buchung.Errormessage = buchung.Errormessage + " (Zuwenig Wechselgeld vorhanden)";
                            }
                        }



                    }
                    else
                    {
                        buchung.Errormessage = buchung.Errormessage + " (Zuwenig Geld eingeworfen) ";
                    }
                }
                else
                {
                    buchung.Errormessage = buchung.Errormessage + " (Ungültiger Fahrkartenpreis Typ 2) ";
                }



            }
            else
            {
                buchung.Errormessage = buchung.Errormessage + " (Ungültiger Fahrkartenpreis Typ 1) ";
            }

            return ruekgabe;
        }






        //Berechnung des Gesamtbestandes
        public decimal gesamtBestandBerechnung()
        {
            gesamtBestand = 0;
            List<decimal> keysBestand = new List<decimal>(bestand.Keys);
            foreach (decimal key in keysBestand)
            {
                gesamtBestand += key * bestand[key];
            }
            return gesamtBestand;
        }


        public void buchungenTesten()
        {
            if (String.IsNullOrEmpty(errorMessage))
            {
                List<int> keysBuchungen = new List<int>(buchungen.Keys);
                foreach (int key in keysBuchungen)
                {
                    
                    if (String.IsNullOrEmpty(buchungen[key].Errormessage))
                    {
                        form1.richTextBox2.AppendText("Buchung" + (key + 1) + ": " + buchungen[key].FahrPreis + "Euro");
                        Dictionary<decimal, int> ruekgabe = Wechselberechnung(buchungen[key]);

                        if (String.IsNullOrEmpty(buchungen[key].Errormessage))
                        {
                            string r = "/Ruekgabe:{" + string.Join(";", ruekgabe.Select(kv => kv.Value).ToArray()) + "}";
                            form1.richTextBox2.AppendText(r + "/");
                            form1.richTextBox2.AppendText("Gesamtbestand = " + gesamtBestandBerechnung() + "Euro(");
                            form1.richTextBox2.AppendText("" + string.Join(";", bestand.Select(kv => kv.Value).ToArray()) + ") \n");
                        }
                        else
                        { 
                            form1.richTextBox2.AppendText("//Error !!!" + buchungen[key].Errormessage + " \n",Color.Red);

                        }
                    }
                    else
                    {
                        form1.richTextBox2.AppendText("//Error !!!" + buchungen[key].Errormessage + " \n", Color.Red);
                    }

                }
            }
            else
            {
                form1.richTextBox2.AppendText("//Error !!!" + errorMessage + " \n", Color.Red);
            }

        }




        public override string ToString()
        {
            return "Bestand {" + string.Join(";", bestand.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
        }

    }
}
