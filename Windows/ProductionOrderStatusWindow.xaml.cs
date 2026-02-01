using LogisticsWPF.Model;
using System.Linq;
using System.Windows;

namespace LogisticsWPF.Windows
{
    public partial class ProductionOrderStatusWindow : Window
    {
        private int orderId;

        public ProductionOrderStatusWindow(int orderId)
        {
            InitializeComponent();
            this.orderId = orderId;
            LoadOrder();
        }

        private void LoadOrder()
        {
            using (var context = new LogisticsEntities())
            {
                var order = context.ProductionOrders
                    .Include("ProductionOrderStatuses")
                    .FirstOrDefault(o => o.ProductionOrderID == orderId);

                if (order == null) return;

                CurrentStatusText.Text = order.ProductionOrderStatuses.Name;

                StatusComboBox.ItemsSource = context.ProductionOrderStatuses.ToList();

                StatusComboBox.SelectedValue = order.ProductionOrderStatusID;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (StatusComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите новый статус");
                return;
            }

            int newStatusId = (int)StatusComboBox.SelectedValue;

            using (var context = new LogisticsEntities())
            {
                var order = context.ProductionOrders.Find(orderId);
                if (order == null) return;

                if (order.ProductionOrderStatusID == newStatusId)
                {
                    MessageBox.Show("Статус не изменился");
                    return;
                }

                order.ProductionOrderStatusID = newStatusId;
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