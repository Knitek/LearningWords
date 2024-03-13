using LearningWords.ViewModel;
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

namespace LearningWords.Views
{
    /// <summary>
    /// Interaction logic for ActivityCalendarWindow.xaml
    /// </summary>
    public partial class ActivityCalendarWindow : Window
    {
        public ActivityCalendarWindow()
        {
            InitializeComponent();
            DataContext = new ActivityCalnedarViewModel();
        }
    }
}
