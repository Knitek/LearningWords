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

namespace LearningWords
{
    /// <summary>
    /// Interaction logic for AddOrEditWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        ViewModel.OptionsViewModel model = new ViewModel.OptionsViewModel();

        public OptionsWindow()
        {
            InitializeComponent();
            model.CloseAction = new Action(() => this.Close());            
            
            DataContext = model;
        }

    }
}
