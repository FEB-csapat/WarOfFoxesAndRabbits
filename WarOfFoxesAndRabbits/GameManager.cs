using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WarOfFoxesAndRabbits
{
    // Manages the game's rules
    public class GameManager
    {
        #region Handlers

        private abstract class AnimalHandler
        {
            public abstract Cell[,] Manage(Cell[,] field, int x, int y);

            protected void Birth<T>(ref List<Cell> surroundingCellsToBirth, ref Cell cellWithFatherAnimal, ref Animal fatherAnimal) where T : Animal
            {
                if (fatherAnimal != null && surroundingCellsToBirth.Count > 0)
                {
                    int r = GameConstants.Random.Next(0, surroundingCellsToBirth.Count);
                    surroundingCellsToBirth[r].Animal = Activator.CreateInstance<T>();
                    surroundingCellsToBirth.RemoveAt(r);
                    fatherAnimal.HasProduced = true;
                    cellWithFatherAnimal.Animal.HasProduced = true;
                }
            }

            protected Cell Move(ref List<Cell> surroundingCellsToMove, ref Cell cellWithAnimal)
            {
                int ran = GameConstants.Random.Next(0, surroundingCellsToMove.Count);
                surroundingCellsToMove[ran].Animal = cellWithAnimal.Animal;
                surroundingCellsToMove[ran].Animal.HasMoved = true;
                cellWithAnimal.Animal = null;

                // Return where it moved
                return surroundingCellsToMove[ran];
            }

            protected bool CanBeFather<T>(Animal mother, Animal possibleFather)
            {
                if (possibleFather != null
                    && mother.CanBreed()
                    && possibleFather.CanBreed()
                    && possibleFather is T
                    && !possibleFather.HasProduced)
                {
                    return true;
                }
                return false;
            }
        }

        // Manage rabbits according to the rules
        private class RabbitHandler : AnimalHandler
        {
            // Perform rules on each cell containing a rabbit
            public override Cell[,] Manage(Cell[,] field, int x, int y)
            {
                Rabbit rabbitOnCurrentCell = (Rabbit)field[x, y].Animal;

                if (rabbitOnCurrentCell.IsDead())
                {
                    field[x, y].Animal = null;
                    Instance.RabbitDeathCounter++;
                    return field;
                }

                if (!rabbitOnCurrentCell.HasMoved)
                {
                    if (field[x, y].Matter is Grass grass && grass.Stage >= 1 && rabbitOnCurrentCell.CanEat())
                    {
                        if (grass.Stage >= 2)
                        {
                            rabbitOnCurrentCell.Eat(2);
                            grass.GrassEaten(2);
                        }
                        else
                        {
                            rabbitOnCurrentCell.Eat(1);
                            grass.GrassEaten(1);
                        }
                    }


                    List<Cell> emptySurroundingCells = new List<Cell>();

                    Animal fatherRabbit = null;

                    for (int py = -1; py <= 1; py++)
                    {
                        for (int px = -1; px <= 1; px++)
                        {
                            if (y + py >= 0 && x + px >= 0
                            && y + py < GameConstants.CellsVerticallyCount && x + px < GameConstants.CellsHorizontallyCount
                            && (px != 0 || py != 0))
                            {
                                #region Find cells to move

                                if (field[x + px, y + py].Animal == null)
                                {
                                    if (field[x + px, y + py].Matter is not Wall)
                                    {
                                        emptySurroundingCells.Add(field[x + px, y + py]);
                                    }
                                }

                                #endregion

                                #region Find father

                                if (fatherRabbit == null && CanBeFather<Rabbit>(rabbitOnCurrentCell, field[x + px, y + py].Animal))
                                {
                                    fatherRabbit = field[x + px, y + py].Animal;
                                }

                                #endregion
                            }
                        }
                    }

                    if (emptySurroundingCells.Count != 0)
                    {
                        Birth<Rabbit>(ref emptySurroundingCells, ref field[x, y], ref fatherRabbit);
                    }

                    #region Move rabbit


                    // Move rabbit to cell with best grass on it

                    List<Cell> optionalCells = new List<Cell>();

                    if (emptySurroundingCells.Count != 0)
                    {
                        if (emptySurroundingCells.Exists(x => x.Matter != null && x.Matter is Grass))
                        {
                            double bestGrassStage = Math.Floor(emptySurroundingCells.Where(x => x.Matter is Grass).Max(y => ((Grass)y.Matter).Stage));
                            foreach (Cell cell in emptySurroundingCells)
                            {
                                if (cell.Matter is not Wall)
                                {
                                    if (cell.Matter is Grass && ((Grass)cell.Matter).Stage >= bestGrassStage)
                                    {
                                        optionalCells.Add(cell);
                                    }
                                }
                            }
                        }
                        else
                        {
                            optionalCells = emptySurroundingCells;
                        }

                        Move(ref optionalCells, ref field[x, y]);
                    }
                    #endregion
                }
                return field;
            }
        }

        // TODO: nerf foxes
        // Manage foxes according to the rules
        private class FoxHandler : AnimalHandler
        {
            // Fills the surroundingCells lists to perform actions latter on it
            private void FindSurroundingCells(Cell[,] field, int x, int y, out List<Cell> surroundingCellsToMove, out List<Cell> surroundingCellsToHunt, ref Animal fatherFox)
            {
                surroundingCellsToMove = new List<Cell>();
                surroundingCellsToHunt = new List<Cell>();

                int vision = 2;

                if (((Fox)field[x, y].Animal).Sate < 20)
                {
                    vision = 3;
                }
                else if (((Fox)field[x, y].Animal).Sate < 10)
                {
                    vision = 4;
                }

                for (int py = -vision; py <= vision; py++)
                {
                    for (int px = -vision; px <= vision; px++)
                    {
                        if (y + py >= 0 && x + px >= 0
                        && y + py < GameConstants.CellsVerticallyCount && x + px < GameConstants.CellsHorizontallyCount
                        && (px != 0 || py != 0))
                        {
                            // Find cells to move
                            if (field[x + px, y + py].Animal == null)
                            {
                                if (field[x + px, y + py].Matter is not Wall)
                                {
                                    surroundingCellsToMove.Add(field[x + px, y + py]);
                                }
                            }
                            // Find cells to hunt
                            else if (field[x + px, y + py].Animal is Rabbit
                                 && ((Fox)field[x, y].Animal).CanEat())
                            {
                                surroundingCellsToHunt.Add(field[x + px, y + py]);
                            }

                            // Find mates to mate with
                            if (fatherFox == null && CanBeFather<Fox>(field[x, y].Animal, field[x + px, y + py].Animal))
                            {
                                fatherFox = field[x + px, y + py].Animal;
                            }
                        }
                    }
                }
            }


            // Hunts. Moves fox to the rabbit's cell. Rabbit dies
            private Cell Hunt(ref List<Cell> surroundingCellsToHunt, ref Cell cellWithFox)
            {
                int ran = GameConstants.Random.Next(0, surroundingCellsToHunt.Count);
                (cellWithFox.Animal as Fox).Eat();
                cellWithFox.Animal.HasAte = true;
                surroundingCellsToHunt[ran].Animal = cellWithFox.Animal;
                cellWithFox.Animal = null;

                // Return where it moved
                return surroundingCellsToHunt[ran];
            }

            // Perform rules on each cell containing a fox
            public override Cell[,] Manage(Cell[,] field, int x, int y)
            {
                if (field[x, y].Animal.IsDead())
                {
                    field[x, y].Animal = null;
                    Instance.FoxDeathCounter++;
                    return field;
                }

                if (!field[x, y].Animal.HasMoved)
                {
                    Cell nextCellWhereFoxMoved;
                    List<Cell> surroundingCellsToMove, surroundingCellsToHunt;
                    Animal fatherFox = null;
                    FindSurroundingCells(field, x, y, out surroundingCellsToMove, out surroundingCellsToHunt, ref fatherFox);

                    if (surroundingCellsToMove.Count > 0)
                    {
                        Birth<Fox>(ref surroundingCellsToMove, ref field[x, y], ref fatherFox);
                    }

                    // Check if there are any rabbits to hunt in the surrounding cells
                    if (surroundingCellsToHunt.Count > 0)
                    {
                        nextCellWhereFoxMoved = Hunt(ref surroundingCellsToHunt, ref field[x, y]);

                        // has moved successfully
                        if (nextCellWhereFoxMoved != null)
                        {
                            FindSurroundingCells(field, nextCellWhereFoxMoved.PosX, nextCellWhereFoxMoved.PosY, out surroundingCellsToMove, out surroundingCellsToHunt, ref fatherFox);
                            if (surroundingCellsToMove.Count > 0)
                            {
                                nextCellWhereFoxMoved = Move(ref surroundingCellsToMove, ref field[nextCellWhereFoxMoved.PosX, nextCellWhereFoxMoved.PosY]);
                            }
                        }
                    }
                    // There were no rabbits in the surrounding cells, so just move if there is a cell to move
                    else if (surroundingCellsToMove.Count > 0)
                    {
                        nextCellWhereFoxMoved = Move(ref surroundingCellsToMove, ref field[x, y]);

                        // has moved successfully
                        if (nextCellWhereFoxMoved != null)
                        {
                            FindSurroundingCells(field, nextCellWhereFoxMoved.PosX, nextCellWhereFoxMoved.PosY, out surroundingCellsToMove, out surroundingCellsToHunt, ref fatherFox);

                            //Check if it can hunt or not
                            if (surroundingCellsToHunt.Count > 0)
                            {
                                nextCellWhereFoxMoved = Hunt(ref surroundingCellsToHunt, ref field[nextCellWhereFoxMoved.PosX, nextCellWhereFoxMoved.PosY]);
                            }
                            else
                            {
                                //It can't hunt, so it makes the second move
                                if (surroundingCellsToMove.Count > 0)
                                {
                                    nextCellWhereFoxMoved = Move(ref surroundingCellsToMove, ref field[nextCellWhereFoxMoved.PosX, nextCellWhereFoxMoved.PosY]);
                                }
                            }
                        }
                    }
                }
                return field;
            }
        }

        #endregion

        #region Counters
        public int RabbitCounter { get; private set; } = 0;
        public int FoxCounter { get; private set; } = 0;


        public long FoxDeathCounter { get; private set; } = 0;
        public long RabbitDeathCounter { get; private set; } = 0;

        public void ResetDeathCounters()
        {
            FoxDeathCounter = 0;
            RabbitDeathCounter = 0;
        }
        #endregion

        #region Ticks

        public long TickCounter { get; set; } = 0;
        public long Tickrate { get; private set; } = 1;

        public void SetTickrateToMinimum()
        {
            Tickrate = 1;
        }

        public void SetTickrateToMaximum()
        {
            Tickrate = 60;
        }

        public void IncrementTickrate()
        {
            if (Tickrate + 2 <= 60)
            {
                Tickrate += 2;
            }
            else
            {
                Tickrate = 60;
            }
        }

        public void DecrementTickrate()
        {
            if (Tickrate - 2 > 0)
            {
                Tickrate -= 2;
            }
            else
            {
                Tickrate = 1;
            }
        }

        #endregion

        #region Generation

        public int Generation { get; private set; } = 0;

        public void IncrementGeneration()
        {
            Generation++;
        }

        public void ResetGeneration()
        {
            Generation = 0;
        }

        #endregion

        #region Field

        private readonly Cell[,] field = new Cell[GameConstants.CellsHorizontallyCount, GameConstants.CellsVerticallyCount];


        public void SetFieldCellAnimal(int x, int y, Animal animal)
        {
            field[x, y].Animal = animal;
        }

        public void SetFieldCellMatter(int x, int y, Matter matter)
        {
            field[x, y].Matter = matter;
        }

        public Cell GetFieldCell(int x, int y)
        {
            return field[x, y];
        }

        public void ClearAnimals()
        {
            for (int y = 0; y < GameConstants.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameConstants.CellsVerticallyCount; x++)
                {
                    if (field[x, y].Animal is not null)
                    {
                        field[x, y].Animal = null;
                    }
                }
            }
        }

        public void GenerateField()
        {
            FastNoiseLite noise = new FastNoiseLite();
            if (IsLakeEnabled)
            {
                noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
                noise.SetSeed(GameConstants.Random.Next(0, 1000000));
            }

            for (int y = 0; y < GameConstants.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameConstants.CellsVerticallyCount; x++)
                {
                    bool hasWater = false;

                    float waterDepth = noise.GetNoise(x, y);

                    if (IsLakeEnabled && waterDepth >= GameConstants.minWaterDepth)
                    {
                        hasWater = true;
                    }

                    field[x, y] = new Cell(new Vector2(x, y), matter: hasWater ? new Water(waterDepth) : null);
                }
            }
        }

        #endregion

        public bool IsPaused { get; set; } = false;
        public bool IsLakeEnabled { get; set; } = false;


        private readonly FoxHandler foxHandler = new FoxHandler();
        private readonly RabbitHandler rabbitHandler = new RabbitHandler();

        #region Singleton
        private static GameManager instance = null;

        private GameManager() { }

        public static GameManager Instance
        {
            get
            {
                instance ??= new GameManager();
                return instance;
            }
        }
        #endregion

        public void Initialize()
        {
            GenerateField();
        }

        // Update the fields according to the rules
        public void Update()
        {
            RabbitCounter = 0;
            FoxCounter = 0;
            for (int y = 0; y < GameConstants.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameConstants.CellsHorizontallyCount; x++)
                {
                    if (field[x, y].Animal != null)
                    {
                        field[x, y].Animal.Update();
                        if (field[x, y].Animal is Rabbit)
                        {
                            rabbitHandler.Manage(field, x, y);
                            RabbitCounter++;
                        }
                        else if (field[x, y].Animal is Fox)
                        {
                            foxHandler.Manage(field, x, y);
                            FoxCounter++;
                        }
                    }
                    else if (field[x, y].Matter is Grass grass)
                    {
                        grass.Grow();
                    }
                }
            }

            // Reset properties of animals
            for (int y = 0; y < GameConstants.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameConstants.CellsHorizontallyCount; x++)
                {
                    if (field[x, y].Animal != null)
                    {
                        field[x, y].Animal.HasMoved = false;
                        field[x, y].Animal.HasProduced = false;
                    }
                }
            }
        }
    }
}
