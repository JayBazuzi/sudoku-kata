using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuKata
{
    internal class TryToFindANumberWhichCanOnlyAppearInOnePlaceInARowColumnBlock : ISudokuSolverStep
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

                Console.WriteLine($"{description} can contain {cell.Value} only at ({cell.Row + 1}, {cell.Col + 1}).");
                return new ChangesMadeStates {CellChanged = true};
            }

            return ChangesMadeStates.None;
        }

        private static List<Tuple<Cell, string>> GetCellsWhichAreTheOnlyPossibleInABlock(SudokuBoard puzzle)
        {
            var cellsWhichAreTheOnlyPossibleInABlock = new List<Tuple<Cell, string>>();

            for (var digit = 1; digit <= 9; digit++)
            {
                cellsWhichAreTheOnlyPossibleInABlock.AddRange(GetCellsWhichAreTheOnlyPossibleInABlockForADigit(puzzle, digit));
            }

            return cellsWhichAreTheOnlyPossibleInABlock;
        }

        private static IEnumerable<Tuple<Cell, string>> GetCellsWhichAreTheOnlyPossibleInABlockForADigit(SudokuBoard puzzle, int digit)
        {
            var mask = 1 << (digit - 1);
            for (var cellGroup = 0; cellGroup < 9; cellGroup++)
            {
                var rowNumberCount = 0;
                var indexInRow = 0;

                var colNumberCount = 0;
                var indexInCol = 0;

                var blockNumberCount = 0;
                var indexInBlock = 0;

                for (var indexInGroup = 0; indexInGroup < 9; indexInGroup++)
                {
                    var rowStateIndex = 9 * cellGroup + indexInGroup;
                    var colStateIndex = 9 * indexInGroup + cellGroup;
                    var blockRowIndex = cellGroup / 3 * 3 + indexInGroup / 3;
                    var blockColIndex = cellGroup % 3 * 3 + indexInGroup % 3;
                    var blockStateIndex = blockRowIndex * 9 + blockColIndex;

                    var candidateMasks = puzzle.GetCandidates().Board;
                    if ((candidateMasks[rowStateIndex] & mask) != 0)
                    {
                        rowNumberCount += 1;
                        indexInRow = indexInGroup;
                    }

                    if ((candidateMasks[colStateIndex] & mask) != 0)
                    {
                        colNumberCount += 1;
                        indexInCol = indexInGroup;
                    }

                    if ((candidateMasks[blockStateIndex] & mask) != 0)
                    {
                        blockNumberCount += 1;
                        indexInBlock = indexInGroup;
                    }
                }

                if (rowNumberCount == 1)
                {
                    var description = $"Row #{cellGroup + 1}";
                    yield return (Tuple.Create(new Cell(cellGroup, indexInRow, digit),
                        description));
                }

                if (colNumberCount == 1)
                {
                    var description = $"Column #{cellGroup + 1}";
                    yield return (Tuple.Create(new Cell(indexInCol, cellGroup, digit),
                        description));
                }

                if (blockNumberCount == 1)
                {
                    var blockRow = cellGroup / 3;
                    var blockCol = cellGroup % 3;

                    var description = $"Block ({blockRow + 1}, {blockCol + 1})";
                    yield return (Tuple.Create(
                        new Cell(blockRow * 3 + indexInBlock / 3, blockCol * 3 + indexInBlock % 3, digit), description));
                }
            } // for (cellGroup = 0..8)
        }
    }
}