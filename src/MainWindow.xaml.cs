/*
 * Projekt: Kalkulator Zapotrzebowania Kalorycznego zrobiony na zaliczenie.
 * Autor: Bartosz Kania
 * Opis: Podajemy podstawowe informacje oraz wybieramy nasz cel, po czym kalkulator opierając się na podanych danych oblicza zapotrzebowanie kaloryczne.
 * Zapotrzebowanie posiada również podział na tłuszcze, węglowodany i białko. Ustawienia można zapisać do pliku .txt jak i można je wczytać.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjektZaliczeniowy
{
    public partial class MainWindow : Window
    {
        //private static FileStream resultFile = GetResultFile();

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double basicMetabolism = CalculateBasicMetabolism();
                double multipliedBasicMetabolism = MultiplyByLifestyle(basicMetabolism);
                double result = Math.Floor(CalculateEntirety(multipliedBasicMetabolism));

                double carbs = Math.Floor(CalculateCarbs(result));
                double fat = Math.Floor(CalculateFat(result));
                double protein = Math.Floor(CalculateProtein(result));

                CaloriesResult.Text = result.ToString() + " kCal";
                Carbohydrates.Text = carbs.ToString() + " g";
                Fats.Text = fat.ToString() + " g";
                Protein.Text = protein.ToString() + " g";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Wprowadzono nieprawidłowe dane");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCalculations(CaloriesResult.Text,
                             Carbohydrates.Text,
                             Fats.Text,
                             Protein.Text,
                             Male.IsSelected ? "Male" : "Female",
                             (Activity1.IsSelected ? "1" : (Activity2.IsSelected ? "2" : (Activity3.IsSelected ? "3" : (Activity4.IsSelected ? "4" : (Activity5.IsSelected ? "5" : (Activity6.IsSelected ? "6" : "0")))))),
                             Age.Text,
                             WeightGain.IsSelected ? "Gain" : WeightLose.IsSelected ? "Lose" : "Keep",
                             Weight.Text,
                             Height.Text);
        }

        private void ReadLastResultButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCalculations();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private double CalculateBasicMetabolism()
        {
            double formula = 0;

            if (Male.IsSelected == true)
            {
                formula = 66.5 + (13.7 * Double.Parse(Weight.Text)) + (5 * Double.Parse(Height.Text)) - (6.8 * Double.Parse(Age.Text));
            }
            else if (Female.IsSelected == true)
            {
                formula = 665 + (9.6 * Double.Parse(Weight.Text)) + (1.85 * Double.Parse(Height.Text)) - (4.7 * Double.Parse(Age.Text));
            }

            return formula;
        }

        private double MultiplyByLifestyle(double basicMetabolism)
        {
            double lifestyleMultiplier = 0;

            if (Activity1.IsSelected == true)
            {
                lifestyleMultiplier = 1.2;
            }
            else if (Activity2.IsSelected == true)
            {
                lifestyleMultiplier = 1.4;
            }
            else if (Activity3.IsSelected == true)
            {
                lifestyleMultiplier = 1.6;
            }
            else if (Activity4.IsSelected == true)
            {
                lifestyleMultiplier = 1.8;
            }
            else if (Activity5.IsSelected == true)
            {
                lifestyleMultiplier = 2;
            }
            else if (Activity6.IsSelected == true)
            {
                lifestyleMultiplier = 2.2;
            }

            return basicMetabolism * lifestyleMultiplier;
        }

        private double CalculateEntirety(double totalMetabolism)
        {
            double formula = 0;

            if (WeightGain.IsSelected == true)
            {
                formula = totalMetabolism + 300;
            }
            else if (WeightLose.IsSelected == true)
            {
                formula = totalMetabolism - 500;
            }
            else if (WeightKeep.IsSelected == true)
            {
                formula = totalMetabolism;
            }

            return formula;
        }

        private double CalculateCarbs(double r)
        {
            //Węglowodany to 55% zapotrzebowania, dzielimy przez 4, by osiągnąć wartość w gramach
            return (((r / 100) * 55) / 4);
        }
        
        private double CalculateFat(double r)
        {
            //Tłuszcze to 25% zapotrzebowania, dzielimy przez 9, by osiągnąć wartość w gramach
            return (((r / 100) * 25) / 9);
        }

        private double CalculateProtein(double r)
        {
            //Białko to 20% zapotrzebowania, dzielimy również przez 4, by osiągnąć wartość w gramach
            return (((r / 100) * 20) / 4);
        }

        private void SaveCalculations(string result, string carbs, string fat, string prot, string sex, string activity, string age, string purpose, string weight, string height)
        {
            string fileName = "results.txt";

            string[] results = { result, carbs, fat, prot, sex, activity, age, purpose, weight, height };

            File.WriteAllLines(fileName, results);
        }

        private void LoadCalculations()
        {
            int counter = 0;

            //To wygląda dziwnie, ale działa póki sie wie co jest w konkretnej lini
            foreach (string line in System.IO.File.ReadLines("results.txt"))
            {
                switch(counter)
                {
                    //Kalorie
                    case 0:
                        CaloriesResult.Text = line;
                        break;
                    //Węglowodany
                    case 1:
                        Carbohydrates.Text = line;
                        break;
                    //Tłuszcze
                    case 2:                  
                        Fats.Text = line;
                        break;
                    //Białko
                    case 3:
                        Protein.Text = line;
                        break;
                    //Płeć
                    case 4:
                        if (line.Equals("Male"))
                            Male.IsSelected = true;
                        else
                            Female.IsSelected = true;
                        break;
                    //Aktywność
                    case 5:
                        if (line.Equals("1"))
                            Activity1.IsSelected = true;
                        else if (line.Equals("2"))
                            Activity2.IsSelected = true;
                        else if (line.Equals("3"))
                            Activity3.IsSelected = true;
                        else if (line.Equals("4"))
                            Activity4.IsSelected = true;
                        else if (line.Equals("5"))
                            Activity5.IsSelected = true;
                        else
                            Activity6.IsSelected = true;
                        break;
                    //Wiek
                    case 6:
                        Age.Text = line;
                        break;
                    //Cel
                    case 7:
                        if (line.Equals("Gain"))
                            WeightGain.IsSelected = true;
                        else if (line.Equals("Lose"))
                            WeightLose.IsSelected = true;
                        else
                            WeightKeep.IsSelected = true;
                        break;
                    //Waga
                    case 8:
                        Weight.Text = line;
                        break;
                    //Wzrost
                    case 9:
                        Height.Text = line;
                        break;
                }

                counter++;
            }
        }
    }
}
