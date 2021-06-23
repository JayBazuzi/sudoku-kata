using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ApprovalUtilities.Utilities;

namespace SudokuKata
{
    public class RemoveDigitsWhenConstrainedToAGroupOfNCells : ISudokuSolverStep
    {
        public ChangesMadeStates Do(Random random, SudokuBoard sudokuBoard)
        {
            var cellGroups = SudokuBoard.BuildCellGroups();

            var digitsForMasks = GetAllCombinationsOfNumbersFromOneToNine();
            var groupsWithNMasks =
                digitsForMasks
                    .SelectMany(digitsForMask =>
                        cellGroups
                            .Where(group => group.All(cell =>
                            {
                                var digit = sudokuBoard.GetValueForCell(cell.Cell);
                                var digitIsNotWhatWeAreLookingFor = !digitsForMask.Contains(digit);
                                var cellIsUnsolved = digit == SudokuBoard.Unknown;
                                return cellIsUnsolved || digitIsNotWhatWeAreLookingFor;
                            }))
                            .Select(cells =>
                            {
                                var digits = digitsForMask;
                                return new
                                {
                                    Digits = digits,
                                    RemainingDigits = SudokuBoard.GetRemainingDigits(digits),
                                    Cells = cells,
                                    CellsWithMask = cells
                                        .Where(cell => sudokuBoard.IsAnyDigitPossible(cell.Cell, digits)).ToList()
                                };
                            }))
                    .Where(group => group.CellsWithMask.Count() == group.Digits.Count)
                    .ToList();

            var stepChangeMade = false;
            foreach (var g in groupsWithNMasks)
            {
                stepChangeMade |= RemoveDigitsWhenConstrainedToAGroupOfNCells_ForGroup(sudokuBoard, g.Cells, g.Digits,
                    g.RemainingDigits, g.CellsWithMask);
            }

            return new ChangesMadeStates {CandidateChanged = stepChangeMade};
        }

        public static IEnumerable<List<int>> GetAllCombinationsOfNumbersFromOneToNine()
        {
            var maskToOnesCount = LookupStructures.Instance._maskToOnesCount;

            IEnumerable<int> masks =
                maskToOnesCount
                    .Where(tuple => 1 < tuple.Value)
                    .Select(tuple => tuple.Key).ToList();

            var digitsForMasks = masks
                .Select(SudokuBoard.GetDigitsForMask);
            return digitsForMasks;
        }

        private static bool RemoveDigitsWhenConstrainedToAGroupOfNCells_ForGroup(SudokuBoard sudokuBoard,
            List<CellWithDescription> cells, List<int> digits, IEnumerable<int> remainingDigits,
            List<CellWithDescription> cellsWithMask)
        {
            var description = cells.First().Description;
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