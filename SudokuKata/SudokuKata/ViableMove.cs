namespace SudokuKata
{
    public class ViableMove
    {
        public ViableMove(int rowToWrite, int colToWrite, bool[] usedDigits, int[] currentState, int currentStateIndex,
            int movedToDigit)
        {
            RowToWrite = rowToWrite;
            ColToWrite = colToWrite;
            UsedDigits = usedDigits;
            CurrentState = currentState;
            CurrentStateIndex = currentStateIndex;
            MovedToDigit = movedToDigit;
        }

        public int RowToWrite { get; }
        public int ColToWrite { get; }
        public bool[] UsedDigits { get; }
        public int[] CurrentState { get; }
        public int CurrentStateIndex { get; }
        public int MovedToDigit { get; }
    }
}