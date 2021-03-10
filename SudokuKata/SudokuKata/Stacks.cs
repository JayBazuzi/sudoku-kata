using System.Collections.Generic;

namespace SudokuKata
{
    public class Stacks
    {
        public Stacks()
        {
        }

        // Top elements are (row, col) of cell which has been modified compared to previous state
        public Stack<int> RowIndexStack { get; private set; } = new Stack<int>();
        public Stack<int> ColIndexStack { get; private set; } = new Stack<int>();

        // Top element indicates candidate digits (those with False) for (row, col)
        public Stack<bool[]> UsedDigitsStack { get; private set; } = new Stack<bool[]>();

        // Top element is the value that was set on (row, col)
        public Stack<int> LastDigitStack { get; private set; } = new Stack<int>();
    }
}