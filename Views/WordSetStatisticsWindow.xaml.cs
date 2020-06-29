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
using LearningWords.Model;
using LearningWords.ViewModel;

namespace LearningWords
{
    /// <summary>
    /// Interaction logic for WordSetStatisticsWindow.xaml
    /// </summary>
    public partial class WordSetStatisticsWindow : Window
    {
        WordSetStatisticsViewModel model { get; set; }
        public WordSetStatisticsWindow(WordSetModel old, WordSetModel notold)
        {
            model = new WordSetStatisticsViewModel(old, notold);
            InitializeComponent();
            this.DataContext = model;
            model.ExitAction = new Action(() => this.Close());
            this.Top = App.Current.MainWindow.Top;
            this.Left = App.Current.MainWindow.Left;
        }
        public WordSetStatisticsWindow(WordSetModel wordset)
        {
            model = new WordSetStatisticsViewModel(wordset);
            InitializeComponent();
            this.DataContext = model;

            this.Top = App.Current.MainWindow.Top;
            this.Left = App.Current.MainWindow.Left;
        }
    }
}
