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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ParkingManager _park = new ParkingManager();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            this.Close();
        }

        private void ForgetPas_Click(object sender, RoutedEventArgs e)
        {
            ForgetPassword forgetpassword = new ForgetPassword();
            forgetpassword.Show();
            this.Close();
        }

        private void EnterTheApp_Click(object sender, RoutedEventArgs e)
        {
            ErrorBlock.Text = null;
            string login = EnterLogin.Text;
            string password = EnterPassword.Password;
            if (_park.UserExisting(login, password))
            {
                ActiveSession active = new ActiveSession(_park);
                active.Show();
                this.Close();
            }
            else
            {
                ErrorBlock.Text = null;
                ErrorBlock.Text = $"Wrong login or password";
            }
        }
    }
}
