using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WarOfFoxesAndRabbits
{

    // Manages the game's rules
    public class GameManager
    {
        private abstract class AnimalHandler
        {
            public abstract Cell[,] Check(Cell[,] field, int x, int y);

            protected void Birth<T>(ref List<Cell> surroundingCellsToBirth, ref Cell cellWithFatherAnimal, ref Animal fatherAnimal) where T : Animal
            {
                if (fatherAnimal != null && surroundingCellsToBirth.Count > 0)
                {
                    int r = GameVariables.Random.Next(0, surroundingCellsToBirth.Count);
                    surroundingCellsToBirth[r].Animal = Activator.CreateInstance<T>();
                    surroundingCellsToBirth.RemoveAt(r);
                    fatherAnimal.HasProduced = true;
                    cellWithFatherAnimal.Animal.HasProduced = true;
                }
            }

            protected Cell Move(ref List<Cell> surroundingCellsToMove, ref Cell cellWithAnimal)
            {
                int ran = GameVariables.Random.Next(0, surroundingCellsToMove.Count);
                surroundingCellsToMove[ran].Animal = cellWithAnimal.Animal;
                surroundingCellsToMove[ran].Animal.HasMoved = true;
                cellWithAnimal.Animal = null;

                // Return where it moved
                return surroundingCellsToMove[ran];
            }

            protected bool CanBeFather<T>( Animal mother, Animal possibleFather)
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
            public override Cell[,] Check(Cell[,] field, int x, int y)
            {
                Rabbit rabbitOnCurrentCell = (Rabbit)field[x, y].Animal;

                if (rabbitOnCurrentCell.IsDead())
                {
                    field[x, y].Animal = null;
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
                            && y + py < GameVariables.CellsVerticallyCount && x + px < GameVariables.CellsHorizontallyCount
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
                        if (emptySurroundingCells.Exists(x=>x.Matter != null && x.Matter is Grass) )
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
                        && y + py < GameVariables.CellsVerticallyCount && x + px < GameVariables.CellsHorizontallyCount
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
                int ran = GameVariables.Random.Next(0, surroundingCellsToHunt.Count);
                (cellWithFox.Animal as Fox).Eat();
                cellWithFox.Animal.HasAte = true;
                surroundingCellsToHunt[ran].Animal = cellWithFox.Animal;
                cellWithFox.Animal = null;

                // Return where it moved
                return surroundingCellsToHunt[ran];
            }

            // Perform rules on each cell containing a fox
            public override Cell[,] Check(Cell[,] field, int x, int y)
            {
                if (field[x, y].Animal.IsDead())
                {
                    field[x, y].Animal = null;
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


        private readonly FoxHandler foxHandler = new FoxHandler();
        private readonly RabbitHandler rabbitHandler = new RabbitHandler();

        private static GameManager instance = null;

        private GameManager()
        {
        }

        public static GameManager Instance
        {
            get
            {
                instance ??= new GameManager();
                return instance;
            }
        }


        // Modify the fields according to the rules
        public Cell[,] Update(Cell[,] field)
        {
            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsHorizontallyCount; x++)
                {
                    if (field[x, y].Animal != null)
                    {
                        field[x, y].Animal.Update();
                        if (field[x, y].Animal is Rabbit)
                        {
                            field = rabbitHandler.Check(field, x, y);
                        }
                        else if (field[x, y].Animal is Fox)
                        {
                            field = foxHandler.Check(field, x, y);
                        }
                    }
                    else if (field[x, y].Matter is Grass)
                    {
                        ((Grass)field[x, y].Matter).Grow();
                    }
                }
            }

            // Reset properties of animals
            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsHorizontallyCount; x++)
                {
                    if (field[x, y].Animal != null)
                    {
                        field[x, y].Animal.HasMoved = false;
                        field[x, y].Animal.HasProduced = false;
                    }
                }
            }
            return field;
        }
    }
}
