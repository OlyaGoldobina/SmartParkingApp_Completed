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

namespace ClientApp
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        ParkingManager _park = new ParkingManager();
        public Settings(ParkingManager parking)
        {
            _park = parking;
            InitializeComponent();
        }


        private void PastPage_Click(object sender, RoutedEventArgs e)
        {
            PastSessions past = new PastSessions(_park);
            past.Show();
            this.Close();
        }

        private void CurrentPage_Click(object sender, RoutedEventArgs e)
        {
            ActiveSession active = new ActiveSession(_park);
            active.Show();
            this.Close();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            ErrorPr.Text = null;
            ErrorNew.Text = null;
            this.Password.Text = null;
            string lastpassword = lastPassword.Text;
            string newpassword = NewPassword.Text;
                if (_park.UserExisting(_park.ActivePhoneNumber, lastpassword))
                {
                    if (newpassword.Length >= 6)
                    {
                        _park.ChangePassword(_park.ActivePhoneNumber, newpassword);
                        Settings setting = new Settings(_park);
                        setting.Show();
                        setting.Password.Text = null;
                        setting.Password.Text = "Password has already changed";
                        this.Close();
                    }
                    else
                    {
                    ErrorNew.Text = null;
                    ErrorNew.Text = ">6 symb";
                    }
                }
                else
            {
                ErrorPr.Text = null;
                ErrorPr.Text = "wrong";
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            mainwindow.Show();
            this.Close();
        }
    }
}
