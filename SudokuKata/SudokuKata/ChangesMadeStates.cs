using System;
using System.Runtime.Remoting.Messaging;

namespace SudokuKata
{
    class ChangesMadeStates
    {
        public bool CandidateChanged; 
        public bool CellChanged;
        public static ChangesMadeStates None => new ChangesMadeStates();
        public bool IsChanged => CandidateChanged || CellChanged;

        public void Reset()
        {
            CandidateChanged = false;
            CellChanged = false;
        }

        public ChangesMadeStates DoIfUnchanged(Func<ChangesMadeStates> func)
        {
            if (!CellChanged && !CandidateChanged)
            {
                return func();
            }
            else
            {
                return this;
            }
        }

        public ChangesMadeStates DoIfUnchanged(ISudokuSolverStep step, Random rng, SudokuBoard puzzle)
        {
            return DoIfUnchanged(() => step.Do(rng, puzzle));
        }
    }
}