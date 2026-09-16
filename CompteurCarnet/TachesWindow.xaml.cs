using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CompteurCarnet.Models;
using CompteurCarnet.Services;

namespace CompteurCarnet
{
    public partial class TachesWindow : Window
    {
        private readonly AppData _data;

        public TachesWindow()
        {
            InitializeComponent();
            _data = DataStore.Load();

            for (int i = 1; i <= _data.NombreOngletsTotal; i++)
            {
                OngletsContainer.Children.Add(BuildOngletCard(i));
            }
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            _data.NombreOngletsTotal++;
            DataStore.Save(_data);
            OngletsContainer.Children.Add(BuildOngletCard(_data.NombreOngletsTotal));
        }

        private Border BuildOngletCard(int numeroOnglet)
        {
            var stack = new StackPanel();

            var card = new Border
            {
                Background = (Brush)FindResource("CardBrush"),
                BorderBrush = (Brush)FindResource("AccentGreenBrush"),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(16),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 20),
                Child = stack
            };

            var titre = new TextBlock
            {
                Text = $"Onglet {numeroOnglet}",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 15)
            };
            stack.Children.Add(titre);

            var selectionPanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
            var btnSelectionner = new Button
            {
                Content = "Sélectionner un carnet",
                Style = (Style)FindResource("SmallButton"),
                Background = (Brush)FindResource("BlueBrush")
            };
            selectionPanel.Children.Add(btnSelectionner);
            stack.Children.Add(selectionPanel);

            var tachePanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Visibility = Visibility.Collapsed
            };
            var infoCarnet = new TextBlock
            {
                Foreground = Brushes.White,
                FontSize = 14,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            var progressionText = new TextBlock
            {
                Foreground = (Brush)FindResource("AccentGreenBrush"),
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 8)
            };
            var numeroFeuilleActuel = new TextBlock
            {
                Foreground = Brushes.White,
                FontSize = 26,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 15)
            };
            var btnVita = new Button
            {
                Content = "Vita",
                Style = (Style)FindResource("SmallButton"),
                Background = (Brush)FindResource("GreenBrush")
            };
            tachePanel.Children.Add(infoCarnet);
            tachePanel.Children.Add(progressionText);
            tachePanel.Children.Add(numeroFeuilleActuel);
            tachePanel.Children.Add(btnVita);
            stack.Children.Add(tachePanel);

            void AfficherTache(Carnet c)
            {
                var nc = CommunesData.NomCommune(c.CodeCommune);
                var nf = CommunesData.NomFokontany(c.CodeFokontany);
                infoCarnet.Text = $"Commune: {nc}\nFokontany: {nf}\nCarnet {c.NumCarnet}";

                if (c.EnAttenteFinalisation)
                {
                    progressionText.Text = $"Feuille {c.TotalValide:D2}/{c.TotalValide:D2}";
                    numeroFeuilleActuel.Text = "";
                    btnVita.Content = "Vita ny tache";
                }
                else
                {
                    progressionText.Text = $"Feuille {(c.Progression + 1):D2}/{c.TotalValide:D2}";
                    numeroFeuilleActuel.Text = (c.Progression >= 0 && c.Progression < c.FeuilletsValides.Count)
                        ? c.FeuilletsValides[c.Progression]
                        : "";
                    btnVita.Content = "Vita";
                }

                selectionPanel.Visibility = Visibility.Collapsed;
                tachePanel.Visibility = Visibility.Visible;
            }

            var carnetTrouve = _data.CarnetsDisponibles.FirstOrDefault(c => c.AssigneOnglet == numeroOnglet);
            if (carnetTrouve != null)
            {
                AfficherTache(carnetTrouve);
            }

            btnSelectionner.Click += (s, e) =>
            {
                var disponibles = _data.CarnetsDisponibles.Where(c => c.AssigneOnglet == -1).ToList();
                if (disponibles.Count == 0)
                {
                    MessageBox.Show("Aucun carnet disponible", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var dlg = new SelectCarnetWindow(disponibles) { Owner = this };
                if (dlg.ShowDialog() == true && dlg.CarnetChoisi != null)
                {
                    var choisi = dlg.CarnetChoisi;
                    choisi.AssigneOnglet = numeroOnglet;
                    choisi.Progression = 0;
                    choisi.EnAttenteFinalisation = false;
                    DataStore.Save(_data);
                    AfficherTache(choisi);
                }
            };

            btnVita.Click += (s, e) =>
            {
                var c = _data.CarnetsDisponibles.FirstOrDefault(x => x.AssigneOnglet == numeroOnglet);
                if (c == null) return;

                if (!c.EnAttenteFinalisation)
                {
                    if (c.Progression < c.TotalValide - 1)
                    {
                        c.Progression++;
                    }
                    else
                    {
                        c.EnAttenteFinalisation = true;
                    }
                    DataStore.Save(_data);
                    AfficherTache(c);
                }
                else
                {
                    _data.CarnetsDisponibles.Remove(c);
                    _data.Historique.Add(new HistoriqueEntry
                    {
                        CodeCommune = c.CodeCommune,
                        CodeFokontany = c.CodeFokontany,
                        NumCarnet = c.NumCarnet,
                        FeuilletsValides = c.FeuilletsValides,
                        Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    });
                    DataStore.Save(_data);
                    OngletsContainer.Children.Remove(card);
                }
            };

            return card;
        }
    }
}
