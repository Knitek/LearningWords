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
    public partial class AddOrEditWindow : Window
    {
        ViewModel.AddOrEditViewModel model = new ViewModel.AddOrEditViewModel();

        public AddOrEditWindow(Model.WordSetModel wordSet = null)
        {
            InitializeComponent();
            model.CloseAction = new Action(() => this.Close());            
            
            if(wordSet==null)
            {
                Title = "Dodaj";
            }
            else
            {
                Title = "Edytuj";
                model.Words = wordSet.Words;
                model.Name = wordSet.Name;
            }
            DataContext = model;
        }

        public Model.WordSetModel RunWindow()
        {
            this.ShowDialog();
            if(model.result)
            {
                return new Model.WordSetModel()
                {
                    Words = model.Words,
                    Name = model.Name,
                    Exercises = 0,
                    Tests = 0
                };
            }
            else
            {
                return null;
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                (e.Source as DataGrid).CurrentColumn = (e.Source as DataGrid).Columns.First();
            }
        }
    }
}
