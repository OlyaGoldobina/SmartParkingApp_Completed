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
using Microsoft.Graph;
using SmartParkingApp;
using SmartParkingApp.Models;

namespace ClientApp
{
    /// <summary>
    /// Логика взаимодействия для ActiveSession.xaml
    /// </summary>
    public partial class ActiveSession : Window
    {
        ParkingManager _park = new ParkingManager();
        public ActiveSession(ParkingManager parking)
        {
            _park = parking;
            InitializeComponent();
            ParkingSession thisses = _park.ShowActive(_park.ActivePhoneNumber);
            if (thisses != null)
            {
                CurrentSessionText.Text = $"Start time: {thisses.EntryDt}\nNeed to pay: {_park.GetRemainingCost(thisses.TicketNumber)}";
            }
            else
            {
                CurrentSessionText.Text = "There is no active session";
            }
        }
               

        private void SettingsPage_Click(object sender, RoutedEventArgs e)
        {
            Settings setting = new Settings(_park);
            setting.Show();
            this.Close();
        }

        private void PastPage_Click(object sender, RoutedEventArgs e)
        {
            PastSessions past = new PastSessions(_park);
            past.Show();
            this.Close();

        }

    }
}
