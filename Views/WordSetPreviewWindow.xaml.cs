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
    public partial class WordSetPreviewWindow : Window
    {
        WordSetPreviewViewModel model { get; set; }
        
        public WordSetPreviewWindow(WordSetModel wordset)
        {            
            model = new WordSetPreviewViewModel(wordset);
            InitializeComponent();
            this.DataContext = model;
            
            this.Top = App.Current.MainWindow.Top;
            this.Left = App.Current.MainWindow.Left;
            Closing += model.SaveHideStatus;
        }        
    }
}
