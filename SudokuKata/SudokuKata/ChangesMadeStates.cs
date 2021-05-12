using System;

namespace SudokuKata
{
    internal class ChangesMadeStates
    {
        public bool CandidateChanged;
        public bool CellChanged;
        public static ChangesMadeStates None => new ChangesMadeStates();
        public bool IsChanged => CandidateChanged || CellChanged;

        public ChangesMadeStates DoIfUnchanged(Func<ChangesMadeStates> func)
        {
            if (!IsChanged)
            {
                return func();
            }

            return this;
        }

        public ChangesMadeStates DoIfUnchanged(ISudokuSolverStep step, Random rng, SudokuBoard puzzle)
        {
            return DoIfUnchanged(() => step.Do(rng, puzzle));
        }
    }
}