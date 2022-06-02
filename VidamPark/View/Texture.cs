using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;

using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace View
{
    internal class Texture
    {

        #region Constants
        public readonly int Handle;
        #endregion

        #region Fields
        public Size size;
        #endregion
        
        #region Contructor
        public Texture(Bitmap image)
        {
            size = new Size(image.Width, image.Height);
            // Új textúra létrehozása
            Handle = GL.GenTexture();
            Use();

            // a kép nyers formátumának létrehozása
            var data = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // kép hozzáadása a GPU memóriájához
            GL.TexImage2D(TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                image.Width,
                image.Height,
                0,
                PixelFormat.Bgra,
                PixelType.UnsignedByte,
                data.Scan0);

            image.UnlockBits(data);
            // egyébb képfeldolgozási paraméterek
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        #endregion

        #region Methods
        public void Delete(string reference = "Unknown")
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.DeleteTexture(Handle);
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
        #endregion


    }
}
