using LogisticsWPF.Model;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LogisticsWPF.Windows;

namespace LogisticsWPF.Pages
{
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();
            Loaded += (_, __) => LoadData();
        }

        private void LoadData()
        {
            using (var context = new LogisticsEntities())
            {
                EmployeesGrid.ItemsSource = context.Employees
                    .Include(e => e.Positions)
                    .Include(e => e.Departments)
                    .Include(e => e.Qualifications)
                    .Include(e => e.SalaryTypes)
                    .Include(e => e.EmployeeStatuses)
                    .ToList();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new EmployeeEditWindow();
            if (window.ShowDialog() == true)
                LoadData();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem is Employees emp)
            {
                var window = new EmployeeEditWindow(emp.EmployeeID);
                if (window.ShowDialog() == true)
                    LoadData();
            }
            else
            {
                MessageBox.Show("Выберите сотрудника",
                                "Внимание",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!(EmployeesGrid.SelectedItem is Employees emp))
                return;

            if (MessageBox.Show("Удалить сотрудника?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            using (var context = new LogisticsEntities())
            {
                var entity = context.Employees.Find(emp.EmployeeID);
                if (entity != null)
                {
                    context.Employees.Remove(entity);
                    context.SaveChanges();
                }
            }

            LoadData();
        }
    }
}