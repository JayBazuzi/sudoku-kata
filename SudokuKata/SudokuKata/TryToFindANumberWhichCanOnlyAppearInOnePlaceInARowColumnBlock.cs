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
                cellsWhichAreTheOnlyPossibleInABlock.AddRange(
                    GetCellsWhichAreTheOnlyPossibleInABlockForADigit(puzzle, digit));
            }

            return cellsWhichAreTheOnlyPossibleInABlock;
        }

        private static IEnumerable<Tuple<Cell, string>> GetCellsWhichAreTheOnlyPossibleInABlockForADigit(
            SudokuBoard puzzle, int digit)
        {
            // TODO: make this beautiful
            //  CheckGroup("rows", SudokuBoard.GetRows(), puzzle) X 3
            //  
            var rows = SudokuBoard.GetRows().ToList();
            var columns = SudokuBoard.GetColumns().ToList();
            var blocks = SudokuBoard.GetBlocks().ToList();
            for (var cellGroup = 0; cellGroup < 9; cellGroup++)
            {
                {
                    Tuple<Cell, string> result = null;
                    {
                        var rowNumberCount = 0;
                        var indexInRow = 0;


                        for (var indexInGroup = 0; indexInGroup < 9; indexInGroup++)
                        {
                            var candidateMasks = puzzle.GetCandidates().Board;

                            var row = rows.ElementAt(cellGroup);
                            var mask = 1 << (digit - 1);
                            if ((candidateMasks[row.ElementAt(indexInGroup).ToIndex()] & mask) != 0)
                            {
                                rowNumberCount += 1;
                                indexInRow = indexInGroup;
                            }
                        }


                        if (rowNumberCount == 1)
                        {
                            var description = $"Row #{cellGroup + 1}";
                            result = Tuple.Create(new Cell(cellGroup, indexInRow, digit),
                                description);
                        }
                    }
                    if (result != null)
                    {
                        yield return result;
                    }
                }

                {
                    Tuple<Cell, string> result = null;
                    Func<int, string> getDescription = c => $"Column #{c + 1}";
                    {
                        var rowNumberCount = 0;
                        var indexInRow = 0;
                        var group = columns;
                        var block = group.ElementAt(cellGroup).ToList();
                        for (var indexInGroup = 0; indexInGroup < 9; indexInGroup++)
                        {
                            var candidateMasks = puzzle.GetCandidates().Board;

                            var mask = 1 << (digit - 1);
                            if ((candidateMasks[block.ElementAt(indexInGroup).ToIndex()] & mask) != 0)
                            {
                                rowNumberCount += 1;
                                indexInRow = indexInGroup;
                            }
                        }


                        if (rowNumberCount == 1)
                        {
                            var description = getDescription(cellGroup);
                            result = Tuple.Create(
                                new Cell(block[indexInRow].Row, block[indexInRow].Col, digit), description);
                        }
                    }
                    if (result != null)
                    {
                        yield return result;
                    }
                }

                {
                    var getDescription = (Func<int, string>)(c => $"Block ({c / 3 + 1}, {c % 3 + 1})");
                    Tuple<Cell, string> result = null;
                    var blockNumberCount = 0;
                    var indexInBlock = 0;
                    var block = blocks.ElementAt(cellGroup).ToList();
                    for (var indexInGroup = 0; indexInGroup < 9; indexInGroup++)
                    {
                        var candidateMasks = puzzle.GetCandidates().Board;
                        var mask = 1 << (digit - 1);
                        if ((candidateMasks[block.ElementAt(indexInGroup).ToIndex()] & mask) != 0)
                        {
                            blockNumberCount += 1;
                            indexInBlock = indexInGroup;
                        }
                    }

                    if (blockNumberCount == 1)
                    {
                        var description = getDescription(cellGroup);
                        result = Tuple.Create(
                            new Cell(block[indexInBlock].Row, block[indexInBlock].Col, digit), description);
                    }
                    if (result != null)
                    {
                        yield return result;
                    }
                }
            } // for (cellGroup = 0..8)
        }
    }
}