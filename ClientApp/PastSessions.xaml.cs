using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SmartParkingApp;
using SmartParkingApp.Models;

namespace ClientApp
{
    /// <summary>
    /// Логика взаимодействия для PastSessions.xaml
    /// </summary>
    public partial class PastSessions : Window
    {
        ParkingManager _park = new ParkingManager();
        public PastSessions(ParkingManager parking)
        {
            _park = parking;
            InitializeComponent();
            UpdateSessions();
        }

        private void UpdateSessions()
        {
            List<ParkingSession> thisses = _park.ShowPast(_park.ActivePhoneNumber);
            Past.ItemsSource =null;
            Past.ItemsSource = thisses;
        }

        private void SettingsPage_Click(object sender, RoutedEventArgs e)
        {
            Settings setting = new Settings(_park);
            setting.Show();
            this.Close();
        }


        private void CurrentPage_Click(object sender, RoutedEventArgs e)
        {
            ActiveSession active = new ActiveSession(_park);
            active.Show();
            this.Close();
        }
    }
}
