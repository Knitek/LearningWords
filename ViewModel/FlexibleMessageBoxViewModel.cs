using LearningWords.Controls;
using LearningWords.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LearningWords.ViewModel
{
    public class FlexibleMessageBoxViewModel : INotifyPropertyChanged
    {       
        string content { get; set; }
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                if(content != value)
                {
                    content = value;
                    RaisePropertyChanged("Content");
                }
            }
        }
        public Action CloseAction { get; set; }
        public CommandBase OKCommand { get; set; }        

        public FlexibleMessageBoxViewModel()
        {
            OKCommand = new CommandBase(Close);
        }
        
        void Close()
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
