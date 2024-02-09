using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LearningWords.Model;
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
            wordSetDataGrid.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(productsDataGrid_PreviewMouseLeftButtonDown);
            wordSetDataGrid.Drop += new DragEventHandler(productsDataGrid_Drop);
            wordSetDataGrid.MouseDoubleClick += new MouseButtonEventHandler(doubleClick);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            model.SaveData();
        }
        public delegate Point GetPosition(IInputElement element);
        int rowIndex = -1;
        
        void doubleClick(object sender, RoutedEventArgs e)
        {
            model.OpenWordSetGroup();
        }

        void productsDataGrid_Drop(object sender, DragEventArgs e)
        {
            if (rowIndex < 0)
                return;
            int index = this.GetCurrentRowIndex(e.GetPosition);
            if (index < 0)
                return;
            if (index == rowIndex)
                return;
            if (index == wordSetDataGrid.Items.Count - 1)
            {
                MessageBox.Show("This row-index cannot be drop");
                return;
            }

            var changedWordSet = model.CurrentWordSetList.ChildWordSets[rowIndex];
            model.CurrentWordSetList.ChildWordSets.RemoveAt(rowIndex);
            model.CurrentWordSetList.ChildWordSets[index].ChildWordSets.Add(changedWordSet);
        }

        void productsDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rowIndex = GetCurrentRowIndex(e.GetPosition);
            if (rowIndex < 0)
                return;
            wordSetDataGrid.SelectedIndex = rowIndex;
            WordSetModel selectedEmp = wordSetDataGrid.Items[rowIndex] as WordSetModel;
            if (selectedEmp == null)
                return;
            DragDropEffects dragdropeffects = DragDropEffects.Move;
            if (DragDrop.DoDragDrop(wordSetDataGrid, selectedEmp, dragdropeffects)
                                != DragDropEffects.None)
            {
                wordSetDataGrid.SelectedItem = selectedEmp;
            }
        }

        private bool GetMouseTargetRow(Visual theTarget, GetPosition position)
        {
            Rect rect = VisualTreeHelper.GetDescendantBounds(theTarget);
            Point point = position((IInputElement)theTarget);
            return rect.Contains(point);
        }

        private DataGridRow GetRowItem(int index)
        {
            if (wordSetDataGrid.ItemContainerGenerator.Status
                    != GeneratorStatus.ContainersGenerated)
                return null;
            return wordSetDataGrid.ItemContainerGenerator.ContainerFromIndex(index)
                                                            as DataGridRow;
        }

        private int GetCurrentRowIndex(GetPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < wordSetDataGrid.Items.Count; i++)
            {
                DataGridRow itm = GetRowItem(i);
                if (GetMouseTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }
    }
}
