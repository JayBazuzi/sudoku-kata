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
            // TODO: stop using GetCandidates here
            //var singleCandidateIndices = puzzle.GetCandidates().GetCellsWithOnlyOneCandidateRemaining();
            var singleCandidates = puzzle.GetPossibilities()
                .Select((possibilities, index) => new CellWithPossiblities(index, possibilities))
                .Where(c => c.Possibilities.Count == 1)
                .ToArray();


            var skip = singleCandidates.Length == 0 ? 0 : rng.Next(singleCandidates.Length);
            var cell = singleCandidates.Skip(skip).FirstOrDefault();
            if (cell == null)
            {
                return ChangesMadeStates.None;
            }

            puzzle.SetValue(cell.Cell.Row, cell.Cell.Column, cell.Possibilities.First());

            Console.WriteLine("({0}, {1}) can only contain {2}.", cell.Cell.Row + 1, cell.Cell.Column + 1, cell.Possibilities.First());

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