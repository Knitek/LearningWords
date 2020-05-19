using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningWords.Model;
using LearningWords.Controls;
using System.Windows;

namespace LearningWords.ViewModel
{
    public class LearningWordsViewModel : INotifyPropertyChanged
    {
        public string title = "Nauka słówek";
        public string version = "20190130 v1.2.1";
        ObservableCollection<WordSetModel> wordSet { get; set; }
        WordSetModel selectedWordSet { get; set; }
        string statusText { get; set; }
        

        public ObservableCollection<WordSetModel> WordSets
        {
            get
            {
                return wordSet;
            }
            set
            {
                if (wordSet != value)
                {
                    wordSet = value;
                    RaisePropertyChanged("WordSet");
                }
            }
        }
        public WordSetModel SelectedWordSet
        {
            get { return selectedWordSet; }
            set
            {
                if(selectedWordSet != value)
                {
                    selectedWordSet = value;
                    RaisePropertyChanged("SelectedWordSet");
                    RaisePropertyChanged("WordSetIsSelected");
                }
            }
        }
        public string StatusText
        {
            get
            {
                return statusText;
            }
            set
            {
                if(statusText!=value)
                {
                    statusText = value;
                    RaisePropertyChanged("StatusText");
                    Task.Factory.StartNew(ClearStatusAction);
                }
            }
        }
        public Action ClearStatusAction { get; set; }
        public Action ExitAction { get; set; }
        public bool WordSetIsSelected
        {
            get
            {
                if (SelectedWordSet == null)
                    return false;
                else
                    return true;
            }
        }

        public CommandBase ExerciseCommand { get; set; }
        public CommandBase AddCommand { get; set; }
        public CommandBase EditCommand { get; set; }
        public CommandBase DeleteCommand { get; set; }
        public CommandBase TestCommand { get; set; }
        public CommandBase AboutWindowCommand { get; set; }

        public CommandBase ShowStatisticsCommand { get; set; }
        public CommandBase ImportCommand { get; set; }
        public CommandBase ExportCommand { get; set; }

        public LearningWordsViewModel()
        {
            SetUpSettings();
            WordSets = new ObservableCollection<WordSetModel>();
            ClearStatusAction = new Action(async () => 
            {
                string textToClear = StatusText;
                await Task.Delay(TimeSpan.FromSeconds(2.5));
                if(textToClear == StatusText)
                    StatusText = "";
            });
            ExerciseCommand = new CommandBase(Exercise);
            AddCommand = new CommandBase(Add);
            EditCommand = new CommandBase(Edit);
            DeleteCommand = new CommandBase(Delete);
            TestCommand = new CommandBase(Test);
            AboutWindowCommand = new CommandBase(AboutWindow);

            ShowStatisticsCommand = new CommandBase(ShowStatistics);
            ImportCommand = new CommandBase(Import);
            ExportCommand = new CommandBase(Export);
            LoadData();
        }
        private void SetUpSettings()
        {
            try
            {
                ToolsLib.Tools.GetOrSetDefaultDataDirectory();
                ToolsLib.Tools.GetOrSetErrorLogPath();                
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void Exercise()
        {
            if(WordSetIsSelected)
            {
                ExerciseTestWindow exerciseTestWindow = new ExerciseTestWindow(SelectedWordSet, false);
                RaisePropertyChanged("WordSets");
            }
        }
        private void Add()
        {
            try
            {
                AddOrEditWindow addOrEditWindow = new AddOrEditWindow();
                var tmp = addOrEditWindow.RunWindow();
                if (tmp == null) return;
                WordSets.Add(tmp);
                RaisePropertyChanged("WordSets");
                StatusText = $"Dodano {tmp.Name} do listy zestawów.";
            }
            catch (Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "AddCommand");
            }
        }
        private void Edit()
        {
            try
            {
                if (WordSetIsSelected)
                {
                    AddOrEditWindow addOrEditWindow = new AddOrEditWindow(SelectedWordSet);
                    addOrEditWindow.Top = App.Current.MainWindow.Top;
                    addOrEditWindow.Left = App.Current.MainWindow.Left;
                    var tmp = addOrEditWindow.RunWindow();
                    if (tmp == null) return;

                    SelectedWordSet.Name = tmp.Name;
                    SelectedWordSet.Tests = 0;
                    SelectedWordSet.Words = tmp.Words;
                    RaisePropertyChanged("WordSets");
                    StatusText = $"Zestaw '{tmp.Name}' został zmodyfikowany.";
                }
            }
            catch(Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "EditCommand");
            }
        }
        private void Delete()
        {
            if(WordSetIsSelected)
            {
                var tmpname = SelectedWordSet.Name;
                WordSets.Remove(SelectedWordSet);
                StatusText = $"Zestaw '{tmpname}' został usunięty.";
            }
        }
        private void AboutWindow()
        {
            ToolsLib.Wpf.AboutWindow aboutWindow = new ToolsLib.Wpf.AboutWindow(title, version, "Program do nauki słówek.");
        }
        private void Test()
        {
            if(WordSetIsSelected)
            {
                ExerciseTestWindow exerciseTestWindow = new ExerciseTestWindow(SelectedWordSet,true);
                
                RaisePropertyChanged("WordSets");
            }
        }
        private void ShowStatistics()
        {
            if (SelectedWordSet == null) return;
            WordSetStatisticsWindow win = new WordSetStatisticsWindow(SelectedWordSet);
            win.Top = App.Current.MainWindow.Top;
            win.Left = App.Current.MainWindow.Left;
            win.Show();
        }
        private void Import()
        {
            try
            {
                string path = SelectFile();
                if (path == null) return;

                List<string> lines = System.IO.File.ReadAllLines(path).ToList();
                if (lines[0] == string.Empty || lines[0] == System.Environment.NewLine) lines.RemoveAt(0);
                if (lines.Last() == string.Empty || lines.Last() == System.Environment.NewLine) lines.Remove(lines.Last());
                WordSetModel wordset = new WordSetModel();
                wordset.Words = new ObservableCollection<WordModel>();

                wordset.Name = "Nowy";
                for (int i = 0; i < lines.Count; i++)
                {
                    if ((i + 1) < lines.Count)
                    {
                        wordset.Words.Add(new WordModel()
                        {
                            Word1 = lines[i],
                            Word2 = lines[i + 1],
                        });
                        i++;
                    }
                }
                if (lines.Count % 2 != 0)
                {
                    StatusText = "Możliwy błąd. Edytuj zestaw.";
                }
                WordSets.Add(wordset);
                RaisePropertyChanged("WordSets");
            }
            catch(Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "Import()");
            }
        }
        private void Export()
        {
            try
            {
                if (SelectedWordSet.Words.Count == 0) { StatusText = "Zestaw jest pusty"; return; }

                List<string> list = new List<string>();
                SelectedWordSet.Words.ToList().ForEach(x => { list.Add(x.Word1); list.Add(x.Word2); });
                var dialog = new System.Windows.Forms.SaveFileDialog();
                dialog.DefaultExt = "txt";
                dialog.Filter = "Text (*.txt)|*.txt|All files (*.*)|*.*";
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    System.IO.File.WriteAllLines(dialog.FileName,list.ToArray());
                    if(System.IO.File.Exists(dialog.FileName))
                    {
                        StatusText = "Zapisano do " + System.IO.Path.GetFileName(dialog.FileName);
                    }
                    else
                    {
                        StatusText = "Nie udało się zapisać.";
                    }
                }
            }
            catch(Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "Export()");
            }
        }
        public void SaveData()
        {
            try
            {
                string path = ToolsLib.Tools.ReadAppSettingPath("defaultDataDirectory");
                path = System.IO.Path.Combine(path, "Config.xml");
                var tmpData = WordSets;
                ToolsLib.Tools.Serialize(tmpData, path);
            }
            catch(Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "SaveData");
            }
        }
        private void LoadData()
        {
            try
            {
                string path = ToolsLib.Tools.ReadAppSettingPath("defaultDataDirectory");
                path = System.IO.Path.Combine(path, "Config.xml");
                if (!System.IO.File.Exists(path))
                {
                    ToolsLib.Tools.Serialize(new ObservableCollection<WordSetModel>(), path);
                    WordSets = new ObservableCollection<WordSetModel>();
                }
                else
                {
                    var tmpData = ToolsLib.Tools.Deserialize<ObservableCollection<WordSetModel>>(path);
                    WordSets = tmpData;
                }                
            }
            catch (Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "LoadData");
            }
        }

        private string SelectFile()
        {
            string tmp = String.Empty;
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                tmp = dialog.FileName;
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    return tmp;
                }
                else
                    return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
