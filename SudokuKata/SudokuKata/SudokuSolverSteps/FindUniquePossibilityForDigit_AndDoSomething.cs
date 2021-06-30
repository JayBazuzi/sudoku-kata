using System;
using System.Collections.Generic;
using System.Linq;
using SudokuKata.Board;

namespace SudokuKata
{
    internal class FindUniquePossibilityForDigit_AndDoSomething : ISudokuSolverStep
    {
        public ChangesMadeStates Do(Random rng,
            SudokuBoard puzzle)
        {
            var cellsWhichAreTheOnlyPossibleInABlock = GetCellsWhichAreTheOnlyPossibleInABlock(puzzle);

            if (cellsWhichAreTheOnlyPossibleInABlock.Count > 0)
            {
                var index = rng.Next(cellsWhichAreTheOnlyPossibleInABlock.Count);
                var (cell, description) = cellsWhichAreTheOnlyPossibleInABlock[index];

                puzzle.SetValue(cell);

                Console.WriteLine($"{description} can contain {cell.Value} only at ({cell.Row + 1}, {cell.Column + 1}).");
                return new ChangesMadeStates {CellChanged = true};
            }

            return ChangesMadeStates.None;
        }

        private static List<Tuple<Cell, string>> GetCellsWhichAreTheOnlyPossibleInABlock(SudokuBoard puzzle)
        {
            var cellsWhichAreTheOnlyPossibleInABlock = new List<Tuple<Cell, string>>();

            for (var digit = 1; digit <= 9; digit++)
            {
                cellsWhichAreTheOnlyPossibleInABlock.AddRange(
                    GetCellsWhichAreTheOnlyPossibleInABlockForADigit(puzzle, digit));
            }

            return cellsWhichAreTheOnlyPossibleInABlock;
        }

        private static IEnumerable<Tuple<Cell, string>> GetCellsWhichAreTheOnlyPossibleInABlockForADigit(
            SudokuBoard puzzle, int digit)
        {
            var rows = SudokuBoard.GetRows().ToList();
            var columns = SudokuBoard.GetColumns().ToList();
            var blocks = SudokuBoard.GetBlocks().ToList();
            var results = new List<Tuple<Cell, string>>();
            for (var cellGroup = 0; cellGroup < 9; cellGroup++)
            {
                results.Add(GetIfOnlyOneChoiceIsPossibleFromGroup(puzzle, digit, rows, cellGroup,
                    c => $"Row #{c + 1}"));

                results.Add(GetIfOnlyOneChoiceIsPossibleFromGroup(puzzle, digit, columns, cellGroup,
                    c => $"Column #{c + 1}"));

                results.Add(GetIfOnlyOneChoiceIsPossibleFromGroup(puzzle, digit, blocks, cellGroup,
                    c => $"Block ({c / 3 + 1}, {c % 3 + 1})"));
            }

            return results.Where(r => r != null);
        }

        private static Tuple<Cell, string> GetIfOnlyOneChoiceIsPossibleFromGroup(SudokuBoard puzzle, int digit,
            List<IEnumerable<Cell>> group, int groupIndex, Func<int, string> getDescription)
        {
            var possibleDigitCount = 0;
            var lastMatchingGroupIndex = 0;
            var block = group.ElementAt(groupIndex).ToList();
            for (var indexInGroup = 0; indexInGroup < 9; indexInGroup++)
            {
                if (puzzle.IsDigitPossible(digit, block.ElementAt(indexInGroup)))
                {
                    possibleDigitCount += 1;
                    lastMatchingGroupIndex = indexInGroup;
                }
            }


            if (possibleDigitCount == 1)
            {
                return Tuple.Create(
                    new Cell(block[lastMatchingGroupIndex].Row, block[lastMatchingGroupIndex].Column, digit), getDescription(groupIndex));
            }

            return null;
        }
    }
}