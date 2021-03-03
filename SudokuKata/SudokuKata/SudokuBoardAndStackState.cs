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

        public static string ToString(char[][] board)
        {
            return string.Join(Environment.NewLine, board.Select(s => new string(s)).ToArray());
        }
    }
}