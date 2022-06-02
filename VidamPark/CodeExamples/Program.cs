using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Drawing;
using View;

namespace CodeExamples
{
    internal class Program
    {
        static MainWindow game;
        static void Main(string[] args)
        {
            GameWindowSettings gws = new GameWindowSettings();
            NativeWindowSettings nws = new NativeWindowSettings();

            nws.Size = new OpenTK.Mathematics.Vector2i(1280, 720);
            //nws.WindowState = WindowState.Fullscreen;

            nws.Title = "VidámPark";
            
            game = new MainWindow(gws,nws);
            game.VSync = VSyncMode.On;


            ImageControl menualap = new ImageControl()
            {
                position = new Point(0,0),
                size = new Size(360,860),
                horizontalOrientation = HorizontalOrientation.CENTER,
                verticalOrientation = VerticalOrientation.CENTER,
            };
            menualap.setColor(new Color4(85,125, 185, 255));
            //menualap.setImage("macska");
            game.child = menualap;

            Button btn = new Button()
            {
                position = new Point(0, 0),
                size = new Size(340, 120),
                horizontalOrientation = HorizontalOrientation.CENTER,
                verticalOrientation = VerticalOrientation.CENTER,
                brush = new SolidBrush(Color.Red),
            };

            btn.setImage("macska");
            btn.Text = "Új játék";

            menualap.children.Add(btn);

            game.Run();
        }



        static void orientations()
        {
            Control Menualap = newControl(0, 0, 1920, 1080, "macska");
            Control macska = newControl(100, 100, 1000, 1000, "macska");



            foreach (VerticalOrientation vo in Enum.GetValues(typeof(VerticalOrientation)))
            {
                foreach (HorizontalOrientation ho in Enum.GetValues(typeof(HorizontalOrientation)))
                {
                    //if (vo != VerticalOrientation.CENTER || ho != HorizontalOrientation.CENTER)
                    //    continue;
                    Control vizx = newControl(0, 0, 100, 100, "water");
                    vizx.verticalOrientation = vo;
                    vizx.horizontalOrientation = ho;
                    Menualap.children.Add(vizx);
                }
            }

            game.child = Menualap;

            Control shiba1 = newControl(-100, 100, 500, 200, "shiba");
            shiba1.verticalOrientation = VerticalOrientation.CENTER;
            shiba1.horizontalOrientation = HorizontalOrientation.RIGHT;

            Menualap.children.Add(macska);
            macska.children.Add(shiba1);

            Control shiba2 = newControl(225, 0, 50, 50, "grass");
            shiba2.verticalOrientation = VerticalOrientation.BOTTOM;
            shiba2.horizontalOrientation = HorizontalOrientation.CENTER;

            shiba1.children.Add(shiba2);
        }

        static Control newControl(int x, int y, int width, int height, string nev)
        {

            ImageControl macska = new ImageControl()
            {
                size = new Size(width, height),
                position = new Point(x, y),
            };
            macska.onClick += ImageClicked;
            macska.setImage(nev);

            return macska;
        }



        #region kattintásMinta

        static void ImageClicked(object sender, EventArgs e)
        {
            Control c = (Control)sender;
            Console.WriteLine(c.imageName);
            Console.WriteLine(c.actPosition);
            c.position = new Point(c.position.X + 10, c.position.Y);
        }

        #endregion
    }
}
