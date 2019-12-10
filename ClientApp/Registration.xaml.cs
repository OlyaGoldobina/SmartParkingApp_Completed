using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        ParkingManager _park = new ParkingManager();
        public event Action<string, string, string, string> userEntered;
        public Registration()
        {
            InitializeComponent();
        }
        private void CreateAcc_Click(object sender, RoutedEventArgs e)
        {
            string login = CreatePhone.Text;
            string password = CreatePassword.Text;
            string name = CreateName.Text;
            string plate = CreatePlate.Text;
            string logreg = @"7\d+";
            userEntered?.Invoke(login, password, name, plate);
            if (name != null & login != null & plate != null & password != null & Regex.IsMatch(login, logreg) & password.Length >= 6)
            {
                if (_park.AddUser(name, login, plate, password))
                {
                    UserRegistrated mainwindow = new UserRegistrated();
                    mainwindow.Show();
                    this.Close();
                }
                else
                {
                    UserExists userexists = new UserExists();
                    userexists.Show();
                    this.Close();
                }
            }
            else
                ErrorBlock.Text = "Fill all the fields, Phone number: 7NNNNNNNNNN,\npassword consists of 6 or more characters";
        }

        private void MainWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            mainwindow.Show();
            this.Close();
        }
    }
}
