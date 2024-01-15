using System;
using System.Windows.Media;
using System.Windows.Navigation;

namespace LearningWords.Model
{    
    public class ActivityDay
    {
        public DateTime Date { get; set; }
        public bool IsActive { get; set; } = false;
        public Color DayColor
        {
            get
            {
                return IsActive is false ? Colors.Transparent : Colors.Green;
            }
        }
    }
}
