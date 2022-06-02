using System;
using OpenTK.Mathematics;

namespace View
{
    /// <summary>
    /// A játéktérre való rálátási pontot adja meg.
    /// </summary>
    internal class Camera
    {

        #region Fields
        private Vector3 _front = -Vector3.UnitZ;

        private Vector3 _up = Vector3.UnitY;

        private Vector3 _right = Vector3.UnitX;


        private float _pitch;


        private float _yaw = -MathHelper.PiOver2;

        private float _fov = MathHelper.PiOver2;

        public Vector3 Front => _front;

        public Vector3 Up => _up;

        public Vector3 Right => _right;
        #endregion

        #region Properties
        public Vector3 Position { get; set; }

        public float AspectRatio { private get; set; }
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 45f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }
        #endregion

        #region Contructor
        /// <summary>
        /// Kamera létrehozása
        /// </summary>
        /// <param name="position">kezdőpozíció</param>
        /// <param name="aspectRatio">képarány</param>
        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Kamera nézetmátrixát visszadó függvény
        /// </summary>
        /// <returns>Kamera nézetmátrixa</returns>
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + _front, _up);
        }
        /// <summary>
        /// Kamera projektciós mátrixát visszadó függvény
        /// </summary>
        /// <returns>Kamera projektciós mátrixa</returns>
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 10000f);
        }

        #endregion

    }
}
