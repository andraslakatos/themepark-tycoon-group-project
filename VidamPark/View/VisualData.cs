using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace View
{
    internal enum ShaderPosition
    {
        StaticImage = 0,
        RelativeToCamera = 1,
    }
    internal static class VisualData
    {

        #region Fields
        private static List<Texture> _textures = new List<Texture>();
        private static List<Shader> _shaders = new List<Shader>();

        private static Dictionary<string, int> _imageNames = new Dictionary<string, int>();
        #endregion


        #region Methods
        public static void LoadTextures(string path = "Textures")
        {
            Shader shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            shader.SetInt("isColor", 0);
            _shaders.Add(shader);

            Shader shaderCamera = new Shader("Shaders/shaderCamera.vert", "Shaders/shader.frag");
            shaderCamera.SetInt("isColor", 0);
            _shaders.Add(shaderCamera);

            foreach (var location in Directory.GetFiles(path))
            {
                string fileName = Path.GetFileNameWithoutExtension(location);
                var bmp = new Bitmap(location);




                _imageNames[Path.GetFileName(fileName)] = _textures.Count;
                var bmptex = new Texture(bmp);
                _textures.Add(bmptex);
            }
        }

        public static void UseTexture(string imageName)
        {
            if (!_imageNames.Keys.Contains(imageName))
                _textures[_imageNames["NoTexture"]].Use();
            else
                _textures[_imageNames[imageName]].Use();
        }

        public static int GetImageId(string image)
        {
            if (!_imageNames.Keys.Contains(image))
                return _imageNames["NoTexture"];
            else
                return _imageNames[image];
        }


        public static Shader GetShader(ShaderPosition id)
        {
            return _shaders[(int)id];
        }

        public static Texture GetTexture(int id)
        {
            if (id > _textures.Count || id < 0)
                return _textures[GetImageId("NoTexture")];
            else
                return _textures[id];
        }
        #endregion

        #region EvenHandlers
        internal static EventHandler? AnimationStepper;
        #endregion
        



        
    }

}
