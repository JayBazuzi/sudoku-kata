﻿using System;

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
    }
}