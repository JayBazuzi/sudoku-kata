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
            var candidateMasks = puzzle.GetCandidates().Board;
            var groupDescriptions = new List<string>();
            var candidateRowIndices = new List<int>();
            var candidateColIndices = new List<int>();
            var candidates = new List<int>();

            for (var digit = 1; digit <= 9; digit++)
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
                        groupDescriptions.Add($"Row #{cellGroup + 1}");
                        candidateRowIndices.Add(cellGroup);
                        candidateColIndices.Add(indexInRow);
                        candidates.Add(digit);
                    }

                    if (colNumberCount == 1)
                    {
                        groupDescriptions.Add($"Column #{cellGroup + 1}");
                        candidateRowIndices.Add(indexInCol);
                        candidateColIndices.Add(cellGroup);
                        candidates.Add(digit);
                    }

                    if (blockNumberCount == 1)
                    {
                        var blockRow = cellGroup / 3;
                        var blockCol = cellGroup % 3;

                        groupDescriptions.Add($"Block ({blockRow + 1}, {blockCol + 1})");
                        candidateRowIndices.Add(blockRow * 3 + indexInBlock / 3);
                        candidateColIndices.Add(blockCol * 3 + indexInBlock % 3);
                        candidates.Add(digit);
                    }
                } // for (cellGroup = 0..8)
            } // for (digit = 1..9)

            if (candidates.Count > 0)
            {
                var index = rng.Next(candidates.Count);
                var description = groupDescriptions.ElementAt(index);
                var row = candidateRowIndices.ElementAt(index);
                var col = candidateColIndices.ElementAt(index);
                var digit = candidates.ElementAt(index);

                var message = $"{description} can contain {digit} only at ({row + 1}, {col + 1}).";

                var cell = new Cell(row, col, digit);
                puzzle.SetValue(cell);

                Console.WriteLine(message);
                return new ChangesMadeStates {CellChanged = true};
            }

            return ChangesMadeStates.None;
        }
    }
}