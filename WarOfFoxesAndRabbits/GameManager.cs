using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WarOfFoxesAndRabbits
{

    // TODO: refactor
    class RabbitHandler
    {
        static public Cell[,] Check(Cell[,] field, int x, int y)
        {
            Rabbit rabbitOnCurrentCell = (Rabbit)field[x, y].animal;

            // Rabbit dies
            if (rabbitOnCurrentCell.IsDead())
            {
                field[x, y].animal = null;
                return field;
            }

            if (!rabbitOnCurrentCell.hasMoved)
            {
                // rules
                if (field[x, y].matter is Grass)
                {
                    Grass grass = (Grass)field[x, y].matter;

                    if (grass.Stage >= 1 && rabbitOnCurrentCell.CanEat())
                    {
                        if (grass.Stage == 2 && rabbitOnCurrentCell.Sate < 4)
                        {
                            (field[x, y].animal as Rabbit).Eat(2);
                            grass.grassEaten();
                        }
                        else if (grass.Stage == 2 && rabbitOnCurrentCell.Sate >= 4)
                        {
                            //Can't eat sate too high!
                        }
                        else
                        {
                            (field[x, y].animal as Rabbit).Eat(1);
                            grass.grassEaten();
                        }
                    }
                    if (grass.Stage < 1)
                    {
                        List<Cell> surroundingCellsToMove = new List<Cell>();

                        List<Cell> surroundingCellsToMoveWater = new List<Cell>();

                        Rabbit fatherRabbit = null;

                        for (int py = -1; py <= 1; py++)
                        {
                            for (int px = -1; px <= 1; px++)
                            {
                                if (y + py >= 0 && x + px >= 0
                                && y + py < GameVariables.CellsVerticallyCount && x + px < GameVariables.CellsHorizontallyCount
                                && (px != 0 || py != 0))
                                {
                                    // Check cells to move
                                    if (field[x + px, y + py].animal == null)
                                    {
                                        if (field[x + px, y + py].matter is Water)
                                        {
                                            surroundingCellsToMoveWater.Add(field[x + px, y + py]);
                                        }
                                        else if (field[x + px, y + py].matter is Grass)
                                        {
                                            surroundingCellsToMove.Add(field[x + px, y + py]);
                                        }
                                    }

                                    // Check mates to mate with
                                    if (fatherRabbit == null
                                    && field[x + px, y + py].animal != null
                                    && rabbitOnCurrentCell.canBreed()
                                    && field[x + px, y + py].animal.canBreed()
                                    && field[x + px, y + py].animal.GetType() == typeof(Rabbit)
                                    && !field[x + px, y + py].animal.hasProduced)
                                    {
                                        fatherRabbit = (Rabbit)field[x + px, y + py].animal;
                                    }
                                }
                            }
                        }

                        if (surroundingCellsToMove.Count != 0)
                        {
                            // Birth rabbit to empty cell
                            if (fatherRabbit != null)
                            {
                                int r = GameVariables.Random.Next(0, surroundingCellsToMove.Count);
                                surroundingCellsToMove[r].animal = new Rabbit();
                                surroundingCellsToMove.RemoveAt(r);
                                fatherRabbit.hasProduced = true;
                                rabbitOnCurrentCell.hasProduced = true;
                            }

                            // Move rabbit to cell with best grass on it
                            if (surroundingCellsToMove.Count != 0)
                            {
                                List<Cell> optionalCells = new List<Cell>();
                                bool hasWaterCell = false;

                                // TODO: sometimes throws exception
                                double bestGrassStage = Math.Floor(surroundingCellsToMove.Where(x => x.matter is Grass).Max(y => ((Grass)y.matter).Stage));
                                foreach (Cell cell in surroundingCellsToMove)
                                {
                                    if (cell.matter is Water && !hasWaterCell)
                                    {
                                        hasWaterCell = false;
                                        optionalCells.Add(cell); ;
                                    }

                                    if (cell.matter is Grass && ((Grass)cell.matter).Stage >= bestGrassStage)
                                    {
                                        optionalCells.Add(cell); ;
                                    }
                                }

                                optionalCells.AddRange(surroundingCellsToMoveWater);

                                int ran = GameVariables.Random.Next(0, optionalCells.Count);

                                optionalCells[ran].animal = field[x, y].animal;
                                field[x, y].animal = null;
                                ((Rabbit)optionalCells[ran].animal).hasMoved = true;
                            }
                        }
                    }
                }

            }
            return field;
        }
    }

    // TODO: refactor
    class FoxHandler
    {
        static private void Birth(ref List<Cell> surroundingCellsToBirth, ref Cell cellWithFatherFox, ref Fox fatherFox)
        {
            if (fatherFox != null)
            {
                int r = GameVariables.Random.Next(0, surroundingCellsToBirth.Count);
                surroundingCellsToBirth[r].animal = new Fox();
                surroundingCellsToBirth.RemoveAt(r);
                fatherFox.hasProduced = true;
                cellWithFatherFox.animal.hasProduced = true;
            }
        }
        // Fills the surroundingCells lists to perform actions latter on it
        private static void FindSurroundingCells(Cell[,] field, int x, int y, out List<Cell> surroundingCellsToMove, out List<Cell> surroundingCellsToHunt, ref Fox fatherFox)
        {
            surroundingCellsToMove = new List<Cell>();
            surroundingCellsToHunt = new List<Cell>();

            for (int py = -2; py <= 2; py++)
            {
                for (int px = -2; px <= 2; px++)
                {
                    if (y + py >= 0 && x + px >= 0
                    && y + py < GameVariables.CellsVerticallyCount && x + px < GameVariables.CellsHorizontallyCount
                    && (px != 0 || py != 0))
                    {
                        // Check cells to move
                        if (field[x + px, y + py].animal == null)
                        {
                            if (field[x + px, y + py].matter is Grass)
                            {
                                surroundingCellsToMove.Add(field[x + px, y + py]);
                            }
                        }
                        // Check cells to hunt
                        else if (field[x + px, y + py].animal.GetType() == typeof(Rabbit)
                             && ((Fox)field[x, y].animal).CanEat())
                        {
                            surroundingCellsToHunt.Add(field[x + px, y + py]);
                        }

                        // Check mates to mate with
                        if (fatherFox == null
                        && field[x + px, y + py].animal != null
                        && field[x, y].animal.canBreed()
                        && field[x + px, y + py].animal.canBreed()
                        && field[x + px, y + py].animal.GetType() == typeof(Fox)
                        && !field[x + px, y + py].animal.hasProduced)
                        {
                            fatherFox = (Fox)field[x + px, y + py].animal;
                        }
                    }
                }
            }
            
        }


        static private void Move(ref List<Cell> surroundingCellsToMove, ref Cell cellWithFox, out Cell nextCellWhereFoxMoved)
        {
            int ran = GameVariables.Random.Next(0, surroundingCellsToMove.Count);
            nextCellWhereFoxMoved = surroundingCellsToMove[ran];
            surroundingCellsToMove[ran].animal = cellWithFox.animal;
            surroundingCellsToMove[ran].animal.hasMoved = true;
            cellWithFox.animal = null;
        }

        // Hunts. Moves fox to the rabbit's cell. Rabbit dies
        static private void Hunt(ref List<Cell> surroundingCellsToHunt, ref Cell cellWithFox, out Cell nextCellWhereFoxMoved)
        {
            int ran = GameVariables.Random.Next(0, surroundingCellsToHunt.Count);
            (cellWithFox.animal as Fox).Eat();
            cellWithFox.animal.hasAte = true;
            surroundingCellsToHunt[ran].animal = cellWithFox.animal;
            nextCellWhereFoxMoved = surroundingCellsToHunt[ran];
            cellWithFox.animal = null;
        }

        // perform rules on each cell containing a fox
        static public Cell[,] Check(Cell[,] field, int x, int y)
        {
            // if fox is dead, remove it
            if (field[x, y].animal.IsDead())
            {
                field[x, y].animal = null;
                return field;
            }


            if (!field[x, y].animal.hasMoved)
            {
                Cell nextCellWhereFoxMoved;
                List<Cell> surroundingCellsToMove, surroundingCellsToHunt;
                Fox fatherFox = null;
                FindSurroundingCells(field, x, y, out surroundingCellsToMove, out surroundingCellsToHunt, ref fatherFox);
               

                // Check if there are bunnies to hunt in the surrounding cells
                if (surroundingCellsToHunt.Count > 0)
                {
                    Birth(ref surroundingCellsToMove, ref field[x, y], ref fatherFox);

                    Hunt(ref surroundingCellsToHunt, ref field[x, y], out nextCellWhereFoxMoved);

                    FindSurroundingCells(field, nextCellWhereFoxMoved.posX, nextCellWhereFoxMoved.posY, out surroundingCellsToMove, out surroundingCellsToHunt, ref fatherFox);

                    Move(ref surroundingCellsToMove, ref field[nextCellWhereFoxMoved.posX, nextCellWhereFoxMoved.posY], out nextCellWhereFoxMoved);

                }
                // There were no rabbits in the surrounding cells, so just move if there is cell to move
                else if (surroundingCellsToMove.Count > 0)
                {
                    Move(ref surroundingCellsToMove, ref field[x, y], out nextCellWhereFoxMoved);

                    FindSurroundingCells(field, nextCellWhereFoxMoved.posX, nextCellWhereFoxMoved.posY, out surroundingCellsToMove, out surroundingCellsToHunt, ref fatherFox);

                    //Checking if it can hunt or not
                    if (surroundingCellsToHunt.Count > 0)
                    {
                        Hunt(ref surroundingCellsToHunt, ref field[nextCellWhereFoxMoved.posX, nextCellWhereFoxMoved.posY], out nextCellWhereFoxMoved);
                    }
                    else
                    {
                        //It can't hunt, so it makes the second move
                        if (surroundingCellsToMove.Count > 0)
                        {  
                            Move(ref surroundingCellsToMove, ref field[nextCellWhereFoxMoved.posX, nextCellWhereFoxMoved.posY], out nextCellWhereFoxMoved);
                        }
                    }
                }
            }
            return field;
        }
    }
    // Manages the game's rules
    class GameManager
    {
        // Modify the fields according to the rules
        public static Cell[,] Update(Cell[,] field)
        {
            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsHorizontallyCount; x++)
                {
                    if (field[x, y].animal != null)
                    {
                        field[x, y].animal.Update();
                        if (field[x, y].animal is Rabbit)
                        {
                            field = RabbitHandler.Check(field, x, y);
                        }
                        else if (field[x, y].animal is Fox)
                        {
                            field = FoxHandler.Check(field, x, y);
                        }
                    }
                    else if (field[x, y].matter is Grass)
                    {
                        ((Grass)field[x, y].matter).Grow();
                    }
                }
            }


            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsHorizontallyCount; x++)
                {
                    if (field[x, y].animal != null && field[x, y].animal is Animal)
                    {
                        field[x, y].animal.hasMoved = false;
                        field[x, y].animal.hasProduced = false;
                    }
                }
            }
            return field;
        }
    }
}
