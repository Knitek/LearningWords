using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningWords.Model;
using LearningWords.Controls;
using ToolsLib;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media.Media3D;

namespace LearningWords.ViewModel 
{
    class ExerciseTestViewModel : INotifyPropertyChanged
    {
        public LearnMode Mode { get; set; }
        WordSetModel wordSet { get; set; }
        WordSetModel oldWordSet { get; set; }
        string statusText { get; set; }
        System.Windows.Media.Brush statusTextColor { get; set; }
        System.Windows.Media.Brush hintCircle { get; set; }
        WordModel currentWordPair { get; set; }
        string answer { get; set; }
        int counter { get; set; }
        int leftCount { get; set; }
        bool direction { get; set; }
        string askedWord { get; set; }
        short state { get; set; }
        bool specialCharactersMode { get; set; }
        bool allowHints { get; set; }
        Dictionary<string,string> specialCharacters { get; set; }
        List<WordModel> correctAnswered { get; set; }
        bool isFinished { get; set; }
        int windowHeight { get; set; }
        int windowWidth { get; set; }

        public int WindowTop
        {
            get
            {
                return int.Parse(Tools.ReadAppSetting("ExerciseTestWindowTop", "0"));
            }
        }
        public int WindowLeft
        {
            get
            {
                return int.Parse(Tools.ReadAppSetting("ExerciseTestWindowLeft", "0"));
            }
        }
        public int WindowHeight
        {
            get
            {
                return windowHeight;
            }
            set
            {
                if (value != windowHeight)
                {
                    windowHeight = value;
                    RaisePropertyChanged("ExerciseTestWindowHeight");
                }
            }
        }
        public int WindowWidth
        {
            get { return windowWidth; }
            set
            {
                if (value != windowWidth)
                {
                    windowWidth = value;
                    RaisePropertyChanged("ExerciseTestWindowWidth");
                }
            }
        }
        public bool SpecialCharactersMode
        {
            get
            {
                return specialCharactersMode;
            }
            set
            {
                if(value != specialCharactersMode)
                {
                    specialCharactersMode = value;
                    RaisePropertyChanged("SpecialCharactersMode");
                }
            }
        }
        public bool AllowHints
        {
            get
            {
                return allowHints;
            }
            set
            {
                if(value != allowHints)
                {
                    allowHints = value;
                    RaisePropertyChanged("AllowHints");
                }
            }
        }
        public Dictionary<string,string> SpecialCharacters
        {
            get
            {
                return specialCharacters;
            }
            set
            {
                if(value != specialCharacters)
                {
                    specialCharacters = value;
                    RaisePropertyChanged("SpecialCharacters");
                }
            }
        }
        public Action ClearStatusLabel { get; set; }
   
        public Action ExitAction { get; set; }
        public Action CursorToEndAction { get; set; }
        public int LeftCount
        {
            get
            {
                return leftCount;
            }
            set
            {
                if(leftCount != value)
                {
                    leftCount = value;
                    RaisePropertyChanged("LeftCountText");
                }
            }
        }
        public string LeftCountText
        {
            get
            {
                return "Pozostało: "+leftCount.ToString();
            }
        }
        public bool Direction
        {
            get
            {
                return direction;
            }
            set
            {
                if (direction != value)
                {
                    direction = value;
                    RaisePropertyChanged("Direction");
                    AskedWord = CurrentWordPair.GetWord(Direction);
                }
            }
        }
        // 1 - waiting for answer, 2 - show correct answer, 3 - go to 1
        public short State
        {
            get
            {
                return this.state;
            }
            set
            {
                if (this.state != value)
                {
                    this.state = value;
                }
            }
        }
        public String AskedWord
        {
            get
            {
                return askedWord;
            }
            set
            {
                if(askedWord!=value)
                {
                    askedWord = value;
                    RaisePropertyChanged("AskedWord");
                }
            }
        }
        public WordModel CurrentWordPair
        {
            get
            {
                return currentWordPair;
            }
            set
            {
                currentWordPair = value;
                RaisePropertyChanged("CurrentWordPair");
            }
        }
        public string Answer
        {
            get { return answer; }
            set
            {
                if (answer != value)
                {
                    if (string.IsNullOrWhiteSpace(answer) && value.Length == 1)
                        StatusText = "";

                    if (SpecialCharactersMode)
                        answer = ReplaceSpecialCharacters(answer, value);
                    else                    
                        answer = value;
                    if(AllowHints)
                    {
                        if (answer.Length > 0)
                        {
                            if (answer.ToLower()[0] == CurrentWordPair.GetAnswer(Direction)[0])
                                HintCircle = System.Windows.Media.Brushes.Green;
                            else
                                HintCircle = System.Windows.Media.Brushes.Red;
                        }
                        else
                            HintCircle = System.Windows.Media.Brushes.Transparent;
                    }
                    RaisePropertyChanged("Answer");
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
                if (statusText != value)
                {
                    statusText = value;
                    RaisePropertyChanged("StatusText");
                }
            }
        }
        public System.Windows.Media.Brush StatusTextColor
        {
            get
            {
                return statusTextColor;
            }
            set
            {
                statusTextColor = value;
                RaisePropertyChanged("StatusTextColor");
            }
        }
        public System.Windows.Media.Brush HintCircle
        {
            get
            {
                return hintCircle;
            }
            set
            {
                hintCircle = value;
                RaisePropertyChanged("HintCircle");
            }
        }
        public WordSetModel WordSet
        {
            get
            {
                return wordSet;
            }
            set
            {
                if(wordSet!=value)
                {
                    wordSet = value;
                    RaisePropertyChanged("WordSet");
                }
            }
        }
        public bool PromptIsActive
        {
            get
            {
                if (Mode == LearnMode.Test)
                    return false;
                else
                    return true;
            }
        }
        public CommandBase CheckCommand { get; set; }
        public CommandBase PromptCommand { get; set; }
        

        public ExerciseTestViewModel(WordSetModel wordset)
        {
            if (Tools.ReadAppSetting("SpecialCharactersMode", "false") == "true")
            {
                specialCharactersMode = true;
                string filename = Tools.ReadAppSettingPath("SpecialCharacters");
                if(File.Exists(filename))
                {
                    LoadSpecialCharacters(filename);
                }
                else
                {
                    MessageBox.Show("Nie znaleziono pliku ustawień znaków specjalnych", "Błąd", MessageBoxButton.OK);
                }
            }

            if(Tools.ReadAppSetting("AllowHints","true")=="true")
            {
                AllowHints = true;                
            }
            else
            {
                AllowHints = false;
            }
            WindowHeight = int.Parse(Tools.ReadAppSetting("ExerciseTestWindowHeight", "235"));
            WindowWidth = int.Parse(Tools.ReadAppSetting("ExerciseTestWindowWidth", "300"));
            HintCircle = System.Windows.Media.Brushes.Transparent;
            isFinished = false;
            counter = 0;
            Mode = LearnMode.Exercise;
            StatusTextColor = System.Windows.Media.Brushes.Black;
            WordSet = wordset;
            LeftCount = WordSet.Words.Count;
            oldWordSet = new WordSetModel(WordSet);
            ToolsLib.Tools.Shuffle(WordSet.Words);
            correctAnswered = new List<WordModel>();

            CurrentWordPair = WordSet.Words.First();
            AskedWord = CurrentWordPair.GetWord(direction);
            State = 1;

            CheckCommand = new CommandBase(Check);
            PromptCommand = new CommandBase(Prompt);

            ClearStatusLabel = new Action(async () =>
            {
                string textToClear = CurrentWordPair.Word1;
                await Task.Delay(TimeSpan.FromSeconds(2.5));
                if (textToClear == CurrentWordPair.Word1)
                {
                    StatusText = "";
                    StatusTextColor = System.Windows.Media.Brushes.Black;
                }
            });
        }
        public void SaveWindowSize(int width, int height)
        {
            Tools.WriteAppSetting("ExerciseTestWindowWidth", width.ToString());
            Tools.WriteAppSetting("ExerciseTestWindowHeight", height.ToString());
        }
        public void SavePosition(int top, int left)
        {
            Tools.WriteAppSetting("ExerciseTestWindowTop", top.ToString());
            Tools.WriteAppSetting("ExerciseTestWindowLeft", left.ToString());
        }
        public void SetMode(bool mode)
        {
            if (mode)
            {
                Mode = LearnMode.Test;                
            }
            else
            {
                Mode = LearnMode.Exercise;
            }
        }
        private void TestExercise()
        {
            counter++;
            state++;
            //states: 1-fresh 2-check answer 3-analize failure
            if (IsFinished()) return;
            if (state == 2)
            {
                if ((Answer?.ToLower()?.Trim() ?? "") == CurrentWordPair.GetAnswer(Direction))
                {
                    StatusText = "Dobrze";
                    StatusTextColor = System.Windows.Media.Brushes.DarkGreen;
                    CurrentWordPair.Correct++;
                    CurrentWordPair.Total++;
                    correctAnswered.Add(CurrentWordPair);
                    state = 3;
                }
                else
                {
                    StatusText = "Źle, powinno być: " + CurrentWordPair.GetAnswer(Direction);
                    StatusTextColor = System.Windows.Media.Brushes.DarkRed;
                    CurrentWordPair.Total++;
                    if (Mode == LearnMode.Test) state++; //when test mode, program don't give time to see what's wrong and automaticly passes to last state             
                }
            }
            switch (Mode)
            {
                case LearnMode.Test: //test
                    {
                        LeftCount = WordSet.Words.Count - counter;
                        if (WordSet.Words.Count <= counter) // finish
                        {
                            Answer = StatusText;
                            WordSet.LastUse = DateTime.Now;
                            WordSet.Tests++;
                            StatusText = "Koniec";
                            isFinished = true;
                            SaveLog(WordSet.Name, Mode);
                            return;
                        }
                        else//next
                        {
                            state = 1;
                            CurrentWordPair = WordSet.Words[counter];
                            AskedWord = CurrentWordPair.GetWord(Direction);
                            Answer = string.Empty;
                        }
                        break;
                    }
                case LearnMode.Exercise: //excercise / ćwiczenie
                    {
                        if (state == 3)
                        {
                            List<WordModel> notAnswered = WordSet.Words.Except(correctAnswered).ToList();
                            LeftCount = notAnswered.Count;
                            if (notAnswered.Count > 0)
                            {
                                WordModel previousPair = CurrentWordPair;
                                ToolsLib.Tools.Shuffle(notAnswered);
                                CurrentWordPair = notAnswered.First();
                                if (previousPair.Word1 == currentWordPair.Word1 && notAnswered.Count > 1)
                                    CurrentWordPair = notAnswered.ElementAt(1);
                                AskedWord = CurrentWordPair.GetWord(Direction);
                                Answer = string.Empty;
                                state = 1;                                
                            }
                            else
                            {
                                Answer = StatusText;
                                WordSet.LastUse = DateTime.Now;
                                WordSet.Exercises++;
                                StatusText = "Koniec";
                                isFinished = true;
                                SaveLog(WordSet.Name,Mode);
                                return;
                            }
                        }
                        break;
                    }                    
            }
            if (state != 2)
                ClearStatus();
            
        }
        private void SaveLog(string wordSetName,LearnMode learnMode)
        {
            System.IO.File.AppendAllText("LearnLog"+DateTime.Now.Year+".txt", $"{DateTime.Now.ToString()}\t{wordSetName}\t{learnMode}\r\n");
        }
        private void ClearStatus()
        {
            Task.Factory.StartNew(ClearStatusLabel);
        }
        private bool IsFinished()
        {
            if (isFinished)
            {
                for (int i = 0; i < oldWordSet.Words.Count; i++)
                {
                    oldWordSet.Words[i].Correct = WordSet.Words.First(x => x.Word1 == oldWordSet.Words[i].Word1).Correct - oldWordSet.Words[i].Correct;
                    oldWordSet.Words[i].Total = WordSet.Words.First(x => x.Word1 == oldWordSet.Words[i].Word1).Total - oldWordSet.Words[i].Total;
                }
                if (Tools.ReadAppSetting("ShowStatistics", "true") == "true")
                {
                    WordSetStatisticsWindow statistic = new WordSetStatisticsWindow(oldWordSet);
                    statistic.Show();
                }
                ExitAction.Invoke();                
                return true;
            }
            else
                return false;
        }
        //after Enter key pressed
        private void Check()
        {
            TestExercise();
        }
        //after Right Ctrl pressed
        private void Prompt()
        {            
            if((Answer?.Length ?? 0 ) == 0 && CurrentWordPair.Word2.Length > 1)
            {
                Answer = CurrentWordPair.Word2.Substring(0, 1);
                CursorToEndAction.Invoke();
                return;
            }
            else if(Answer.Length==1 && CurrentWordPair.Word2.Length >2)
            {
                Answer = CurrentWordPair.Word2.Substring(0, 2);
                CursorToEndAction.Invoke();
                return;
            }
            StatusText = "Nie można podpowiedzieć.";
        }


        string ReplaceSpecialCharacters(string oldValue, string newValue)
        {
            string result = newValue;

            if (SpecialCharactersMode)
            {
                string newPart = string.Empty;
                if (string.IsNullOrWhiteSpace(oldValue))
                    newPart = newValue;
                else
                    newPart = newValue.TrimStart(oldValue.ToCharArray());

                if (!string.IsNullOrWhiteSpace(newPart))
                {
                    if (SpecialCharacters.ContainsKey(newPart))
                    {
                        return newValue.Replace(newPart, SpecialCharacters[newPart]);
                    }   
                }
            }
            return result;
        }
        void LoadSpecialCharacters(string filename)
        {
            try
            {
                var lines = File.ReadAllLines(filename);
                SpecialCharacters = new Dictionary<string, string>();
                foreach (var line in lines)
                {
                    var fields = line.Split(';');
                    SpecialCharacters.Add(fields.First(), fields.Last());
                }
            }
            catch
            {
                MessageBox.Show("Nie udało się wczytać tablicy znaków specjalnych");
                SpecialCharactersMode = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
    enum LearnMode
    {
        Test,
        Exercise,
    }
}
