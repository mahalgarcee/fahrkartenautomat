using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FahrkartenautomatUi
{
    class Buchung
    {
        private Dictionary<decimal, int> geschaeftFall = new Dictionary<decimal, int>();
        private decimal fahrPreis;
        private decimal[] keys = new decimal[] { 10, 5, 2, 1, 0.5M, 0.2M, 0.1M };
        private string errorMessage;
        /*eingabe ist eine Tabelle : enthält die Werten einer Buchung, die von die eingabe gelesen sind.
         * Von dieser Tabelle ist die Fahrpreis und das eingeworfene Geld und extrahiert
           */
        public Buchung(String eingabeZeile)
        {
            try
            {
                decimal[] eingabe = Array.ConvertAll(eingabeZeile.Split(';'), decimal.Parse);
                fahrPreis = eingabe[0];
                for (int i = 0; i < 6; i++)
                {
                    geschaeftFall.Add(keys[i], Convert.ToInt32(eingabe[i + 1]));
                }
                errorMessage = "";
            }
            catch (Exception e)
            {

                errorMessage = errorMessage + " Buchung falsch angegeben";
            }

        }
        public Dictionary<decimal, int> GeschaeftFall { get => geschaeftFall; set => geschaeftFall = value; }
        public decimal FahrPreis { get => fahrPreis; set => fahrPreis = value; }
        public string Errormessage { get => errorMessage; set => errorMessage = value; }

        public override string ToString()
        {
            return "Fahrpreis : " + fahrPreis + "euro \n {Entworfen Geld:" + string.Join(";", geschaeftFall.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}"; ;
        }

    }
}
