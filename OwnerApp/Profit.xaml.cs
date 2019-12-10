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
namespace OwnerApp
{
    /// <summary>
    /// Логика взаимодействия для Profit.xaml
    /// </summary>
    public partial class Profit : Window
    {
        ParkingManager _park = new ParkingManager();
        public Profit(ParkingManager parking)
        {
            _park = parking;
            InitializeComponent();
        }

        private void Current_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            mainwindow.Show();
            this.Close();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            Period.Text = null;
            ProfitTx.Text = null;
            string start = StartDt.Text;
            string end = EndDt.Text;
            DateTime startDt;
            DateTime endDt;
            if (DateTime.TryParse(start,out startDt) & DateTime.TryParse(end, out endDt))
            {
                if (startDt <= DateTime.Now.Date & endDt <= DateTime.Now.Date & endDt >= startDt)
                {
                    endDt = endDt.AddHours(23);
                    endDt = endDt.AddMinutes(59);
                    endDt = endDt.AddSeconds(59);
                    decimal? totalprofit = _park.TotalProfit(startDt,endDt);
                    ProfitTx.Text = $"Total profit is:\n{totalprofit:f2}RUB";
                }
                else
                {
                    Period.Text = "Logical problem, edit the data";
                }
            }
            else
            {
                Period.Text = "Following form YYYY-MM-DD";
            }
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            Period.Text = null;
            ProfitTx.Text = null;
            string start = StartDt.Text;
            string end = EndDt.Text;
            DateTime startDt;
            DateTime endDt;
            if (DateTime.TryParse(start, out startDt) & DateTime.TryParse(end, out endDt))
            {
                if (startDt <= DateTime.Now.Date & endDt <= DateTime.Now.Date & endDt >= startDt)
                {
                    Table tablewindow = new Table(_park,startDt,endDt);
                    tablewindow.Show();
                    this.Close();
                }
                else
                {
                    Period.Text = "Logical problem, edit the data";
                }
            }
            else
            {
                Period.Text = "Following form YYYY-MM-DD";
            }
        }
    }
}
