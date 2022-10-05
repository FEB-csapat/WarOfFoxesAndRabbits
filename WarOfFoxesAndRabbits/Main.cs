using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace WarOfFoxesAndRabbits
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D rectangleBlock;

        Cell[,] field = new Cell[GameVariables.cellsHorizontallyCount, GameVariables.cellsVerticallyCount];

        List<Component> components = new List<Component>();

        SpriteFont spriteFont;

        MouseState previousMouseState;

        static bool paused = false;
        
        
        BrushType brushSelected = BrushType.NONE;

        static long tickCounter = 0;
        static int tickrate = 1;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = GameVariables.windowWidth;
            _graphics.PreferredBackBufferHeight = GameVariables.windowHeight;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            Window.Title = GameVariables.title;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            #region Components

            spriteFont = Content.Load<SpriteFont>("Fonts/Arial");

            // Button to pause the game
            Button pauseButton = new Button();
            pauseButton = new Button(new Vector2(550, 20), () => {
                paused = !paused;
                if (paused) pauseButton.text = "Continue";
                else pauseButton.text = "Pause";
            }, text: "Pause", width: 150, height: 50);
            components.Add(pauseButton);

            // Label to show the tickrate
            Label tickrateLabel = new Label(tickrate.ToString() + " tick", new Vector2(602, 96));
            components.Add(tickrateLabel);

            // Increment tickrate button
            components.Add(new Button(new Vector2(650, 80), ()=> {
                if (tickrate < 59)
                {
                    tickrate+=2;
                    tickrateLabel.text = tickrate.ToString() + " tick";
                }
            }, text: "+", width: 50, height: 50));

            // Decrement tickrate button
            components.Add(new Button(new Vector2(550, 80), () => {
                if (tickrate > 1)
                {
                    tickrate-=2;
                    tickrateLabel.text = tickrate.ToString() + " tick";
                }
            }, text: "-", width: 50, height: 50));

            // Button to draw animals on the field
            Button bunnyBrush = new Button();
            Button foxBrush = new Button();
            bunnyBrush = new Button(new Vector2(550, 140), () =>
            {
                brushSelected = BrushType.BUNNY;
                bunnyBrush.isSelected = true;
                foxBrush.isSelected = false;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Rabbit"), width: 50, height: 50);
            components.Add(bunnyBrush);

            // Fox brush
            foxBrush = new Button(new Vector2(550, 200), () =>
            {
                brushSelected = BrushType.FOX;
                bunnyBrush.isSelected = false;
                foxBrush.isSelected = true;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Fox"), width: 50, height: 50);
            components.Add(foxBrush);

            // Button to empty the fields from animals
            Button emptyButton = new Button(new Vector2(550, 260), () => {
                for (int y = 0; y < GameVariables.cellsVerticallyCount; y++)
                {
                    for (int x = 0; x < GameVariables.cellsVerticallyCount; x++)
                    {
                        field[x, y].inhabitant = null;
                    }
                }
            }, text: "Empty", width: 150, height: 50);
            components.Add(emptyButton);

            #endregion

            rectangleBlock = new Texture2D(GraphicsDevice, 1, 1);
            Color xnaColorBorder = new Color(255, 255, 255);
            rectangleBlock.SetData(new[] { xnaColorBorder });
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Creating the field
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            for (int y = 0; y < GameVariables.cellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.cellsVerticallyCount; x++)
                {
                    field[x, y] = new Cell(new Vector2(x * GameVariables.cellSize, y * GameVariables.cellSize), GraphicsDevice);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            tickCounter++;
            if (tickCounter % (60/tickrate) == 0)
            {
                if (!paused)
                {
                    GameManager.Update(field);
                }
            }

            MouseState currentMouseState = Mouse.GetState();
            // Mouse clicked
            if (previousMouseState.LeftButton == ButtonState.Released
            && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (Component component in components)
                {
                    if (component.GetType() == typeof(Button))
                    {
                        if(currentMouseState.X >= ((Button)component).position.X && currentMouseState.X <= ((Button)component).position.X + ((Button)component).width
                        && currentMouseState.Y >= ((Button)component).position.Y && currentMouseState.Y <= ((Button)component).position.Y + ((Button)component).height)
                        {
                            ((Button)component).onClick();
                        }
                    }
                }
            }

            // Mouse being held
            if (brushSelected == BrushType.FOX || brushSelected == BrushType.BUNNY) {
                if (currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    for (int y = 0; y < GameVariables.cellsVerticallyCount; y++)
                    {
                        for (int x = 0; x < GameVariables.cellsVerticallyCount; x++)
                        {

                            if (currentMouseState.X >= field[x, y].Position.X && currentMouseState.X <= field[x, y].Position.X + GameVariables.cellSize
                            && currentMouseState.Y >= field[x, y].Position.Y && currentMouseState.Y <= field[x, y].Position.Y + GameVariables.cellSize
                            )
                            {
                                if (field[x, y].inhabitant == null)
                                {
                                    if (brushSelected == BrushType.BUNNY)
                                    {
                                        field[x, y].inhabitant = new Rabbit();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            previousMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        

        protected override void Draw(GameTime gameTime)
        {
                GraphicsDevice.Clear(Color.DarkGray);
                _spriteBatch.Begin();
                for (int y = 0; y < GameVariables.cellsVerticallyCount; y++)
                {
                    for (int x = 0; x < GameVariables.cellsVerticallyCount; x++)
                    {
                        _spriteBatch.Draw(rectangleBlock, new Rectangle(x * GameVariables.cellSize, y * GameVariables.cellSize, GameVariables.cellSize, GameVariables.cellSize), field[x, y].Color);
                    }
                }

                foreach (Component component in components)
                {
                    if (component.GetType() == typeof(Button))
                    {
                    if ( ((Button)component).image != null)
                        {
                            
                            if ( ((Button)component).isSelected ) {
                                _spriteBatch.Draw(rectangleBlock, new Rectangle(((int)((Button)component).position.X)-4, ((int)((Button)component).position.Y)-4, ((Button)component).width+6, ((Button)component).height+6), Color.Black);
                            }
                            _spriteBatch.Draw(((Button)component).image, ((Button)component).position, Color.White);
                        }
                        else
                        {
                            _spriteBatch.Draw(rectangleBlock, new Rectangle((int)((Button)component).position.X, (int)((Button)component).position.Y, ((Button)component).width, ((Button)component).height), Color.Gray);
                        }
                        
                        _spriteBatch.DrawString(spriteFont, ((Button)component).text, new Vector2(((Button)component).position.X + ((Button)component).width / 4, ((Button)component).position.Y + ((Button)component).height / 3), Color.Black);
                    }else if (component.GetType() == typeof(Label))
                    {
                        _spriteBatch.DrawString(spriteFont, ((Label)component).text, new Vector2(component.position.X, component.position.Y), Color.Black);

                    }
                }
                _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}