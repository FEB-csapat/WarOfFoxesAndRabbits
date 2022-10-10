using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace WarOfFoxesAndRabbits
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D rectangleBlock;

        Cell[,] field = new Cell[GameVariables.CellsHorizontallyCount, GameVariables.CellsVerticallyCount];

        List<Component> components = new List<Component>();

        SpriteFont spriteFont;

        MouseState previousMouseState;

        static bool paused = false;

        // Variables for components
        Label generationLabel;
        int generation = 0;
        Label foxLabel;
        Label rabbitLabel;

        BrushType brushSelected = BrushType.NONE;

        static long tickCounter = 0;
        static int tickrate = 1;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = GameVariables.WindowWidth;
            _graphics.PreferredBackBufferHeight = GameVariables.WindowHeight;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            Window.Title = GameVariables.Title;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            #region Components

            spriteFont = Content.Load<SpriteFont>("Fonts/Arial");

            // TODO: Generation counter resets with clear button
            generationLabel = new Label("Generation: " + generation, new Vector2(GameVariables.GetGameCanvasWidth() + 50, 4));
            components.Add(generationLabel);

            // Button to pause the game
            Button pauseButton = new Button();
            pauseButton = new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 50, 30), () =>
            {
                paused = !paused;
                if (paused) pauseButton.text = "Continue";
                else pauseButton.text = "Pause";
            }, text: "Pause", width: 150, height: 50);
            components.Add(pauseButton);

            // Label to show the tickrate
            Label tickrateLabel = new Label(tickrate.ToString() + " tick", new Vector2(GameVariables.GetGameCanvasWidth() + 100, 106));
            components.Add(tickrateLabel);

            // Increment tickrate button
            components.Add(new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 150, 90), () =>
            {
                if (tickrate < 59)
                {
                    tickrate += 2;
                    tickrateLabel.text = tickrate.ToString() + " tick";
                }
            }, text: "+", width: 50, height: 50));

            // Decrement tickrate button
            components.Add(new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 50, 90), () =>
            {
                if (tickrate > 1)
                {
                    tickrate -= 2;
                    tickrateLabel.text = tickrate.ToString() + " tick";
                }
            }, text: "-", width: 50, height: 50));

            // Button to draw animals on the field
            Button bunnyBrush = new Button();
            Button foxBrush = new Button();
            Button wallBrush = new Button();
            bunnyBrush = new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 50, 150), () =>
            {
                brushSelected = BrushType.BUNNY;
                bunnyBrush.isSelected = true;
                foxBrush.isSelected = false;
                wallBrush.isSelected = false;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Rabbit"), width: 50, height: 50);
            components.Add(bunnyBrush);

            // Fox brush
            foxBrush = new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 50, 210), () =>
            {
                brushSelected = BrushType.FOX;
                bunnyBrush.isSelected = false;
                foxBrush.isSelected = true;
                wallBrush.isSelected = false;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Fox"), width: 50, height: 50);
            components.Add(foxBrush);

            wallBrush = new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 130, 190), () =>
            {
                brushSelected = BrushType.WALL;
                bunnyBrush.isSelected = false;
                foxBrush.isSelected = false;
                wallBrush.isSelected = true;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Fox"), width: 50, height: 50);
            components.Add(wallBrush);

            // Button to empty the fields from animals
            Button emptyButton = new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 50, 270), () =>
            {
                for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
                {
                    for (int x = 0; x < GameVariables.CellsVerticallyCount; x++)
                    {
                        field[x, y].inhabitant = null;
                    }
                }
            }, text: "Empty", width: 150, height: 50);
            components.Add(emptyButton);

            // Label to count foxes and rabbits
            foxLabel = new Label("Number of rabbits: " + 0, new Vector2(GameVariables.GetGameCanvasWidth() + 50, 330));
            components.Add(foxLabel);
            rabbitLabel = new Label("Number of rabbits: " + 0, new Vector2(GameVariables.GetGameCanvasWidth() + 50, 350));
            components.Add(rabbitLabel);


            #endregion

            rectangleBlock = new Texture2D(GraphicsDevice, 1, 1);
            Color xnaColorBorder = new Color(255, 255, 255);
            rectangleBlock.SetData(new[] { xnaColorBorder });
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Creating the field
            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsVerticallyCount; x++)
                {
                    field[x, y] = new Cell(x, y, GraphicsDevice);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            tickCounter++;
            if (tickCounter % (60 / tickrate) == 0)
            {
                if (!paused)
                {
                    GameManager.Update(field);
                    generation++;
                }
            }

            // Calculations for labels
            if (field.CountInhabitants() == 0)
            {
                generation = 0;
            }
            else
            {
                generationLabel.text = "Generation: " + generation;
            }

            rabbitLabel.text = "Rabbits: " + field.CountRabbits();
            foxLabel.text = "Foxes: " + field.CountFoxes();


            MouseState currentMouseState = Mouse.GetState();

            // Mouse clicked
            if (previousMouseState.LeftButton == ButtonState.Released
            && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (Component component in components)
                {
                    if (component.GetType() == typeof(Button))
                    {
                        if (currentMouseState.X >= ((Button)component).position.X && currentMouseState.X <= ((Button)component).position.X + ((Button)component).width
                        && currentMouseState.Y >= ((Button)component).position.Y && currentMouseState.Y <= ((Button)component).position.Y + ((Button)component).height)
                        {
                            ((Button)component).onClick();
                        }
                    }
                }
            }

            // Mouse being held 
            if (brushSelected == BrushType.FOX || brushSelected == BrushType.BUNNY)
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
                    {
                        for (int x = 0; x < GameVariables.CellsVerticallyCount; x++)
                        {
                            if (currentMouseState.X >= field[x, y].posX * GameVariables.CellSize && currentMouseState.X <= field[x, y].posX * GameVariables.CellSize + GameVariables.CellSize
                            && currentMouseState.Y >= field[x, y].posY * GameVariables.CellSize && currentMouseState.Y <= field[x, y].posY * GameVariables.CellSize + GameVariables.CellSize
                            )
                            {
                                if (field[x, y].inhabitant == null)
                                {
                                    if (brushSelected == BrushType.BUNNY)
                                    {
                                        field[x, y].inhabitant = new Rabbit();
                                    }
                                    else if (brushSelected == BrushType.FOX)
                                    {
                                        field[x, y].inhabitant = new Fox();
                                    }
                                    //TODO: Implement walls
                                    else if (brushSelected == BrushType.WALL)
                                    {
                                        //field[x, y].inhabitant = new Wall();
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
            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsVerticallyCount; x++)
                {
                    _spriteBatch.Draw(rectangleBlock, new Rectangle(x * GameVariables.CellSize, y * GameVariables.CellSize, GameVariables.CellSize, GameVariables.CellSize), field[x, y].Color);
                }
            }

            foreach (Component component in components)
            {
                if (component.GetType() == typeof(Button))
                {
                    if (((Button)component).image != null)
                    {

                        if (((Button)component).isSelected)
                        {
                            _spriteBatch.Draw(rectangleBlock, new Rectangle(((int)((Button)component).position.X) - 4, ((int)((Button)component).position.Y) - 4, ((Button)component).width + 6, ((Button)component).height + 6), Color.Black);
                        }
                        _spriteBatch.Draw(((Button)component).image, ((Button)component).position, Color.White);
                    }
                    else
                    {
                        _spriteBatch.Draw(rectangleBlock, new Rectangle((int)((Button)component).position.X, (int)((Button)component).position.Y, ((Button)component).width, ((Button)component).height), Color.Gray);
                    }

                    _spriteBatch.DrawString(spriteFont, ((Button)component).text, new Vector2(((Button)component).position.X + ((Button)component).width / 4, ((Button)component).position.Y + ((Button)component).height / 3), Color.Black);
                }
                else if (component.GetType() == typeof(Label))
                {
                    _spriteBatch.DrawString(spriteFont, ((Label)component).text, new Vector2(component.position.X, component.position.Y), Color.Black);
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}