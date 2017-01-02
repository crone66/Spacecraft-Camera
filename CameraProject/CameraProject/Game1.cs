using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CameraProject
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera camera;
        MouseState centerState;

        Model model;
        Texture2D texture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            camera = new Camera(new Vector3(0, 0, 20), 0.01f, 0.0001f, 0.01f, 45, graphics.GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000000f);

            Mouse.SetPosition(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            centerState = Mouse.GetState();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            model = Content.Load<Model>("Sonne-1-15");
            texture = Content.Load<Texture2D>("Sonne-1");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            RotationResetInput(keyboardState);
            camera.Update(gameTime, CamMovementInput(keyboardState), CamRotationInput(mouseState, keyboardState), CamZoomInput(keyboardState));
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateTranslation(Vector3.Zero);
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.Texture = texture;
                    effect.TextureEnabled = true;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
            
            base.Draw(gameTime);
        }
        
        /// <summary>
        /// Handles camera input for rotation
        /// </summary>
        /// <param name="mouseState">Current mouse state</param>
        /// <param name="keyboardState">Current keyboard state</param>
        /// <returns>Returns a rotation vector (pitch, yaw, roll)</returns>
        private Vector3 CamRotationInput(MouseState mouseState, KeyboardState keyboardState)
        {
            float xDiff = (mouseState.X - centerState.X) * -1;
            float yDiff = (mouseState.Y - centerState.Y) * -1;
            
            Mouse.SetPosition(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            
            return new Vector3(yDiff, xDiff, FPRollInput(keyboardState));
        }

        /// <summary>
        /// Handles camera input for movement
        /// </summary>
        /// <param name="keyboardState">Current keyboard state</param>
        /// <returns>Returns a movement vector</returns>
        private Vector3 CamMovementInput(KeyboardState keyboardState)
        {
            Vector3 moveVec = Vector3.Zero;
            if (keyboardState.IsKeyDown(Keys.W))
                moveVec.Z = -1f;
            else if (keyboardState.IsKeyDown(Keys.S))
                moveVec.Z = 1f;

            if (keyboardState.IsKeyDown(Keys.A))
                moveVec.X = -1f;
            else if (keyboardState.IsKeyDown(Keys.D))
                moveVec.X = 1f;

            if (keyboardState.IsKeyDown(Keys.Y))
                moveVec.Y = -1f;
            else if (keyboardState.IsKeyDown(Keys.Z))
                moveVec.Y = 1f;

            return moveVec;
        }

        /// <summary>
        /// Handles camera input for rolling
        /// </summary>
        /// <param name="keyboardState">Current keyboard state</param>
        /// <returns>Returns a roll direction</returns>
        private float FPRollInput(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Q))
                return -1;
            else if (keyboardState.IsKeyDown(Keys.E))
                return 1;

            return 0;
        }

        /// <summary>
        /// Handles camrea input for zooming
        /// </summary>
        /// <param name="keyboardState">Current keyboard state</param>
        /// <returns>Retruns a zoom direction</returns>
        private float CamZoomInput(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Add))
                return -1;
            else if (keyboardState.IsKeyDown(Keys.Subtract))
                return 1;

            return 0;
        }

        /// <summary>
        /// Resets camera rotation (yaw, pitch, roll)
        /// </summary>
        /// <param name="keyboardState">Current keyboard state</param>
        private void RotationResetInput(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.R))
            {
                camera.Pitch = 0f;
                camera.Yaw = 0f;
                camera.Roll = 0f;
            }
        }
    }
}
