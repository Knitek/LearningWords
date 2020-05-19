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
        public bool TestMode { get; set; }
        WordSetModel wordSet { get; set; }
        WordSetModel oldWordSet { get; set; }
        string statusText { get; set; }
        WordModel currentWordPair { get; set; }
        string answer { get; set; }
        int counter { get; set; }
        bool direction { get; set; }
        string askedWord { get; set; }

        List<WordModel> correctAnswered { get; set; }

        public Action ClearStatusLabel { get; set; }
        public Action ExitAction { get; set; }

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
                if (TestMode)
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
            TestMode = false;
            WordSet = wordset;
            oldWordSet = new WordSetModel(WordSet);
            ToolsLib.Tools.Shuffle(WordSet.Words);
            correctAnswered = new List<WordModel>();

            CurrentWordPair = WordSet.Words.First();
            AskedWord = CurrentWordPair.GetWord(direction);

            CheckCommand = new CommandBase(Check);
            PromptCommand = new CommandBase(Prompt);
        }

        private void Check()
        {
            if (TestMode)
            {
                counter++;
               
                if (StatusText == "Koniec")
                {
                    WordSetStatisticsWindow stat = new WordSetStatisticsWindow(oldWordSet, WordSet);
                    stat.Show();
                    ExitAction.Invoke();
                    return;
                }
                CurrentWordPair.Total++;
                if ((Answer?.ToLower()?.Trim() ?? "") == CurrentWordPair.GetAnswer(Direction))
                {
                    StatusText = "Dobrze";
                    CurrentWordPair.Correct++;
                    correctAnswered.Add(CurrentWordPair);
                }
                else
                {
                    StatusText = "Źle, powinno być: " + CurrentWordPair.GetAnswer(Direction);
                }    
                if (WordSet.Words.Count <= counter)
                {
                    Answer = StatusText;
                    WordSet.LastUse = DateTime.Now;
                    WordSet.Tests++;
                    StatusText = "Koniec";
                    return;
                }
                else
                {
                    CurrentWordPair = WordSet.Words[counter];
                    AskedWord = CurrentWordPair.GetWord(Direction);
                }
                Answer = string.Empty;
                
            }
            else
            {
                counter++;

                

                if (StatusText == "Koniec")
                {
                    WordSetStatisticsWindow stat = new WordSetStatisticsWindow(oldWordSet, WordSet);
                    stat.Show();
                    ExitAction.Invoke();
                    return;
                }
                CurrentWordPair.Total++;
                if ((Answer?.ToLower()?.Trim() ?? "") == CurrentWordPair.GetAnswer(Direction))
                {
                    StatusText = "Dobrze";
                    CurrentWordPair.Correct++;
                    correctAnswered.Add(CurrentWordPair);
                }
                else
                {
                    StatusText = "Źle, powinno być " + CurrentWordPair.GetAnswer(Direction);
                }

                List<WordModel> notAnswered = WordSet.Words.Except(correctAnswered).ToList();
                if (notAnswered.Count>0)
                {
                    ToolsLib.Tools.Shuffle(notAnswered);
                    CurrentWordPair = notAnswered.First();
                    AskedWord = CurrentWordPair.GetWord(Direction);
                }
                else
                {
                    Answer = StatusText;
                    WordSet.LastUse = DateTime.Now;
                    WordSet.Exercises++;
                    StatusText = "Koniec";
                    return;
                }
                Answer = string.Empty;
                
            }
        }
        private void Prompt()
        {
            if((Answer?.Length ?? 0 ) == 0 && CurrentWordPair.Word2.Length > 1)
            {
                Answer = CurrentWordPair.Word2.Substring(0, 1);
                return;
            }
            else if(Answer.Length==1 && CurrentWordPair.Word2.Length >2)
            {
                Answer = CurrentWordPair.Word2.Substring(0, 2);
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
}
