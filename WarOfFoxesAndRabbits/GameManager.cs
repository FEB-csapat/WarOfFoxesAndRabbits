using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WarOfFoxesAndRabbits
{
    class RabbitHandler
    {
        static Random rnd = new Random();
        static public Cell[,] Check(Cell[,] field, int x, int y)
        {

            // Rabbit dies
            if (field[x, y].inhabitant.IsDead())
            {
                field[x, y].inhabitant = null;
                return field;
            }


            if (!field[x, y].inhabitant.hasMoved)
            {
                // rules
                if (field[x, y].Grass >= 1 && field[x, y].inhabitant.Sate < 5)
                {
                    if (field[x, y].Grass == 2 && field[x, y].inhabitant.Sate < 4)
                    {
                        (field[x, y].inhabitant as Rabbit).Eat(2);
                        field[x, y].grassEaten();
                    }
                    else if (field[x, y].Grass == 2 && field[x, y].inhabitant.Sate >= 4)
                    {
                        //Can't eat sate too high!
                    }
                    else
                    {
                        (field[x, y].inhabitant as Rabbit).Eat(1);
                        field[x, y].grassEaten();
                    }
                }
                if (field[x, y].Grass < 1)
                {

                    List<Cell> surroundingCellsToMove = new List<Cell>();

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
                                if (field[x + px, y + py].inhabitant == null)
                                {
                                    surroundingCellsToMove.Add(field[x + px, y + py]);
                                }

                                // Check mates to mate with
                                if (fatherRabbit == null
                                && field[x, y].inhabitant.Sate == 5
                                && field[x + px, y + py].inhabitant != null
                                && field[x + px, y + py].inhabitant.GetType() == typeof(Rabbit)
                                && !field[x + px, y + py].inhabitant.hasProduced)
                                {
                                    if (field[x + px, y + py].inhabitant.Sate >= 4)
                                    {
                                        fatherRabbit = (Rabbit)field[x + px, y + py].inhabitant;
                                    }
                                }
                            }
                        }
                    }

                    if (surroundingCellsToMove.Count != 0)
                    {
                        // Birth rabbit to empty cell
                        if (fatherRabbit != null)
                        {

                            int r = new Random().Next(0, surroundingCellsToMove.Count);
                            surroundingCellsToMove[r].inhabitant = new Rabbit();
                            surroundingCellsToMove.RemoveAt(r);

                            fatherRabbit.hasProduced = true;
                            field[x, y].inhabitant.hasProduced = true;
                        }

                        // Move rabbit to cell with best grass on it
                        if (surroundingCellsToMove.Count != 0)
                        {
                            List<Cell> optionalCells = surroundingCellsToMove.Where(x => x.Grass == surroundingCellsToMove.Max(y => y.Grass)).ToList();

                            //System.Diagnostics.Debug.WriteLine($"Lehetőségek: {surroundingCellsToMove.Count}||Opcionális: {optionalCells.Count}");

                            int ran = rnd.Next(0, optionalCells.Count);

                            optionalCells[ran].inhabitant = field[x, y].inhabitant;
                            //System.Diagnostics.Debug.WriteLine($"Maximum szám: {optionalCells.Count}||Választott: {ran}");
                            field[x, y].inhabitant = null;
                            optionalCells[ran].inhabitant.hasMoved = true;
                        }

                    }
                }
            }
            return field;
        }
    }
    //foxhandler
    class FoxHandler
    {
        static Random rnd = new Random();
        static Cell saved;
        private static void PossibleTileChecking(Cell[,] field, int x, int y, out List<Cell> FoxsurroundingCellsToMove, out List<Cell> surroundingCellsToHunt)
        {
            FoxsurroundingCellsToMove = new List<Cell>();
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
                        if (field[x + px, y + py].inhabitant == null)
                        {
                            FoxsurroundingCellsToMove.Add(field[x + px, y + py]);
                        }
                        else if (field[x + px, y + py].inhabitant.GetType() == typeof(Rabbit)
                                && field[x, y].inhabitant.Sate < 7) //The foodlevel is examined here
                        {
                            surroundingCellsToHunt.Add(field[x + px, y + py]);
                        }
                    }
                }
            }
        }
        static public Cell[,] Check(Cell[,] field, int x, int y)
        {
            // Fox dies
            if (field[x, y].inhabitant.IsDead())
            {
                field[x, y].inhabitant = null;
                return field;  //rabbithandler contains the same code?
            }
            if (!field[x, y].inhabitant.hasMoved)
            {
                List<Cell> FoxsurroundingCellsToMove, surroundingCellsToHunt;

                //checking surrounding cells
                PossibleTileChecking(field, x, y, out FoxsurroundingCellsToMove, out surroundingCellsToHunt);
                //First checking that in the first search are any bunnies
                if (surroundingCellsToHunt.Count != 0) //If there are, hunt
                {
                    int ran = rnd.Next(0, surroundingCellsToHunt.Count);
                    (field[x, y].inhabitant as Fox).Eat(3);
                    field[x, y].inhabitant.hasAte = true;
                    surroundingCellsToHunt[ran].inhabitant = field[x, y].inhabitant; //A bunny died :(
                    saved = surroundingCellsToHunt[ran];
                    field[x, y].inhabitant = null;
                    //The original cell have been updated!
                    PossibleTileChecking(field, saved.posX, saved.posY, out FoxsurroundingCellsToMove, out surroundingCellsToHunt);
                    //Second step, hunted already so just a move
                    ran = rnd.Next(0, FoxsurroundingCellsToMove.Count);
                    field[saved.posX, saved.posY].inhabitant.hasMoved = true;
                    FoxsurroundingCellsToMove[ran].inhabitant = field[saved.posX, saved.posY].inhabitant;
                    field[saved.posX, saved.posY].inhabitant = null;

                }
                else if (FoxsurroundingCellsToMove.Count != 0)// Moving fox on a random cell in a 2block radius
                {
                    int ran = rnd.Next(0, FoxsurroundingCellsToMove.Count);
                    FoxsurroundingCellsToMove[ran].inhabitant = field[x, y].inhabitant;
                    saved = FoxsurroundingCellsToMove[ran];
                    field[x, y].inhabitant = null;  //In this case it can have a hunt

                    PossibleTileChecking(field, saved.posX, saved.posY, out FoxsurroundingCellsToMove, out surroundingCellsToHunt);
                    //Checking if it can hunt or not
                    if (surroundingCellsToHunt.Count != 0) //It can hunt
                    {
                        ran = rnd.Next(0, surroundingCellsToHunt.Count);
                        (field[saved.posX, saved.posY].inhabitant as Fox).Eat(3);
                        field[saved.posX, saved.posY].inhabitant.hasAte = true;
                        field[saved.posX, saved.posY].inhabitant.hasMoved = true;
                        surroundingCellsToHunt[ran].inhabitant = field[saved.posX, saved.posY].inhabitant; //A bunny died :(
                        field[saved.posX, saved.posY].inhabitant = null;
                    }
                    else //It can't hunt, so it makes the second move
                    {
                        ran = rnd.Next(0, FoxsurroundingCellsToMove.Count);
                        field[saved.posX, saved.posY].inhabitant.hasMoved = true;
                        FoxsurroundingCellsToMove[ran].inhabitant = field[saved.posX, saved.posY].inhabitant;
                        field[saved.posX, saved.posY].inhabitant = null;
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
                    if (field[x, y].inhabitant != null)
                    {
                        if (field[x, y].inhabitant is Rabbit)
                        {
                            field = RabbitHandler.Check(field, x, y);

                        }
                        else if (field[x, y].inhabitant is Fox)
                        {
                            field = FoxHandler.Check(field, x, y);
                        }
                    }

                    field[x, y].inhabitant?.Update();
                    if (field[x, y].inhabitant == null)
                    {
                        field[x, y].Grow();
                    }
                }
            }


            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsHorizontallyCount; x++)
                {
                    if (field[x, y].inhabitant != null)
                    {
                        field[x, y].inhabitant.hasMoved = false;
                        field[x, y].inhabitant.hasProduced = false;
                    }
                }
            }

            return field;
        }
    }
}
