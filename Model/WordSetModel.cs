﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LearningWords.Model
{
    public class WordSetModel : INotifyPropertyChanged
    {
        string name { get; set; }
        string groupName { get; set; }
        int exercises { get; set; }
        int tests { get; set; }
        bool isTemporary { get; set; }
        ObservableCollection<WordModel> words { get; set; }
        ObservableCollection<WordSetModel> childWordSets { get; set; }
        DateTime lastUse { get; set; }
        bool isGroup { get; set; }
        
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
                    RaisePropertyChanged("WordsCount");
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
                    RaisePropertyChanged("LastUseText");
                }
            }
        }
        [XmlIgnore]
        public string LastUseText
        {
            get
            {
                if (IsGroup)
                {
                    return "Grupa";
                }
                else
                    return LastUse.ToString("yyyy-MM-dd HH:mm:ss");
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
                    RaisePropertyChanged("WordsCount");
                }
            }
        }
        public bool IsGroup
        {
            get
            {
                return isGroup;
            }
            set
            {
                if(value!= isGroup)
                {
                    isGroup = value;
                    RaisePropertyChanged("IsGroup");
                    RaisePropertyChanged("WordsCount");
                }
            }
        }
        public bool IsTemporary
        {
            get
            {
                return isTemporary;
            }
            set
            {
                if(isTemporary != value)
                {
                    isTemporary = value;
                    RaisePropertyChanged("IsTemporary");
                }
            }
        }
        public int WordsCount
        {
            get
            {                
                return (ChildWordSets?.Where(x=>x.IsTemporary is false)?.Sum(x => x.WordsCount) ?? 0) + (Words?.Count ?? 0);                
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
        public List<WordSetModel> GetChilds()
        {
            var tmp = new List<WordSetModel>();
            tmp.Add(new WordSetModel()
            {
                Name = this.Name,
                Words = this.Words
            });
            if(ChildWordSets!=null)
                foreach (var c in ChildWordSets)
                    tmp.AddRange(c.GetChilds());
            return tmp;
        }
        public int TodayComplete(DateTime today)
        {
            return (IsGroup ? 0 : (LastUse.Date == today.Date ? Words.Count : 0)) + (ChildWordSets!=null ? ChildWordSets.Sum(x=>x.TodayComplete(today)) : 0 );
        }
        public WordSetModel( List<WordSetModel> wordSetModels,string name)
        {
            Name = name;
            childWordSets = new ObservableCollection<WordSetModel>(wordSetModels);
        }
        public void RefreshWordsCount()
        {
            RaisePropertyChanged("WordsCount");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}