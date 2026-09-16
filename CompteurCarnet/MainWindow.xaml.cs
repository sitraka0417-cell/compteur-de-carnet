using System.Windows;

namespace CompteurCarnet
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnListe_Click(object sender, RoutedEventArgs e)
        {
            new ListeWindow().ShowDialog();
        }

        private void BtnTaches_Click(object sender, RoutedEventArgs e)
        {
            new TachesWindow().ShowDialog();
        }

        private void BtnHistorique_Click(object sender, RoutedEventArgs e)
        {
            new HistoriqueWindow().ShowDialog();
        }
    }
}
