using LogisticsWPF.Model;
using System;
using System.Linq;
using System.Windows;

namespace LogisticsWPF.Windows
{
    public partial class PurchaseOrderEditWindow : Window
    {
        private int? orderId;

        public PurchaseOrderEditWindow(int? orderId = null)
        {
            InitializeComponent();
            this.orderId = orderId;
            LoadCombos();

            if (orderId.HasValue)
                LoadOrder();
        }

        private void LoadCombos()
        {
            using (var context = new LogisticsEntities())
            {
                SupplierComboBox.ItemsSource = context.Suppliers.ToList();
                WarehouseComboBox.ItemsSource = context.Warehouses.ToList();

                var employees = context.Employees
                    .Select(emp => new
                    {
                        emp.EmployeeID,
                        FullName = emp.Surname + " " + emp.Name + " " + emp.MiddleName
                    })
                    .ToList();

                EmployeeComboBox.ItemsSource = employees;
            }
        }

        private void LoadOrder()
        {
            using (var context = new LogisticsEntities())
            {
                var order = context.PurchaseOrders.Find(orderId);
                if (order == null) return;

                SupplierComboBox.SelectedValue = order.SupplierID;
                WarehouseComboBox.SelectedValue = order.WarehouseID;
                EmployeeComboBox.SelectedValue = order.EmployeeID;
                ExpectedDatePicker.SelectedDate = order.ExpectedDate;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierComboBox.SelectedValue == null ||
                WarehouseComboBox.SelectedValue == null ||
                EmployeeComboBox.SelectedValue == null ||
                !ExpectedDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            if (ExpectedDatePicker.SelectedDate.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Ожидаемая дата не может быть меньше текущей");
                return;
            }

            using (var context = new LogisticsEntities())
            {
                PurchaseOrders order;

                if (orderId.HasValue)
                {
                    order = context.PurchaseOrders.Find(orderId);
                    if (order == null) return;
                }
                else
                {
                    order = new PurchaseOrders
                    {
                        OrderDate = DateTime.Now,
                        PurchaseStatusID = 1
                    };
                    context.PurchaseOrders.Add(order);
                }

                order.SupplierID = (int)SupplierComboBox.SelectedValue;
                order.WarehouseID = (int)WarehouseComboBox.SelectedValue;
                order.EmployeeID = (int)EmployeeComboBox.SelectedValue;
                order.ExpectedDate = ExpectedDatePicker.SelectedDate.Value;

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