﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LearningWords.ViewModel;
using ToolsLib;

namespace LearningWords
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel.LearningWordsViewModel model = new LearningWordsViewModel();
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            Tools.WriteAppSetting("DoUpdateCheck", "false");
#endif
            ToolsLib.Tools.CheckForUpdates(model.title, model.version);   
            model.ExitAction = new Action(() => this.Close());
            DataContext = model;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            model.SaveData();
        }
    }
}
