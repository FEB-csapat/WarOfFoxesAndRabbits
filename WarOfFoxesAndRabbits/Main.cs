using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace WarOfFoxesAndRabbits
{
    public class Main : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D rectangleBlock;
        SpriteFont spriteFont;


        readonly Cell[,] field = new Cell[GameVariables.CellsHorizontallyCount, GameVariables.CellsVerticallyCount];

        MouseState previousMouseState;

        bool paused = false;
        bool enabledLakes = false;

        // Variables for components
        readonly List<Component> components = new();
        Label generationLabel;
        int generation = 0;
        Label foxLabel;
        Label rabbitLabel;

        Graph graph;

        PencilType pencilSelected = PencilType.NONE;

        static long tickCounter = 0;
        static int tickrate = 1;

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
            #region Components

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
            Label tickrateLabel = new Label(tickrate.ToString() + " tick", new Vector2(GameVariables.GameCanvasWidth + 100, 166));
            components.Add(tickrateLabel);

            // Increment tickrate button
            components.Add(new Button(new Vector2(GameVariables.GameCanvasWidth + 150, 150), () =>
            {
                if (tickrate < 59)
                {
                    tickrate += 2;
                    tickrateLabel.Text = tickrate.ToString() + " tick";
                }
            }, text: "+", width: 50, height: 50));

            // Decrement tickrate button
            components.Add(new Button(new Vector2(GameVariables.GameCanvasWidth + 50, 150), () =>
            {
                if (tickrate > 1)
                {
                    tickrate -= 2;
                    tickrateLabel.Text = tickrate.ToString() + " tick";
                }
            }, text: "-", width: 50, height: 50));

            // Max tickrate
            components.Add(new Button(new Vector2(GameVariables.GameCanvasWidth + 270, 150), () =>
            {
                tickrate = 59;
                tickrateLabel.Text = tickrate.ToString() + " tick";
            }, text: "Max", width: 50, height: 50));
            //Min tickrate
            components.Add(new Button(new Vector2(GameVariables.GameCanvasWidth + 210, 150), () =>
            {
                tickrate = 1;
                tickrateLabel.Text = tickrate.ToString() + " tick";
            }, text: "Min", width: 50, height: 50));

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
                generation = 0;
                generationLabel.Text = "Generation: " + generation;
            }, text: "Clear", width: 150, height: 50);
            components.Add(clearButton);

            // Label to count foxes and rabbits
            foxLabel = new Label("Number of rabbits: " + 0, new Vector2(GameVariables.GameCanvasWidth + 50, 330));
            components.Add(foxLabel);
            rabbitLabel = new Label("Number of rabbits: " + 0, new Vector2(GameVariables.GameCanvasWidth + 50, 350));
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

                    if (enabledLakes && noise.GetNoise(x, y) >= 0.55)
                    {
                        hasWater = true;
                    }

                    field[x, y] = new Cell(new Vector2(x,y), matter: hasWater ? new Water() : null);
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

                    // Calculations for components
                    if (field.CountAnimals() == 0)
                    {
                        generation = 0;
                    }
                    else
                    {
                        generationLabel.Text = "Generation: " + generation;
                    }

                    int rabbitCount = field.CountRabbits();
                    int foxCount = field.CountFoxes();

                    rabbitLabel.Text = "Rabbits: " + rabbitCount;
                    foxLabel.Text = "Foxes: " + foxCount;


                    if (rabbitCount > 0)
                    {
                        graph.AddData(AnimalType.RABBIT, rabbitCount);

                    }
                    if (foxCount > 0)
                    {
                        graph.AddData(AnimalType.FOX, foxCount);
                    }

                    graph.Update();
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
                        if (currentMouseState.X >= ((Button)component).Position.X && currentMouseState.X <= ((Button)component).Position.X + ((Button)component).Width
                        && currentMouseState.Y >= ((Button)component).Position.Y && currentMouseState.Y <= ((Button)component).Position.Y + ((Button)component).Height)
                        {
                            ((Button)component).onClick();
                        }
                    }
                }
            }

            // Mouse being held
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
                                            field[x, y].Matter = new Water();
                                            break;
                                        case PencilType.GRASS:
                                            field[x, y].Matter = new Grass();
                                            break;
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
                    if (((Button)component).ImageTexture != null)
                    {

                        if (((Button)component).IsSelected)
                        {
                            _spriteBatch.Draw(rectangleBlock, new Rectangle(((int)((Button)component).Position.X) - 4, ((int)((Button)component).Position.Y) - 4, ((Button)component).Width + 6, ((Button)component).Height + 6), Color.Black);
                        }
                        _spriteBatch.Draw(((Button)component).ImageTexture, ((Button)component).Position, Color.White);
                    }
                    else
                    {
                        _spriteBatch.Draw(rectangleBlock, new Rectangle((int)((Button)component).Position.X, (int)((Button)component).Position.Y, ((Button)component).Width, ((Button)component).Height), Color.Gray);
                    }

                    _spriteBatch.DrawString(spriteFont, ((Button)component).Text, new Vector2(((Button)component).Position.X + ((Button)component).Width / 4, ((Button)component).Position.Y + ((Button)component).Height / 3), Color.Black);
                }
                else if (component.GetType() == typeof(Label))
                {
                    _spriteBatch.DrawString(spriteFont, ((Label)component).Text, new Vector2(component.Position.X, component.Position.Y), Color.Black);
                }
            }

            // Draw graph

            // background
            _spriteBatch.Draw(rectangleBlock, new Rectangle((int)graph.Position.X, (int)graph.Position.Y, graph.Width, graph.Height), Color.DimGray);

            foreach (GraphData d in graph.Datas)
            {
                _spriteBatch.Draw(rectangleBlock, new Rectangle((int)d.position.X, (int)d.position.Y, GameVariables.GraphRectSize, GameVariables.GraphRectSize), d.Color);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}