using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApprovalUtilities.Utilities;

namespace SudokuKata
{
    internal class RemoveDigitsWhenConstrainedToAGroupOfNCells : ISudokuSolverStep
    {
        public ChangesMadeStates Do(Random random, SudokuBoard sudokuBoard)
        {
            // TODO: clean up here next
            var cellGroups = SudokuBoard.BuildCellGroups();

            var state = sudokuBoard.GetBoardAsNumbers();
            var candidateMasks = sudokuBoard.GetCandidates().Masks;
            var maskToOnesCount = LookupStructures.Instance._maskToOnesCount;

            #region Try to find groups of digits of size N which only appear in N cells within row/column/block

            // When a set of N digits only appears in N cells within row/column/block, then no other digit can appear in the same set of cells
            // All other candidates can then be removed from those cells


            IEnumerable<int> masks =
                maskToOnesCount
                    .Where(tuple => 1 < tuple.Value)
                    .Select(tuple => tuple.Key).ToList();

            var groupsWithNMasks =
                masks
                    .SelectMany(mask =>
                        cellGroups
                            .Where(group => group.All(cell =>
                                state[cell.Index] == 0 || (mask & (1 << (state[cell.Index] - 1))) == 0))
                            .Select(group => new
                            {
                                Mask = mask,
                                Digits = SudokuBoard.GetDigitsForMask(mask),
                                RemainingDigits = SudokuBoard.GetRemainingDigits(SudokuBoard.GetDigitsForMask(mask)),
                                group.First().Description,
                                Cells = group,
                                CellsWithMask = group.Where(cell =>
                                        state[cell.Index] == 0 && (candidateMasks[cell.Index] & mask) != 0)
                                    .ToList(),
                                CleanableCellsCount = group.Count(
                                    cell => state[cell.Index] == 0 &&
                                            (candidateMasks[cell.Index] & mask) != 0 &&
                                            (candidateMasks[cell.Index] & ~mask) != 0)
                            }))
                    .Where(group => group.CellsWithMask.Count() == maskToOnesCount[group.Mask])
                    .ToList();

            var stepChangeMade = false;
            foreach (var groupWithNMasks in groupsWithNMasks)
            {
                if (groupWithNMasks.Cells
                    .Any(cell =>
                        sudokuBoard.IsAnyDigitPossible(cell.Cell, groupWithNMasks.Digits) &&
                        sudokuBoard.IsAnyDigitPossible(cell.Cell, groupWithNMasks.RemainingDigits)))
                {
                    var digitsAsText = string.Join(", ", groupWithNMasks.Digits);
                    var cellsAsText = groupWithNMasks.CellsWithMask.Select(cell => $"({cell.Row + 1}, {cell.Column + 1}) ").JoinWith("");
                    Console.WriteLine($"In {groupWithNMasks.Description} values {digitsAsText} appear only in cells {cellsAsText}and other values cannot appear in those cells.");
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