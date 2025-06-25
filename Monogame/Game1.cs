using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

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

        SpriteBatch miniMapRect;
        Texture2D miniMap;

        Texture2D titleImage;
        float titleAlpha = 1f;
        bool showTitle = true;
        bool fadingOut = false;
        float fadeSpeed = 1f; // fades in 1 second

        SoundEffect backgroundHum;
        SoundEffectInstance backgroundHumInstance;
        bool backgroundSoundStarted = false;

        SoundEffect doorSound;  

        SoundEffect titleMusic;
        SoundEffectInstance titleMusicInstance;

        int officeX;
        int officeY;

        public bool leftLightOn = false;
        public bool rightLightOn = false;

        int power = 100;

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

        // === CAMERA SCREEN VARIABLES ===
        Texture2D cameraScreen;

        bool cameraActive = false;   
        bool cameraSliding = false;   
        bool slidingUp = false; 
        float cameraY;             
        float cameraSlideDuration = 0.25f; 
        float cameraSlideElapsed = 0f;

        float cameraPanX = 0f;       
        float cameraPanSpeed = 100f;  
        bool panDirectionRight = true;

        float panPauseDuration = 1f; 
        float panPauseTimer = 0f;

        Texture2D staticOverlay;
        float staticOverlayAlpha = 0.3f;
        bool staticFlipHorizontal = false;

        SoundEffect cameraToggleSound;

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

            _graphics.ApplyChanges();

            Window.Position = new Microsoft.Xna.Framework.Point(
                (screenWidth - _graphics.PreferredBackBufferWidth) / 2,
                (screenHeight - _graphics.PreferredBackBufferHeight) / 2
            );

            previousKeyboardState = Keyboard.GetState();

            cameraY = _graphics.PreferredBackBufferHeight; 
        }

        protected override void LoadContent()
        {
            officeRect = new SpriteBatch(GraphicsDevice);
            leftLightRect = new SpriteBatch(GraphicsDevice);
            rightLightRect = new SpriteBatch(GraphicsDevice);
            doorLeftRect = new SpriteBatch(GraphicsDevice);
            doorRightRect = new SpriteBatch(GraphicsDevice);

            officeTexture = Content.Load<Texture2D>("office");

            leftLightTexture = Content.Load<Texture2D>("leftLightOn");
            rightLightTexture = Content.Load<Texture2D>("rightLightOn");

            doorLeftTexture = Content.Load<Texture2D>("doorLeft");
            doorRightTexture = Content.Load<Texture2D>("doorRight");

            titleImage = Content.Load<Texture2D>("Title");

            backgroundHum = Content.Load<SoundEffect>("BackgroundHum");
            backgroundHumInstance = backgroundHum.CreateInstance();
            backgroundHumInstance.IsLooped = true;

            doorSound = Content.Load<SoundEffect>("DoorSound");

            cameraScreen = Content.Load<Texture2D>("cameraScreen");
            miniMap = Content.Load<Texture2D>("miniMap");

            staticOverlay = Content.Load<Texture2D>("staticOverlay");

            cameraToggleSound = Content.Load<SoundEffect>("CameraToggleSound");

            titleMusic = Content.Load<SoundEffect>("TitleMusic");
            titleMusicInstance = titleMusic.CreateInstance();
            titleMusicInstance.IsLooped = true;
            titleMusicInstance.Volume = 1f;
            titleMusicInstance.Play();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!fadingOut && showTitle && keyboardState.IsKeyDown(Keys.Enter) && !previousKeyboardState.IsKeyDown(Keys.Enter))
            {
                fadingOut = true;

                if (!backgroundSoundStarted)
                {
                    backgroundHumInstance.Play();
                    backgroundSoundStarted = true;
                }
            }

            if (fadingOut)
            {
                titleAlpha -= elapsedSeconds * fadeSpeed;
                if (titleAlpha <= 0f)
                {
                    titleAlpha = 0f;
                    showTitle = false;
                    fadingOut = false;
                    titleMusicInstance.Stop();
                }
                else
                {
                    titleMusicInstance.Volume = titleAlpha;
                }
            }

            if (!showTitle)
            {
                if (keyboardState.IsKeyDown(Keys.Space) && !previousKeyboardState.IsKeyDown(Keys.Space))
                {
                    cameraToggleSound.Play();

                    if (!cameraActive && !cameraSliding)
                    {
                        cameraSliding = true;
                        slidingUp = true;
                        cameraSlideElapsed = 0f;
                    }
                    else if (cameraActive && !cameraSliding)
                    {
                        cameraSliding = true;
                        slidingUp = false;
                        cameraSlideElapsed = 0f;
                    }
                }

                if (cameraSliding)
                {
                    cameraSlideElapsed += elapsedSeconds;
                    float progress = cameraSlideElapsed / cameraSlideDuration;
                    if (progress > 1f) progress = 1f;

                    int windowHeight = _graphics.PreferredBackBufferHeight;

                    if (slidingUp)
                    {
                        cameraY = windowHeight * (1f - progress);
                    }
                    else
                    {
                        cameraY = windowHeight * progress;
                    }

                    if (progress >= 1f)
                    {
                        cameraSliding = false;
                        cameraActive = slidingUp;
                        if (cameraActive)
                        {
                            cameraPanX = 0f;
                            panDirectionRight = true;
                            panPauseTimer = 0f;
                        }
                    }
                }

                if (cameraActive && !cameraSliding)
                {
                    int windowWidth = _graphics.PreferredBackBufferWidth;
                    int windowHeight = _graphics.PreferredBackBufferHeight;

                    float aspectRatio = (float)cameraScreen.Width / cameraScreen.Height;
                    int camHeight = windowHeight;
                    int camWidth = (int)(camHeight * aspectRatio);

                    int maxPanX = camWidth - windowWidth;
                    if (maxPanX < 0) maxPanX = 0;

                    // Handle pause at edges
                    if (panPauseTimer > 0f)
                    {
                        panPauseTimer -= elapsedSeconds;
                        if (panPauseTimer < 0f)
                            panPauseTimer = 0f;
                    }
                    else
                    {
                        if (panDirectionRight)
                        {
                            cameraPanX += cameraPanSpeed * elapsedSeconds;
                            if (cameraPanX >= maxPanX)
                            {
                                cameraPanX = maxPanX;
                                panDirectionRight = false;
                                panPauseTimer = panPauseDuration;
                            }
                        }
                        else
                        {
                            cameraPanX -= cameraPanSpeed * elapsedSeconds;
                            if (cameraPanX <= 0)
                            {
                                cameraPanX = 0;
                                panDirectionRight = true;
                                panPauseTimer = panPauseDuration;
                            }
                        }
                    }

                    staticFlipHorizontal = !staticFlipHorizontal;
                }

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

                if (doorLeftClosing)
                {
                    doorLeftAnimationProgress++;
                    if (doorLeftAnimationProgress == 1)
                        doorSound.Play();

                    if (doorLeftAnimationProgress >= animationDuration)
                    {
                        doorLeftAnimationProgress = 0;
                        doorLeftClosing = false;
                        doorLeftClosed = true;
                    }
                }
                else if (doorLeftOpening)
                {
                    doorLeftAnimationProgress++;
                    if (doorLeftAnimationProgress == 1)
                        doorSound.Play();

                    if (doorLeftAnimationProgress >= animationDuration)
                    {
                        doorLeftAnimationProgress = 0;
                        doorLeftOpening = false;
                        doorLeftClosed = false;
                    }
                }

                if (doorRightClosing)
                {
                    doorRightAnimationProgress++;
                    if (doorRightAnimationProgress == 1)
                        doorSound.Play();

                    if (doorRightAnimationProgress >= animationDuration)
                    {
                        doorRightAnimationProgress = 0;
                        doorRightClosing = false;
                        doorRightClosed = true;
                    }
                }
                else if (doorRightOpening)
                {
                    doorRightAnimationProgress++;
                    if (doorRightAnimationProgress == 1)
                        doorSound.Play();

                    if (doorRightAnimationProgress >= animationDuration)
                    {
                        doorRightAnimationProgress = 0;
                        doorRightOpening = false;
                        doorRightClosed = false;
                    }
                }

                // Office Position follows mouse as before (only if camera inactive)
                if (!cameraActive && !cameraSliding)
                {
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
                }
            }

            previousKeyboardState = keyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (showTitle)
            {
                officeRect.Begin();
                officeRect.Draw(titleImage, GraphicsDevice.Viewport.Bounds, Color.White * titleAlpha);
                officeRect.End();
            }
            else
            {
                int windowWidth = _graphics.PreferredBackBufferWidth;
                int windowHeight = _graphics.PreferredBackBufferHeight;

                float aspectRatioOffice = (float)officeTexture.Width / officeTexture.Height;

                int newHeight = windowHeight;
                int newWidth = (int)(newHeight * aspectRatioOffice);

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

                if (cameraActive || cameraSliding)
                {
                    float aspectRatioCamera = (float)cameraScreen.Width / cameraScreen.Height;
                    int camHeight = windowHeight;
                    int camWidth = (int)(camHeight * aspectRatioCamera);

                    int maxPanX = camWidth - windowWidth;
                    if (maxPanX < 0) maxPanX = 0;

                    int sourceWidth = (int)((float)windowWidth / camWidth * cameraScreen.Width);
                    int sourceX = (int)((cameraPanX / camWidth) * cameraScreen.Width);

                    if (sourceX + sourceWidth > cameraScreen.Width)
                        sourceX = cameraScreen.Width - sourceWidth;

                    Rectangle sourceRect = new Rectangle(sourceX, 0, sourceWidth, cameraScreen.Height);

                    Rectangle destRect = new Rectangle(0, (int)cameraY, windowWidth, windowHeight);

                    officeRect.Begin();
                    officeRect.Draw(cameraScreen, destRect, sourceRect, Color.White);

                    SpriteEffects effects = staticFlipHorizontal ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    officeRect.Draw(staticOverlay, destRect, null, Color.White * staticOverlayAlpha, 0f, Vector2.Zero, effects, 0f);

                    officeRect.End();

                    int minimapWidth = windowWidth / 3;
                    int minimapHeight = (int)(minimapWidth / ((float)miniMap.Width / miniMap.Height));
                    int minimapX = windowWidth - minimapWidth - 20;
                    int minimapY = windowHeight - minimapHeight - 20 + (int)cameraY;

                    officeRect.Begin();
                    officeRect.Draw(miniMap,
                        new Rectangle(minimapX, minimapY, minimapWidth, minimapHeight),
                        Color.White * 0.8f);
                    officeRect.End();
                }
                else
                {
                    officeRect.Begin();
                    officeRect.Draw(officeTexture, new Rectangle(officeX, officeY, newWidth, newHeight), Color.White);
                    officeRect.End();
                }
            }

            base.Draw(gameTime);
        }
    }
}

