using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace View
{
    /// <summary>
    /// Alacsony szintű képmegjelenítő osztály
    /// </summary>
    public class ImageControl : Control
    {

        #region Fields
        internal Color4? BackgroundColor;
        #endregion

        #region Methods
        /// <summary>
        /// Vezérlőhöz tartozó textúra lekérdezése.
        /// </summary>
        /// <returns>textúra mely a vezérlő kirajzolásához kell</returns>
        internal override Texture GetTexture()
        {
            return VisualData.GetTexture(Visualization);
        }
        /// <summary>
        /// Vezérlő típusát lekérdező függvény.
        /// </summary>
        /// <returns>vezérlő típusa</returns>
        internal override ControlType GetControlType()
        {
            return ControlType.Image;
        }
        /// <summary>
        /// Szín megjelenítéséhez szín megadás.
        /// </summary>
        /// <param name="color">szín a kirajzoláshoz</param>
        public void SetColor(Color4 color)
        {
            DrawType = DrawType.Color;
            ImageName = "";
            BackgroundColor = color;
        }
        /// <summary>
        /// Kép megjelenítéséhez képnév megadás.
        /// </summary>
        /// <param name="image">kép a kirajzoláshoz</param>
        public void SetImage(string image)
        {
            DrawType = DrawType.Image;
            ImageName = image;
            Visualization = VisualData.GetImageId(image);
        }
        #endregion
    }
    /// <summary>
    /// Alacsony szintű animált képosztály
    /// </summary>
    public class AnimatedImageControl : Control
    {
        #region Fields
        internal List<string> ImageNames = new List<string>();
        internal List<Color4> BackgroundColors = new List<Color4>();
        internal List<int> ActiveTimes = new List<int>();

        internal int CurrentDraw = 0;
        internal int CurrTime = 0;

        internal Color4? BackgroundColor;
        #endregion

        #region Contructor
        /// <summary>
        /// Animált képoszály létrehozása
        /// </summary>
        public AnimatedImageControl()
        {
            VisualData.AnimationStepper += StepAnimationTime;
        }
        /// <summary>
        /// Animált képoszály törlése, leiratkozás a animációs időzítőről
        /// </summary>
        ~AnimatedImageControl()
        {
            VisualData.AnimationStepper -= StepAnimationTime;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Animációs léptetés
        /// </summary>
        /// <param name="sender">esemény küldője</param>
        /// <param name="e">eseményparaméter</param>
        private void StepAnimationTime(object sender, EventArgs e)
        {
            if (!IsVisible || (ActiveTimes.Count == 0))
                return;
            if (CurrTime == ActiveTimes[CurrentDraw])
            {
                CurrTime = 0;
                CurrentDraw++;
                if (CurrentDraw == ActiveTimes.Count)
                    CurrentDraw = 0;
            }
            CurrTime++;

        }
        /// <summary>
        /// Jelenlegi szín lekérdezése.
        /// </summary>
        /// <returns>kép aktuális színe</returns>
        internal Color4 GetCurrentColor()
        {
            return BackgroundColors[CurrentDraw];
        }

        /// <summary>
        /// Jelenlegi textúra lekérezése.
        /// </summary>
        /// <returns>vezérlő aktuális textúrája</returns>
        internal override Texture GetTexture()
        {
            return VisualData.GetTexture(VisualData.GetImageId(ImageNames[CurrentDraw]));
        }

        /// <summary>
        /// Vezérlő típusát visszaadó függvény.
        /// </summary>
        /// <returns>vezérlő típusa</returns>
        internal override ControlType GetControlType()
        {
            return ControlType.AnimatedImage;
        }
        /// <summary>
        /// Szín időzítéssel párosított lista megadása.
        /// </summary>
        /// <param name="colors">Váltakozó színek listája</param>
        /// <param name="times">Időzítési lista</param>
        /// <exception cref="ArgumentException">colors és times listának ugyanolyan hosszúnak kell lennie</exception>
        public void SetColors(List<Color4> colors, List<int> times)
        {
            if (colors.Count != times.Count)
                throw new ArgumentException($"{colors.Count} != {times.Count}: Colors and times count are not equal.");
            DrawType = DrawType.Color;
            ImageName = "";

            BackgroundColor = colors[0];

            BackgroundColors = colors;
            ActiveTimes = times;

        }
        /// <summary>
        /// Kép időzítéssel párosított lista megadása.
        /// </summary>
        /// <param name="images">Váltakozó képek listája</param>
        /// <param name="times">Időzítési lista</param>
        /// <exception cref="ArgumentException">images és times listának ugyanolyan hosszúnak kell lennie</exception>
        public void SetImages(List<string> images, List<int> times)
        {
            if (images.Count != times.Count)
                throw new ArgumentException($"{images.Count} != {times.Count}: Images and times count are not equal.");
            DrawType = DrawType.Image;
            ActiveTimes = times;
            ImageNames = images;
            ImageName = images[0];
            Visualization = VisualData.GetImageId(ImageName);
        }
        #endregion

    }
}

