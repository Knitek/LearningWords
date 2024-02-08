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
using ToolsLib;

namespace LearningWords.ViewModel
{
    public class LearningWordsViewModel : INotifyPropertyChanged
    {
        public string title = "Nauka słówek";
        //public string version = "20200809 v1.2.4";
        public string version = "20220406 v1.2.6";
        WordSetModel wordSet { get; set; }
        WordSetModel selectedWordSet { get; set; }
        string statusText { get; set; }
        public int dayGoal { get; set; }


        public WordSetModel WordSet
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
        public int DayGoal
        {
            get
            {
                return this.dayGoal;
            }
            set
            {
                if(this.dayGoal != value)
                {
                    this.dayGoal = value;
                    RaisePropertyChanged("DayGoal");
                }
            }
        }
        public string GoalStatus
        {
            get
            {
                var todayComplete = wordSet.ChildWordSets.Where(x => x.LastUse.Date == DateTime.Today.Date).Select(x=>x.Words.Count).Sum();
                return todayComplete.ToString() + "/" + DayGoal.ToString();
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
                    if(StatusText.StartsWith("Dzienny") is false)
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
        public CommandBase OptionsWindowCommand { get; set; }

        public CommandBase ShowStatisticsCommand { get; set; }
        public CommandBase ImportCommand { get; set; }
        public CommandBase ImportClipboardCommand { get; set; }

        public CommandBase ExportCommand { get; set; }

        public LearningWordsViewModel()
        {
            SetUpSettings();            
            WordSet = new WordSetModel();
            ClearStatusAction = new Action(async () => 
            {
                string textToClear = StatusText;
                await Task.Delay(TimeSpan.FromSeconds(2.5));
                if(textToClear == StatusText)
                {
                    DayGoalStatusText();
                }
            });
            ExerciseCommand = new CommandBase(Exercise);
            AddCommand = new CommandBase(Add);
            EditCommand = new CommandBase(Edit);
            DeleteCommand = new CommandBase(Delete);
            TestCommand = new CommandBase(Test);
            AboutWindowCommand = new CommandBase(AboutWindow);
            OptionsWindowCommand = new CommandBase(OptionsWindow);

            ShowStatisticsCommand = new CommandBase(ShowStatistics);
            ImportCommand = new CommandBase(Import);
            ImportClipboardCommand = new CommandBase(ImportClipboard);
            ExportCommand = new CommandBase(Export);
            LoadData();
            LoadGoalSettings();
            DayGoalStatusText();
        }
        private void SetUpSettings()
        {
            try
            {
                Tools.GetOrSetDefaultDataDirectory();
                Tools.GetOrSetErrorLogPath();                
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
        private void LoadGoalSettings()
        {
            var dayGoalActive = bool.Parse(Tools.ReadAppSetting("DayGoalActive", "false"));
            if (dayGoalActive)
            {
                int tmp = int.Parse(Tools.ReadAppSetting("DayGoal", "0"));
                if (tmp > 0)
                {
                    DayGoal = tmp;
                    RaisePropertyChanged("StatusText");
                }
            }
            else
            {
                DayGoal = 0;
                RaisePropertyChanged("StatusText");
            }
        }
        private void DayGoalStatusText()
        {
            if (DayGoal != 0)
                StatusText = "Dzienny cel: " + GoalStatus;
            else
                StatusText = "";
        }
        
        private void Add()
        {
            try
            {
                AddOrEditWindow addOrEditWindow = new AddOrEditWindow();
                SetPosition(addOrEditWindow);
                var tmp = addOrEditWindow.RunWindow();
                if (tmp == null) return;
                WordSet.ChildWordSets.Add(tmp);
                RaisePropertyChanged("WordSet");
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
                    SetPosition(addOrEditWindow);
                    var tmp = addOrEditWindow.RunWindow();
                    if (tmp == null) return;

                    SelectedWordSet.Name = tmp.Name;
                    SelectedWordSet.Tests = 0;
                    SelectedWordSet.Words = tmp.Words;
                    RaisePropertyChanged("WordSet");
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
                WordSet.ChildWordSets.Remove(SelectedWordSet);
                StatusText = $"Zestaw '{tmpname}' został usunięty.";
            }
        }
        private void OptionsWindow()
        {
            OptionsWindow optionsWindow = new OptionsWindow();
            SetPosition(optionsWindow);
            optionsWindow.Show();
            optionsWindow.Closed += (o, ea) => { LoadGoalSettings(); DayGoalStatusText(); };
        }
        private void AboutWindow()
        {
            ToolsLib.Wpf.AboutWindow aboutWindow = new ToolsLib.Wpf.AboutWindow(title, version, "Program do nauki słówek.") 
            { Top = App.Current.MainWindow.Top, Left = App.Current.MainWindow.Left };
        }
        private void Exercise()
        {
            if (WordSetIsSelected)
            {
                bool? startExercise = true;
                if (Tools.ReadAppSetting("ShowPreview", "true") == "true")
                {
                    WordSetPreviewWindow wordSetPreviewWindow = new WordSetPreviewWindow(SelectedWordSet);
                    startExercise = wordSetPreviewWindow.ShowDialog() ?? false;
                }
                if (startExercise is false) return;
                ExerciseTestWindow exerciseTestWindow = new ExerciseTestWindow(SelectedWordSet, false);
                RaisePropertyChanged("WordSet");
                DayGoalStatusText();
            }
        }

        private void Test()
        {
            if(WordSetIsSelected)
            {
                ExerciseTestWindow exerciseTestWindow = new ExerciseTestWindow(SelectedWordSet,true);                
                RaisePropertyChanged("WordSet");
                DayGoalStatusText();
            }
        }
        private void ShowStatistics()
        {
            if (SelectedWordSet == null) return;
            WordSetStatisticsWindow win = new WordSetStatisticsWindow(SelectedWordSet);
            SetPosition(win);
            win.Show();
        }
        private void ImportClipboard()
        {
            try
            {
                if (Clipboard.ContainsText(TextDataFormat.Text))
                {
                    string clipboardText = Clipboard.GetText(TextDataFormat.Text);
                    var lines = clipboardText.Split( "\r\n".ToCharArray(),StringSplitOptions.RemoveEmptyEntries).ToList().Select(x=>x.Split('\t')).ToList();
                    WordSetModel wordset = new WordSetModel();
                    wordset.Words = new ObservableCollection<WordModel>();

                    wordset.Name = "Nowy";
                    for (int i = 0; i < lines.Count; i++)
                    {
                        wordset.Words.Add(new WordModel()
                        {
                            Word1 = lines[i].First().Trim(),
                            Word2 = lines[i].Last().Trim(),
                        });
                    }
                    if (lines.Any(x=>x.Length != 2))
                    {
                        StatusText = "Możliwy błąd. Edytuj zestaw.";
                    }
                    WordSet.ChildWordSets.Add(wordset);
                    RaisePropertyChanged("WordSet");
                }
            }
            catch (Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "Import()");
            }
        }
        private void Import()
        {
            try
            {
                string path = SelectFile();
                if (path == null) return;
                if (System.IO.Path.GetFileName(path).ToLower().Equals("config.xml"))
                {
                    var tmpData = ToolsLib.Tools.Deserialize<WordSetModel>(path);
                    WordSet = tmpData;
                    RaisePropertyChanged("WordSet");
                }
                else
                {
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
                    WordSet.ChildWordSets.Add(wordset);
                    RaisePropertyChanged("WordSet");
                }
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

                var tmpData = WordSet;
                
                Tools.Serialize(tmpData, path);
            }
            catch(Exception exc)
            {
                Tools.ExceptionLogAndShow(exc, "SaveData");
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
                    ToolsLib.Tools.Serialize(new WordSetModel(), path);
                    WordSet = new WordSetModel();
                }
                else
                {
                    var tmpData = ToolsLib.Tools.Deserialize<WordSetModel>(path);
                    WordSet = tmpData;
                }                
            }
            catch (Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "LoadData");
            }
        }
        private void SetPosition(Window window)
        {
            window.Top = App.Current.MainWindow.Top;
            window.Left = App.Current.MainWindow.Left;
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
