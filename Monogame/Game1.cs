using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FnafRecoded
{
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;

        SpriteBatch officeRect;
        Texture2D officeTexture;

        SpriteBatch leftLightRect;
        Texture2D leftLightTexture;

        SpriteBatch rightLightRect;
        Texture2D rightLightTexture;

        SpriteBatch doorLeftRect;
        Texture2D doorLeftTexture;

        SpriteBatch doorRightRect;
        Texture2D doorRightTexture;

        int officeX;
        int officeY;

        public bool leftLightOn = false; 
        public bool rightLightOn = false;

        int power = 100;

        // Door states and animation
        bool doorLeftClosed = false; 
        bool doorRightClosed = false;  

        int doorLeftAnimationProgress = 0;
        int doorRightAnimationProgress = 0;

        bool doorLeftClosing = false;
        bool doorLeftOpening = false;
        bool doorRightClosing = false;
        bool doorRightOpening = false;

        int animationDuration = 20;

        KeyboardState previousKeyboardState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            base.Initialize();

            var screenWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            var screenHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;

            _graphics.PreferredBackBufferWidth = screenWidth;
            _graphics.PreferredBackBufferHeight = screenHeight;

            //_graphics.IsFullScreen = false;

            _graphics.ApplyChanges();

            Window.Position = new Microsoft.Xna.Framework.Point(
                (screenWidth - _graphics.PreferredBackBufferWidth) / 2,
                (screenHeight - _graphics.PreferredBackBufferHeight) / 2
            );

            previousKeyboardState = Keyboard.GetState();
        }

        protected override void LoadContent()
        {
            officeRect = new SpriteBatch(GraphicsDevice);
            leftLightRect = new SpriteBatch(GraphicsDevice);
            rightLightRect = new SpriteBatch(GraphicsDevice);
            doorLeftRect = new SpriteBatch(GraphicsDevice);
            doorRightRect = new SpriteBatch(GraphicsDevice);

            officeTexture = Content.Load<Texture2D>("office");

            //Lights
            leftLightTexture = Content.Load<Texture2D>("leftLightOn");
            rightLightTexture = Content.Load<Texture2D>("rightLightOn");

            //Doors
            doorLeftTexture = Content.Load<Texture2D>("doorLeft");
            doorRightTexture = Content.Load<Texture2D>("doorRight");
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            MouseState mouseState = Mouse.GetState();

            // Toggle doorLeft on Q
            if (keyboardState.IsKeyDown(Keys.Q) && !previousKeyboardState.IsKeyDown(Keys.Q) && power > 0)
            {
                if (!doorLeftClosed && doorLeftAnimationProgress == 0 && !doorLeftClosing && !doorLeftOpening)
                {
                    doorLeftClosing = true;
                    doorLeftOpening = false;
                }
                else if (doorLeftClosed && doorLeftAnimationProgress == 0 && !doorLeftClosing && !doorLeftOpening)
                {
                    doorLeftOpening = true;
                    doorLeftClosing = false;
                }
            }

            // Toggle doorRight on E
            if (keyboardState.IsKeyDown(Keys.E) && !previousKeyboardState.IsKeyDown(Keys.E) && power > 0)
            {
                if (!doorRightClosed && doorRightAnimationProgress == 0 && !doorRightClosing && !doorRightOpening)
                {
                    doorRightClosing = true;
                    doorRightOpening = false;
                }
                else if (doorRightClosed && doorRightAnimationProgress == 0 && !doorRightClosing && !doorRightOpening)
                {
                    doorRightOpening = true;
                    doorRightClosing = false;
                }
            }

            // Toggle Lights
            if (keyboardState.IsKeyDown(Keys.A) && !previousKeyboardState.IsKeyDown(Keys.A) && power > 0)
                leftLightOn = !leftLightOn;

            if (keyboardState.IsKeyDown(Keys.D) && !previousKeyboardState.IsKeyDown(Keys.D) && power > 0)
                rightLightOn = !rightLightOn;

            // Door Animations
            if (doorLeftClosing)
            {
                doorLeftAnimationProgress += 1;
                if (doorLeftAnimationProgress >= animationDuration)
                {
                    doorLeftAnimationProgress = 0;
                    doorLeftClosing = false;
                    doorLeftClosed = true;
                }
            }
            else if (doorLeftOpening)
            {
                doorLeftAnimationProgress += 1;
                if (doorLeftAnimationProgress >= animationDuration)
                {
                    doorLeftAnimationProgress = 0;
                    doorLeftOpening = false;
                    doorLeftClosed = false;
                }
            }

            if (doorRightClosing)
            {
                doorRightAnimationProgress += 1;
                if (doorRightAnimationProgress >= animationDuration)
                {
                    doorRightAnimationProgress = 0;
                    doorRightClosing = false;
                    doorRightClosed = true;
                }
            }
            else if (doorRightOpening)
            {
                doorRightAnimationProgress += 1;
                if (doorRightAnimationProgress >= animationDuration)
                {
                    doorRightAnimationProgress = 0;
                    doorRightOpening = false;
                    doorRightClosed = false;
                }
            }

            // Office Position
            officeX = mouseState.X - officeTexture.Width / 2;
            officeY = mouseState.Y - officeTexture.Height / 2;

            int windowWidth = GraphicsDevice.Viewport.Width;
            int windowHeight = GraphicsDevice.Viewport.Height;

            if (officeX + officeTexture.Width > windowWidth)
                officeX = windowWidth - officeTexture.Width;

            if (officeY <= 0 || officeY >= 0)
                officeY = 0;

            if (officeX > 0)
                officeX = 0; // Left boundary

            if (officeX + officeTexture.Width > windowWidth)
                officeX = windowWidth - officeTexture.Width; //Right Boundary

            previousKeyboardState = keyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            int windowWidth = _graphics.PreferredBackBufferWidth;
            int windowHeight = _graphics.PreferredBackBufferHeight;

            float aspectRatio = (float)officeTexture.Width / officeTexture.Height;

            int newHeight = windowHeight;
            int newWidth = (int)(newHeight * aspectRatio);

            int doorHeight = newHeight;
            int extraHeight = 250;
            int doorTotalHeight = doorHeight + extraHeight;

            int doorLeftOffsetY = 0;
            if (doorLeftClosing)
            {
                doorLeftOffsetY = (doorLeftAnimationProgress * doorTotalHeight) / animationDuration;
            }
            else if (doorLeftOpening)
            {
                doorLeftOffsetY = doorTotalHeight - (doorLeftAnimationProgress * doorTotalHeight) / animationDuration;
            }
            else
            {
                doorLeftOffsetY = doorLeftClosed ? doorTotalHeight : 0;
            }

            int doorRightOffsetY = 0;
            if (doorRightClosing)
            {
                doorRightOffsetY = (doorRightAnimationProgress * doorTotalHeight) / animationDuration;
            }
            else if (doorRightOpening)
            {
                doorRightOffsetY = doorTotalHeight - (doorRightAnimationProgress * doorTotalHeight) / animationDuration;
            }
            else
            {
                doorRightOffsetY = doorRightClosed ? doorTotalHeight : 0;
            }

            if (leftLightOn)
            {
                leftLightRect.Begin();
                leftLightRect.Draw(leftLightTexture, new Rectangle(officeX, officeY, newWidth, newHeight), Color.White);
                leftLightRect.End();
            }

            if (rightLightOn)
            {
                rightLightRect.Begin();
                rightLightRect.Draw(rightLightTexture, new Rectangle(officeX, officeY, newWidth, newHeight), Color.White);
                rightLightRect.End();
            }

            doorLeftRect.Begin();
            doorLeftRect.Draw(doorLeftTexture,
                new Rectangle(officeX, -doorTotalHeight + doorLeftOffsetY, newWidth, doorTotalHeight),
                Color.White);
            doorLeftRect.End();

            doorRightRect.Begin();
            doorRightRect.Draw(doorRightTexture,
                new Rectangle(officeX, -doorTotalHeight + doorRightOffsetY, newWidth, doorTotalHeight),
                Color.White);
            doorRightRect.End();

            officeRect.Begin();
            officeRect.Draw(officeTexture, new Rectangle(officeX, officeY, newWidth, newHeight), Color.White);
            officeRect.End();

            base.Draw(gameTime);
        }


    }
}

