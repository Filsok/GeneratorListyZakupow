﻿using System;
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
            Log("Cześć, zaczynamy!");
            Log("Pamiętaj że muszisz mieć przygotowane pliki Config.cfg i skladniki.cfg w tej samej lokalizacji co .exe");

            #region odczyt z pliku
            if (File.Exists("Config.cfg") && File.Exists("Skladniki.cfg"))
            {
                Log("Super, ten pliki istnieją. Ciekawe czy mają poprawną zawartość...");
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
                        Log("############################### NASTĘPNY SŁOWNIK ###############################", ConsoleColor.Blue);
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
                                        Log($"ERROR Skladnik {tmp[0]} lub jego ilosc {tmp[1]} są puste!",ConsoleColor.Red);
                                    }
                                    else if (Skladniki.Contains(tmp[0])) p.ListaSkladnikow.Add(tmp[0].ToString(), int.Parse(tmp[1]));
                                    else
                                    {
                                        Log($"ERROR Brak skladnika {tmp[0]} na liscie skladnikow!", ConsoleColor.Red);
                                    }
                                    break;
                                }
                        }
                        Log($"Do slownika wchodzi linijka: {tmp[0]}  =  {tmp[1]}");
                    }
                }
            }
            else
            {
                Log("Brak pliku!!!");
            }
            #endregion

            #region obliczenia
            ConcurrentDictionary<String, int> Wyjsciowe = new ConcurrentDictionary<String, int>();
            foreach (Przepis prz in Przepisy)
            {
                //prz.ListaSkladnikow.Select(m=>Wyjsciowe.ContainsKey(m.Key)?Wyjsciowe[m.Key]=m.Value:Wyjsciowe.Add(m.Key,m.Value));
                //prz.ListaSkladnikow.Select(m => Wyjsciowe.AddOrUpdate(m.Key, m.Value, (k, v) => v + m.Value));      //tu nie dziala
                foreach (KeyValuePair<String,int> kvp in prz.ListaSkladnikow)
                {
                    Wyjsciowe.AddOrUpdate(kvp.Key, kvp.Value, (k, v) => v + kvp.Value);
                }
            }
            #endregion


            #region zapis do pliku
            //if (!File.Exists("ListaZakupow.txt")) File.Create("ListaZakupow.txt");
            //File.Open("ListaZakupow.txt", FileMode.Open,FileAccess.Write);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "ListaZakupow.txt")))
            {
                foreach (KeyValuePair<String, int> line in Wyjsciowe)
                { outputFile.WriteLine($"{line.Key.ToString()}       {line.Value.ToString()}g"); }
            }
            #endregion

            Log("KONIEC", ConsoleColor.Green);
            Console.ReadKey();
        }

        private static void Log(string msg, ConsoleColor color=ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
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
