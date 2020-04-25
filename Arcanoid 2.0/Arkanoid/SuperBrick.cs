using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatildaWinLib;

namespace Arkanoid
{
    public class SuperBrick : SuperImage
    {
        public SuperBrick(Matilda matilda, string path_img, int left, int top, int width, int height, string name)
            :base (matilda, path_img, left, top, width, height, name)
        {

            
        }

        public int count_live = 3;
        public bool multi_live = false;

        public bool include_fireballpack = false;
    }
}
