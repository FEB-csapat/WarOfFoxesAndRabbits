using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WarOfFoxesAndRabbits
{
    public class ComponentManager
    {
        #region Singleton
        private static ComponentManager instance = null;

        private ComponentManager() { }

        public static ComponentManager Instance
        {
            get
            {
                instance ??= new ComponentManager();
                return instance;
            }
        }
        #endregion

        #region Variables for components


        private readonly List<Panel> panels = new ();
        private readonly List<Button> pencilTypeButtons = new();
        private readonly List<Button> pencilSizeButtons = new();
        private readonly List<Component> components = new();

        public List<Component> GetAllButtons() { 

            List<Component> allButtons = new();
            allButtons.AddRange(pencilTypeButtons);
            allButtons.AddRange(pencilSizeButtons);
            allButtons.AddRange(components.Where(x => x is Button).ToList());

            return allButtons;
            
        }


        private Label generationLabel;

        private Label foxLabel;
        private Label rabbitLabel;

        private Label foxDeathLabel;
        private Label rabbitDeathLabel;
        private Label coordLabel;

        private Graph graph;

        private PencilType pencilSelected = PencilType.NONE;

        private PencilSizeType pencilSizeSelected = PencilSizeType.SMALL;

        #endregion


        public void Initialize(ContentManager content)
        {
            #region Initializing components

            // TODO: Generation counter resets with clear button
            generationLabel = new Label("Generation: " + GameManager.Instance.Generation, new Vector2(GameConstants.GameCanvasWidth + 50, 4));
            components.Add(generationLabel);

            // Button to regenerate the field
            Button regenerateButton = new Button(new Vector2(GameConstants.GameCanvasWidth + 50, 30), () =>
            {
                GameManager.Instance.GenerateField();
            }, text: "Regenerate", width: 150, height: 50);
            components.Add(regenerateButton);

            // Button to enable/disable lake generation
            Button lakeSwitchButton = new Button();
            lakeSwitchButton = new Button(new Vector2(GameConstants.GameCanvasWidth + 210, 30), () =>
            {
                GameManager.Instance.IsLakeEnabled = !GameManager.Instance.IsLakeEnabled;
                if (GameManager.Instance.IsLakeEnabled)
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
            pauseButton = new Button(new Vector2(GameConstants.GameCanvasWidth + 50, 90), () =>
            {
                GameManager.Instance.IsPaused = !GameManager.Instance.IsPaused;
                if (GameManager.Instance.IsPaused)
                {
                    pauseButton.Text = "Continue";
                }
                else
                {
                    pauseButton.Text = "Pause";
                }
            }, text: "Pause", width: 150, height: 50);
            components.Add(pauseButton);

            // Label to show the tickrate
            Label tickrateLabel = new Label(GameManager.Instance.Tickrate.ToString() + " tick", new Vector2(GameConstants.GameCanvasWidth + 102, 166));
            components.Add(tickrateLabel);


            // Label to show the fox death counter
            foxDeathLabel = new Label("Fox death: " + GameManager.Instance.FoxDeathCounter, new Vector2(GameConstants.GameCanvasWidth + 50, 740));
            components.Add(foxDeathLabel);

            // Label to show the rabbit death counter
            rabbitDeathLabel = new Label("Rabbit death: " + GameManager.Instance.RabbitDeathCounter, new Vector2(GameConstants.GameCanvasWidth + 50, 760));
            components.Add(rabbitDeathLabel);


            

            // Increment tickrate button
            components.Add(new Button(new Vector2(GameConstants.GameCanvasWidth + 160, 150), () =>
            {
                GameManager.Instance.IncrementTickrate();
                tickrateLabel.Text = GameManager.Instance.Tickrate.ToString() + " tick";
            }, text: "+", width: 50, height: 50));

            // Decrement tickrate button
            components.Add(new Button(new Vector2(GameConstants.GameCanvasWidth + 50, 150), () =>
            {
                GameManager.Instance.DecrementTickrate();

                tickrateLabel.Text = GameManager.Instance.Tickrate.ToString() + " tick";
            }, text: "-", width: 50, height: 50));

            //Min tickrate
            components.Add(new Button(new Vector2(GameConstants.GameCanvasWidth + 220, 150), () =>
            {
                GameManager.Instance.SetTickrateToMinimum();
                tickrateLabel.Text = GameManager.Instance.Tickrate.ToString() + " tick";
            }, text: "Min", width: 50, height: 50));
            // Max tickrate
            components.Add(new Button(new Vector2(GameConstants.GameCanvasWidth + 280, 150), () =>
            {
                GameManager.Instance.SetTickrateToMaximum();
                tickrateLabel.Text = GameManager.Instance.Tickrate.ToString() + " tick";
            }, text: "Max", width: 50, height: 50));


            // Button to clear the fields from animals
            Button clearButton = new Button(new Vector2(GameConstants.GameCanvasWidth + 50, 210), () =>
            {
                GameManager.Instance.ClearAnimals();
                GameManager.Instance.ResetDeathCounters();
                GameManager.Instance.ResetGeneration();
                generationLabel.Text = "Generation: " + GameManager.Instance.Generation;
            }, text: "Clear", width: 150, height: 50);
            components.Add(clearButton);



            // Button to draw animals on the field
            Button rabbitPencil = new();
            Button foxPencil = new();
            Button wallPencil = new();
            Button waterPencil = new();
            Button grassPencil = new();
            rabbitPencil = new Button(new Vector2(GameConstants.GameCanvasWidth + 50, 280), () =>
            {
                pencilSelected = PencilType.BUNNY;
                // Icon from https://icons8.com
            }, image: content.Load<Texture2D>("Images/Rabbit"), width: 50, height: 50, id: PencilType.BUNNY);
            pencilTypeButtons.Add(rabbitPencil);

            // Fox pencil
            foxPencil = new Button(new Vector2(GameConstants.GameCanvasWidth + 50, 340), () =>
            {

                pencilSelected = PencilType.FOX;
                // Icon from https://icons8.com
            }, image: content.Load<Texture2D>("Images/Fox"), width: 50, height: 50, id: PencilType.FOX);
            pencilTypeButtons.Add(foxPencil);

            // Wall pencil
            wallPencil = new Button(new Vector2(GameConstants.GameCanvasWidth + 110, 280), () =>
            {
                pencilSelected = PencilType.WALL;
                // Icon from https://icons8.com
            }, image: content.Load<Texture2D>("Images/Wall"), width: 50, height: 50, id: PencilType.WALL);
            pencilTypeButtons.Add(wallPencil);

            // Water pencil
            waterPencil = new Button(new Vector2(GameConstants.GameCanvasWidth + 110, 340), () =>
            {
                pencilSelected = PencilType.WATER;
                // Icon from https://icons8.com
            }, image: content.Load<Texture2D>("Images/Water"), width: 50, height: 50, id: PencilType.WATER);
            pencilTypeButtons.Add(waterPencil);


            // Grass pencil
            grassPencil = new Button(new Vector2(GameConstants.GameCanvasWidth + 170, 280), () =>
            {
                pencilSelected = PencilType.GRASS;
                // Icon from https://icons8.com
            }, image: content.Load<Texture2D>("Images/Grass"), width: 50, height: 50, id: PencilType.GRASS);
            pencilTypeButtons.Add(grassPencil);



            // Small brush button
            pencilSizeButtons.Add(new Button(new Vector2(GameConstants.GameCanvasWidth + 280, 280), () =>
            {
                pencilSizeSelected = PencilSizeType.SMALL;
            }, image: content.Load<Texture2D>("Images/Pencil1"), width: 50, height: 50, id: PencilSizeType.SMALL));

            // Medium brush button
            pencilSizeButtons.Add(new Button(new Vector2(GameConstants.GameCanvasWidth + 280, 340), () =>
            {
                pencilSizeSelected = PencilSizeType.MEDIUM;
            }, image: content.Load<Texture2D>("Images/Pencil2"), width: 50, height: 50, id: PencilSizeType.MEDIUM));

            // Large brush button
            pencilSizeButtons.Add(new Button(new Vector2(GameConstants.GameCanvasWidth + 280, 400), () =>
            {
                pencilSizeSelected = PencilSizeType.LARGE;
            }, image: content.Load<Texture2D>("Images/Pencil3"), width: 50, height: 50,id: PencilSizeType.LARGE));


            


            // Label to count foxes and rabbits
            foxLabel = new Label("Foxes: " + 0, new Vector2(GameConstants.GameCanvasWidth + 50, 470));
            components.Add(foxLabel);
            rabbitLabel = new Label("Rabbits: " + 0, new Vector2(GameConstants.GameCanvasWidth + 50, 490));
            components.Add(rabbitLabel);

            


            graph = new Graph(new Vector2(GameConstants.GameCanvasWidth + 50, 530));

            Panel pencilPanel = new Panel(new Vector2(GameConstants.GameCanvasWidth + 40, 270), 300, 190, new Color(130, 130, 130));
            Panel graphPanel = new Panel(new Vector2((int)graph.Position.X, (int)graph.Position.Y), graph.Width, graph.Height, Color.DimGray);

            panels.Add(pencilPanel);
            panels.Add(graphPanel);

            coordLabel = new Label($"(x: ,y:)", new Vector2(0, 0));

            #endregion
        }

        public void Update()
        {
            rabbitLabel.Text = "Rabbits: " + GameManager.Instance.RabbitCounter;
            foxLabel.Text = "Foxes: " + GameManager.Instance.FoxCounter;

            rabbitDeathLabel.Text = "Rabbit deaths: " + GameManager.Instance.RabbitDeathCounter;
            foxDeathLabel.Text = "Fox deaths: " + GameManager.Instance.FoxDeathCounter;

            GameManager.Instance.IncrementGeneration();
            if (GameManager.Instance.RabbitCounter + GameManager.Instance.FoxCounter == 0)
            {
                GameManager.Instance.ResetGeneration();
            }
            else
            {
                generationLabel.Text = "Generation: " + GameManager.Instance.Generation;
            }

            if (GameManager.Instance.RabbitCounter > 0)
            {
                graph.AddData(AnimalType.RABBIT, GameManager.Instance.RabbitCounter);

            }
            if (GameManager.Instance.FoxCounter > 0)
            {
                graph.AddData(AnimalType.FOX, GameManager.Instance.FoxCounter);
            }

            graph.Update();
        }

        public void UpdateCoordinationLabel(MouseState currentMouseState)
        {
            coordLabel.Position = new Vector2(currentMouseState.X + 12, currentMouseState.Y + 12);
            coordLabel.Text = $"({1 + currentMouseState.X / GameConstants.CellSize}, {1 + currentMouseState.Y / GameConstants.CellSize})";
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D rectangleBlock, SpriteFont spriteFont)
        {
            
            foreach (Panel panel in panels)
            {
                spriteBatch.Draw(rectangleBlock, new Rectangle((int)panel.Position.X, (int)panel.Position.Y, panel.Width, panel.Height), panel.Color);
            }

            foreach (Button button in pencilTypeButtons)
            {
                if (button.Id != null && (PencilType)button.Id == pencilSelected)
                {
                    spriteBatch.Draw(rectangleBlock, new Rectangle(((int)button.Position.X) - 4, ((int)button.Position.Y) - 4, button.Width + 6, button.Height + 6), Color.Black);
                }
                spriteBatch.Draw(button.ImageTexture, button.Position, Color.White);
            }

            foreach (Button button in pencilSizeButtons)
            {
                if (button.Id != null && (PencilSizeType)button.Id == pencilSizeSelected)
                {
                    spriteBatch.Draw(rectangleBlock, new Rectangle(((int)button.Position.X) - 4, ((int)button.Position.Y) - 4, button.Width + 6, button.Height + 6), Color.Black);
                }
                spriteBatch.Draw(button.ImageTexture, button.Position, Color.White);
            }

            foreach (Component component in components)
            {
                if (component is Button button)
                {
                    spriteBatch.Draw(rectangleBlock, new Rectangle((int)button.Position.X, (int)button.Position.Y, button.Width, button.Height), Color.Gray);
                    
                    spriteBatch.DrawString(spriteFont, button.Text, new Vector2(button.Position.X + button.Width / 4, button.Position.Y + button.Height / 3), Color.Black);
                }
                else if (component is Label label)
                {
                    spriteBatch.DrawString(spriteFont, label.Text, new Vector2(component.Position.X, component.Position.Y), Color.Black);
                }
            }

            #region Draw graph

           
            foreach (GraphData d in graph.Datas)
            {
                spriteBatch.Draw(rectangleBlock, new Rectangle((int)d.Position.X, (int)d.Position.Y, GameConstants.GraphRectSize, GameConstants.GraphRectSize), d.Color);
            }

            #endregion

            #region Draw coordinate label

            if (coordLabel.Position.X - 10 <= GameConstants.GameCanvasWidth
                && coordLabel.Position.X - 10 >= 0
                && coordLabel.Position.Y - 10 >= 0
                && coordLabel.Position.Y - 10 <= GameConstants.GameCanvasWidth)
            {
                spriteBatch.DrawString(spriteFont, coordLabel.Text, new Vector2(coordLabel.Position.X, coordLabel.Position.Y), Color.Black);
            }

            #endregion
        }


        public void CheckIfButtonWasClicked(MouseState currentMouseState)
        {
            foreach (Button component in GetAllButtons())
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

        public void CheckIfCanDraw(MouseState currentMouseState)
        {
            if (pencilSelected != PencilType.NONE)
            {
                for (int y = 0; y < GameConstants.CellsVerticallyCount; y++)
                {
                    for (int x = 0; x < GameConstants.CellsVerticallyCount; x++)
                    {
                        if (currentMouseState.X >= x * GameConstants.CellSize && currentMouseState.X <= x * GameConstants.CellSize + GameConstants.CellSize
                        && currentMouseState.Y >= y * GameConstants.CellSize && currentMouseState.Y <= y * GameConstants.CellSize + GameConstants.CellSize
                        )
                        {
                            DrawWithPencilAt(x, y);
                        }
                    }
                }
            }
        }

        void DrawWithPencilAt(int x, int y)
        {
            int size = (int)pencilSizeSelected;
            for (int py = -size; py <= size; py++)
            {
                for (int px = -size; px <= size; px++)
                {
                    if (y + py >= 0 && x + px >= 0
                    && y + py < GameConstants.CellsVerticallyCount && x + px < GameConstants.CellsHorizontallyCount
                    && GameManager.Instance.GetFieldCell(x, y).Animal == null)
                    {
                        switch (pencilSelected)
                        {
                            case PencilType.BUNNY:
                                GameManager.Instance.SetFieldCellAnimal(x + px, y + py, new Rabbit());
                                break;
                            case PencilType.FOX:
                                GameManager.Instance.SetFieldCellAnimal(x + px, y + py, new Fox());
                                break;
                            case PencilType.WALL:
                                GameManager.Instance.SetFieldCellMatter(x + px, y + py, new Wall());
                                break;
                            case PencilType.WATER:
                                GameManager.Instance.SetFieldCellMatter(x + px, y + py, new Water(GameConstants.minWaterDepth));
                                break;
                            case PencilType.GRASS:
                                GameManager.Instance.SetFieldCellMatter(x + px, y + py, new Grass());
                                break;
                        }
                    }
                }
            }
        }

    }
}
