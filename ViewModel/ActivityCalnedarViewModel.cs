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
    public class ActivityCalnedarViewModel : INotifyPropertyChanged
    {
        int currentYear { get; set; }
        List<int> yearList { get; set; }
        List<ActivityDay> AllDays { get; set; }
        ObservableCollection<ActivityDay> currentDays { get; set; }
        public List<int> YearList
        {
            get
            {
                return yearList;
            }
            set
            {
                if(value!=yearList)
                {
                    yearList = value;
                    RaisePropertyChanged("YearList");
                }
            }
        }
        public int CurrentYear
        {
            get
            {
                return currentYear;
            }
            set
            {
                if(currentYear != value)
                {
                    currentYear = value;
                    RaisePropertyChanged("CurrentYear");
                    RaisePropertyChanged("PreviousEnabled");
                    RaisePropertyChanged("NextEnabled");
                }
            }
        }
        public ObservableCollection<ActivityDay> CurrentDays
        { 
            get
            {
                return currentDays;
            }
            set
            {
                if(currentDays!=value)
                {
                    currentDays = value;
                    RaisePropertyChanged("CurrentDays");
                    RaisePropertyChanged("StartDate");
                    RaisePropertyChanged("EndDate");
                }
            }
        }
        public string StartDate
        {
            get
            {
                return CurrentDays.First().ActivityDate.ToString("dd.MM.yyyy");
            }
        }

        public string EndDate
        {
            get
            {
                return CurrentDays.Last().ActivityDate.ToString("dd.MM.yyyy");
            }
        }
        public bool PreviousEnabled
        {
            get
            {
                if (YearList.IndexOf(CurrentYear) == 0)
                    return false;
                else
                    return true;
            }
        }
        public bool NextEnabled
        {
            get
            {
                if (YearList.IndexOf(CurrentYear) == (YearList.Count - 1))
                    return false;
                else
                    return true;
            }
        }

        public CommandBase NextCommand { get; set; }
        public CommandBase PreviousCommand { get; set; }
        

        public ActivityCalnedarViewModel()
        {
            CurrentYear = DateTime.Now.Year;
            YearList = new List<int>();
            InitializeCalendar();
            NextCommand = new CommandBase(NextYear);
            PreviousCommand = new CommandBase(PreviousYear);
        }
        private void NextYear()
        {
            var yearIndex = YearList.IndexOf(CurrentYear);
            if (yearIndex == (YearList.Count-1)) return;
            var tmpYear = YearList[yearIndex + 1];
            if (YearList.Any(x => x == tmpYear) is false) return;
            
            CurrentDays = new ObservableCollection<ActivityDay>(AllDays.Where(x=>x.ActivityDate.Year==tmpYear));
            CurrentYear = tmpYear;
        }
        private void PreviousYear()
        {
            var yearIndex = YearList.IndexOf(CurrentYear);
            if (yearIndex == 0) return;
            var tmpYear = YearList[yearIndex - 1];
            if (YearList.Any(x => x == tmpYear) is false) return;
            CurrentDays = new ObservableCollection<ActivityDay>(AllDays.Where(x => x.ActivityDate.Year == tmpYear));
            CurrentYear = tmpYear;
        }
        private void InitializeCalendar()
        {
            AllDays = new List<ActivityDay>();
            var logFiles = System.IO.Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "LearnLog*");
            List<DateTime> dates = new List<DateTime>();
            foreach (var logFile in logFiles)
                dates.AddRange(System.IO.File.ReadAllLines(logFile).Where(x=>string.IsNullOrWhiteSpace(x) is false).Select(x=> DateTime.Parse(x.Split('\t').First()).Date).ToList());
            
            YearList = dates.Select(x => x.Year).Distinct().OrderBy(x => x).ToList();
            dates = dates.OrderBy(x => x).ToList();

            AllDays = new List<ActivityDay>();
            foreach (var year in YearList)
            {
                DateTime begin = new DateTime(year, 1, 1);
                DateTime end = new DateTime(year, 12, 31);

                for (DateTime date = begin; date <= end; date = date.AddDays(1))
                {
                    if (date.Day == 30) ;
                    AllDays.Add(new ActivityDay { ActivityDate = date });
                    var activityPerDay = dates.Where(x => x == AllDays.Last().ActivityDate.Date).Count();
                    AllDays.Last().TestCount = activityPerDay;
                    if (activityPerDay == 0)
                        AllDays.Last().ActivityLvl = ActivityLevel.None;
                    else if (activityPerDay >= 1 && activityPerDay < 3)
                        AllDays.Last().ActivityLvl = ActivityLevel.First;
                    else if (activityPerDay >= 3 && activityPerDay < 5)
                        AllDays.Last().ActivityLvl = ActivityLevel.Second;
                    else if (activityPerDay >= 5)
                        AllDays.Last().ActivityLvl = ActivityLevel.Third;
                    if (date.Date == DateTime.Now.Date)
                        AllDays.Last().ActivityLvl = ActivityLevel.Today;
                }
            }
            CurrentDays = new ObservableCollection<ActivityDay>(AllDays.Where(x => x.ActivityDate.Year == CurrentYear).ToList());
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
