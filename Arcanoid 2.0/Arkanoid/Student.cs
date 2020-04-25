using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid
{
    class Student: Human
    {
        public Student (String FIO, int Year, int Level)
            :base (FIO,Year)
        {
            this.Level= Level;
        }
        public int Level;
        public override string ToString()
        {
            return base.ToString() + ", я здесь учусь " +Level.ToString()+" лет.";
        }
       

    }
}
