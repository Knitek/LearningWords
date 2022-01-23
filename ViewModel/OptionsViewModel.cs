using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningWords.Model;
using LearningWords.Controls;
using System.Speech.Synthesis;
using System.Windows;
using ToolsLib;

namespace LearningWords.ViewModel
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        bool showPreview { get; set; }
        bool showStatistics { get; set; }
        bool hideFirst { get; set; }
        bool hideSecond { get; set; }
        bool allowHints { get; set; }
        bool specialCharactersMode { get; set; }
        bool showNewVersionInfo { get; set; }
        bool showNewVersionInfoVisibility { get; set; }

        public bool ShowPreview
        {
            get
            {
                return this.showPreview;
            }
            set
            {
                if(value!=this.showPreview)
                {
                    this.showPreview = value;
                    RaisePropertyChanged("ShowPreview");
                }
            }
        }
        public bool ShowStatistics
        {
            get
            {
                return this.showStatistics;
            }
            set
            {
                if(value!=this.showStatistics)
                {
                    this.showStatistics = value;
                    RaisePropertyChanged("ShowStatistics");
                }
            }
        }
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
                }
            }
        }
        public bool AllowHints
        {
            get
            {
                return this.allowHints;
            }
            set
            {
                if(value!=this.allowHints)
                {
                    this.allowHints = value;
                    RaisePropertyChanged("AllowHints");
                }
            }
        }
        public bool SpecialCharactersMode
        {
            get
            {
                return this.specialCharactersMode;
            }
            set
            {
                if(value != this.specialCharactersMode)
                {
                    this.specialCharactersMode = value;
                    RaisePropertyChanged("SpecialCharactersMode");
                }
            }
        }
        public bool ShowNewVersionInfo
        {
            get
            {
                return this.showNewVersionInfo;
            }
            set
            {
                if(value!= this.showNewVersionInfo)
                {
                    this.showNewVersionInfo = value;
                    RaisePropertyChanged("ShowNewVersionInfo");
                }
            }
        }
        public bool ShowNewVersionInfoVisibility
        {
            get
            {
                return this.showNewVersionInfoVisibility;
            }
            set
            {
                if(value != this.showNewVersionInfoVisibility)
                {
                    this.showNewVersionInfoVisibility = value;
                    RaisePropertyChanged("ShowNewVersionInfoVisibility");
                }
            }
        }


        public Action CloseAction { get; set; }

        public CommandBase SaveCommand { get; set; }
        public CommandBase CancelCommand { get; set; }
        
        public OptionsViewModel()
        {
            ShowPreview = bool.Parse(Tools.ReadAppSetting("ShowPreview", "true"));
            HideFirst = bool.Parse(Tools.ReadAppSetting("HideFirstPrevievCollumn", "false"));
            HideSecond = bool.Parse(Tools.ReadAppSetting("HideSecondPrevievCollumn", "false"));
            ShowStatistics = bool.Parse(Tools.ReadAppSetting("ShowStatistics", "true"));
            SpecialCharactersMode = bool.Parse(Tools.ReadAppSetting("SpecialCharactersMode", "false"));
            AllowHints = bool.Parse(Tools.ReadAppSetting("AllowHints", "true"));
            ShowNewVersionInfo = bool.Parse(Tools.ReadAppSetting("ShowUpdateNote", "true"));
            if (Tools.ReadAppSetting("NewVersionAvailable", "false") == "false")
                ShowNewVersionInfoVisibility = false;
            else
                ShowNewVersionInfoVisibility = true;
            SaveCommand = new CommandBase(Save);
            CancelCommand = new CommandBase(Cancel);
           
        }

        private void Save()
        {
            Tools.WriteAppSetting("ShowPreview", ShowPreview.ToString().ToLower());
            Tools.WriteAppSetting("HideFirstPrevievCollumn", HideFirst.ToString().ToLower());
            Tools.WriteAppSetting("HideSecondPrevievCollumn", HideSecond.ToString().ToLower());
            Tools.WriteAppSetting("ShowStatistics", ShowStatistics.ToString().ToLower());
            Tools.WriteAppSetting("SpecialCharactersMode", SpecialCharactersMode.ToString().ToLower());
            Tools.WriteAppSetting("AllowHints", AllowHints.ToString().ToLower());
            Tools.WriteAppSetting("ShowUpdateNote", showNewVersionInfo.ToString().ToLower());
            CloseAction.Invoke();
        }
        private void Cancel()
        {
            CloseAction.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
