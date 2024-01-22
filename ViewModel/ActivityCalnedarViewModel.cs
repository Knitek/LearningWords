using LearningWords.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LearningWords.ViewModel
{
    public class ActivityCalnedarViewModel
    {
        public ObservableCollection<ActivityDay> CalendarDays { get; set; }
        public DateTime StartDate
        {
            get
            {
                return CalendarDays.First().Date;
            }
        }

        public DateTime EndDate
        {
            get
            {
                return CalendarDays.Last().Date;
            }
        }
        

        public ActivityCalnedarViewModel()
        {
            InitializeCalendar();
            // Tutaj możesz również załadować dane z modelu
        }

        private void InitializeCalendar()
        {
            int year = 2024;
            var lines = System.IO.File.ReadAllLines("LearnLog.txt").Select(x=> DateTime.Parse(x.Split('\t').First()).Date).ToList();
            // dodać kontorlę błedów do parsowania daty, dodac sprawdzanie istnienia pliku i innych nazw zaczynajacych sie od LearnLog
            CalendarDays = new ObservableCollection<ActivityDay>();

            DateTime begin = new DateTime(year, 1, 1);
            DateTime end = new DateTime(year, 12, 31);

            for (DateTime date = begin; date <= end; date = date.AddDays(1))
            {
                CalendarDays.Add(new ActivityDay { Date = date });
                var activityPerDay = lines.Where(x => x == CalendarDays.Last().Date.Date).Count();
                if (activityPerDay == 0)
                    CalendarDays.Last().ActivityLvl = ActivityLevel.None;
                else if (activityPerDay == 1 && activityPerDay < 3)
                    CalendarDays.Last().ActivityLvl = ActivityLevel.First;
                else if (activityPerDay == 3 && activityPerDay < 5)
                    CalendarDays.Last().ActivityLvl = ActivityLevel.Second;
                else if (activityPerDay >= 5)
                    CalendarDays.Last().ActivityLvl = ActivityLevel.Third;
            }
        }
    }
}
