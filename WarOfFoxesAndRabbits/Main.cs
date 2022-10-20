using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;

namespace WarOfFoxesAndRabbits
{
    public class Main : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D rectangleBlock;
        private SpriteFont spriteFont;

        private readonly Cell[,] field = new Cell[GameVariables.CellsHorizontallyCount, GameVariables.CellsVerticallyCount];

        private MouseState previousMouseState;

        private bool paused = false;
        private bool enabledLakes = false;

        #region Variables for components

        private readonly List<Component> components = new();
        private Label generationLabel;
        private int generation = 0;
        private Label foxLabel;
        private Label rabbitLabel;

        private Label foxDeathLabel;
        private Label rabbitDeathLabel;
        private Label cordLabel;

        private Graph graph;

        private PencilType pencilSelected = PencilType.NONE;

        private PencilSizeType pencilSizeSelected = PencilSizeType.SMALL;

        #endregion

        private static long tickCounter = 0;
        private static int tickrate = 1;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = GameVariables.WindowWidth,
                PreferredBackBufferHeight = GameVariables.WindowHeight
            };
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
            #region Initializing components

            spriteFont = Content.Load<SpriteFont>("Fonts/Arial");

            // TODO: Generation counter resets with clear button
            generationLabel = new Label("Generation: " + generation, new Vector2(GameVariables.GameCanvasWidth + 50, 4));
            components.Add(generationLabel);

            // Button to regenerate the field
            Button regenerateButton = new Button(new Vector2(GameVariables.GameCanvasWidth + 50, 30), () =>
            {
                GenerateField();
            }, text: "Regenerate", width: 150, height: 50);
            components.Add(regenerateButton);

            // Button to enable/disable lake generation
            Button lakeSwitchButton = new Button();
            lakeSwitchButton = new Button(new Vector2(GameVariables.GameCanvasWidth + 210, 30), () =>
            {
                enabledLakes = !enabledLakes;
                if (enabledLakes)
                {
                    lakeSwitchButton.Text = "Disable lake";
                }
                else
                {
                    lakeSwitchButton.Text = "Enable lake";
                }
            }, text: "Enable lake", width: 150, height: 50);
            components.Add(lakeSwitchButton);

            // Button to pause the game
            Button pauseButton = new Button();
            pauseButton = new Button(new Vector2(GameVariables.GameCanvasWidth + 50, 90), () =>
            {
                paused = !paused;
                if (paused) pauseButton.Text = "Continue";
                else pauseButton.Text = "Pause";
            }, text: "Pause", width: 150, height: 50);
            components.Add(pauseButton);

            // Label to show the tickrate
            Label tickrateLabel = new Label(tickrate.ToString() + " tick", new Vector2(GameVariables.GameCanvasWidth + 102, 166));
            components.Add(tickrateLabel);


            // Label to show the fox death counter
            foxDeathLabel = new Label("Fox death: " + GameManager.Instance.FoxDeathCounter, new Vector2(GameVariables.GameCanvasWidth + 50, 650));
            components.Add(foxDeathLabel);

            // Label to show the rabbit death counter
            rabbitDeathLabel = new Label("Rabbit death: " + GameManager.Instance.RabbitDeathCounter, new Vector2(GameVariables.GameCanvasWidth + 50, 700));
            components.Add(rabbitDeathLabel);

            // Small brush button
            components.Add(new Button(new Vector2(GameVariables.GameCanvasWidth + 280, 210), () =>
            {
                pencilSizeSelected = PencilSizeType.SMALL;
            }, text: "Small Brush", width: 50, height: 50));

            // Medium brush button
            components.Add(new Button(new Vector2(GameVariables.GameCanvasWidth + 280, 270), () =>
            {
                pencilSizeSelected = PencilSizeType.MEDIUM;
            }, text: "Medium Brush", width: 50, height: 50));

            // Large brush button
            components.Add(new Button(new Vector2(GameVariables.GameCanvasWidth + 280, 330), () =>
            {
                pencilSizeSelected = PencilSizeType.LARGE;
            }, text: "Large Brush", width: 50, height: 50));
            cordLabel = new Label($"(x: ,y:)", new Vector2(0,0));
            // Increment tickrate button
            components.Add(new Button(new Vector2(GameVariables.GameCanvasWidth + 160, 150), () =>
            {
                if (tickrate + 2 <= 60)
                {
                    tickrate += 2;
                }
                else
                {
                    tickrate = 60;
                }
                tickrateLabel.Text = tickrate.ToString() + " tick";
            }, text: "+", width: 50, height: 50));

            // Decrement tickrate button
            components.Add(new Button(new Vector2(GameVariables.GameCanvasWidth + 50, 150), () =>
            {
                if (tickrate - 2 > 0)
                {
                    tickrate -= 2;
                }
                else
                {
                    tickrate = 1;
                }
                tickrateLabel.Text = tickrate.ToString() + " tick";
            }, text: "-", width: 50, height: 50));

            //Min tickrate
            components.Add(new Button(new Vector2(GameVariables.GameCanvasWidth + 220, 150), () =>
            {
                tickrate = 1;
                tickrateLabel.Text = tickrate.ToString() + " tick";
            }, text: "Min", width: 50, height: 50));
            // Max tickrate
            components.Add(new Button(new Vector2(GameVariables.GameCanvasWidth + 280, 150), () =>
            {
                tickrate = 60;
                tickrateLabel.Text = tickrate.ToString() + " tick";
            }, text: "Max", width: 50, height: 50));


            // Button to draw animals on the field
            Button rabbitPencil = new();
            Button foxPencil = new();
            Button wallPencil = new();
            Button waterPencil = new();
            Button grassPencil = new();
            rabbitPencil = new Button(new Vector2(GameVariables.GameCanvasWidth + 50, 210), () =>
            {
                pencilSelected = PencilType.BUNNY;
                rabbitPencil.IsSelected = true;
                foxPencil.IsSelected = false;
                wallPencil.IsSelected = false;
                waterPencil.IsSelected = false;
                grassPencil.IsSelected = false;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Rabbit"), width: 50, height: 50);
            components.Add(rabbitPencil);

            // Fox pencil
            foxPencil = new Button(new Vector2(GameVariables.GameCanvasWidth + 50, 270), () =>
            {

                pencilSelected = PencilType.FOX;
                rabbitPencil.IsSelected = false;
                foxPencil.IsSelected = true;
                wallPencil.IsSelected = false;
                waterPencil.IsSelected = false;
                grassPencil.IsSelected = false;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Fox"), width: 50, height: 50);
            components.Add(foxPencil);

            // Wall pencil
            wallPencil = new Button(new Vector2(GameVariables.GameCanvasWidth + 110, 210), () =>
            {
                pencilSelected = PencilType.WALL;
                rabbitPencil.IsSelected = false;
                foxPencil.IsSelected = false;
                wallPencil.IsSelected = true;
                waterPencil.IsSelected = false;
                grassPencil.IsSelected = false;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Wall"), width: 50, height: 50);
            components.Add(wallPencil);

            // Water pencil
            waterPencil = new Button(new Vector2(GameVariables.GameCanvasWidth + 110, 270), () =>
            {
                pencilSelected = PencilType.WATER;
                rabbitPencil.IsSelected = false;
                foxPencil.IsSelected = false;
                wallPencil.IsSelected = false;
                waterPencil.IsSelected = true;
                grassPencil.IsSelected = false;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Water"), width: 50, height: 50);
            components.Add(waterPencil);


            // Grass pencil
            grassPencil = new Button(new Vector2(GameVariables.GameCanvasWidth + 170, 210), () =>
            {
                pencilSelected = PencilType.GRASS;
                rabbitPencil.IsSelected = false;
                foxPencil.IsSelected = false;
                wallPencil.IsSelected = false;
                waterPencil.IsSelected = false;
                grassPencil.IsSelected = true;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Grass"), width: 50, height: 50);
            components.Add(grassPencil);
            
            // Button to clear the fields from animals
            Button clearButton = new Button(new Vector2(GameVariables.GameCanvasWidth + 50, 385), () =>
            {
                for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
                {
                    for (int x = 0; x < GameVariables.CellsVerticallyCount; x++)
                    {
                        if (field[x, y].Animal is not null)
                        {
                            field[x, y].Animal = null;
                        }

                    }
                }
                GameManager.Instance.ResetDeathCounter();
                generation = 0;
                generationLabel.Text = "Generation: " + generation;
            }, text: "Clear", width: 150, height: 50);
            components.Add(clearButton);
            
            // Label to count foxes and rabbits
            foxLabel = new Label("Foxes: " + 0, new Vector2(GameVariables.GameCanvasWidth + 50, 330));
            components.Add(foxLabel);
            rabbitLabel = new Label("Rabbits: " + 0, new Vector2(GameVariables.GameCanvasWidth + 50, 350));
            components.Add(rabbitLabel);

            graph = new Graph(new Vector2(GameVariables.GameCanvasWidth + 50, 450));


            #endregion

            rectangleBlock = new Texture2D(GraphicsDevice, 1, 1);
            Color xnaColorBorder = new Color(255, 255, 255);
            rectangleBlock.SetData(new[] { xnaColorBorder });
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            GenerateField();
        }

        protected void GenerateField()
        {
            FastNoiseLite noise = new FastNoiseLite();
            if (enabledLakes)
            {
                noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
                noise.SetSeed(GameVariables.Random.Next(0, 1000000));
            }

            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsVerticallyCount; x++)
                {
                    bool hasWater = false;

                    float waterDepth = noise.GetNoise(x, y);

                    if (enabledLakes && waterDepth >= GameVariables.minWaterDepth)
                    {
                        hasWater = true;
                    }

                    field[x, y] = new Cell(new Vector2(x, y), matter: hasWater ? new Water(waterDepth) : null);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // If escape button is pressed
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            tickCounter++;
            if (tickCounter % (60 / tickrate) == 0)
            {
                if (!paused)
                {
                    GameManager.Instance.Update(field);

                    #region Updating components

                    int rabbitCount = field.CountRabbits();
                    int foxCount = field.CountFoxes();

                    rabbitLabel.Text = "Rabbits: " + rabbitCount;
                    foxLabel.Text = "Foxes: " + foxCount;

                    rabbitDeathLabel.Text = "Rabbit deaths: " + GameManager.Instance.RabbitDeathCounter;
                    foxDeathLabel.Text = "Fox deaths: " + GameManager.Instance.FoxDeathCounter;

                    generation++;
                    if (rabbitCount + foxCount == 0)
                    {
                        generation = 0;
                    }
                    else
                    {
                        generationLabel.Text = "Generation: " + generation;
                    }

                    if (rabbitCount > 0)
                    {
                        graph.AddData(AnimalType.RABBIT, rabbitCount);

                    }
                    if (foxCount > 0)
                    {
                        graph.AddData(AnimalType.FOX, foxCount);
                    }

                    graph.Update();

                    #endregion
                }
            }
            MouseState currentMouseState = Mouse.GetState();
                cordLabel.Position = new Vector2(currentMouseState.X + 10, currentMouseState.Y + 10);
                cordLabel.Text = $"(x:{currentMouseState.X} ,y:{currentMouseState.Y})";
            #region Mouse clicked

            if (previousMouseState.LeftButton == ButtonState.Released
            && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (Component component in components)
                {
                    if (component is Button button)
                    {
                        if (currentMouseState.X >= button.Position.X && currentMouseState.X <= button.Position.X + button.Width
                        && currentMouseState.Y >= button.Position.Y && currentMouseState.Y <= button.Position.Y + button.Height)
                        {
                            button.OnClick();
                        }
                    }
                }
            }

            #endregion

            #region Mouse being held

            if (pencilSelected != PencilType.NONE)
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
                    {
                        for (int x = 0; x < GameVariables.CellsVerticallyCount; x++)
                        {

                            if (currentMouseState.X >= x * GameVariables.CellSize && currentMouseState.X <= x * GameVariables.CellSize + GameVariables.CellSize
                                && currentMouseState.Y >= y * GameVariables.CellSize && currentMouseState.Y <= y * GameVariables.CellSize + GameVariables.CellSize
                                )
                            {
                                if (pencilSizeSelected == PencilSizeType.SMALL)
                                {
                                    if (field[x, y].Animal == null)
                                    {
                                        switch (pencilSelected)
                                        {
                                            case PencilType.BUNNY:
                                                field[x, y].Animal = new Rabbit();
                                                break;
                                            case PencilType.FOX:
                                                field[x, y].Animal = new Fox();
                                                break;
                                            case PencilType.WALL:
                                                field[x, y].Matter = new Wall();
                                                break;
                                            case PencilType.WATER:
                                                field[x, y].Matter = new Water(GameVariables.minWaterDepth);
                                                break;
                                            case PencilType.GRASS:
                                                field[x, y].Matter = new Grass();
                                                break;
                                        }
                                    }
                                }

                                else if (pencilSizeSelected == PencilSizeType.MEDIUM)
                                {
                                    for (int py = -1; py <= 1; py++)
                                    {
                                        for (int px = -1; px <= 1; px++)
                                        {
                                            if (y + py >= 0 && x + px >= 0
                                            && y + py < GameVariables.CellsVerticallyCount && x + px < GameVariables.CellsHorizontallyCount
                                            && field[x, y].Animal == null)
                                            {
                                                switch (pencilSelected)
                                                {
                                                    case PencilType.BUNNY:
                                                        field[x+px, y+py].Animal = new Rabbit();
                                                        break;
                                                    case PencilType.FOX:
                                                        field[x + px, y + py].Animal = new Fox();
                                                        break;
                                                    case PencilType.WALL:
                                                        field[x + px, y + py].Matter = new Wall();
                                                        break;
                                                    case PencilType.WATER:
                                                        field[x + px, y + py].Matter = new Water(GameVariables.minWaterDepth);
                                                        break;
                                                    case PencilType.GRASS:
                                                        field[x + px, y + py].Matter = new Grass();
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }

                                else if (pencilSizeSelected == PencilSizeType.LARGE)
                                {
                                    for (int py = -2; py <= 2; py++)
                                    {
                                        for (int px = -2; px <= 2; px++)
                                        {
                                            if (y + py >= 0 && x + px >= 0
                                            && y + py < GameVariables.CellsVerticallyCount && x + px < GameVariables.CellsHorizontallyCount
                                            && field[x, y].Animal == null)
                                            {
                                                switch (pencilSelected)
                                                {
                                                    case PencilType.BUNNY:
                                                        field[x + px, y + py].Animal = new Rabbit();
                                                        break;
                                                    case PencilType.FOX:
                                                        field[x + px, y + py].Animal = new Fox();
                                                        break;
                                                    case PencilType.WALL:
                                                        field[x + px, y + py].Matter = new Wall();
                                                        break;
                                                    case PencilType.WATER:
                                                        field[x + px, y + py].Matter = new Water(GameVariables.minWaterDepth);
                                                        break;
                                                    case PencilType.GRASS:
                                                        field[x + px, y + py].Matter = new Grass();
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            previousMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Set background color
            GraphicsDevice.Clear(Color.DarkGray);
            _spriteBatch.Begin();

            #region Draw cells

            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsVerticallyCount; x++)
                {
                    _spriteBatch.Draw(rectangleBlock, new Rectangle(x * GameVariables.CellSize, y * GameVariables.CellSize, GameVariables.CellSize, GameVariables.CellSize), field[x, y].Color);
                }
            }

            #endregion

            #region Draw components

            foreach (Component component in components)
            {
                if (component is Button button)
                {
                    if (button.ImageTexture != null)
                    {
                        // Draw black "border" to selected button
                        if (button.IsSelected)
                        {
                            _spriteBatch.Draw(rectangleBlock, new Rectangle(((int)button.Position.X) - 4, ((int)button.Position.Y) - 4, button.Width + 6, button.Height + 6), Color.Black);
                        }
                        _spriteBatch.Draw(button.ImageTexture, button.Position, Color.White);
                    }
                    else
                    {
                        _spriteBatch.Draw(rectangleBlock, new Rectangle((int)button.Position.X, (int)button.Position.Y, button.Width, button.Height), Color.Gray);
                    }
                    // Draw text of button
                    _spriteBatch.DrawString(spriteFont, button.Text, new Vector2(button.Position.X + button.Width / 4, button.Position.Y + button.Height / 3), Color.Black);
                }
                else if (component is Label label)
                {
                    _spriteBatch.DrawString(spriteFont, label.Text, new Vector2(component.Position.X, component.Position.Y), Color.Black);
                }
            }

            #endregion

            #region Draw graph

            // background
            _spriteBatch.Draw(rectangleBlock, new Rectangle((int)graph.Position.X, (int)graph.Position.Y, graph.Width, graph.Height), Color.DimGray);

            foreach (GraphData d in graph.Datas)
            {
                _spriteBatch.Draw(rectangleBlock, new Rectangle((int)d.Position.X, (int)d.Position.Y, GameVariables.GraphRectSize, GameVariables.GraphRectSize), d.Color);
            }

            #endregion
            if (cordLabel.Position.X-10<=GameVariables.GameCanvasWidth
                && cordLabel.Position.X-10>=0
                && cordLabel.Position.Y-10>=0
                && cordLabel.Position.Y-10<=GameVariables.GameCanvasWidth)
            {
                _spriteBatch.DrawString(spriteFont, cordLabel.Text, new Vector2(cordLabel.Position.X, cordLabel.Position.Y), Color.Black);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}