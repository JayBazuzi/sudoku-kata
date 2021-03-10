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

        public int RowToWrite { get; private set; }
        public int ColToWrite { get; private set; }
        public bool[] UsedDigits { get; private set; }
        public int[] CurrentState { get; private set; }
        public int CurrentStateIndex { get; private set; }
        public int MovedToDigit { get; private set; }
    }
}