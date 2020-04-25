using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MatildaWinLib;
using System.IO;

namespace Arkanoid
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            LoadSettingsFromFile();

            speed_bonuses = start_speed;
        }

        int labels_bottom_height;
        int speed_bonuses;

        // доски, патроны, шары, всего понемногу
        int bullet_count = 0;
        SuperImage desk;
        List<SuperBall> balls = new List<SuperBall>();
        List<SuperImage> fireballs_pack = new List<SuperImage>();
        List<SuperImage> bullets = new List<SuperImage>();

        bool first_start = true;

        //Создание доски с нуля, по точкам
        void DeskOnPaint(SuperImage s, SuperImagePainter p)
        {
            if (bullet_count > 0)
            {
                p.FillPolygon(desk_points, Color.Pink);
                p.DrawString(bullet_count.ToString(), 6, s.Width / 2 - 4, 2, Color.Black);
            }
            else
            {
                p.FillPolygon(desk_points, Color.Yellow);
            }

            p.DrawPolygon(desk_points, Color.Black, 2);
        }


        const int desk_width = 130;
        const int desk_ex_areas = desk_width / 100 *15;
        const int desk_heigth = 13;
        List<Point> desk_points = new List<Point> {
                new Point(0, desk_heigth / 2),
                new Point(desk_heigth / 2, 0),
                new Point(desk_width - desk_heigth / 2, 0),
                new Point(desk_width, desk_heigth / 2),
                new Point(desk_width - desk_heigth / 2, desk_heigth),
                new Point(desk_heigth / 2, desk_heigth)
            };


        void CreateBreacks()
        {
            int[,] x = {
                // 4
                { 90, 50, 25, 60 },
                { 90, 110, 60, 25 },
                { 150, 50, 25, 160 },
                
                // С
                { 260, 50, 25, 160 },
                { 285, 50, 60, 25 },
                { 285, 185, 60, 25 },

                // Е
                { 375, 50, 25, 160 },
                { 400, 50, 60, 25 },
                { 400, 115, 60, 25 },
                { 400, 185, 60, 25 },

                // З
                { 490, 50, 60, 25 },
                { 490, 115, 60, 25 },
                { 490, 185, 60, 25 },
                { 550, 50, 25, 160 },
                
                // О
                { 605, 50, 25, 135 },
                { 630, 50, 60, 25 },
                { 605, 185, 60, 25 },
                { 665, 75, 25, 135 }, 

                // Н
                { 720, 50, 25, 160 },
                { 745, 115, 35, 25 },
                { 780, 50, 25, 160 }
            };

            for (int i = 0; i < x.GetLength(0); i++)
            {
                SuperBrick siBrick = new SuperBrick(matilda1, null, x[i, 0], x[i, 1], x[i, 2], x[i, 3], "Кирпич");
                siBrick.OnPaint += SiBrick_OnPaint;

                if (matilda1.RandomInt(1, 10) == 1)
                {
                    siBrick.include_fireballpack = true;
                }
                else
                if (matilda1.RandomInt(1, 10) == 1)
                {
                    siBrick.multi_live = true;
                }

                bricks.Add(siBrick);
            }
        }

        //const int brick_width = 70;
        //const int brick_heigth = 30;
        private void Form1_Load(object sender, EventArgs e)
        {
            CreateBreacks();

            desk = matilda1.CreateSuperImage(
                null, 
                ClientRectangle.Width / 2 - desk_width - 2,
                GetDeskTop(), 
                desk_width, 
                desk_heigth, 
                "доска");
            desk.OnPaint += DeskOnPaint;


            labels_bottom_height = ClientRectangle.Height - label1.Top;
        }

        int GetDeskTop()
        {
            return ClientRectangle.Height - 40;
        }

        // Объявление поля (без картошки) в классе
        List<SuperBrick> bricks = new List<SuperBrick>();                

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Запускаем цикл с перемещением шариков
            for (int i = balls.Count - 1; i >= 0; i--)
            {
                SuperBall ball = balls[i];

                ball.Top += ball.top_step;

                if (ball.Top <= menuStrip1.Height)
                {
                    ball.top_step= -ball.top_step; 
                }

                if (ball.Top > ClientRectangle.Height - ball.Height + 5) // на 5px увеличил, так как если долго играть, то шаг станет очень большим, и при условии отбивания о дощечку все равно шарик может вылететь за границы экрана
                {
                    ball.Dispose();
                    balls.RemoveAt(i);

                    if (balls.Count <= 0)
                    {
                        StopGame("СТОП ИГРА", Color.Black);

                        break;
                    }
                }

                ball.Left += ball.left_step;

                if (ball.Left <= 0)
                {
                    ball.left_step = -ball.left_step;
                }

                if (ball.Left >= ClientRectangle.Width - ball.Width)
                {
                    ball.left_step = -ball.left_step;
                }
            }


            for (int i = fireballs_pack.Count - 1; i >= 0; i--)
            {
                fireballs_pack[i].Top += speed_bonuses;

                if (fireballs_pack[i].Top >= ClientRectangle.Height - fireballs_pack[i].Height)
                {
                    fireballs_pack[i].Dispose();
                    fireballs_pack.RemoveAt(i);
                }
            }

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Top -= speed_bonuses;

                if (bullets[i].Top <= menuStrip1.Height)
                {
                    bullets[i].Dispose();
                    bullets.RemoveAt(i);
                }
            }
        }
       
        //Доска привязанна к мышке
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            desk.Left = e.X;
        }

        // создание самого faire ball^а по клику мышки
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (bullet_count > 0)
            {
                SuperImage fball = matilda1.CreateSuperImage("3_fireball.png", desk.Left + desk.Width / 2 - 30 / 2, desk.Top - 30, 30, 30, "fire!");
                bullets.Add(fball);
                bullet_count--;
            }
        }


        private void matilda1_OnConnection(SuperImage a, SuperImage b)
        {
            //Отбиваемся от шариков доской
            if (a.Name== "Шарик" && b.Name == "доска" )
            {
                BallAndDeskContact((SuperBall)a);
            }

            if (a.Name == "доска" && b.Name == "Шарик")
            {
                BallAndDeskContact((SuperBall)b);
            }


            // получаем снаряды
            if (a.Name == "firepack" && b.Name == "доска")
            {
                a.Dispose();
                fireballs_pack.Remove(a);

                bullet_count += 3;
            }
            if (a.Name == "доска" && b.Name == "firepack")
            {
                b.Dispose();
                fireballs_pack.Remove(b);
                bullet_count += 3;
            }
            
            
            // попадание снарядами
            if (a.Name == "Кирпич" && b.Name == "fire!")
            {
                b.Dispose();
                bullets.Remove(b);

                BrickContact((SuperBrick)a);

                CheckVictory();
            }

            if (a.Name == "fire!" && b.Name == "Кирпич")
            {
                a.Dispose();
                bullets.Remove(a);

                BrickContact((SuperBrick)b);

                CheckVictory();
            }


            if (a.Name == "Кирпич" && b.Name == "Шарик")
            {
                SuperBall Ball = (SuperBall)b;
                Ball.top_step *= -1;

                BrickContact((SuperBrick)a);

                CheckVictory();
            }

            if (a.Name == "Шарик" && b.Name == "Кирпич")
            {
                SuperBall Ball = (SuperBall)a;
                Ball.top_step *= -1;

                BrickContact((SuperBrick)b);

                CheckVictory();
            }

        }



        void CheckVictory()
        {
            if (bricks.Count == 0)
            {
                StopGame("ПОБЕДА", Color.Red);
            }
        }

        //Подготавливаемся к изменению размера окна
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (desk != null)
            {
                desk.Top = GetDeskTop();
            }

            label1.Top = label2.Top = label3.Top = label4.Top = ClientRectangle.Height - labels_bottom_height;
        }

        int iSecond;
        private void timer2_Tick(object sender, EventArgs e)
        {
            // Выводим секунды и уровень
            iSecond++;
            label1.Text = iSecond.ToString();
        }


        const int level_step = 3;

        int iLevel;
        private void tLevel_Tick(object sender, EventArgs e)
        {
            iLevel++;
            label4.Text = iLevel.ToString();

            foreach (SuperBall ball in balls)
            {
                if (ball.top_step > 0)
                {
                    ball.top_step += level_step;
                }
                else
                {
                    ball.top_step -= level_step;
                }
            }
        }

        const int size_ball = 30;
        int start_speed = 7;

        //Создание шариков при старте
        private void стартToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (timer.Enabled)
            {
                return;
            }

            if (first_start)
            {
                first_start = false;
            }
            else
            {
                foreach (SuperBrick a in bricks)
                {
                    a.Dispose();
                }
                bricks.Clear();

                CreateBreacks();
            }


            for (int i = 0; i < count_balls; i++)
            { 
                SuperBall ball = new SuperBall(
                    matilda1, 
                    "4_ball.png", 
                    ClientRectangle.Width / 2 - size_ball / 2, 
                    ClientRectangle.Height / 2 - size_ball / 2, 
                    size_ball, 
                    size_ball, 
                    "Шарик");

                int offset = matilda1.RandomInt(1, start_speed);
                int route = matilda1.RandomInt(0, 1);
                if (route == 0)
                {
                    ball.left_step = offset;
                    ball.top_step = start_speed * 2 - offset;
                }
                else
                {
                    ball.top_step = offset;
                    ball.left_step = start_speed * 2 - offset;
                }
                /*
                ball.left_step = matilda1.RandomInt(-27, 27);
                ball.top_step = matilda1.RandomInt(-27, 27);*/
                balls.Add(ball);
            }

            iSecond = 0;
            label1.Text = iSecond.ToString();
            iLevel = 1;
            label4.Text = iLevel.ToString();

            tSecond.Start();
            tLevel.Start();

            lStopGame.Hide();

            timer.Start();
        }

        int count_balls = 3;
        //Работаем с настройками
        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fOptions options = new fOptions();
            options.SetCurrentValues(start_speed, count_balls);
            options.ShowDialog();
            if (options.ok == true)
            {
                start_speed = options.GeStartSpeed();
                count_balls = options.GetCountBalls();

                String settings_file_path = GetSettingsFilePath();
                File.WriteAllText(
                    settings_file_path, 
                    String.Join(";", start_speed.ToString(), count_balls.ToString())
                    );

                speed_bonuses = start_speed;
            }
        }

        const String settings_file_name = "settings.txt";
        String GetSettingsFilePath()
        {
            return Application.StartupPath + "\\" + settings_file_name;
        }


        void LoadSettingsFromFile()
        {
            String settings_file_path = GetSettingsFilePath();
            if (File.Exists(settings_file_path))
            {
                String[] str_values = File.ReadAllText(settings_file_path).Split(';');
                start_speed = int.Parse(str_values[0]);
                count_balls = int.Parse(str_values[1]);
            }
        }


        void StopGame(String text_result, Color color_result)
        {
            foreach(SuperImage a in balls)
            {
                a.Dispose();
            }
            balls.Clear();

            foreach (SuperImage a in bullets)
            {
                a.Dispose();
            }
            bullets.Clear();

            foreach (SuperImage a in fireballs_pack)
            {
                a.Dispose();
            }
            fireballs_pack.Clear();

            tSecond.Stop();
            tLevel.Stop();

            lStopGame.Show();
            lStopGame.Text = text_result;

            lStopGame.ForeColor = color_result;
            timer.Stop();

            bullet_count = 0;
        }

        // Цветом зарисовываем кирпич, да с рамкой
        private void SiBrick_OnPaint(SuperImage s, SuperImagePainter p)
        {
            SuperBrick brick = (SuperBrick)s;
            if (brick.include_fireballpack)
            {
                p.FillRectangle(0, 0, s.Width, s.Height, Color.Red);
            }
            else if(brick.multi_live)
            {
                p.FillRectangle(0, 0, s.Width, s.Height, Color.Gray);

                p.FillRectangle(6, 6, 12, 16, Color.White);

                p.DrawString(brick.count_live.ToString(), 10, 6, 6, Color.Black);
            }
            else
            {
                p.FillRectangle(0, 0, s.Width, s.Height, Color.LightBlue);
            }

            p.DrawRectangle(0, 0, s.Width, s.Height, Color.Black, 1);
        }

        void BrickContact(SuperBrick brick)
        {
            if (brick.include_fireballpack)
            {
                SuperBall pack = new SuperBall(matilda1, "3_fireballs_pack.png", brick.Left + brick.Width / 2 - 25 / 2, brick.Top, 25, 25, "firepack");
                fireballs_pack.Add(pack);
            }

            if (brick.multi_live)
            {
                brick.count_live--;
                if (brick.count_live > 0)
                {
                    return;
                }
            }

            brick.Dispose();
            bricks.Remove(brick);
        }



        void BallAndDeskContact(SuperBall ball)
        {
            ball.top_step = -ball.top_step;

            if (ball.Left - desk.Left < desk_ex_areas)
            {
                ball.left_step -= 3;
            }
            else
            if (ball.Left >= desk.Left + desk.Width - desk_ex_areas)
            {
                ball.left_step += 3;
            }
        }
    }
}