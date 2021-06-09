﻿using System;

namespace SudokuKata
{
    public class Program
    {
        public static void Play()
        {
            var rng = new Random();

            Play(rng);
        }


        public static void Play(Random rng)
        {
            var solvedBoard = SudokoBoardGenerator.ConstructFullySolvedBoard(rng);
            var puzzle = solvedBoard.GeneratePuzzleFromCompletelySolvedBoard(rng);
            LogStartOfSolution(puzzle);

            SolvePuzzle(rng, puzzle, solvedBoard);
        }

        private static void LogStartOfSolution(SudokuBoard puzzle)
        {
            Console.WriteLine();
            Console.WriteLine("Starting look of the board to solve:");
            Console.WriteLine(puzzle.ToString());


            PrintLineOfEquals();
        }


        private static void SolvePuzzle(Random rng, SudokuBoard puzzle,
            SudokuBoard solvedBoard)
        {
            ChangesMadeStates changesMadeStates;

            do
            {
                var candidates2 = puzzle.GetCandidates(true);

                do
                {
                    changesMadeStates = ChangesMadeStates.None;
                    foreach (var step in GetSudokuSolverSteps())
                    {
                        changesMadeStates = changesMadeStates.DoIfUnchanged(step, rng, puzzle);
                    }
                } while (changesMadeStates.CandidateChanged);

                changesMadeStates = changesMadeStates.DoIfUnchanged(
                    () => CheckForMultipleSolutions.Do(rng,
                        candidates2.Masks,
                        solvedBoard.GetBoardAsNumbers(), puzzle));

                PrintBoardChange(changesMadeStates.CellChanged, puzzle);
            } while (changesMadeStates.IsChanged);
        }

        private static ISudokuSolverStep[] GetSudokuSolverSteps()
        {
            return new ISudokuSolverStep[]
            {
                new PickCellsWithOnlyOneCandidateRemaining(),
                new FindUniquePossibilityForDigit_AndDoSomething(),
                new FindUniquePairPossibilityForDigits_AndDoSomething(),
                new RemoveDigitsWhenConstrainedToAGroupOfNCells()
            };
        }


        private static void PrintLineOfEquals()
        {
            Console.WriteLine();
            Console.WriteLine(new string('=', 80));
            Console.WriteLine();
        }

        private static void PrintBoardChange(bool changeMade, SudokuBoard sudokuBoard)
        {
            if (changeMade)
            {
                #region Print the board as it looks after one change was made to it

                Console.WriteLine(sudokuBoard);

                Console.WriteLine("Code: {0}", sudokuBoard.ToCodeString());
                Console.WriteLine();

                #endregion
            }
        }

        public static void Main(string[] args)
        {
            Play();

            Console.WriteLine();
            Console.Write("Press ENTER to exit... ");
            Console.ReadLine();
        }
    }
}