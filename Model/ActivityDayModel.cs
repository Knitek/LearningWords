using System;
using System.Windows.Media;
using System.Windows.Navigation;

namespace LearningWords.Model
{
    public class ActivityDay
    {
        public DateTime ActivityDate { get; set; }
        public ActivityLevel ActivityLvl { get; set; } = ActivityLevel.None;
        public int TestCount { get; set; }
        public string Title
        {
            get
            {
                return ActivityDate.ToString("d") + " (" + TestCount + ")";
            }
        }
        public Color DayColor
        {
            get
            {
                switch (ActivityLvl)
                {
                    case ActivityLevel.None:
                        return Colors.Transparent;
                    case ActivityLevel.First:
                        return Colors.GreenYellow;
                    case ActivityLevel.Second:
                        return Colors.Green;
                    case ActivityLevel.Third:
                        return Colors.DarkGreen;
                    case ActivityLevel.Today:
                        return Colors.Red;
                    default:
                        return Colors.Transparent;
                }
            }
        }
    }
    //return IsActive is false ? Colors.Transparent : Colors.Green;
    public enum ActivityLevel
    {
        None = 0,
        First = 1,
        Second = 2,
        Third = 3,
        Today = 4,
    }
}
