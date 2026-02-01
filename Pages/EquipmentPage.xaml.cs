using LogisticsWPF.Model;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LogisticsWPF.Windows;

namespace LogisticsWPF.Pages
{
    public partial class EquipmentPage : Page
    {
        public EquipmentPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new LogisticsEntities())
            {
                EquipmentGrid.ItemsSource = context.Equipment
                    .Include("Models")
                    .Include("EquipmentTypes")
                    .Include("EquipmentStatuses")
                    .ToList();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var win = new EquipmentEditWindow();
            if (win.ShowDialog() == true)
                LoadData();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentGrid.SelectedItem is Equipment eq)
            {
                var win = new EquipmentEditWindow(eq.EquipmentID);
                if (win.ShowDialog() == true)
                    LoadData();
            }
            else
            {
                MessageBox.Show("Сначала выберите оборудование для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentGrid.SelectedItem is Equipment eq)
            {
                if (MessageBox.Show("Удалить оборудование?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                using (var context = new LogisticsEntities())
                {
                    var entity = context.Equipment.Find(eq.EquipmentID);
                    if (entity != null)
                    {
                        context.Equipment.Remove(entity);
                        context.SaveChanges();
                    }
                }

                LoadData();
            }
        }
    }
}