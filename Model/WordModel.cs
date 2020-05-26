using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningWords.Model
{
    public class WordModel
    {
        public string Word1 { get; set; }
        public string Word2 { get; set; }
        public int Correct { get; set; }
        public int Total { get; set; }

        public string GetWord(bool mode)
        {
            if (!mode)
                return Word1;
            else
                return Word2;
        }
        public string GetAnswer(bool mode)
        {
            if (mode)
                return Word1.Trim().ToLower();
            else
                return Word2.Trim().ToLower();
        }
    }

}
