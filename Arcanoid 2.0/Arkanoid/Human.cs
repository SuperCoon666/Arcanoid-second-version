using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid
{
   public class Human
    {
        public Human (String FIO, int Year)
        {
            this.FIO = FIO;
            this.Year = Year;
        }
        public String FIO;
        public int Year;
        public override string ToString()
        {
            return "Я " +FIO + ", мне " + Year.ToString();
        }
    }
}
