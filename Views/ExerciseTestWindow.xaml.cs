using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
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
        public ExerciseTestWindow(Model.WordSetModel wordset,bool testMode)
        {
            IObservable<SizeChangedEventArgs> ObservableSizeChanges = Observable
    .FromEventPattern<SizeChangedEventArgs>(this, "SizeChanged")
    .Select(x => x.EventArgs)
    .Throttle(TimeSpan.FromMilliseconds(200));

            IDisposable SizeChangedSubscription = ObservableSizeChanges
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(x => {
                    Window_SizeChanged(x);
                });
            model = new ViewModel.ExerciseTestViewModel(wordset);
            model.ExitAction = new Action(() => { model.SavePosition((int)this.Top, (int)this.Left); this.Close();});
            model.CursorToEndAction = new Action(() => this.TextBoxCursorToEnd());
            InitializeComponent();
            model.SetMode(testMode);
            if (testMode == true)
                Title = "Sprawdzian";
            else
                Title = "Nauka";
            DataContext = model;
            this.Top = model.WindowTop;
            if(this.Top==0) this.Top = App.Current.MainWindow.Top;
            this.Left = model.WindowLeft;
            if(this.Left==0) this.Left = App.Current.MainWindow.Left;
            this.ShowDialog();
        }
        private void TextBoxCursorToEnd()
        {            
            var txtBx = this.AnswerTextBox;
            if (txtBx == null || txtBx.Text == null) return;
            txtBx.CaretIndex = txtBx.Text?.Length ?? 0;
        }

        private void Window_SizeChanged(SizeChangedEventArgs e)
        {
            model.SaveWindowSize((int)e.NewSize.Width, (int)e.NewSize.Height);
            model.SavePosition((int)this.Top, (int)this.Left);
        }
    }
}
