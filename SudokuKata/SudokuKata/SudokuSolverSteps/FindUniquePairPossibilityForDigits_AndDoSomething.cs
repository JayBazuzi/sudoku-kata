using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuKata
{
    public class FindUniquePairPossibilityForDigits_AndDoSomething : ISudokuSolverStep
    {
        public ChangesMadeStates Do(
            Random rng, SudokuBoard sudokuBoard)
        {
            var cellGroups = SudokuBoard.BuildCellGroups();
            var candidateMasks = sudokuBoard.GetCandidates().Masks;
            var maskToOnesCount = LookupStructures.Instance._maskToOnesCount;

            IEnumerable<int> twoDigitMasks =
                candidateMasks.Where(mask => maskToOnesCount[mask] == 2).Distinct().ToList();

            var groups = twoDigitMasks
                .SelectMany(mask =>
                    cellGroups
                        .Where(group => group.Count(tuple => candidateMasks[tuple.Index] == mask) == 2)
                        .Where(group => group.Any(tuple =>
                            candidateMasks[tuple.Index] != mask &&
                            SudokuBoard.IsAnyDigitPossible(candidateMasks, mask, tuple.Index)))
                        .Select(group => new
                        {
                            Mask = mask,
                            Cells = group
                        }))
                .ToList();

            if (!groups.Any())
            {
                return ChangesMadeStates.None;
            }

            var stepChangeMade = false;
            foreach (var group in groups)
            {
                var cellWithDescriptions = group.Cells;
                var mask = group.Mask;
                stepChangeMade |= RemovePairs(sudokuBoard, cellWithDescriptions, SudokuBoard.GetDigitsForMask(mask));
            }

            return new ChangesMadeStates {CandidateChanged = stepChangeMade};
        }

        private static bool RemovePairs(SudokuBoard sudokuBoard, List<CellWithDescription> cellWithDescriptions,
            List<int> digitsToRemove)
        {
            var remainingDigits = SudokuBoard.GetRemainingDigits(digitsToRemove);
            var cellsWithAdditionalPossibilities = cellWithDescriptions
                .Where(cell => sudokuBoard.IsAnyDigitPossible(cell.Cell, remainingDigits))
                .Where(cell => sudokuBoard.IsAnyDigitPossible(cell.Cell, digitsToRemove))
                .ToList();
            if (!cellsWithAdditionalPossibilities.Any())
            {
                return false;
            }

            var maskCells =
                cellWithDescriptions
                    .Where(cell => sudokuBoard.IsExactly(cell.Cell, digitsToRemove))
                    .ToArray();


            Console.WriteLine(
                $"Values {digitsToRemove.Min()} and {digitsToRemove.Max()} in {cellWithDescriptions.First().Description} are in cells ({maskCells[0].Row + 1}, {maskCells[0].Column + 1}) and ({maskCells[1].Row + 1}, {maskCells[1].Column + 1}).");

            var stepChange = false;
            foreach (var cell in cellsWithAdditionalPossibilities)
            {
                var valuesToRemove = digitsToRemove
                    .Where(d => sudokuBoard.IsDigitPossible(d, cell.Cell)).ToList();

                var valuesReport = string.Join(", ", valuesToRemove.ToArray());
                Console.WriteLine(
                    $"{valuesReport} cannot appear in ({cell.Row + 1}, {cell.Column + 1}).");

                sudokuBoard.RemovePossibilities(cell, valuesToRemove);
                stepChange = true;
            }

            return stepChange;
        }
    }
}