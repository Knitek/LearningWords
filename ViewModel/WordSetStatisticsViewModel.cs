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
        
        public WordSetStatisticsViewModel( WordSetModel wordSetModel)
        {
            this.wordSet = wordSetModel;
        }
        public WordSetStatisticsViewModel(WordSetModel old, WordSetModel after)
        {
            for(int i=0;i<old.Words.Count;i++)
            {
                old.Words[i].Correct = after.Words.First(x => x.Word1 == old.Words[i].Word1).Correct - old.Words[i].Correct;
                old.Words[i].Total = after.Words.First(x => x.Word1 == old.Words[i].Word1).Total - old.Words[i].Total;
            }
            this.wordSet = old;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
