using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ActivityMonitor.Forms
{
    /// <summary>
    /// Logika interakcji dla klasy FormChoose.xaml
    /// </summary>
    public partial class FormChoose : Window
    {
        public FormChoose()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            int itemIndex = cbxChoose.SelectedIndex;
            Window newWindow;
            switch (itemIndex)
            {
                case 0:
                    newWindow = new Forms.FormActivityWindow();
                    newWindow.Show();
                    break;
                case 1:
                    newWindow = new Forms.FormMealWindow();
                    newWindow.Show();
                    break;
                default:
                    throw new ArgumentException();
            }
            this.Close();
        }
    }
}
