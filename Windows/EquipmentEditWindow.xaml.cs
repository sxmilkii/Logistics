using LogisticsWPF.Model;
using System;
using System.Linq;
using System.Windows;

namespace LogisticsWPF.Windows
{
    public partial class EquipmentEditWindow : Window
    {
        private int? equipmentId;

        public EquipmentEditWindow(int? equipmentId = null)
        {
            InitializeComponent();
            this.equipmentId = equipmentId;
            LoadCombos();

            if (equipmentId.HasValue)
                LoadEquipment();
        }

        private void LoadCombos()
        {
            using (var context = new LogisticsEntities())
            {
                ModelComboBox.ItemsSource = context.Models.ToList();
                TypeComboBox.ItemsSource = context.EquipmentTypes.ToList();
                StatusComboBox.ItemsSource = context.EquipmentStatuses.ToList();
            }
        }

        private void LoadEquipment()
        {
            using (var context = new LogisticsEntities())
            {
                var eq = context.Equipment.Find(equipmentId);
                if (eq == null) return;

                InventoryTextBox.Text = eq.InventoryNumber;
                ModelComboBox.SelectedValue = eq.ModelID;
                TypeComboBox.SelectedValue = eq.EquipmentTypeID;
                StatusComboBox.SelectedValue = eq.EquipmentStatusID;
                CommissioningDatePicker.SelectedDate = eq.CommissionDate;
                MaintenanceTextBox.Text = eq.MaintenanceSchedule;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InventoryTextBox.Text))
            {
                MessageBox.Show("Инвентарный номер обязателен");
                return;
            }

            if (string.IsNullOrWhiteSpace(MaintenanceTextBox.Text))
            {
                MessageBox.Show("Профилактика не должна быть пустой");
                return;
            }

            if (ModelComboBox.SelectedValue == null ||
                TypeComboBox.SelectedValue == null ||
                StatusComboBox.SelectedValue == null)
            {
                MessageBox.Show("Заполните все справочные поля");
                return;
            }

            if (!CommissioningDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Укажите дату ввода в эксплуатацию");
                return;
            }

            using (var context = new LogisticsEntities())
            {
                string inventoryNumber = InventoryTextBox.Text.Trim();

                bool exists = context.Equipment.Any(eq =>
                    eq.InventoryNumber == inventoryNumber &&
                    (!equipmentId.HasValue || eq.EquipmentID != equipmentId.Value));

                if (exists)
                {
                    MessageBox.Show("Оборудование с таким инвентарным номером уже существует");
                    return;
                }

                Equipment equipment;

                if (equipmentId.HasValue)
                {
                    equipment = context.Equipment.Find(equipmentId);
                    if (equipment == null) return;
                }
                else
                {
                    equipment = new Equipment();
                    context.Equipment.Add(equipment);
                }

                equipment.InventoryNumber = inventoryNumber;
                equipment.ModelID = (int)ModelComboBox.SelectedValue;
                equipment.EquipmentTypeID = (int)TypeComboBox.SelectedValue;
                equipment.EquipmentStatusID = (int)StatusComboBox.SelectedValue;
                equipment.CommissionDate = CommissioningDatePicker.SelectedDate.Value;
                equipment.MaintenanceSchedule = MaintenanceTextBox.Text.Trim();

                context.SaveChanges();
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}