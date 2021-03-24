using System.Collections.Generic;

namespace SudokuKata
{
    public class Stacks
    {
        public readonly Stack<int[]> StateStack = new Stack<int[]>();
        // Top elements are (row, col) of cell which has been modified compared to previous state
        public Stack<int> RowIndexStack { get; } = new Stack<int>();
        public Stack<int> ColIndexStack { get; } = new Stack<int>();

        // Top element indicates candidate digits (those with False) for (row, col)
        public Stack<bool[]> UsedDigitsStack { get; } = new Stack<bool[]>();

        // Top element is the value that was set on (row, col)
        public Stack<int> LastDigitStack { get; } = new Stack<int>();
    }
}