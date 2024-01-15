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
            var lines = System.IO.File.ReadAllLines("LearnLog.txt").Select(x=> DateTime.Parse(x.Split('\t').First()).Date).ToList();
            // Inicjalizacja kalendarza (możesz dostosować do własnych potrzeb)
            CalendarDays = new ObservableCollection<ActivityDay>();
            for (int i = 1; i <= 31; i++)
            {
                CalendarDays.Add(new ActivityDay { Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, i) });
                if (lines.Contains(CalendarDays.Last().Date.Date)) CalendarDays.Last().IsActive = true;
            }
        }
    }
}
