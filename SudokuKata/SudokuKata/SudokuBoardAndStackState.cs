using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuKata
{
    public class SudokuBoardAndStackState
    {
        public   SudokuBoardAndStackState()
        {
            // Prepare empty board
            string line = "+---+---+---+";
            string middle = "|...|...|...|";
            Board = new char[][]
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

        public Stack<int[]> StateStack { get; private set; }
        public char[][] Board { get; private set; }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Board.Select(s => new string(s)).ToArray());
        }

        public static SudokuBoardAndStackState ConstructFullyPopulatedBoard(Random rng)
        {
            var sudokuBoardAndStackState = new SudokuBoardAndStackState();

            // Top elements are (row, col) of cell which has been modified compared to previous state
            Stack<int> rowIndexStack = new Stack<int>();
            Stack<int> colIndexStack = new Stack<int>();

            // Top element indicates candidate digits (those with False) for (row, col)
            Stack<bool[]> usedDigitsStack = new Stack<bool[]>();

            // Top element is the value that was set on (row, col)
            Stack<int> lastDigitStack = new Stack<int>();


            // Indicates operation to perform next
            // - expand - finds next empty cell and puts new state on stacks
            // - move - finds next candidate number at current pos and applies it to current state
            // - collapse - pops current state from stack as it did not yield a solution
            Command command = Command.Expand;
            while (sudokuBoardAndStackState.StateStack.Count <= 9 * 9)
            {
                command = Applesauce4(rng, command, sudokuBoardAndStackState.StateStack, rowIndexStack, colIndexStack, usedDigitsStack, lastDigitStack,
                    sudokuBoardAndStackState.Board);
            }


            Console.WriteLine();
            Console.WriteLine("Final look of the solved board:");
            var result = sudokuBoardAndStackState.ToString();
            Console.WriteLine(result);

            return sudokuBoardAndStackState;
        }

        private static Command Applesauce4(Random rng, Command command, Stack<int[]> stateStack, Stack<int> rowIndexStack,
            Stack<int> colIndexStack, Stack<bool[]> usedDigitsStack, Stack<int> lastDigitStack, char[][] board)
        {
            if (command == Command.Expand)
            {
                command = Applesauce_Expand(rng, stateStack, rowIndexStack, colIndexStack, usedDigitsStack, lastDigitStack);
            }
            else if (command == Command.Collapse)
            {
                command = Applesauce_Collapse(stateStack, rowIndexStack, colIndexStack, usedDigitsStack, lastDigitStack);
            }
            else if (command == Command.Move)
            {
                command = Applesauce_Move(stateStack, rowIndexStack, colIndexStack, usedDigitsStack, lastDigitStack, board);
            }

            return command;
        }

        private static Command Applesauce_Move(Stack<int[]> stateStack, Stack<int> rowIndexStack, Stack<int> colIndexStack, Stack<bool[]> usedDigitsStack,
            Stack<int> lastDigitStack, char[][] board)
        {
            Command command;
            int rowToMove = rowIndexStack.Peek();
            int colToMove = colIndexStack.Peek();
            int digitToMove = lastDigitStack.Pop();

            int rowToWrite = rowToMove + rowToMove / 3 + 1;
            int colToWrite = colToMove + colToMove / 3 + 1;

            bool[] usedDigits = usedDigitsStack.Peek();
            int[] currentState = stateStack.Peek();
            int currentStateIndex = 9 * rowToMove + colToMove;

            int movedToDigit = digitToMove + 1;
            while (movedToDigit <= 9 && usedDigits[movedToDigit - 1])
                movedToDigit += 1;

            if (digitToMove > 0)
            {
                usedDigits[digitToMove - 1] = false;
                currentState[currentStateIndex] = 0;
                board[rowToWrite][colToWrite] = '.';
            }

            if (movedToDigit <= 9)
            {
                lastDigitStack.Push(movedToDigit);
                usedDigits[movedToDigit - 1] = true;
                currentState[currentStateIndex] = movedToDigit;
                board[rowToWrite][colToWrite] = (char) ('0' + movedToDigit);

                // Next possible digit was found at current position
                // Next step will be to expand the state
                command = Command.Expand;
            }
            else
            {
                // No viable candidate was found at current position - pop it in the next iteration
                lastDigitStack.Push(0);
                command = Command.Collapse;
            }

            return command;
        }

        private static Command Applesauce_Collapse(Stack<int[]> stateStack, Stack<int> rowIndexStack, Stack<int> colIndexStack,
            Stack<bool[]> usedDigitsStack, Stack<int> lastDigitStack)
        {
            stateStack.Pop();
            rowIndexStack.Pop();
            colIndexStack.Pop();
            usedDigitsStack.Pop();
            lastDigitStack.Pop();

            return Command.Move;
        }

        private static Command Applesauce_Expand(Random rng, Stack<int[]> stateStack, Stack<int> rowIndexStack, Stack<int> colIndexStack,
            Stack<bool[]> usedDigitsStack, Stack<int> lastDigitStack)
        {
            int[] currentState = new int[9 * 9];

            if (stateStack.Count > 0)
            {
                Array.Copy(stateStack.Peek(), currentState, currentState.Length);
            }

            int bestRow = -1;
            int bestCol = -1;
            bool[] bestUsedDigits = null;
            int bestCandidatesCount = -1;
            int bestRandomValue = -1;
            bool containsUnsolvableCells = false;

            for (int index = 0; index < currentState.Length; index++)
                if (currentState[index] == 0)
                {
                    int row = index / 9;
                    int col = index % 9;
                    int blockRow = row / 3;
                    int blockCol = col / 3;

                    bool[] isDigitUsed = new bool[9];

                    for (int i = 0; i < 9; i++)
                    {
                        int rowDigit = currentState[9 * i + col];
                        if (rowDigit > 0)
                            isDigitUsed[rowDigit - 1] = true;

                        int colDigit = currentState[9 * row + i];
                        if (colDigit > 0)
                            isDigitUsed[colDigit - 1] = true;

                        int blockDigit = currentState[(blockRow * 3 + i / 3) * 9 + (blockCol * 3 + i % 3)];
                        if (blockDigit > 0)
                            isDigitUsed[blockDigit - 1] = true;
                    } // for (i = 0..8)

                    int candidatesCount = isDigitUsed.Where(used => !used).Count();

                    if (candidatesCount == 0)
                    {
                        containsUnsolvableCells = true;
                        break;
                    }

                    int randomValue = rng.Next();

                    if (bestCandidatesCount < 0 ||
                        candidatesCount < bestCandidatesCount ||
                        (candidatesCount == bestCandidatesCount && randomValue < bestRandomValue))
                    {
                        bestRow = row;
                        bestCol = col;
                        bestUsedDigits = isDigitUsed;
                        bestCandidatesCount = candidatesCount;
                        bestRandomValue = randomValue;
                    }
                } // for (index = 0..81)

            if (!containsUnsolvableCells)
            {
                stateStack.Push(currentState);
                rowIndexStack.Push(bestRow);
                colIndexStack.Push(bestCol);
                usedDigitsStack.Push(bestUsedDigits);
                lastDigitStack.Push(0); // No digit was tried at this position
            }

            // Always try to move after expand
            var command = Command.Move;
            return command;
        }
    }
}