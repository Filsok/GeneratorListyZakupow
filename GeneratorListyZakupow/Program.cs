using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime;

namespace GeneratorListyZakupow
{
    class Program
    {
        int KcalTarget = 2200;

        static void Main(String[] args)
        {
            List<Przepis> Przepisy = new List<Przepis>();
            List<String> Skladniki = new List<String>();
            Console.WriteLine("Cześć, zaczynamy!");
            Console.WriteLine("Pamiętaj że muszisz mieć przygotowane pliki Config.cfg i skladniki.cfg w tej samej lokalizacji co .exe");

            #region odczyt z pliku
            if (File.Exists("Config.cfg") && File.Exists("Skladniki.cfg"))
            {
                Console.WriteLine("Super, ten pliki istnieją. Ciekawe czy mają poprawną zawartość...");
                String[] sa = File.ReadAllLines("Skladniki.cfg");
                String[] tmp;

                foreach (String ln in sa)
                {
                    Skladniki.Add(ln);
                }

                Przepis p = new Przepis();
                String[] sa2 = File.ReadAllLines("Config.cfg");

                foreach (String ln in sa2)
                {
                    if (ln == "KONIEC") {; }
                    else if (ln == "")
                    {
                        Przepisy.Add(p);
                        p.ListaSkladnikow.Clear();
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("############################### NASTĘPNY SŁOWNIK ###############################");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        tmp = ln.Split('=');
                        switch (tmp[0].ToString())
                        {
                            case "Name": p.Nazwa = tmp[1]; break;
                            case "Kcal": p.Kcal = int.Parse(tmp[1]); break;
                            case "FilipDni": p.FilipDni = int.Parse(tmp[1]); break;
                            case "IzabelaKcal": p.IzabelaKcal = int.Parse(tmp[1]); break;
                            case "Opis": p.Opis = tmp[1]; break;
                            default:
                                {
                                    if (tmp[0].ToString().Equals("") || tmp[1].ToString().Equals(""))
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine($"ERROR Skladnik {tmp[0]} lub jego ilosc {tmp[1]} są puste!");
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                    else if (Skladniki.Contains(tmp[0])) p.ListaSkladnikow.Add(tmp[0].ToString(), int.Parse(tmp[1]));
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine($"ERROR Brak skladnika {tmp[0]} na liscie skladnikow!");
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                    break;
                                }
                        }
                        Console.WriteLine($"Do slownika wchodzi linijka: {tmp[0]}  =  {tmp[1]}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Brak pliku!!!");
            }
            #endregion

            #region obliczenia
            ConcurrentDictionary<String, int> Wyjsciowe = new ConcurrentDictionary<String, int>();

            foreach (Przepis prz in Przepisy)
            {
                //prz.ListaSkladnikow.Select(m=>Wyjsciowe.ContainsKey(m.Key)?Wyjsciowe[m.Key]=m.Value:Wyjsciowe.Add(m.Key,m.Value));
                prz.ListaSkladnikow.Select(m => Wyjsciowe.AddOrUpdate(m.Key, m.Value, (k, v) => v + m.Value));      //tu nie dziala!!!!!!!!!!!!
            }
            #endregion


            #region zapis do pliku
            //if (!File.Exists("ListaZakupow.txt")) File.Create("ListaZakupow.txt");
            //File.Open("ListaZakupow.txt", FileMode.Open,FileAccess.Write);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "ListaZakupow.txt")))
            {
                foreach (KeyValuePair<String, int> line in Wyjsciowe)
                { outputFile.WriteLine($"{line.Key.ToString()}  test    {line.Value.ToString()}g"); }
            }
            #endregion

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"KONIEC");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
        }
    }
    class Przepis
    {
        public String Nazwa;
        public Dictionary<String, int> ListaSkladnikow;
        public String Opis;
        public int Kcal;
        public int FilipDni;
        public int IzabelaKcal;

        public Przepis()
        {
            ListaSkladnikow = new Dictionary<String, int>();
        }
    }
}
