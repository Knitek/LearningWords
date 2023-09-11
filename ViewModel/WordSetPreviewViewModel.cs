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
using System.Windows.Forms;

namespace LearningWords.ViewModel
{
    public class WordSetPreviewViewModel : INotifyPropertyChanged
    {
        WordSetModel wordSet { get; set; }
        bool hideFirst { get; set; }
        bool hideSecond { get; set; }

        public bool HideFirst
        {
            get
            {
                return this.hideFirst;
            }
            set
            {
                if(value!=this.hideFirst)
                {
                    this.hideFirst = value;
                    RaisePropertyChanged("HideFirst");
                    RaisePropertyChanged("FirstCollumnVisibility");
                }
            }
        }
        public bool HideSecond
        {
            get
            {
                return this.hideSecond;
            }
            set
            {
                if(value!=this.hideSecond)
                {
                    this.hideSecond = value;
                    RaisePropertyChanged("HideSecond");
                    RaisePropertyChanged("SecondCollumnVisibility");
                }
            }
        }
        public Visibility FirstCollumnVisibility
        {
            get
            {
                if (this.hideFirst)
                    return Visibility.Hidden;
                else
                    return Visibility.Visible;
            }
        }
        public Visibility SecondCollumnVisibility
        {
            get
            {
                if (this.hideSecond)
                    return Visibility.Hidden;
                else
                    return Visibility.Visible;
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
        public Action ExitAction { get; set; }

        public delegate void ChangeResult(bool result);
        public ChangeResult ChangeDialogResult;
        public CommandBase CloseCommand { get; set; }
        public WordSetPreviewViewModel( WordSetModel wordSetModel)
        {
            CloseCommand = new CommandBase(Close);
            HideFirst = bool.Parse(Tools.ReadAppSetting("HideFirstPrevievCollumn", "false"));
            HideSecond = bool.Parse(Tools.ReadAppSetting("HideSecondPrevievCollumn", "false"));
            this.wordSet = wordSetModel;
        }
        ~WordSetPreviewViewModel()
        {
            SaveHideStatus(null, null);
        }   
        public void SaveHideStatus(object sender, CancelEventArgs e)
        {
            Tools.WriteAppSetting("HideFirstPrevievCollumn", this.hideFirst.ToString().ToLower());
            Tools.WriteAppSetting("HideSecondPrevievCollumn", this.hideSecond.ToString().ToLower());
        }
        void Close()
        {
            ChangeDialogResult(true);
            ExitAction.Invoke();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
