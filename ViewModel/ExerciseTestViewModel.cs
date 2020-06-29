using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningWords.Model;
using LearningWords.Controls;

namespace LearningWords.ViewModel 
{
    class ExerciseTestViewModel : INotifyPropertyChanged
    {
        public LearnMode Mode { get; set; }
        WordSetModel wordSet { get; set; }
        WordSetModel oldWordSet { get; set; }
        string statusText { get; set; }
        System.Windows.Media.Brush statusTextColor { get; set; }
        WordModel currentWordPair { get; set; }
        string answer { get; set; }
        int counter { get; set; }
        bool direction { get; set; }
        string askedWord { get; set; }
        short state { get; set; }

        List<WordModel> correctAnswered { get; set; }

        public Action ClearStatusLabel { get; set; }
        public Action ExitAction { get; set; }
        public Action CursorToEndAction { get; set; }
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
                    answer = value;
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
            counter = 0;
            Mode = LearnMode.Exercise;
            StatusTextColor = System.Windows.Media.Brushes.Black;
            WordSet = wordset;
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
                string textToClear = StatusText;
                await Task.Delay(TimeSpan.FromSeconds(1.5));
                if (textToClear == StatusText)
                {
                    StatusText = "";
                    StatusTextColor = System.Windows.Media.Brushes.Black;
                }
            });
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


            if (IsFinished()) return;
            
            if ((Answer?.ToLower()?.Trim() ?? "") == CurrentWordPair.GetAnswer(Direction))
            {
                StatusText = "Dobrze";
                StatusTextColor = System.Windows.Media.Brushes.DarkGreen;
                CurrentWordPair.Correct++;
                correctAnswered.Add(CurrentWordPair);
                State = 3; 
                
            }
            else
            {
                StatusText = "Źle, powinno być: " + CurrentWordPair.GetAnswer(Direction);
                StatusTextColor = System.Windows.Media.Brushes.DarkRed;
                if (Mode == LearnMode.Test) State=3; //when test mode, program don't give time to see what's wrong and automaticly passes to last state
            }

            if (State != 2)
                CurrentWordPair.Total++;

            switch (Mode)
            {
                case LearnMode.Test: //test
                    {
                        if (WordSet.Words.Count <= counter) // finish
                        {
                            Answer = StatusText;
                            WordSet.LastUse = DateTime.Now;
                            WordSet.Tests++;
                            StatusText = "Koniec";
                            return;
                        }
                        else//next
                        {
                            State = 1;
                            CurrentWordPair = WordSet.Words[counter];
                            AskedWord = CurrentWordPair.GetWord(Direction);
                            Answer = string.Empty;
                        }
                        break;
                    }
                case LearnMode.Exercise: //excercise / ćwiczenie
                    {
                        if (state != 2)
                        {
                            List<WordModel> notAnswered = WordSet.Words.Except(correctAnswered).ToList();
                            if (notAnswered.Count > 0)
                            {
                                WordModel previousPair = CurrentWordPair;
                                ToolsLib.Tools.Shuffle(notAnswered);
                                CurrentWordPair = notAnswered.First();
                                if (previousPair.Word1 == currentWordPair.Word1 && notAnswered.Count > 1)
                                    CurrentWordPair = notAnswered.ElementAt(1);
                                AskedWord = CurrentWordPair.GetWord(Direction);
                                Answer = string.Empty;
                                State = 1;
                                
                            }
                            else
                            {
                                Answer = StatusText;
                                WordSet.LastUse = DateTime.Now;
                                WordSet.Exercises++;
                                StatusText = "Koniec";
                                return;
                            }
                        }
                        break;
                    }
            }
            if (state != 2)
                ClearStatus();
            
        }
        private void ClearStatus()
        {
            Task.Factory.StartNew(ClearStatusLabel);
        }
        private bool IsFinished()
        {
            if (StatusText == "Koniec")
            {
                WordSetStatisticsWindow statistic = new WordSetStatisticsWindow(oldWordSet, WordSet);
                statistic.Show();
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
