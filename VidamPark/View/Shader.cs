using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace View
{
    internal class Shader
    {

        #region Constants
        public readonly int Handle;
        private readonly Dictionary<string, int> _uniformLocations;
        #endregion

        #region Contructor
        /// <summary>
        /// Shader létrehozása fájlokból.
        /// </summary>
        /// <param name="vertPath">vertex shader relatív útvonala</param>
        /// <param name="fragPath">fragment shader relatív útvonala</param>
        public Shader(string vertPath, string fragPath)
        {
            var shaderSource = LoadSource(vertPath);
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, shaderSource);

            CompileShader(vertexShader);

            shaderSource = LoadSource(fragPath);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            CompileShader(fragmentShader);


            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);

            LinkProgram(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            _uniformLocations = new Dictionary<string, int>();

            for (var i = 0; i < numberOfUniforms; i++)
            {

                var key = GL.GetActiveUniform(Handle, i, out _, out _);
                var location = GL.GetUniformLocation(Handle, key);
                _uniformLocations.Add(key, location);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Shader lefordítása.
        /// </summary>
        /// <param name="shader">shader azonosítója</param>
        /// <exception cref="Exception">Hiba ha a shader nem fordítható le</exception>
        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        /// <summary>
        /// Shader összekötése az alkalmazással.
        /// </summary>
        /// <param name="program">shader amit össze akarunk kötni az alkalmazással</param>
        /// <exception cref="Exception">Hiba ha nem lehet összekötni az alkalmazást a shaderrel</exception>
        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst linking Program({program})");
            }
        }
        /// <summary>
        /// Shader aktiválása.
        /// </summary>
        public void Use()
        {
            GL.UseProgram(Handle);
        }

        /// <summary>
        /// Shaderben lévő attribútum helyének lekérdezése.
        /// </summary>
        /// <param name="attribName">Shader attribútum</param>
        /// <returns>attribútum pozíciója</returns>
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }

        /// <summary>
        /// Fájl beolvasása.
        /// </summary>
        /// <param name="path">fájl relatív helye</param>
        /// <returns>fájl tartalma szövegesen</returns>
        private static string LoadSource(string path)
        {
            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Shaderben lévő adat beállításra a megadott értékre.
        /// </summary>
        /// <param name="name">Attribútum amibe be akarunk állítani adatot</param>
        /// <param name="data">adat amire állítjuk az attribítum értékét</param>
        public void SetInt(string name, int data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], data);
        }
        /// <summary>
        /// Shaderben lévő adat beállításra a megadott értékre.
        /// </summary>
        /// <param name="name">Attribútum amibe be akarunk állítani adatot</param>
        /// <param name="data">adat amire állítjuk az attribítum értékét</param>
        public void SetFloat(string name, float data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], data);
        }
        /// <summary>
        /// Shaderben lévő adat beállításra a megadott értékre.
        /// </summary>
        /// <param name="name">Attribútum amibe be akarunk állítani adatot</param>
        /// <param name="data">adat amire állítjuk az attribítum értékét</param>
        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }
        /// <summary>
        /// Shaderben lévő adat beállításra a megadott értékre.
        /// </summary>
        /// <param name="name">Attribútum amibe be akarunk állítani adatot</param>
        /// <param name="data">adat amire állítjuk az attribítum értékét</param>
        public void SetColor4(string name, Color4 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform4(_uniformLocations[name], data);
        }
        /// <summary>
        /// Shaderben lévő adat beállításra a megadott értékre.
        /// </summary>
        /// <param name="name">Attribútum amibe be akarunk állítani adatot</param>
        /// <param name="data">adat amire állítjuk az attribítum értékét</param>
        public void SetVector4(string name, Vector4 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform4(_uniformLocations[name], data);
        }
        /// <summary>
        /// Shaderben lévő adat beállításra a megadott értékre.
        /// </summary>
        /// <param name="name">Attribútum amibe be akarunk állítani adatot</param>
        /// <param name="data">adat amire állítjuk az attribítum értékét</param>
        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(_uniformLocations[name], data);
        }
        /// <summary>
        /// Shaderben lévő adat beállításra a megadott értékre.
        /// </summary>
        /// <param name="name">Attribútum amibe be akarunk állítani adatot</param>
        /// <param name="data">adat amire állítjuk az attribítum értékét</param>
        public void SetVector2(string name, Vector2 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform2(_uniformLocations[name], data);
        }
        #endregion





    }
}
