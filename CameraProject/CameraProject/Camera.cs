using Microsoft.Xna.Framework;

namespace CameraProject
{
    /// <summary>
    /// Spacecraft camera
    /// </summary>
    public class Camera
    {
        private float pitch; //Up/Down
        private float yaw; //left/right
        private float roll;
        
        private float fov;
        private float aspectRatio;
        private float nearPlane;
        private float farPlane;
        private bool projectionIsDirty;

        private Vector3 position;
        private Matrix rotationMatrix;

        public float MoveSpeed;
        public float RotationSpeed;
        public float ZoomSpeed;

        private BoundingFrustum viewArea;

        private Matrix view;
        private Matrix projection;

        /// <summary>
        /// Camera field of view (min. value 0.01f, max. value 179.99f)
        /// </summary>
        public float Fov
        {
            get
            {
                return fov;
            }

            set
            {
                fov = value;

                if (fov > 179.99f)
                    fov = 179.99f;
                else if (fov < 0.01f)
                    fov = 0.01f;

                projectionIsDirty = true;
            }
        }

        /// <summary>
        /// Window aspect ratio
        /// </summary>
        public float AspectRatio
        {
            get
            {
                return aspectRatio;
            }

            set
            {
                aspectRatio = value;
                projectionIsDirty = true;
            }
        }

        /// <summary>
        /// Camera near plane
        /// </summary>
        public float NearPlane
        {
            get
            {
                return nearPlane;
            }

            set
            {
                nearPlane = value;
                projectionIsDirty = true;
            }
        }

        /// <summary>
        /// Camera far plane
        /// </summary>
        public float FarPlane
        {
            get
            {
                return farPlane;
            }

            set
            {
                farPlane = value;
                projectionIsDirty = true;
            }
        }

        /// <summary>
        /// Camera view matrix
        /// </summary>
        public Matrix View
        {
            get
            {
                return view;
            }

            private set
            {
                view = value;
                viewArea = new BoundingFrustum(view * projection);
            }
        }

        /// <summary>
        /// Camera projection matrix
        /// </summary>
        public Matrix Projection
        {
            get
            {
                return projection;
            }

            private set
            {
                projection = value;
                viewArea = new BoundingFrustum(view * projection);
            }
        }

        /// <summary>
        /// Camera pitch rotation value
        /// </summary>
        public float Pitch
        {
            get
            {
                return pitch;
            }

            set
            {
                pitch = value;

                if (pitch > MathHelper.TwoPi)
                    pitch -= MathHelper.TwoPi;
                else if (pitch < 0)
                    pitch += MathHelper.TwoPi;
            }
        }

        /// <summary>
        /// Camera yaw rotation value
        /// </summary>
        public float Yaw
        {
            get
            {
                return yaw;
            }
            set
            {
                yaw = value;

                if (yaw > MathHelper.TwoPi)              
                    yaw -= MathHelper.TwoPi;         
                else if (yaw < 0)
                    yaw += MathHelper.TwoPi;
            }
        }

        /// <summary>
        /// Camera roll rotation value
        /// </summary>
        public float Roll
        {
            get
            {
                return roll;
            }

            set
            {
                roll = value;
                if (roll > MathHelper.TwoPi)
                    roll -= MathHelper.TwoPi;
                else if (roll < 0)
                    roll += MathHelper.TwoPi;
            }
        }

        /// <summary>
        /// Camera position
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }       

        /// <summary>
        /// Camera rotation matrix
        /// </summary>
        public Matrix RotationMatrix
        {
            get
            {
                return rotationMatrix;
            }
        }

        /// <summary>
        /// Initzializes a camera
        /// </summary>
        /// <param name="cameraPosition">Camera spawn position</param>
        /// <param name="moveSpeed">Movement speed</param>
        /// <param name="rotationSpeed">Rotation speed</param>
        /// <param name="zoomSpeed">Zoom speed</param>
        /// <param name="fov">Field of view</param>
        /// <param name="aspectRatio">Window aspect ratio</param>
        /// <param name="nearPlane">Camera near plane</param>
        /// <param name="farPlane">Camera far plane</param>
        public Camera(Vector3 cameraPosition, float moveSpeed, float rotationSpeed, float zoomSpeed, float fov, float aspectRatio, float nearPlane, float farPlane)
        {
            Position = cameraPosition;

            Fov = fov;
            AspectRatio = aspectRatio;
            NearPlane = nearPlane;
            FarPlane = farPlane;

            MoveSpeed = moveSpeed;
            RotationSpeed = rotationSpeed;
            ZoomSpeed = zoomSpeed;
        }

        /// <summary>
        /// Updates camera
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="movementVector">Indicates movement directions</param>
        /// <param name="rotationVector">Indicates rotation directions (x=pitch, y=yaw, z=roll)</param>
        /// <param name="zoomDirection">Indicates zoom direction</param>
        public void Update(GameTime gameTime, Vector3 movementVector, Vector3 rotationVector, float zoomDirection)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            Rotate(elapsedTime, rotationVector.X, rotationVector.Y, rotationVector.Z);
            Move(elapsedTime, movementVector);
            Zoom(elapsedTime, zoomDirection);

            UpdateProjection();
            UpdateView();
        }

        /// <summary>
        /// Updates view and rotation matrix
        /// </summary>
        private void UpdateView()
        {
            rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            View = Matrix.CreateLookAt(Position, Position + rotationMatrix.Forward, rotationMatrix.Up);
        }

        /// <summary>
        /// Updates projection matrix
        /// </summary>
        private void UpdateProjection()
        {
            if (projectionIsDirty)
            {
                projectionIsDirty = false;
                Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), aspectRatio, nearPlane, farPlane);
            }
        }

        /// <summary>
        /// Updates camera position by the given movement vector
        /// </summary>
        /// <param name="elapsedTime">Elapsed time since last update</param>
        /// <param name="moveVector">Indicates movement directions</param>
        private void Move(float elapsedTime, Vector3 moveVector)
        {
            if(moveVector != Vector3.Zero)
            {
                Position += Vector3.Transform(moveVector, rotationMatrix) * MoveSpeed * elapsedTime;
            }
        }

        /// <summary>
        /// Updates rotation values
        /// </summary>
        /// <param name="elapsedTime">Elapsed time since last update</param>
        /// <param name="pitch">Indicates whenther to rotate the camera around the x axis</param>
        /// <param name="yaw">Indicates whenther to rotate the camera around the y axis</param>
        /// <param name="roll">Indicates whenther to rotate the camera around the z axis</param>
        private void Rotate(float elapsedTime, float pitch, float yaw, float roll)
        {
            if(roll != 0)
                Roll += roll * RotationSpeed * elapsedTime;

            if(yaw != 0)
                Yaw += yaw * RotationSpeed * elapsedTime;

            if(pitch != 0)
                Pitch += pitch * RotationSpeed * elapsedTime;

            UpdateView();
        }

        /// <summary>
        /// Updates field of view to zoom
        /// </summary>
        /// <param name="elapsedTime">Elapsed time since last update</param>
        /// <param name="zoomDirection">Indicates the zoom direction (1 = Zoom In, 2 = Zoom out)</param>
        private void Zoom(float elapsedTime, float zoomDirection)
        {
            if(zoomDirection != 0)
                Fov += zoomDirection * ZoomSpeed * elapsedTime;
        }
    }
}
