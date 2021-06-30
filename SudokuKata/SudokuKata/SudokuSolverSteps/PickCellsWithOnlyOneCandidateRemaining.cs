using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuKata
{
    internal class PickCellsWithOnlyOneCandidateRemaining : ISudokuSolverStep
    {
        public ChangesMadeStates Do(Random rng, SudokuBoard puzzle)
        {
            var singleCandidates = puzzle.GetPossibilities()
                .Select((possibilities, index) => new CellWithPossiblities(index, possibilities))
                .Where(c => c.Possibilities.Count == 1)
                .ToArray();


            var skip = singleCandidates.Length == 0 ? 0 : rng.Next(singleCandidates.Length);
            var single = singleCandidates.Skip(skip).FirstOrDefault();
            if (single == null)
            {
                return ChangesMadeStates.None;
            }

            var cell = single.Cell.WithValue(single.Possibilities.First());
            puzzle.SetValue(cell.Row, cell.Column, cell.Value);

            Console.WriteLine("({0}, {1}) can only contain {2}.", cell.Row + 1, cell.Column + 1, cell.Value);

            return new ChangesMadeStates {CellChanged = true};
        }
    }

    internal class CellWithPossiblities
    {
        public readonly List<int> Possibilities;
        public readonly Cell Cell;

        public CellWithPossiblities(int index, List<int> possibilities)
        {
            this.Possibilities = possibilities;
            this.Cell = Cell.FromIndex(index);
        }
    }
}