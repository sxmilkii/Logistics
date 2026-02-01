using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LogisticsWPF.Model;
using LogisticsWPF.Windows;

namespace LogisticsWPF.Pages
{
    public partial class MaterialsPage : Page
    {
        public MaterialsPage()
        {
            InitializeComponent();
            LoadMaterials();
        }

        private void LoadMaterials()
        {
            using (var context = new LogisticsEntities())
            {
                var materials = context.Materials
                    .Include("MaterialTypes")
                    .Include("Units")
                    .Select(m => new
                    {
                        m.MaterialID,
                        m.Code,
                        m.Name,
                        MaterialTypeName = m.MaterialTypes != null ? m.MaterialTypes.Name : "",
                        UnitName = m.Units != null ? m.Units.Name : "",
                        m.MinStock,
                        m.MarketPrice
                    })
                    .ToList();

                MaterialsDataGrid.ItemsSource = materials;
            }
        }

        private int? GetSelectedMaterialId()
        {
            if (MaterialsDataGrid.SelectedItem == null) return null;

            dynamic item = MaterialsDataGrid.SelectedItem;
            return (int?)item.MaterialID;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new MaterialEditWindow();
            if (editWindow.ShowDialog() == true)
                LoadMaterials();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedMaterialId();
            if (id == null)
            {
                MessageBox.Show("Выберите материал для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var editWindow = new MaterialEditWindow(id);
            if (editWindow.ShowDialog() == true)
                LoadMaterials();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedMaterialId();
            if (id == null)
            {
                MessageBox.Show("Выберите материал для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить выбранный материал?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var context = new LogisticsEntities())
                {
                    var material = context.Materials.Find(id.Value);
                    if (material != null)
                    {
                        context.Materials.Remove(material);
                        context.SaveChanges();
                    }
                }
                LoadMaterials();
            }
        }
    }
}