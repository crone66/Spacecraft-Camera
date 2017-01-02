using Microsoft.Xna.Framework;

namespace CameraProject
{
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

        public Matrix RotationMatrix
        {
            get
            {
                return rotationMatrix;
            }
        }

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

        public void Update(GameTime gameTime, Vector3 movementVector, Vector3 rotationVector, float zoomDirection)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            Rotate(elapsedTime, rotationVector.X, rotationVector.Y, rotationVector.Z);
            Move(elapsedTime, movementVector);
            Zoom(elapsedTime, zoomDirection);

            UpdateProjection();
            UpdateView();
        }

        private void UpdateView()
        {
            rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            View = Matrix.CreateLookAt(Position, Position + rotationMatrix.Forward, rotationMatrix.Up);
        }

        private void UpdateProjection()
        {
            if (projectionIsDirty)
            {
                projectionIsDirty = false;
                Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), aspectRatio, nearPlane, farPlane);
            }
        }

        private void Move(float elapsedTime, Vector3 moveVector)
        {
            if(moveVector != Vector3.Zero)
            {
                Position += Vector3.Transform(moveVector, rotationMatrix) * MoveSpeed * elapsedTime;
            }
        }

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

        private void Zoom(float elapsedTime, float zoomDirection)
        {
            if(zoomDirection != 0)
                Fov += zoomDirection * ZoomSpeed * elapsedTime;
        }
    }
}
