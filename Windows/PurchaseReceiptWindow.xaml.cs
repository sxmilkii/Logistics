using LogisticsWPF.Model;
using System;
using System.Linq;
using System.Windows;

namespace LogisticsWPF.Windows
{
    public partial class PurchaseReceiptWindow : Window
    {
        private int orderId;

        public PurchaseReceiptWindow(int orderId)
        {
            InitializeComponent();
            this.orderId = orderId;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(AcceptedTextBox.Text, out decimal accepted) ||
                !decimal.TryParse(DefectTextBox.Text, out decimal defect))
            {
                MessageBox.Show("Введите корректные числа");
                return;
            }

            if (accepted < 0 || defect < 0)
            {
                MessageBox.Show("Количество не может быть отрицательным");
                return;
            }

            if (accepted < defect)
            {
                MessageBox.Show("Количество принятого не может быть меньше количества брака");
                return;
            }

            using (var context = new LogisticsEntities())
            {
                var order = context.PurchaseOrders.Find(orderId);
                if (order == null) return;

                var receipt = new PurchaseReceipts
                {
                    PurchaseOrderID = orderId,
                    ReceiptDate = DateTime.Now,
                    AcceptedQuantity = accepted,
                    DefectQuantity = defect
                };

                context.PurchaseReceipts.Add(receipt);

                var stock = context.StockBalances.FirstOrDefault(s =>
                    s.WarehouseID == order.WarehouseID &&
                    s.MaterialID == receipt.PurchaseOrderID);

                if (stock != null)
                    stock.Quantity += accepted;

                context.SaveChanges();
            }

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}