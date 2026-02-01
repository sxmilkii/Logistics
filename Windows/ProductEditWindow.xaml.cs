using System.Linq;
using System.Windows;
using LogisticsWPF.Model;

namespace LogisticsWPF.Windows
{
    public partial class ProductEditWindow : Window
    {
        private int? productId;

        public ProductEditWindow(int? productId = null)
        {
            InitializeComponent();
            this.productId = productId;

            LoadComboboxes();
            if (productId.HasValue)
                LoadProductData();
        }

        private void LoadComboboxes()
        {
            using (var context = new LogisticsEntities())
            {
                CategoryComboBox.ItemsSource = context.ProductCategories.ToList();
                UnitComboBox.ItemsSource = context.Units.ToList();
                StatusComboBox.ItemsSource = context.ProductStatuses.ToList();
            }
        }

        private void LoadProductData()
        {
            using (var context = new LogisticsEntities())
            {
                var product = context.Products.Find(productId.Value);
                if (product != null)
                {
                    ArticleTextBox.Text = product.Article;
                    NameTextBox.Text = product.Name;
                    DescriptionTextBox.Text = product.Description;
                    CategoryComboBox.SelectedValue = product.ProductCategoryID;
                    UnitComboBox.SelectedValue = product.UnitID;
                    StatusComboBox.SelectedValue = product.ProductStatusID;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string article = ArticleTextBox.Text.Trim();
            string name = NameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(article))
            {
                MessageBox.Show("Артикул не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Наименование не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (CategoryComboBox.SelectedValue == null || UnitComboBox.SelectedValue == null || StatusComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию, единицу измерения и статус", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new LogisticsEntities())
            {
                bool articleExists = context.Products
                    .Any(p => p.Article == article && (!productId.HasValue || p.ProductID != productId.Value));

                if (articleExists)
                {
                    MessageBox.Show("Продукт с таким артикулом уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Products product;

                if (productId.HasValue)
                {
                    product = context.Products.Find(productId.Value);
                    if (product == null) return;
                }
                else
                {
                    product = new Products();
                    context.Products.Add(product);
                }

                product.Article = article;
                product.Name = name;
                product.Description = DescriptionTextBox.Text.Trim();
                product.ProductCategoryID = (int)CategoryComboBox.SelectedValue;
                product.UnitID = (int)UnitComboBox.SelectedValue;
                product.ProductStatusID = (int)StatusComboBox.SelectedValue;

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