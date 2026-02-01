using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LogisticsWPF.Model;
using LogisticsWPF.Windows;

namespace LogisticsWPF.Pages
{
    public partial class ProductPage : Page
    {
        public ProductPage()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            using (var context = new LogisticsEntities())
            {
                var products = context.Products
                    .Include(p => p.ProductCategories)
                    .Include(p => p.Units)
                    .Include(p => p.ProductStatuses)
                    .ToList();

                ProductsDataGrid.ItemsSource = products;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var editPage = new ProductEditWindow();
            if (editPage.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Products selectedProduct)
            {
                var editPage = new ProductEditWindow(selectedProduct.ProductID);
                if (editPage.ShowDialog() == true)
                {
                    LoadProducts();
                }
            }
            else
            {
                MessageBox.Show("Выберите продукт для редактирования", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Products selectedProduct)
            {
                var result = MessageBox.Show($"Удалить продукт '{selectedProduct.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new LogisticsEntities())
                    {
                        var product = context.Products.Find(selectedProduct.ProductID);
                        if (product != null)
                        {
                            context.Products.Remove(product);
                            context.SaveChanges();
                        }
                    }
                    LoadProducts();
                }
            }
            else
            {
                MessageBox.Show("Выберите продукт для удаления", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}