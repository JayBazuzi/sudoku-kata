using System.Collections.Generic;

namespace SudokuKata
{
    public class Stacks
    {
        public Stacks()
        {
            // Top elements are (row, col) of cell which has been modified compared to previous state
            var rowIndexStack = new Stack<int>();
            var colIndexStack = new Stack<int>();

            // Top element indicates candidate digits (those with False) for (row, col)
            var usedDigitsStack = new Stack<bool[]>();

            // Top element is the value that was set on (row, col)
            var lastDigitStack = new Stack<int>();

            RowIndexStack = rowIndexStack;
            ColIndexStack = colIndexStack;
            UsedDigitsStack = usedDigitsStack;
            LastDigitStack = lastDigitStack;
        }

        public Stack<int> RowIndexStack { get; private set; }
        public Stack<int> ColIndexStack { get; private set; }
        public Stack<bool[]> UsedDigitsStack { get; private set; }
        public Stack<int> LastDigitStack { get; private set; }

    }
}