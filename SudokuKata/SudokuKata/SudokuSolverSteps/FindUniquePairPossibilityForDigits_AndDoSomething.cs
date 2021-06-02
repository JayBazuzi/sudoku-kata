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
            var candidateMasks = sudokuBoard.GetCandidates().Board;
            var maskToOnesCount = LookupStructures.Instance._maskToOnesCount;

            // TODO: start cleaning here
            IEnumerable<int> twoDigitMasks =
                candidateMasks.Where(mask => maskToOnesCount[mask] == 2).Distinct().ToList();

            var groups =
                twoDigitMasks
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
                stepChangeMade = Applesauce(sudokuBoard, cellWithDescriptions, candidateMasks, mask, stepChangeMade);
            }

            return new ChangesMadeStates {CandidateChanged = stepChangeMade};
        }

        private static bool Applesauce(SudokuBoard sudokuBoard, List<CellWithDescription> cellWithDescriptions,
            int[] candidateMasks, int mask, bool stepChangeMade)
        {
            var cells =
                cellWithDescriptions
                    .Where(
                        cell =>
                            // Check that we're not removing the cells that *only* have these two digits.
                            candidateMasks[cell.Index] != mask &&
                            sudokuBoard.IsAnyDigitPossible(cell.Cell, SudokuBoard.GetDigitsForMask(mask)))
                    .ToList();

            var maskCells =
                cellWithDescriptions
                    .Where(cell => candidateMasks[cell.Index] == mask)
                    .ToArray();


            if (cells.Any())
            {
                var upper = 0;
                var lower = 0;
                var temp = mask;

                var value = 1;
                while (temp != 0)
                {
                    if ((temp & 1) != 0)
                    {
                        lower = upper;
                        upper = value;
                    }

                    temp = temp >> 1;
                    value += 1;
                }

                Console.WriteLine(
                    $"Values {lower} and {upper} in {cellWithDescriptions.First().Description} are in cells ({maskCells[0].Row + 1}, {maskCells[0].Column + 1}) and ({maskCells[1].Row + 1}, {maskCells[1].Column + 1}).");

                foreach (var cell in cells)
                {
                    var valuesToRemove = SudokuBoard.GetDigitsForMask(mask)
                        .Where(d => sudokuBoard.IsDigitPossible(d, cell.Cell)).ToList();

                    var valuesReport = string.Join(", ", valuesToRemove.ToArray());
                    Console.WriteLine(
                        $"{valuesReport} cannot appear in ({cell.Row + 1}, {cell.Column + 1}).");

                    sudokuBoard.RemovePossibilities(cell, valuesToRemove);
                    stepChangeMade = true;
                }
            }

            return stepChangeMade;
        }
    }
}