using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuKata
{
    internal class RemoveDigitsWhenConstrainedToAGroupOfNCells: ISudokuSolverStep
    {
        public ChangesMadeStates Do(Random random, SudokuBoard sudokuBoard)
        {
            var cellGroups = sudokuBoard.BuildCellGroups();

            var state = sudokuBoard.GetBoardAsNumbers();
            var candidateMasks = sudokuBoard.GetCandidates().Board;
            var maskToOnesCount = Program.PrepareLookupStructures()._maskToOnesCount;

            #region Try to find groups of digits of size N which only appear in N cells within row/column/block

            // When a set of N digits only appears in N cells within row/column/block, then no other digit can appear in the same set of cells
            // All other candidates can then be removed from those cells


            IEnumerable<int> masks =
                maskToOnesCount
                    .Where(tuple => tuple.Value > 1)
                    .Select(tuple => tuple.Key).ToList();

            var groupsWithNMasks =
                masks
                    .SelectMany(mask =>
                        cellGroups
                            .Where(group => @group.All(cell =>
                                state[cell.Index] == 0 || (mask & (1 << (state[cell.Index] - 1))) == 0))
                            .Select(group => new Applesauce3
                            {
                                Mask = mask, Description = @group.First().Description, Cells = @group,
                                CellsWithMask = @group.Where(cell =>
                                        state[cell.Index] == 0 && (candidateMasks[cell.Index] & mask) != 0)
                                    .ToList(),
                                CleanableCellsCount = @group.Count(
                                    cell => state[cell.Index] == 0 &&
                                            (candidateMasks[cell.Index] & mask) != 0 &&
                                            (candidateMasks[cell.Index] & ~mask) != 0)
                            }))
                    .Where(group => @group.CellsWithMask.Count() == maskToOnesCount[@group.Mask])
                    .ToList();

            bool stepChangeMade = false;
            foreach (var groupWithNMasks in groupsWithNMasks)
            {
                var mask = groupWithNMasks.Mask;

                if (groupWithNMasks.Cells
                    .Any(cell =>
                        (candidateMasks[cell.Index] & mask) != 0 &&
                        (candidateMasks[cell.Index] & ~mask) != 0))
                {
                    var message = new StringBuilder();
                    message.Append($"In {groupWithNMasks.Description} values ");

                    var separator = string.Empty;
                    var temp = mask;
                    var curValue = 1;
                    while (temp > 0)
                    {
                        if ((temp & 1) > 0)
                        {
                            message.Append($"{separator}{curValue}");
                            separator = ", ";
                        }

                        temp = temp >> 1;
                        curValue += 1;
                    }

                    message.Append(" appear only in cells");
                    foreach (var cell in groupWithNMasks.CellsWithMask)
                    {
                        message.Append($" ({cell.Row + 1}, {cell.Column + 1})");
                    }

                    message.Append(" and other values cannot appear in those cells.");

                    Console.WriteLine(message.ToString());
                }

                foreach (var cell in groupWithNMasks.CellsWithMask)
                {
                    var maskToClear = candidateMasks[cell.Index] & ~groupWithNMasks.Mask;
                    if (maskToClear == 0)
                    {
                        continue;
                    }

                    candidateMasks[cell.Index] &= groupWithNMasks.Mask;
                    stepChangeMade = true;

                    var valueToClear = 1;

                    var separator = string.Empty;
                    var message = new StringBuilder();

                    while (maskToClear > 0)
                    {
                        if ((maskToClear & 1) > 0)
                        {
                            message.Append($"{separator}{valueToClear}");
                            separator = ", ";
                        }

                        maskToClear = maskToClear >> 1;
                        valueToClear += 1;
                    }

                    message.Append($" cannot appear in cell ({cell.Row + 1}, {cell.Column + 1}).");
                    Console.WriteLine(message.ToString());
                }
            }

            #endregion

            return new ChangesMadeStates {CandidateChanged = stepChangeMade};
        }
    }
}