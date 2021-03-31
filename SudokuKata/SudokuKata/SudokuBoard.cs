using System;
using System.Collections.Generic;

namespace SudokuKata
{
    internal static class _
    {
        public static int[,] SetAll(this int[,] that, int value)
        {
            return that.ForEachRowColumn((row, column) => that[row, column] = value);
        }

        public static int[,] ForEachRowColumn(this int[,] board, Action<int, int> actionOnRowAndColumn)
        {
            for (var row = 0; row < board.GetLength(0); row++)
            {
                for (var column = 0; column < board.GetLength(1); column++)
                {
                    actionOnRowAndColumn(row, column);
                }
            }

            return board;
        }
    }

    public class SudokuBoard
    {
        public const int Unknown = 0;

        private readonly int[,] _board = new int[9, 9].SetAll(Unknown);

        public override string ToString()
        {
            var result = "";

            _board.ForEachRowColumn((row, column) => result
                += PrintSpace_OrSomething(row, column));

            return result;
        }

        private string PrintSpace_OrSomething(int row, int column)
        {
            var result = "";
            var line = "+---+---+---+";
            if (column == 0 && row % 3 == 0)
            {
                result += line + "\n";
            }

            if (column % 3 == 0)
            {
                result += "|";
            }

            result += _board[row, column] == Unknown ? "." : _board[row, column].ToString();

            if (column == 8)
            {
                result += "|\n";
            }

            if (row == 8 && column == 8)
            {
                result += line;
            }

            return result;
        }

        public void SetValue(int row, int column, int value)
        {
            _board[row, column] = value;
        }

        public string ToCodeString()
        {
            var result = "";

            _board.ForEachRowColumn((r, c) => result += _board[r, c]);

            return result;
        }

        public int[] GetBoardAsNumber()
        {
            var result = new List<int>();

            _board.ForEachRowColumn((r, c) => result.Add(_board[r, c]));

            return result.ToArray();
        }

        public static SudokuBoard FromNumbers(int[] state)
        {
            var result = new SudokuBoard();

            result._board.ForEachRowColumn((r, c) => result._board[r, c] = state[r * 9 + c]);

            return result;
        }

        public SudokuBoard Clone()
        {
            return SudokuBoard.FromNumbers(GetBoardAsNumber());
        }
    }
}