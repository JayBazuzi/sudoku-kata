using System;

namespace SudokuKata
{
    class ChangesMadeStates
    {
        public bool CandidateChanged; 
        public bool CellChanged;

        public void Reset()
        {
            CandidateChanged = false;
            CellChanged = false;
        }

        public ChangesMadeStates DoIfCellUnchanged(Func<ChangesMadeStates> func)
        {
            if (!CellChanged)
            {
                return func();
            }
            else
            {
                return this;
            }
        }
    }
}