using System;
using System.Windows;
using LearningWords.ViewModel;

namespace LearningWords.Views
{
    /// <summary>
    /// Interaction logic for FlexibleMessageBoxWindow.xaml
    /// </summary>
    public partial class FlexibleMessageBoxWindow : Window
    {
        public FlexibleMessageBoxWindow()
        {
            InitializeComponent();
        }
        public FlexibleMessageBoxWindow(string content)
        {
            InitializeComponent();
            DataContext = new FlexibleMessageBoxViewModel() { Content = content, CloseAction = new Action(() => this.Close()) };
        }        
    }
}
