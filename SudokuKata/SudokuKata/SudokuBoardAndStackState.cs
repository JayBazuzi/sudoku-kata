using System;
using System.Collections.Generic;
using System.Linq;

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

        public string ToString()
        {
            return string.Join(Environment.NewLine, ReturnValue.Select(s => new string(s)).ToArray());
        }
    }
}