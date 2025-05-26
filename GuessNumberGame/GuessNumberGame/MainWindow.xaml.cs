using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GuessNumberGame
{
    public partial class MainWindow : Window
    {
        private int targetNumber;
        private int attempts;
        private List<GameResult> results = new List<GameResult>();

        public MainWindow()
        {
            InitializeComponent();
            RestartGame();
            LoadResults(); // Загружаем результаты при запуске приложения
        }

        private void RestartGame_Click(object sender, RoutedEventArgs e) => RestartGame();

        private void RestartGame()
        {
            Random rnd = new Random();
            targetNumber = rnd.Next(1, 101);
            attempts = 0;
            HintText.Text = "";
            AttemptsText.Text = "Попыток: 0";
            GuessBox.Text = "";
        }

        private void CheckGuess_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(GuessBox.Text, out int guess))
            {
                MessageBox.Show("Введите число!");
                return;
            }

            attempts++;
            AttemptsText.Text = $"Попыток: {attempts}";

            if (guess < targetNumber)
                HintText.Text = "Загаданное число больше!";
            else if (guess > targetNumber)
                HintText.Text = "Загаданное число меньше!";
            else
            {
                MessageBox.Show($"Поздравляем! Вы угадали число за {attempts} попыток.");
                HintText.Text = "Вы угадали!";
            }
        }

        private void SaveResult_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PlayerNameBox.Text))
            {
                MessageBox.Show("Введите имя!");
                return;
            }

            var result = new GameResult
            {
                PlayerName = PlayerNameBox.Text,
                Attempts = attempts,
                Date = DateTime.Now
            };

            results.Add(result);
            SaveResultsToFile();
            LoadResults(); // Загружаем обновлённый список
        }

        private void SaveResultsToFile()
        {
            using (StreamWriter writer = new StreamWriter("results.txt"))
            {
                foreach (var result in results)
                {
                    writer.WriteLine($"{result.PlayerName},{result.Attempts},{result.Date}");
                }
            }
        }

        private void LoadResults()
        {
            results.Clear();
            if (File.Exists("results.txt"))
            {
                foreach (var line in File.ReadAllLines("results.txt"))
                {
                    var parts = line.Split(',');
                    results.Add(new GameResult
                    {
                        PlayerName = parts[0],
                        Attempts = int.Parse(parts[1]),
                        Date = DateTime.Parse(parts[2])
                    });
                }
            }

            ResultsListBox.ItemsSource = results.Select(r => $"{r.PlayerName} - {r.Attempts} попыток ({r.Date.ToShortDateString()})").ToList();
        }
    }
}
