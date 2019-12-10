using SmartParkingApp;
using System;
using System.Collections.Generic;
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

namespace OwnerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ParkingManager _park = new ParkingManager();
        public MainWindow()
        {
            InitializeComponent();
            Percent.Text = $"Current state of parking lot:\n{_park.FilledSpaces():f2}%";
            UpdateActiveSessions();
            UpdatePastSesssions();
        }
        private void Profit_Click(object sender, RoutedEventArgs e)
        {
            Profit profit = new Profit(_park);
            profit.Show();
            this.Close();
        }
        private void UpdateActiveSessions()
        {
            CurrentSessionText.ItemsSource = null;
            CurrentSessionText.ItemsSource = _park.Actives();
        }
        private void UpdatePastSesssions()
        {
            LastSessionText.ItemsSource = null;
            LastSessionText.ItemsSource = _park.Pasts();
        }
    }
}
