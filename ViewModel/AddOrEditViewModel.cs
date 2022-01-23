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

namespace LearningWords.ViewModel
{
    public class AddOrEditViewModel : INotifyPropertyChanged
    {
        string name { get; set; }
        ObservableCollection<WordModel> words { get; set; }
        WordModel selectedWordModel { get; set; }
        public bool result = false;
        private SpeechSynthesizer speechSynthesizer { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                if(name!=value)
                {
                    name = value;
                    RaisePropertyChanged("Name");
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
        public WordModel SelectedWordModel
        {
            get { return selectedWordModel; }
            set
            {
                if(value!= selectedWordModel)
                {
                    selectedWordModel = value;
                    RaisePropertyChanged("SelectedWordModel");
                }
            }
        }
        public Action CloseAction { get; set; }

        public CommandBase SaveCommand { get; set; }
        public CommandBase CancelCommand { get; set; }
        public CommandBase ListenCommand { get; set; }

        public AddOrEditViewModel()
        {
            Words = new ObservableCollection<WordModel>();
            SaveCommand = new CommandBase(Save);
            CancelCommand = new CommandBase(Cancel);
            ListenCommand = new CommandBase(Listen);
            speechSynthesizer = new SpeechSynthesizer();
            try
            {
                speechSynthesizer.SelectVoice("Microsoft Hazel Desktop");
            }
            catch
            {
                var tmpVoices = speechSynthesizer.GetInstalledVoices();
//#if DEBUG
//                MessageBox.Show(string.Join(System.Environment.NewLine, tmpVoices.Select(x => x.VoiceInfo.Name)));
//#endif
                if(tmpVoices.Count>=1)
                    speechSynthesizer.SelectVoice(tmpVoices.FirstOrDefault().VoiceInfo.Name);
            }
        }

        private void Save()
        {
            words =  new ObservableCollection<WordModel>(words.ToList().Where(x => !(string.IsNullOrWhiteSpace(x.Word1) && string.IsNullOrWhiteSpace(x.Word2))));
            result = true;
            CloseAction.Invoke();
        }
        private void Cancel()
        {
            result = false;
            CloseAction.Invoke();
        }
        private void Listen()
        {
            if(SelectedWordModel!=null)
            {
                speechSynthesizer.Speak(SelectedWordModel.Word2);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
