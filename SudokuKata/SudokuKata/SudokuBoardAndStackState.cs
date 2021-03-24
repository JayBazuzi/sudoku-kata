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
    }

    public class SudokuBoardAndStackState
    {
        public const int Unknown = -1;

        public SudokuBoardAndStackState()
        {
            StateStack = new Stack<int[]>();
        }

        public Stack<int[]> StateStack { get; }
        private readonly int[,] _board = new int[9, 9].SetAll(Unknown);

        public override string ToString()
        {
            var line = "+---+---+---+";

            string result = "";
            for (int row = 0; row < 9; row++)
            {
                if (row % 3 == 0)
                {
                    result += line + "\n";
                }
                {
                    for (int column = 0; column < 9; column++)
                    {
                        if (column % 3 == 0)
                        {
                            result += "|";
                        }
                        {
                            result += _board[row,column] == Unknown ? "." : _board[row, column].ToString();
                        }
                    }

                    result += "|\n";
                }
            }
            result += line;

            return result;
        }

        public static SudokuBoardAndStackState ConstructFullySolvedBoard(Random rng)
        {
            var sudokuBoardAndStackState = new SudokuBoardAndStackState();

            var stacks = new Stacks();


            // Indicates operation to perform next
            // - expand - finds next empty cell and puts new state on stacks
            // - move - finds next candidate number at current pos and applies it to current state
            // - collapse - pops current state from stack as it did not yield a solution
            var command = Command.Expand;
            while (sudokuBoardAndStackState.StateStack.Count <= 9 * 9)
            {
                command = PopulateBoard(rng, command, stacks, sudokuBoardAndStackState);
            }


            Console.WriteLine();
            Console.WriteLine("Final look of the solved board:");
            var result = sudokuBoardAndStackState.ToString();
            Console.WriteLine(result);

            return sudokuBoardAndStackState;
        }

        private static Command PopulateBoard(Random rng, Command command, Stacks stacks,
            SudokuBoardAndStackState sudokuBoardAndStackState)
        {
            switch (command)
            {
                case Command.Expand:
                    return DoExpand(rng, stacks, sudokuBoardAndStackState);
                case Command.Collapse:
                    return DoCollapse(stacks, sudokuBoardAndStackState);
                case Command.Move:
                    return DoMove(stacks, sudokuBoardAndStackState);

                default:
                    return command;
            }
        }

        private static Command DoMove(Stacks stacks, SudokuBoardAndStackState sudokuBoardAndStackState)
        {
            var viableMove = GetViableMove(sudokuBoardAndStackState, stacks.RowIndexStack, stacks.ColIndexStack,
                stacks.UsedDigitsStack, stacks.LastDigitStack);

            if (viableMove != null)
            {
                stacks.LastDigitStack.Push(viableMove.MovedToDigit);
                viableMove.UsedDigits[viableMove.MovedToDigit - 1] = true;
                viableMove.CurrentState[viableMove.CurrentStateIndex] = viableMove.MovedToDigit;
                sudokuBoardAndStackState.SetValue(viableMove.RowToWrite, viableMove.ColToWrite,
                    viableMove.MovedToDigit);

                // Next possible digit was found at current position
                // Next step will be to expand the state
                return Command.Expand;
            }
            else
            {
                // No viable candidate was found at current position - pop it in the next iteration
                stacks.LastDigitStack.Push(0);
                return Command.Collapse;
            }
        }

        private static Command DoCollapse(Stacks stacks, SudokuBoardAndStackState sudokuBoardAndStackState)
        {
            sudokuBoardAndStackState.StateStack.Pop();
            stacks.RowIndexStack.Pop();
            stacks.ColIndexStack.Pop();
            stacks.UsedDigitsStack.Pop();
            stacks.LastDigitStack.Pop();

            return Command.Move;
        }

        private static Command DoExpand(Random rng, Stacks stacks, SudokuBoardAndStackState sudokuBoardAndStackState)
        {
            var currentState = new int[9 * 9];

            if (sudokuBoardAndStackState.StateStack.Count > 0)
            {
                Array.Copy(sudokuBoardAndStackState.StateStack.Peek(), currentState, currentState.Length);
            }

            var bestRow = -1;
            var bestCol = -1;
            bool[] bestUsedDigits = null;
            var bestCandidatesCount = -1;
            var bestRandomValue = -1;
            var containsUnsolvableCells = false;

            for (var index = 0; index < currentState.Length; index++)
            {
                if (currentState[index] == 0)
                {
                    var row = index / 9;
                    var col = index % 9;
                    var blockRow = row / 3;
                    var blockCol = col / 3;

                    var isDigitUsed = new bool[9];

                    for (var i = 0; i < 9; i++)
                    {
                        var rowDigit = currentState[9 * i + col];
                        if (rowDigit > 0)
                        {
                            isDigitUsed[rowDigit - 1] = true;
                        }

                        var colDigit = currentState[9 * row + i];
                        if (colDigit > 0)
                        {
                            isDigitUsed[colDigit - 1] = true;
                        }

                        var blockDigit = currentState[(blockRow * 3 + i / 3) * 9 + blockCol * 3 + i % 3];
                        if (blockDigit > 0)
                        {
                            isDigitUsed[blockDigit - 1] = true;
                        }
                    } // for (i = 0..8)

                    var candidatesCount = isDigitUsed.Where(used => !used).Count();

                    if (candidatesCount == 0)
                    {
                        containsUnsolvableCells = true;
                        break;
                    }

                    var randomValue = rng.Next();

                    if (bestCandidatesCount < 0 ||
                        candidatesCount < bestCandidatesCount ||
                        candidatesCount == bestCandidatesCount && randomValue < bestRandomValue)
                    {
                        bestRow = row;
                        bestCol = col;
                        bestUsedDigits = isDigitUsed;
                        bestCandidatesCount = candidatesCount;
                        bestRandomValue = randomValue;
                    }
                } // for (index = 0..81)
            }

            if (!containsUnsolvableCells)
            {
                sudokuBoardAndStackState.StateStack.Push(currentState);
                stacks.RowIndexStack.Push(bestRow);
                stacks.ColIndexStack.Push(bestCol);
                stacks.UsedDigitsStack.Push(bestUsedDigits);
                stacks.LastDigitStack.Push(0); // No digit was tried at this position
            }

            // Always try to move after expand
            return Command.Move;
        }

        public void SetValue(int row, int column, int value)
        {
            _board[row, column] = value;
        }

        private static ViableMove GetViableMove(SudokuBoardAndStackState sudokuBoardAndStackState,
            Stack<int> rowIndexStack,
            Stack<int> colIndexStack, Stack<bool[]> usedDigitsStack,
            Stack<int> lastDigitStack)
        {
            var rowToMove = rowIndexStack.Peek();
            var colToMove = colIndexStack.Peek();
            var digitToMove = lastDigitStack.Pop();

            var usedDigits = usedDigitsStack.Peek();
            var currentState = sudokuBoardAndStackState.StateStack.Peek();
            var currentStateIndex = 9 * rowToMove + colToMove;

            var movedToDigit = digitToMove + 1;
            while (movedToDigit <= 9 && usedDigits[movedToDigit - 1])
            {
                movedToDigit += 1;
            }

            if (digitToMove > 0)
            {
                usedDigits[digitToMove - 1] = false;
                currentState[currentStateIndex] = 0;
                sudokuBoardAndStackState.SetValue(rowToMove, colToMove, Unknown);
            }

            if (movedToDigit <= 9)
            {
                return new ViableMove(rowToMove, colToMove, usedDigits, currentState, currentStateIndex,
                    movedToDigit);
            }

            return null;
        }

        public string ToCodeString()
        {
            string result = "";

            for (int row = 0; row < _board.GetLength(0); row++)
            {
                for (int column = 0; column < _board.GetLength(1); column++)
                {
                    var value = _board[row, column];
                    result += value == Unknown ? 0 : value;
                }
            }

            return result;
        }

        public int[] GetState()
        {
            var result = new List<int>();

            for (int row = 0; row < _board.GetLength(0); row++)
            {
                for (int column = 0; column < _board.GetLength(1); column++)
                {
                    var value = _board[row, column];
                    //result += value == Unknown ? 0 : value;
                    result.Add(value == Unknown ? 0 : value);
                }
            }

            return result.ToArray();
        }
    }
}