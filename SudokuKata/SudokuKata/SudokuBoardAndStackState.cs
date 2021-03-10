using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuKata
{
    public class ViableMove
    {
        public ViableMove(int rowToWrite, int colToWrite, bool[] usedDigits, int[] currentState, int currentStateIndex,
            int movedToDigit)
        {
            RowToWrite = rowToWrite;
            ColToWrite = colToWrite;
            UsedDigits = usedDigits;
            CurrentState = currentState;
            CurrentStateIndex = currentStateIndex;
            MovedToDigit = movedToDigit;
        }

        public int RowToWrite { get; private set; }
        public int ColToWrite { get; private set; }
        public bool[] UsedDigits { get; private set; }
        public int[] CurrentState { get; private set; }
        public int CurrentStateIndex { get; private set; }
        public int MovedToDigit { get; private set; }
    }

    public class SudokuBoardAndStackState
    {
        public SudokuBoardAndStackState()
        {
            // Prepare empty board
            var line = "+---+---+---+";
            var middle = "|...|...|...|";
            Board = new[]
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

            StateStack = new Stack<int[]>();
        }

        public Stack<int[]> StateStack { get; }
        public char[][] Board { get; }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Board.Select(s => new string(s)).ToArray());
        }

        public static SudokuBoardAndStackState ConstructFullyPopulatedBoard(Random rng)
        {
            var sudokuBoardAndStackState = new SudokuBoardAndStackState();

            // Top elements are (row, col) of cell which has been modified compared to previous state
            var rowIndexStack = new Stack<int>();
            var colIndexStack = new Stack<int>();

            // Top element indicates candidate digits (those with False) for (row, col)
            var usedDigitsStack = new Stack<bool[]>();

            // Top element is the value that was set on (row, col)
            var lastDigitStack = new Stack<int>();


            // Indicates operation to perform next
            // - expand - finds next empty cell and puts new state on stacks
            // - move - finds next candidate number at current pos and applies it to current state
            // - collapse - pops current state from stack as it did not yield a solution
            var command = Command.Expand;
            while (sudokuBoardAndStackState.StateStack.Count <= 9 * 9)
            {
                command = Applesauce4(rng, command, rowIndexStack, colIndexStack,
                    usedDigitsStack, lastDigitStack, sudokuBoardAndStackState);
            }


            Console.WriteLine();
            Console.WriteLine("Final look of the solved board:");
            var result = sudokuBoardAndStackState.ToString();
            Console.WriteLine(result);

            return sudokuBoardAndStackState;
        }

        private static Command Applesauce4(Random rng, Command command,
            Stack<int> rowIndexStack,
            Stack<int> colIndexStack, Stack<bool[]> usedDigitsStack, Stack<int> lastDigitStack,
            SudokuBoardAndStackState sudokuBoardAndStackState)
        {
            switch (command)
            {
                case Command.Expand:
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
                        rowIndexStack.Push(bestRow);
                        colIndexStack.Push(bestCol);
                        usedDigitsStack.Push(bestUsedDigits);
                        lastDigitStack.Push(0); // No digit was tried at this position
                    }

                    // Always try to move after expand
                    return Command.Move;
                case Command.Collapse:
                    sudokuBoardAndStackState.StateStack.Pop();
                    rowIndexStack.Pop();
                    colIndexStack.Pop();
                    usedDigitsStack.Pop();
                    lastDigitStack.Pop();

                    return Command.Move;
                case Command.Move:
                    var viableMove = GetViableMove(sudokuBoardAndStackState.StateStack, rowIndexStack, colIndexStack, usedDigitsStack, lastDigitStack,
                        sudokuBoardAndStackState.Board);

                    if (viableMove != null)
                    {
                        lastDigitStack.Push(viableMove.MovedToDigit);
                        viableMove.UsedDigits[viableMove.MovedToDigit - 1] = true;
                        viableMove.CurrentState[viableMove.CurrentStateIndex] = viableMove.MovedToDigit;
                        SetValue(sudokuBoardAndStackState, viableMove.RowToWrite, viableMove.ColToWrite, viableMove.MovedToDigit);

                        // Next possible digit was found at current position
                        // Next step will be to expand the state
                        return Command.Expand;
                    }
                    else
                    {
                        // No viable candidate was found at current position - pop it in the next iteration
                        lastDigitStack.Push(0);
                        return Command.Collapse;
                    }

                default:
                    return command;
            }
        }

        private static void SetValue(SudokuBoardAndStackState board, int row, int column, int value)
        {
            board.Board[row][column] = (char) ('0' + value);
        }

        private static ViableMove GetViableMove(Stack<int[]> stateStack, Stack<int> rowIndexStack,
            Stack<int> colIndexStack, Stack<bool[]> usedDigitsStack,
            Stack<int> lastDigitStack, char[][] board)
        {
            var rowToMove = rowIndexStack.Peek();
            var colToMove = colIndexStack.Peek();
            var digitToMove = lastDigitStack.Pop();

            var rowToWrite = rowToMove + rowToMove / 3 + 1;
            var colToWrite = colToMove + colToMove / 3 + 1;

            var usedDigits = usedDigitsStack.Peek();
            var currentState = stateStack.Peek();
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
                board[rowToWrite][colToWrite] = '.';
            }

            if (movedToDigit <= 9)
            {
                return new ViableMove(rowToWrite, colToWrite, usedDigits, currentState, currentStateIndex,
                    movedToDigit);
            }

            return null;
        }
    }
}