using System;
using System.Collections.Generic;
using System.Linq;
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
                            .Select(cells =>
                            {
                                var digits = SudokuBoard.GetDigitsForMask(mask);
                                return new
                                {
                                    Mask = mask,
                                    Digits = digits,
                                    RemainingDigits =
                                        SudokuBoard.GetRemainingDigits(digits),
                                    cells.First().Description,
                                    Cells = cells,
                                    CellsWithMask = cells
                                        .Where(cell => sudokuBoard.IsAnyDigitPossible(cell.Cell, digits)).ToList()
                                };
                            }))
                    .Where(group => group.CellsWithMask.Count() == maskToOnesCount[group.Mask])
                    .ToList();

            var stepChangeMade = false;
            foreach (var g in groupsWithNMasks)
            {
                stepChangeMade |= RemoveDigitsWhenConstrainedToAGroupOfNCells_ForGroup(sudokuBoard, g.Cells, g.Digits,
                    g.RemainingDigits, g.CellsWithMask, g.Description);
            }

            return new ChangesMadeStates {CandidateChanged = stepChangeMade};
        }

        private static bool RemoveDigitsWhenConstrainedToAGroupOfNCells_ForGroup(SudokuBoard sudokuBoard,
            List<CellWithDescription> cells, List<int> digits, IEnumerable<int> remainingDigits,
            List<CellWithDescription> cellsWithMask, string description)
        {
            if (cells.Any(cell =>
                sudokuBoard.IsAnyDigitPossible(cell.Cell, digits) &&
                sudokuBoard.IsAnyDigitPossible(cell.Cell, remainingDigits)))
            {
                var digitsAsText = string.Join(", ", digits);
                var cellsAsText = cellsWithMask.Select(cell => $"({cell.Row + 1}, {cell.Column + 1})").JoinWith(" ");
                Console.WriteLine(
                    $"In {description} values {digitsAsText} appear only in cells {cellsAsText} and other values cannot appear in those cells.");
            }

            var stepChangeMade = false;
            foreach (var cell in cellsWithMask)
            {
                var possible = remainingDigits.Where(d => sudokuBoard.IsDigitPossible(d, cell.Cell)).ToList();
                if (!possible.Any())
                {
                    continue;
                }

                sudokuBoard.RemovePossibilities(cell, possible);
                stepChangeMade = true;

                Console.WriteLine(
                    $"{string.Join(", ", possible)} cannot appear in cell ({cell.Row + 1}, {cell.Column + 1}).");
            }

            return stepChangeMade;
        }
    }
}