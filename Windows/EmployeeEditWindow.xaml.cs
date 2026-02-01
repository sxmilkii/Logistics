using LogisticsWPF.Model;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace LogisticsWPF.Windows
{
    public partial class EmployeeEditWindow : Window
    {
        private int? employeeId;

        public EmployeeEditWindow(int? employeeId = null)
        {
            InitializeComponent();
            this.employeeId = employeeId;
            LoadCombos();

            if (employeeId.HasValue)
                LoadEmployee();
        }

        private void LoadCombos()
        {
            using (var context = new LogisticsEntities())
            {
                PositionComboBox.ItemsSource = context.Positions.ToList();
                DepartmentComboBox.ItemsSource = context.Departments.ToList();
                QualificationComboBox.ItemsSource = context.Qualifications.ToList();
                SalaryTypeComboBox.ItemsSource = context.SalaryTypes.ToList();
                StatusComboBox.ItemsSource = context.EmployeeStatuses.ToList();
            }
        }

        private void LoadEmployee()
        {
            using (var context = new LogisticsEntities())
            {
                var emp = context.Employees.Find(employeeId);
                if (emp == null) return;

                SurnameTextBox.Text = emp.Surname;
                NameTextBox.Text = emp.Name;
                MiddleNameTextBox.Text = emp.MiddleName;
                PhoneTextBox.Text = emp.PhoneNumber;

                PositionComboBox.SelectedValue = emp.PositionID;
                DepartmentComboBox.SelectedValue = emp.DepartmentID;
                QualificationComboBox.SelectedValue = emp.QualificationID;
                SalaryTypeComboBox.SelectedValue = emp.SalaryTypeID;
                StatusComboBox.SelectedValue = emp.EmployeeStatusID;
            }
        }

        private bool IsValidName(string value)
        {
            return Regex.IsMatch(value, @"^[A-Za-zА-Яа-яЁё]+$");
        }

        private bool IsValidPhone(string value)
        {
            return Regex.IsMatch(value, @"^[0-9]+$");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SurnameTextBox.Text) ||
                string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Фамилия и имя обязательны",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            if (!IsValidName(SurnameTextBox.Text) ||
                !IsValidName(NameTextBox.Text) ||
                (!string.IsNullOrWhiteSpace(MiddleNameTextBox.Text) &&
                 !IsValidName(MiddleNameTextBox.Text)))
            {
                MessageBox.Show("ФИО может содержать только буквы",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show("Телефон обязателен",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            if (!IsValidPhone(PhoneTextBox.Text))
            {
                MessageBox.Show("Телефон может содержать только цифры",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            if (PositionComboBox.SelectedValue == null ||
                DepartmentComboBox.SelectedValue == null ||
                QualificationComboBox.SelectedValue == null ||
                SalaryTypeComboBox.SelectedValue == null ||
                StatusComboBox.SelectedValue == null)
            {
                MessageBox.Show("Необходимо заполнить все справочные поля",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            using (var context = new LogisticsEntities())
            {
                Employees emp;

                if (employeeId.HasValue)
                {
                    emp = context.Employees.Find(employeeId);
                    if (emp == null) return;
                }
                else
                {
                    emp = new Employees();
                    context.Employees.Add(emp);
                }

                emp.Surname = SurnameTextBox.Text.Trim();
                emp.Name = NameTextBox.Text.Trim();
                emp.MiddleName = MiddleNameTextBox.Text.Trim();
                emp.PhoneNumber = PhoneTextBox.Text.Trim();

                emp.PositionID = (int)PositionComboBox.SelectedValue;
                emp.DepartmentID = (int)DepartmentComboBox.SelectedValue;
                emp.QualificationID = (int)QualificationComboBox.SelectedValue;
                emp.SalaryTypeID = (int)SalaryTypeComboBox.SelectedValue;
                emp.EmployeeStatusID = (int)StatusComboBox.SelectedValue;

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