using System;
using System.Collections.Generic;
using System.Linq;
using SudokuKata.Board;

namespace SudokuKata
{
    public class FindUniquePairPossibilityForDigits_AndDoSomething : ISudokuSolverStep
    {
        public ChangesMadeStates Do(
            Random rng, SudokuBoard sudokuBoard)
        {
            var cellGroups = SudokuBoard.BuildCellGroups();

            var pairPossibilities = sudokuBoard.GetPossibilities()
                .Where(p => p.Count == 2)
                .Distinct();

            var constraintsWithUniquePairPossibilities = pairPossibilities
                .SelectMany(possibleDigits =>
                    cellGroups
                        .Select(group => new
                        {
                            Cells = group,
                            PossibileDigits = possibleDigits,
                        }))
                .Where(group =>
                    group.Cells.Count(tuple => sudokuBoard.IsExactly(tuple.Cell, group.PossibileDigits)) == 2)
                .Where(group => group.Cells.Any(tuple =>
                    sudokuBoard.IsAnyDigitPossible(tuple.Cell, SudokuBoard.GetRemainingDigits(group.PossibileDigits))))
                .ToList();

            if (!constraintsWithUniquePairPossibilities.Any())
            {
                return ChangesMadeStates.None;
            }

            var stepChangeMade = false;
            foreach (var group in constraintsWithUniquePairPossibilities)
            {
                var cellWithDescriptions = group.Cells;
                stepChangeMade |= RemovePairs(sudokuBoard, cellWithDescriptions, group.PossibileDigits);
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