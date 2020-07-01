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
    public class WordSetStatisticsViewModel : INotifyPropertyChanged
    {
        WordSetModel wordSet { get; set; }

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
        public CommandBase CloseCommand { get; set; }
        public Action ExitAction { get; set; }
        public WordSetStatisticsViewModel( WordSetModel wordSetModel)
        {
            CloseCommand = new CommandBase(Close);
            this.wordSet = wordSetModel;
        }
        //public WordSetStatisticsViewModel(WordSetModel old)
        //{
        //    CloseCommand = new CommandBase(Close);
            
        //    this.wordSet = old;
        //}
        void Close()
        {
            ExitAction.Invoke();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
