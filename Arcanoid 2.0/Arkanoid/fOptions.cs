using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    public partial class fOptions : Form
    {
        public bool ok = false;

        public fOptions()
        {
            InitializeComponent();

        }

        public void SetCurrentValues(int start_speed, int count_balls)
        {
            numericUpDown1.Value = count_balls;
            numericUpDown2.Value = start_speed;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            ok = true;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }


        public int GetCountBalls()
        {
            int a = Convert.ToInt32(numericUpDown1.Value);
            return a;
        }

        public int GeStartSpeed()
        {
            return Convert.ToInt32(numericUpDown2.Value);
        }
    }
}
