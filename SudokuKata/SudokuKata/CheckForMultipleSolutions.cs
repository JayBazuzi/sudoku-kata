using System;
using System.Collections.Generic;
using System.Linq;
using SudokuKata.Board;

namespace SudokuKata
{
    internal static class CheckForMultipleSolutions
    {
        public static ChangesMadeStates Do(Random rng, int[] finalState, SudokuBoard sudokuBoard)
        {
            // TODO: Make this class tell a better story, so the flow of this class is more obvious
            var state = sudokuBoard.GetBoardAsNumbers();

            // This is the last chance to do something in this iteration:
            // If this attempt fails, board will not be entirely solved.

            // Try to see if there are pairs of values that can be exchanged arbitrarily
            // This happens when board has more than one valid solution

            var candidatesOfIndexesAndDigits = GetDeadlockedCellsWithTwoPossibilities(sudokuBoard);

            // At this point we have the lists with pairs of cells that might pick one of two digits each
            // Now we have to check whether that is really true - does the board have two solutions?

            var stateIndexesAndValues = SolveAlternateBoard(rng, finalState, sudokuBoard, candidatesOfIndexesAndDigits, state);

            return MergeTheStateWithValuesOfFinalStateFromCells1And2ForPossibleElementAndLog(rng, finalState, sudokuBoard, stateIndexesAndValues, state);
        }

        private static List<Tuple<Cell, Cell, int, int>> SolveAlternateBoard(Random rng, int[] finalState,
            SudokuBoard sudokuBoard,
            Queue<Tuple<Cell, Cell, int, int>> candidatesOfIndexesAndDigits, int[] state)
        {
            var sudokuBoardClone = sudokuBoard.Clone();

            var stateIndexesAndValues = new List<Tuple<Cell, Cell, int, int>>();

            while (candidatesOfIndexesAndDigits.Any())
            {
                var (index1, index2, digit1, digit2) = candidatesOfIndexesAndDigits.Dequeue();

                var alternateState = ConstructAlternateState(finalState, state, index1, digit1, digit2, index2);

                var command = SudokoBoardGenerator.SolveBoard(rng, sudokuBoardClone, alternateState);

                if (command == Command.Complete)
                {
                    // Board was solved successfully even with two digits swapped
                    stateIndexesAndValues.Add(Tuple.Create(
                        index1,
                        index2,
                        digit1,
                        digit2));
                }
            }

            return stateIndexesAndValues;
        }

        private static int[] ConstructAlternateState(int[] finalState, int[] state, Cell index1, int digit1, int digit2,
            Cell index2)
        {
            var alternateState = new int[finalState.Length];
            Array.Copy(state, alternateState, alternateState.Length);

            var digitsNeedToBeSwapped = finalState[index1.ToIndex()] == digit1;
            if (digitsNeedToBeSwapped)
            {
                (digit1, digit2) = (digit2, digit1);
            }

            alternateState[index1.ToIndex()] = digit1;
            alternateState[index2.ToIndex()] = digit2;

            return alternateState;
        }

        private static ChangesMadeStates MergeTheStateWithValuesOfFinalStateFromCells1And2ForPossibleElementAndLog(Random rng, int[] finalState, SudokuBoard sudokuBoard,
            List<Tuple<Cell, Cell, int, int>> stateIndexesAndValues, int[] state)
        {
            if (!stateIndexesAndValues.Any())
            {
                return new ChangesMadeStates {CellChanged = false};
            }

            var (cell1, cell2, digit1, digit2) = stateIndexesAndValues.GetRandomElement(rng);

            MergeTheStateWithValuesOfFinalStateFromCells1And2(sudokuBoard, finalState, state, cell1, cell2);

            Console.WriteLine(
                $"Guessing that {digit1} and {digit2} are arbitrary in {GetDescription(cell1, cell2)} (multiple solutions): Pick {finalState[cell1.ToIndex()]}->({cell1.Row + 1}, {cell1.Column + 1}), {finalState[cell2.ToIndex()]}->({cell2.Row + 1}, {cell2.Column + 1}).");
            return new ChangesMadeStates {CellChanged = true};
        }

        private static void MergeTheStateWithValuesOfFinalStateFromCells1And2(SudokuBoard sudokuBoard,
            int[] finalState,
            int[] state,
            Cell cell1,
            Cell cell2)
        {
            sudokuBoard.SetValue(cell1.Row, cell1.Column, finalState[cell1.ToIndex()]);
            sudokuBoard.SetValue(cell2.Row, cell2.Column, finalState[cell2.ToIndex()]);
        }

        private static string GetDescription(Cell cell1, Cell cell2)
        {
            if (cell1.IsSameRow(cell2))
            {
                return $"row #{cell1.Row + 1}";
            }

            if (cell1.IsSameColumn(cell2))
            {
                return $"column #{cell1.Column + 1}";
            }

            return $"block ({cell1.Row / 3 + 1}, {cell1.Column / 3 + 1})";
        }

        private static Queue<Tuple<Cell, Cell, int, int>> GetDeadlockedCellsWithTwoPossibilities(
            SudokuBoard sudokuBoard)
        {
            var candidatesOfIndexesAndDigits = new Queue<Tuple<Cell, Cell, int, int>>();

            var cellsWithTwoPossible = sudokuBoard.GetCellsWithPossibilities().Where(c => c.Possibilities.Count == 2);
            foreach (var possibility in cellsWithTwoPossible)
            {
                var cell1 = possibility.Cell;

                foreach (var cell2 in Cell.ForBoard().Skip(cell1.ToIndex() + 1))
                {
                    var matchingTwoPossiblesCell =
                        possibility.Possibilities.SequenceEqual(sudokuBoard.GetPossibilities(cell2));
                    var isMatchingGroup =
                        cell1.IsSameRow(cell2) || cell1.IsSameColumn(cell2) || cell1.IsCellBlock(cell2);
                    if (isMatchingGroup && matchingTwoPossiblesCell)
                    {
                        var upper = possibility.Possibilities.Max();
                        var lower = possibility.Possibilities.Min();
                        candidatesOfIndexesAndDigits.Enqueue(Tuple.Create(cell1, cell2, lower, upper));
                    }
                }
            }

            return candidatesOfIndexesAndDigits;
        }
    }

    internal static class _
    {
        public static T GetRandomElement<T>(this IEnumerable<T> source, Random random)
        {
            return source.ElementAt(random.Next(source.Count()));
        }
    }
}