using LogisticsWPF.Model;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LogisticsWPF.Windows;

namespace LogisticsWPF.Pages
{
    public partial class ProductionOrdersPage : Page
    {
        public ProductionOrdersPage()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            using (var context = new LogisticsEntities())
            {
                OrdersGrid.ItemsSource = context.ProductionOrders
                    .Include("Clients")
                    .Include("Products")
                    .Include("ProductionOrderStatuses")
                    .ToList();
            }
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            var win = new ProductionOrderEditWindow();
            if (win.ShowDialog() == true)
                LoadOrders();
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is ProductionOrders order)
            {
                var win = new ProductionOrderEditWindow(order.ProductionOrderID);
                if (win.ShowDialog() == true)
                    LoadOrders();
            }
            else
            {
                MessageBox.Show("Сначала выберите заказ для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is ProductionOrders order)
            {
                var win = new ProductionOrderStatusWindow(order.ProductionOrderID);
                if (win.ShowDialog() == true)
                    LoadOrders();
            }
        }
    }
}