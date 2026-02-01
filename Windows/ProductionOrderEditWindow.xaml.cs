using LogisticsWPF.Model;
using System;
using System.Linq;
using System.Windows;

namespace LogisticsWPF.Windows
{
    public partial class ProductionOrderEditWindow : Window
    {
        private int? orderId;

        public ProductionOrderEditWindow(int? orderId = null)
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
                ClientComboBox.ItemsSource = context.Clients.ToList();
                ProductComboBox.ItemsSource = context.Products.ToList();
            }
        }

        private void LoadOrder()
        {
            using (var context = new LogisticsEntities())
            {
                var order = context.ProductionOrders.Find(orderId);
                if (order == null) return;

                ClientComboBox.SelectedValue = order.ClientID;
                ProductComboBox.SelectedValue = order.ProductID;
                QuantityTextBox.Text = order.Quantity.ToString();
                PriorityComboBox.SelectedIndex = order.Priority - 1;
                DeadlinePicker.SelectedDate = order.Deadline;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ClientComboBox.SelectedValue == null ||
                ProductComboBox.SelectedValue == null ||
                string.IsNullOrWhiteSpace(QuantityTextBox.Text) ||
                !int.TryParse(QuantityTextBox.Text, out int qty) ||
                DeadlinePicker.SelectedDate == null)
            {
                MessageBox.Show("Заполните корректно все поля");
                return;
            }

            if (DeadlinePicker.SelectedDate.Value < DateTime.Now)
            {
                MessageBox.Show("Дедлайн не может быть в прошлом");
                return;
            }

            using (var context = new LogisticsEntities())
            {
                ProductionOrders order;

                if (orderId.HasValue)
                {
                    order = context.ProductionOrders.Find(orderId);
                    if (order == null) return;
                }
                else
                {
                    order = new ProductionOrders
                    {
                        ProductionOrderStatusID = context.ProductionOrderStatuses.FirstOrDefault(s => s.Name == "Запланирован").ProductionOrderStatusID
                    };
                    context.ProductionOrders.Add(order);
                }

                order.ClientID = (int)ClientComboBox.SelectedValue;
                order.ProductID = (int)ProductComboBox.SelectedValue;
                order.Quantity = qty;
                order.Priority = PriorityComboBox.SelectedIndex + 1;
                order.Deadline = DeadlinePicker.SelectedDate.Value;

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