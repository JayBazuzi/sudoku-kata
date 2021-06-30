using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalUtilities.Utilities;
using SudokuKata.Board;

namespace SudokuKata
{
    public class RemoveDigitsWhenConstrainedToAGroupOfNCells : ISudokuSolverStep
    {
        public ChangesMadeStates Do(Random random, SudokuBoard sudokuBoard)
        {
            var groupsWhichAreConstrainedToNCells = FindGroupsWhichAreConstrainedForDigits(sudokuBoard);

            var stepChangeMade = false;
            foreach (var g in groupsWhichAreConstrainedToNCells)
            {
                stepChangeMade |= RemoveDigitsWhenConstrainedToAGroupOfNCells_ForGroup(sudokuBoard, g);
            }

            return new ChangesMadeStates {CandidateChanged = stepChangeMade};
        }

        private static List<CellGroupsForDigits> FindGroupsWhichAreConstrainedForDigits(SudokuBoard sudokuBoard)
        {
            var cellGroups = SudokuBoard.BuildCellGroups();

            var digitPossibilities = GetAllCombinationsOfNumbersFromOneToNine();
            var groupsWhichAreConstrainedToNCells =
                digitPossibilities
                    .SelectMany(possibleDigits =>
                        cellGroups
                            .Where(group => group.All(cell => NoDigitsAreSolved(sudokuBoard, cell, possibleDigits)))
                            .Select(cells => CellGroupsForDigits.Create(sudokuBoard, possibleDigits, cells)))
                    .Where(group => group.CellsWhereADigitIsPossible.Count() == group.Digits.Count)
                    .ToList();
            return groupsWhichAreConstrainedToNCells;
        }

        private static bool NoDigitsAreSolved(SudokuBoard sudokuBoard, CellWithDescription cell,
            List<int> digitsForMask)
        {
            var digit = sudokuBoard.GetValueForCell(cell.Cell);
            var digitIsNotWhatWeAreLookingFor = !digitsForMask.Contains(digit);
            var cellIsUnsolved = digit == SudokuBoard.Unknown;
            return cellIsUnsolved || digitIsNotWhatWeAreLookingFor;
        }

        public static IEnumerable<List<int>> GetAllCombinationsOfNumbersFromOneToNine()
        {
            return LookupStructures.Instance._maskToOnesCount.Keys
                .Select(SudokuBoard.GetDigitsForMask)
                .Where(d => d.Count > 1);
        }

        private static bool RemoveDigitsWhenConstrainedToAGroupOfNCells_ForGroup(SudokuBoard sudokuBoard,
            CellGroupsForDigits g)
        {
            var description = g.Cells.First().Description;
            if (g.Cells.Any(cell =>
                sudokuBoard.IsAnyDigitPossible(cell.Cell, g.Digits) &&
                sudokuBoard.IsAnyDigitPossible(cell.Cell, g.RemainingDigits)))
            {
                var digitsAsText = string.Join(", ", g.Digits);
                var cellsAsText = g.CellsWhereADigitIsPossible.Select(cell => $"({cell.Row + 1}, {cell.Column + 1})")
                    .JoinWith(" ");
                Console.WriteLine(
                    $"In {description} values {digitsAsText} appear only in cells {cellsAsText} and other values cannot appear in those cells.");
            }

            var stepChangeMade = false;
            foreach (var cell in g.CellsWhereADigitIsPossible)
            {
                var possible = g.RemainingDigits.Where(d => sudokuBoard.IsDigitPossible(d, cell.Cell)).ToList();
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

        public class CellGroupsForDigits
        {
            public CellGroupsForDigits(List<int> digits, IEnumerable<int> remainingDigits,
                List<CellWithDescription> cells, List<CellWithDescription> cellsWhereADigitIsPossible)
            {
                Digits = digits;
                RemainingDigits = remainingDigits;
                Cells = cells;
                CellsWhereADigitIsPossible = cellsWhereADigitIsPossible;
            }

            public List<int> Digits { get; }
            public IEnumerable<int> RemainingDigits { get; }
            public List<CellWithDescription> Cells { get; }
            public List<CellWithDescription> CellsWhereADigitIsPossible { get; }

            public static CellGroupsForDigits Create(SudokuBoard sudokuBoard, List<int> possibleDigits,
                List<CellWithDescription> cells)
            {
                var remainingDigits = SudokuBoard.GetRemainingDigits(possibleDigits);
                var cellWithDescriptions = cells
                    .Where(cell => sudokuBoard.IsAnyDigitPossible(cell.Cell, possibleDigits)).ToList();
                return new CellGroupsForDigits(possibleDigits, remainingDigits, cells, cellWithDescriptions);
            }
        }
    }
}