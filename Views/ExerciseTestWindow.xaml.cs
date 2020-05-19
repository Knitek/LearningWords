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
    /// Interaction logic for ExerciseTestWindow.xaml
    /// </summary>
    public partial class ExerciseTestWindow : Window
    {
        ViewModel.ExerciseTestViewModel model { get; set; }
        public ExerciseTestWindow(Model.WordSetModel wordset,bool Test)
        {
            model = new ViewModel.ExerciseTestViewModel(wordset);
            model.ExitAction = new Action(() => this.Close());
            InitializeComponent();
            if(Test)
            {
                model.TestMode = true;
                Title = "Sprawdzian";
            }
            else
            {
                Title = "Nauka";
                model.TestMode = false;
            }
            DataContext = model;

            this.Top = App.Current.MainWindow.Top;
            this.Left = App.Current.MainWindow.Left;
            this.ShowDialog();
        }
    }
}
