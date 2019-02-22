using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);




        //Mouse actions
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const int MOUSEEVENTF_LEFTUP = 0x0004;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;
       
        public int x = 0;
        public int y = 0;
        List<Monster> mntsList = new List<Monster>();

        public Form1()
        {

            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            CreateMonsters();
            Bitmap battleIcon = (Bitmap)Bitmap.FromFile("C://Users//Gabriel Patatt//Desktop//BATTLEICON.jpg");
 
            InputSimulator s = new InputSimulator();
            
            Process cointainsProcess = Process.GetProcessesByName("client").FirstOrDefault();

            if (cointainsProcess != null)
            {
                IntPtr h = cointainsProcess.MainWindowHandle;

                SetForegroundWindow(h);
                printImageScreen();

                //verify if battle is opened
                if (verifyImage(ConvertToFormat(printImageScreen(), battleIcon.PixelFormat), battleIcon))
                {
                    //if determinado pixel dentro do battle estiver vermelho, não faz nada abaixo
                    do
                    {
                        if (haveMonster())
                        {
                            if (!isAttacking()) {
                                SetCursorPos(x + 10, y + 65);
                                Point p = new Point(x + 10, y + 65);
                                
                                

                                int position = ((y << 0x10) | (x & 0xFFFF));
                                PostMessage(FindWindow(null, cointainsProcess.MainWindowTitle, ), MOUSEEVENTF_RIGHTUP, new IntPtr(0), new IntPtr(0));
                                // Send the click message
                         
                                    Thread.Sleep(150);
                                    //start function input simulator package
                                    s.Mouse.RightButtonClick();
                                    Thread.Sleep(150);
                                    SetCursorPos(x + 25, y + 135);
                                    Thread.Sleep(300);
                                    s.Mouse.LeftButtonClick();
                                    Thread.Sleep(300);
                                    string monsterCopiedName = "";

                                    monsterCopiedName = Clipboard.GetText();
                                    if (MonstersToAttack(monsterCopiedName))
                                    {
                                        SetCursorPos(x + 10, y + 60);
                                        Thread.Sleep(150);
                                        s.Mouse.LeftButtonClick();
                                        SetCursorPos(x + 100, y + 100);
                                }
                            }
                        }
                    } while (true);
                }
            }
        }
        private bool haveMonster(){
            Thread.Sleep(500);
            Bitmap verifyBattle = printImageScreen();
            if (verifyBattle.GetPixel(x + 10, y + 60).R.Equals(73) && verifyBattle.GetPixel(x + 10, y + 60).G.Equals(73) && verifyBattle.GetPixel(x + 10, y + 60).B.Equals(73))
                return false;
            return true;
        }
        private bool isAttacking()
        {
            Rectangle crop = new Rectangle(x, y, 100, 200);

            var bmp = new Bitmap(crop.Width, crop.Height);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(printImageScreen(), new Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
            }

            if (bmp.GetPixel(24, 73).R.Equals(237) && bmp.GetPixel(24, 73).G.Equals(8) && bmp.GetPixel(24, 73).B.Equals(8))
                return true;
            return false;
        }
        private bool MonstersToAttack(string monsterName)
        {
            foreach (Monster monster in mntsList)
            {
                if(monsterName == monster.NameMonster)
                {
                    return true;
                }
            }
            return false;          
        }

        private void CreateMonsters()
        {
          
            Monster m = new Monster();
            m.NameMonster = "Spider";
            mntsList.Add(m);
            Monster m1 = new Monster();
            m1.NameMonster = "Poison Spider";
            mntsList.Add(m1);

        }
        
        private Bitmap printImageScreen()
        {



            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            
            Graphics gr = Graphics.FromImage(bmp);
            gr.CopyFromScreen(0, 0, 0, 0, bmp.Size);
            return bmp;
        }
        private Bitmap ConvertToFormat(System.Drawing.Image image, PixelFormat format)
        {
            Bitmap copy = new Bitmap(image.Width, image.Height, format);
            using (Graphics gr = Graphics.FromImage(copy))
            {
                gr.DrawImage(image, new Rectangle(0, 0, copy.Width, copy.Height));
            }
            return copy;
        }
        private bool verifyImage(Bitmap sourceImage, Bitmap template){            
            
            // create template matching algorithm's instance
            // (set similarity threshold to 92.1%)

            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.921f);
            // find all matchings with specified above similarity

            TemplateMatch[] matchings = tm.ProcessImage(sourceImage, template);
            // highlight found matchings

            if(matchings.Length == 0)
                return false;   
            
            BitmapData data = sourceImage.LockBits(
            new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
            ImageLockMode.ReadWrite, sourceImage.PixelFormat);
            foreach (TemplateMatch m in matchings)
            {
                Drawing.Rectangle(data, m.Rectangle, System.Drawing.Color.White);
                x = m.Rectangle.Location.X;
                y = m.Rectangle.Location.Y ;
            }
            // do something else with matching
            sourceImage.UnlockBits(data);
            



            return true;
        }

        private void Images()
        {
           
        }

    }
}
