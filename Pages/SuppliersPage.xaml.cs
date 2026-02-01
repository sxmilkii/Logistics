using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LogisticsWPF.Model;
using LogisticsWPF.Windows;

namespace LogisticsWPF.Pages
{
    public partial class SuppliersPage : Page
    {
        public SuppliersPage()
        {
            InitializeComponent();
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            using (var context = new LogisticsEntities())
            {
                var suppliers = context.Suppliers
                    .Select(s => new
                    {
                        s.SupplierID,
                        s.INN,
                        s.Name,
                        s.Email,
                        s.ReliabilityRating,
                        s.PaymentTerms
                    })
                    .ToList();

                SuppliersDataGrid.ItemsSource = suppliers;
            }
        }

        private int? GetSelectedSupplierId()
        {
            if (SuppliersDataGrid.SelectedItem == null) return null;

            dynamic item = SuppliersDataGrid.SelectedItem;
            return (int?)item.SupplierID;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new SupplierEditWindow();
            if (editWindow.ShowDialog() == true)
                LoadSuppliers();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedSupplierId();
            if (id == null)
            {
                MessageBox.Show("Выберите поставщика для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var editWindow = new SupplierEditWindow(id);
            if (editWindow.ShowDialog() == true)
                LoadSuppliers();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedSupplierId();
            if (id == null)
            {
                MessageBox.Show("Выберите поставщика для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить выбранного поставщика?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var context = new LogisticsEntities())
                {
                    var supplier = context.Suppliers.Find(id.Value);
                    if (supplier != null)
                    {
                        context.Suppliers.Remove(supplier);
                        context.SaveChanges();
                    }
                }
                LoadSuppliers();
            }
        }
    }
}