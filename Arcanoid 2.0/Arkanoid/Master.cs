using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid
{
   
        class Master : Human
        {
            public Master(String FIO, int Year, int Staj)
                : base(FIO, Year)
            {
                this.Staj = Staj;
            }
            public int Staj;
            public override string ToString()
            {
                return base.ToString() + ", я здесь пашу учителем " + Staj.ToString() + " лет.";
            }


        }
    
}
