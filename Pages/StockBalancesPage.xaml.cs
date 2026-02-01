using LogisticsWPF.Model;
using System.Linq;
using System.Windows.Controls;

namespace LogisticsWPF.Pages
{
    public partial class StockBalancesPage : Page
    {
        public StockBalancesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new LogisticsEntities())
            {
                StockGrid.ItemsSource = context.StockBalances
                    .Include("Warehouses")
                    .Include("Materials.Units")
                    .ToList();
            }
        }
    }
}