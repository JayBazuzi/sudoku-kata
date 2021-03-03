using System.Collections.Generic;

namespace SudokuKata
{
    public class SudokuBoardAndStackState
    {
        public SudokuBoardAndStackState(Stack<int[]> stateStack, char[][] returnValue)
        {
            StateStack = stateStack;
            ReturnValue = returnValue;
        }

        public Stack<int[]> StateStack { get; private set; }
        public char[][] ReturnValue { get; private set; }
    }
}