using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuKata
{
    public class SudokuBoardAndStackState
    {
        public   SudokuBoardAndStackState()
        {
            SudokuBoardAndStackState sudokuBoardAndStackState;
            // Prepare empty board
            string line = "+---+---+---+";
            string middle = "|...|...|...|";
            char[][] board = new char[][]
            {
                line.ToCharArray(),
                middle.ToCharArray(),
                middle.ToCharArray(),
                middle.ToCharArray(),
                line.ToCharArray(),
                middle.ToCharArray(),
                middle.ToCharArray(),
                middle.ToCharArray(),
                line.ToCharArray(),
                middle.ToCharArray(),
                middle.ToCharArray(),
                middle.ToCharArray(),
                line.ToCharArray()
            };

            // Construct board to be solved

            // Top element is current state of the board
            var stateStack = new Stack<int[]>();

            StateStack = stateStack;
            Board = board;

        }

        public Stack<int[]> StateStack { get; private set; }
        public char[][] Board { get; private set; }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Board.Select(s => new string(s)).ToArray());
        }
    }
}