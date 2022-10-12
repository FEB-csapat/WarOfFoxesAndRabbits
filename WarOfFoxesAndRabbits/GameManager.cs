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

                    if (grass.Stage >= 1 && rabbitOnCurrentCell.Sate < 5)
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
        static Random rnd = new Random();
        static Cell saved;
        private static void PossibleTileChecking(Cell[,] field, int x, int y, out List<Cell> foxSurroundingCellsToMove, out List<Cell> foxSurroundingCellsToMoveWater, out List<Cell> surroundingCellsToHunt, ref Fox fatherFox)
        {
            foxSurroundingCellsToMove = new List<Cell>();
            foxSurroundingCellsToMoveWater = new List<Cell>();
            surroundingCellsToHunt = new List<Cell>();


            for (int py = -2; py <= 2; py++)
            {
                for (int px = -2; px <= 2; px++)
                {
                    if (y + py >= 0 && x + px >= 0
                    && y + py < GameVariables.CellsVerticallyCount && x + px < GameVariables.CellsHorizontallyCount
                    && (px != 0 || py != 0))
                    {
                        // Check cells to move or to hunt
                        if (field[x + px, y + py].animal == null)
                        {
                            if (field[x + px, y + py].matter is Water)
                            {
                                foxSurroundingCellsToMoveWater.Add(field[x + px, y + py]);
                            }
                            else if (field[x + px, y + py].matter is Grass)
                            {
                                foxSurroundingCellsToMove.Add(field[x + px, y + py]);
                            }
                        }
                        else if (field[x + px, y + py].animal.GetType() == typeof(Rabbit)
                                && field[x, y].animal.Sate < 7) //The foodlevel of the fox is examined here
                        {
                            surroundingCellsToHunt.Add(field[x + px, y + py]);
                        }

                        // Check mates to mate with
                        if (fatherFox == null
                        && field[x + px, y + py].animal != null
                        && field[x,y].animal.canBreed()
                        && field[x + px, y + py].animal.canBreed()
                        && field[x + px, y + py].animal.GetType() == typeof(Fox)
                        && !field[x + px, y + py].animal.hasProduced)
                        {
                            fatherFox= (Fox)field[x + px, y + py].animal;
                        }
                    }
                }
            }
        }
        static public Cell[,] Check(Cell[,] field, int x, int y)
        {
            // Fox dies
            if (field[x, y].animal.IsDead())
            {
                field[x, y].animal = null;
                return field;  //rabbithandler contains the same code?
            }
            if (!field[x, y].animal.hasMoved)
            {
                List<Cell> foxSurroundingCellsToMove, foxSurroundingCellsToMoveWater, surroundingCellsToHunt;

                Fox fatherFox = null;

                //checking surrounding cells
                PossibleTileChecking(field, x, y, out foxSurroundingCellsToMove, out foxSurroundingCellsToMoveWater, out surroundingCellsToHunt, ref fatherFox);
                //First checking that in the first search are any bunnies
                if (surroundingCellsToHunt.Count != 0) //If there are, hunt (1.step)
                {
                    int ran = rnd.Next(0, surroundingCellsToHunt.Count);
                    (field[x, y].animal as Fox).Eat();
                    field[x, y].animal.hasAte = true;
                    surroundingCellsToHunt[ran].animal = field[x, y].animal; //A bunny died :(
                    saved = surroundingCellsToHunt[ran];
                    field[x, y].animal = null;
                    //The original cell have been updated!
                    PossibleTileChecking(field, saved.posX, saved.posY, out foxSurroundingCellsToMove, out foxSurroundingCellsToMoveWater, out surroundingCellsToHunt, ref fatherFox);
                    //Second step, hunted already so just a move (2. step)
                        ran = rnd.Next(0, foxSurroundingCellsToMove.Count);
                        field[saved.posX, saved.posY].animal.hasMoved = true;
                        foxSurroundingCellsToMove[ran].animal = field[saved.posX, saved.posY].animal;
                        field[saved.posX, saved.posY].animal = null;

                }
                else if (foxSurroundingCellsToMove.Count > 1)// Moving fox on a random cell in a 2block radius (1.step)
                {
                    int ran;
                    ran = rnd.Next(0, foxSurroundingCellsToMove.Count);
                    foxSurroundingCellsToMove[ran].animal = field[x, y].animal;
                    saved = foxSurroundingCellsToMove[ran];
                    field[x, y].animal = null;  //In this case it can have a hunt


                    PossibleTileChecking(field, saved.posX, saved.posY, out foxSurroundingCellsToMove,out foxSurroundingCellsToMoveWater, out surroundingCellsToHunt, ref fatherFox);
                    //Checking if it can hunt or not
                    if (surroundingCellsToHunt.Count != 0) //It can hunt (2. step)
                    {
                        ran = rnd.Next(0, surroundingCellsToHunt.Count);
                        (field[saved.posX, saved.posY].animal as Fox).Eat();
                        field[saved.posX, saved.posY].animal.hasAte = true;
                        field[saved.posX, saved.posY].animal.hasMoved = true;
                        surroundingCellsToHunt[ran].animal = field[saved.posX, saved.posY].animal; //A bunny died :(
                        field[saved.posX, saved.posY].animal = null;
                    }
                    else //It can't hunt, so it makes the second move (2. step)
                    {
                        if (!(field[saved.posX, saved.posY].matter is Wall) && foxSurroundingCellsToMove.Count != 0)
                        {
                            ran = rnd.Next(0, foxSurroundingCellsToMove.Count);
                            field[saved.posX, saved.posY].animal.hasMoved = true;
                            foxSurroundingCellsToMove[ran].animal = field[saved.posX, saved.posY].animal;
                            field[saved.posX, saved.posY].animal = null;
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
