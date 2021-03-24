using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuKata
{
    static class _
    {
        public static int[,] SetAll(this int[,] that, int value)
        {
            for (int i = 0; i < that.GetLength(0); i++)
            {
                for (int j = 0; j < that.GetLength(1); j++)
                {
                    that[i,j] = value;
                }
            }

            return that;
        }

        public static void ForEachRowColumn(this int[,] board, Action<int, int> action)
        {
            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int column = 0; column < board.GetLength(1); column++)
                {
                    action(row, column);
                }
            }
        }
    }

    public class SudokuBoard
    {
        public const int Unknown = 0;

        private int[,] _board = new int[9, 9].SetAll(Unknown);

        public override string ToString()
        {

            string result1 = "";
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    string result = "";
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

                    result1 += result;
                }

            }
            

            return result1;
        }

        public void SetValue(int row, int column, int value)
        {
            _board[row, column] = value;
        }

        public string ToCodeString()
        {
            string result = "";

            _board.ForEachRowColumn((r, c) => result+=_board[r, c]);

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
    }
}

