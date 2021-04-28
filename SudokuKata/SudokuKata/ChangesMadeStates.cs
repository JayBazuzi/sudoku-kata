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
    }
}