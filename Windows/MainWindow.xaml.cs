using System.Windows;
using LogisticsWPF.Pages;

namespace LogisticsWPF.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductPage());
        }

        private void MaterialsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MaterialsPage());
        }

        private void SuppliersButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SuppliersPage());
        }

        private void EmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeesPage());
        }

        private void EquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EquipmentPage());
        }

        private void PurchasesButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PurchaseOrdersPage());
        }

        private void ProductionOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductionOrdersPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();

            this.Close();
        }
    }
}