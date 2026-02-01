using System;
using System.Linq;
using System.Windows;
using LogisticsWPF.Model;

namespace LogisticsWPF.Windows
{
    public partial class SupplierEditWindow : Window
    {
        private int? supplierId;

        public SupplierEditWindow(int? supplierId = null)
        {
            InitializeComponent();
            this.supplierId = supplierId;

            if (supplierId.HasValue)
                LoadSupplierData();
        }

        private void LoadSupplierData()
        {
            using (var context = new LogisticsEntities())
            {
                var supplier = context.Suppliers.Find(supplierId.Value);
                if (supplier != null)
                {
                    INNTextBox.Text = supplier.INN;
                    NameTextBox.Text = supplier.Name;
                    EmailTextBox.Text = supplier.Email;
                    ReliabilityTextBox.Text = supplier.ReliabilityRating?.ToString();
                    PaymentTermsTextBox.Text = supplier.PaymentTerms;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string inn = INNTextBox.Text.Trim();
            string name = NameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string payment = PaymentTermsTextBox.Text.Trim();
            string reliabilityText = ReliabilityTextBox.Text.Trim();

            if (string.IsNullOrEmpty(inn))
            {
                MessageBox.Show("ИНН не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!inn.All(char.IsDigit))
            {
                MessageBox.Show("ИНН должен содержать только цифры", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Наименование не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Email не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int? reliability = null;
            if (!string.IsNullOrEmpty(reliabilityText))
            {
                if (!int.TryParse(reliabilityText, out int r) || r < 0 || r > 5)
                {
                    MessageBox.Show("Рейтинг должен быть числом от 0 до 5", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                reliability = r;
            }

            if (string.IsNullOrEmpty(payment))
            {
                MessageBox.Show("Условия оплаты не могут быть пустыми", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new LogisticsEntities())
            {
                bool innExists = context.Suppliers.Any(s =>
                    s.INN == inn && (!supplierId.HasValue || s.SupplierID != supplierId.Value));

                if (innExists)
                {
                    MessageBox.Show("Поставщик с таким ИНН уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Suppliers supplier;

                if (supplierId.HasValue)
                {
                    supplier = context.Suppliers.Find(supplierId.Value);
                    if (supplier == null) return;
                }
                else
                {
                    supplier = new Suppliers();
                    context.Suppliers.Add(supplier);
                }

                supplier.INN = inn;
                supplier.Name = name;
                supplier.Email = email;
                supplier.ReliabilityRating = reliability;
                supplier.PaymentTerms = payment;

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