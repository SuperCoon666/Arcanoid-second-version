using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatildaWinLib;

namespace Arkanoid
{
    public class SuperBall : SuperImage
    {
        public SuperBall(Matilda matilda, string path_img, int left, int top, int width, int height, string name)
            :base (matilda, path_img, left, top, width, height, name)
        {

             
        }
 
        public int left_step;
        public int top_step;
    }
}
