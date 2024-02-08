﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningWords.Model
{
    public class WordSetModel : INotifyPropertyChanged
    {
        string name { get; set; }
        string groupName { get; set; }
        int exercises { get; set; }
        int tests { get; set; }
        ObservableCollection<WordModel> words { get; set; }
        WordSetModel paretWordSet { get; set; }
        ObservableCollection<WordSetModel> childWordSets { get; set; }
        DateTime lastUse { get; set; }
        
        
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }
        public int Exercises
        {
            get
            {
                return exercises;
            }
            set
            {
                if(exercises!=value)
                {
                    exercises = value;
                    RaisePropertyChanged("Exercises");
                }
            }
        }
        public int Tests
        {
            get
            {
                return tests;
            }
            set
            {
                if(tests!=value)
                {
                    tests = value;
                    RaisePropertyChanged("Tests");
                }
            }
        }
        public ObservableCollection<WordModel> Words
        {
            get { return words; }
            set
            {
                if(words!=value)
                {
                    words = value;
                    RaisePropertyChanged("Words");
                }
            }
        }
        public DateTime LastUse
        {
            get
            {
                return lastUse;
            }
            set
            {
                if(lastUse!=value)
                {
                    lastUse = value;
                    RaisePropertyChanged("LastUse");
                }
            }
        }
        public ObservableCollection<WordSetModel> ChildWordSets
        {
            get
            {
                return childWordSets;
            }
            set
            {
                if (childWordSets != value)
                {
                    childWordSets = value;
                    RaisePropertyChanged("ChildWordSets");
                }
            }
        }
        public WordSetModel ParentWordSet
        {
            get
            {
                return paretWordSet;
            }
            set
            {
                if (paretWordSet != value)
                {
                    paretWordSet = value;
                    RaisePropertyChanged("ParentWordSet");
                }
            }
        }
        public WordSetModel()
        {
            Words = new ObservableCollection<WordModel>();
        }
        public WordSetModel(WordSetModel source)
        {
            this.Name = source.Name;
            this.Exercises = source.Exercises;
            this.Tests = source.Tests;
            this.LastUse = source.LastUse;
            Words = new ObservableCollection<WordModel>();
            foreach(var pair in source.Words)
            {
                this.Words.Add(new WordModel()
                {
                    Word1 = pair.Word1,
                    Word2 = pair.Word2,
                    Correct = pair.Correct,
                    Total = pair.Total,
                });
            }
        }
        public WordSetModel(ref WordSetModel parent, List<WordSetModel> wordSetModels,string name)
        {
            Name = name;
            ParentWordSet = parent;
            childWordSets = new ObservableCollection<WordSetModel>(wordSetModels);
        }
        
        

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}