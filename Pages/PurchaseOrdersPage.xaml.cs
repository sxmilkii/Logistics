using LogisticsWPF.Model;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using LogisticsWPF.Windows;

namespace LogisticsWPF.Pages
{
    public partial class PurchaseOrdersPage : Page
    {
        public PurchaseOrdersPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new LogisticsEntities())
            {
                OrdersGrid.ItemsSource = context.PurchaseOrders
                    .Include("Suppliers")
                    .Include("Warehouses")
                    .Include("PurchaseStatuses")
                    .ToList();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var win = new PurchaseOrderEditWindow();
            if (win.ShowDialog() == true)
                LoadData();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is PurchaseOrders po)
            {
                var win = new PurchaseOrderEditWindow(po.PurchaseOrderID);
                if (win.ShowDialog() == true)
                    LoadData();
            }
            else
            {
                MessageBox.Show("Сначала выберите закупку для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Receipt_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is PurchaseOrders po)
            {
                var win = new PurchaseReceiptWindow(po.PurchaseOrderID);
                win.ShowDialog();
            }
        }

        private void StockBalances_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Frame frame)
            {
                frame.Navigate(new StockBalancesPage());
            }
            else
            {
                var win = new Window
                {
                    Title = "Остатки на складе",
                    Content = new StockBalancesPage(),
                    Width = 800,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                win.Show();
            }
        }
    }
}