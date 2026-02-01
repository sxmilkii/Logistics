using System;
using System.Linq;
using System.Windows;
using LogisticsWPF.Model;

namespace LogisticsWPF.Windows
{
    public partial class MaterialEditWindow : Window
    {
        private int? materialId;

        public MaterialEditWindow(int? materialId = null)
        {
            InitializeComponent();
            this.materialId = materialId;

            LoadComboboxes();
            if (materialId.HasValue)
                LoadMaterialData();
        }

        private void LoadComboboxes()
        {
            using (var context = new LogisticsEntities())
            {
                TypeComboBox.ItemsSource = context.MaterialTypes.ToList();
                UnitComboBox.ItemsSource = context.Units.ToList();
            }
        }

        private void LoadMaterialData()
        {
            using (var context = new LogisticsEntities())
            {
                var material = context.Materials.Find(materialId.Value);
                if (material != null)
                {
                    CodeTextBox.Text = material.Code;
                    NameTextBox.Text = material.Name;
                    TypeComboBox.SelectedValue = material.MaterialTypeID;
                    UnitComboBox.SelectedValue = material.UnitID;
                    MinStockTextBox.Text = material.MinStock.ToString();
                    MarketPriceTextBox.Text = material.MarketPrice.ToString();
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string code = CodeTextBox.Text.Trim();
            string name = NameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Код не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Наименование не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (TypeComboBox.SelectedValue == null || UnitComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите тип и единицу измерения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(MinStockTextBox.Text.Trim(), out decimal minStock))
            {
                MessageBox.Show("Минимальный остаток должен быть числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(MarketPriceTextBox.Text.Trim(), out decimal marketPrice))
            {
                MessageBox.Show("Рыночная цена должна быть числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new LogisticsEntities())
            {
                bool codeExists = context.Materials
                    .Any(m => m.Code == code && (!materialId.HasValue || m.MaterialID != materialId.Value));

                if (codeExists)
                {
                    MessageBox.Show("Материал с таким кодом уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Materials material;

                if (materialId.HasValue)
                {
                    material = context.Materials.Find(materialId.Value);
                    if (material == null) return;
                }
                else
                {
                    material = new Materials();
                    context.Materials.Add(material);
                }

                material.Code = code;
                material.Name = name;
                material.MaterialTypeID = (int)TypeComboBox.SelectedValue;
                material.UnitID = (int)UnitComboBox.SelectedValue;
                material.MinStock = minStock;
                material.MarketPrice = marketPrice;

                context.SaveChanges();
            }

            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}