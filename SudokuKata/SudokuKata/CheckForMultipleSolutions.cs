using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ApprovalUtilities.Utilities;
using SudokuKata.Board;

namespace SudokuKata
{
    internal static class CheckForMultipleSolutions
    {
        public static ChangesMadeStates Do(Random rng, int[] candidateMasks,
            int[] finalState,
            SudokuBoard sudokuBoard)
        {
            var state = sudokuBoard.GetBoardAsNumbers();

            // This is the last chance to do something in this iteration:
            // If this attempt fails, board will not be entirely solved.

            // Try to see if there are pairs of values that can be exchanged arbitrarily
            // This happens when board has more than one valid solution

            var candidatesOfIndexesAndDigits = GetDeadlockedCellsWithTwoPossibilities(sudokuBoard);

            // At this point we have the lists with pairs of cells that might pick one of two digits each
            // Now we have to check whether that is really true - does the board have two solutions?

            // TODO: clean up these
            var stateIndexesAndValues = Applesauce2(rng, finalState, sudokuBoard, candidatesOfIndexesAndDigits, state);

            return Applesauce1(rng, finalState, sudokuBoard, stateIndexesAndValues, state);
        }

        private static List<Tuple<Cell, int, int, int>> Applesauce2(Random rng, int[] finalState, SudokuBoard sudokuBoard,
            Queue<Tuple<Cell, int, int, int>> candidatesOfIndexesAndDigits, int[] state)
        {
            var stateIndexesAndValues = new List<Tuple<Cell, int, int, int>>();

            while (candidatesOfIndexesAndDigits.Any())
            {
                var (index1, index2, digit1, digit2) = candidatesOfIndexesAndDigits.Dequeue();

                var alternateState = new int[finalState.Length];
                Array.Copy(state, alternateState, alternateState.Length);

                if (finalState[index1.ToIndex()] == digit1)
                {
                    alternateState[index1.ToIndex()] = digit2;
                    alternateState[index2] = digit1;
                }
                else
                {
                    alternateState[index1.ToIndex()] = digit1;
                    alternateState[index2] = digit2;
                }

                // What follows below is a complete copy-paste of the solver which appears at the beginning of this method
                // However, the algorithm couldn't be applied directly and it had to be modified.
                // Implementation below assumes that the board might not have a solution.
                var stateStack = new Stack<int[]>();
                var rowIndexStack = new Stack<int>();
                var colIndexStack = new Stack<int>();
                var usedDigitsStack = new Stack<bool[]>();
                var lastDigitStack = new Stack<int>();

                var command = Command.Expand;
                while (command != Command.Complete && command != Command.Fail)
                {
                    if (command == Command.Expand)
                    {
                        var currentState = new int[9 * 9];

                        if (stateStack.Any())
                        {
                            Array.Copy(stateStack.Peek(), currentState, currentState.Length);
                        }
                        else
                        {
                            Array.Copy(alternateState, currentState, currentState.Length);
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

                                    var blockDigit =
                                        currentState[(blockRow * 3 + i / 3) * 9 + blockCol * 3 + i % 3];
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
                            stateStack.Push(currentState);
                            rowIndexStack.Push(bestRow);
                            colIndexStack.Push(bestCol);
                            usedDigitsStack.Push(bestUsedDigits);
                            lastDigitStack.Push(0); // No digit was tried at this position
                        }

                        // Always try to move after expand
                        command = Command.Move;
                    } // if (command == Command.expand")
                    else if (command == Command.Collapse)
                    {
                        stateStack.Pop();
                        rowIndexStack.Pop();
                        colIndexStack.Pop();
                        usedDigitsStack.Pop();
                        lastDigitStack.Pop();

                        if (stateStack.Any())
                        {
                            command = Command.Move; // Always try to move after collapse
                        }
                        else
                        {
                            command = Command.Fail;
                        }
                    }
                    else if (command == Command.Move)
                    {
                        var rowToMove = rowIndexStack.Peek();
                        var colToMove = colIndexStack.Peek();
                        var digitToMove = lastDigitStack.Pop();

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
                            sudokuBoard.SetValue(rowToMove, colToMove,
                                SudokuBoard.Unknown);
                        }

                        if (movedToDigit <= 9)
                        {
                            lastDigitStack.Push(movedToDigit);
                            usedDigits[movedToDigit - 1] = true;
                            currentState[currentStateIndex] = movedToDigit;
                            sudokuBoard.SetValue(rowToMove, colToMove, movedToDigit);

                            if (currentState.Any(digit => digit == 0))
                            {
                                command = Command.Expand;
                            }
                            else
                            {
                                command = Command.Complete;
                            }
                        }
                        else
                        {
                            // No viable candidate was found at current position - pop it in the next iteration
                            lastDigitStack.Push(0);
                            command = Command.Collapse;
                        }
                    } // if (command == Command.move")
                } // while (command != "complete" && command != "fail")

                if (command == Command.Complete)
                {
                    // Board was solved successfully even with two digits swapped
                    stateIndexesAndValues.Add(Tuple.Create(
                        index1,
                        index2,
                        digit1,
                        digit2));
                }
            } // while (candidateIndex1.Any())

            return stateIndexesAndValues;
        }

        private static ChangesMadeStates Applesauce1(Random rng, int[] finalState, SudokuBoard sudokuBoard,
            List<Tuple<Cell, int, int, int>> stateIndexesAndValues, int[] state)
        {
            if (stateIndexesAndValues.Any())
            {
                var pos = rng.Next(stateIndexesAndValues.Count());
                var (cell1, index2, digit1, digit2) = stateIndexesAndValues.ElementAt(pos);
                var row1 = cell1.Row;
                var col1 = cell1.Column;
                var row2 = index2 / 9;
                var col2 = index2 % 9;

                string description;

                if (row1 == index2 / 9)
                {
                    description = $"row #{row1 + 1}";
                }
                else if (col1 == index2 % 9)
                {
                    description = $"column #{col1 + 1}";
                }
                else
                {
                    description = $"block ({row1 / 3 + 1}, {col1 / 3 + 1})";
                }

                state[cell1.ToIndex()] = finalState[cell1.ToIndex()];
                state[index2] = finalState[index2];

                for (var i = 0; i < state.Length; i++)
                {
                    var tempRow = i / 9;
                    var tempCol = i % 9;

                    var value = state[i];
                    sudokuBoard.SetValue(tempRow, tempCol, value);
                }

                Console.WriteLine(
                    $"Guessing that {digit1} and {digit2} are arbitrary in {description} (multiple solutions): Pick {finalState[cell1.ToIndex()]}->({row1 + 1}, {col1 + 1}), {finalState[index2]}->({row2 + 1}, {col2 + 1}).");
                return new ChangesMadeStates {CellChanged = true};
            }

            return new ChangesMadeStates {CellChanged = false};
        }

        private static Queue<Tuple<Cell, int, int, int>> GetDeadlockedCellsWithTwoPossibilities(SudokuBoard sudokuBoard)
        {
            var candidatesOfIndexesAndDigits = new Queue<Tuple<Cell, int, int, int>>();

            var cellsWithTwoPossible = sudokuBoard.GetCellsWithPossibilities().Where(c => c.Possibilities.Count == 2);
            foreach (var possibility in cellsWithTwoPossible)
            {
                var cell1 = possibility.Cell;

                foreach(var cell2 in Cell.ForBoard().Skip(cell1.ToIndex()+1))
                {
                    var matchingTwoPossiblesCell =
                        possibility.Possibilities.SequenceEqual(sudokuBoard.GetPossibilities(cell2));
                    var isMatchingGroup = cell1.IsSameRow(cell2) || cell1.IsSameColumn(cell2) || cell1.IsCellBlock(cell2);
                    if (isMatchingGroup && matchingTwoPossiblesCell)
                    {
                        var upper = possibility.Possibilities.Max();
                        var lower = possibility.Possibilities.Min();
                        candidatesOfIndexesAndDigits.Enqueue(Tuple.Create(cell1, cell2.ToIndex(), lower, upper));
                    }
                }
            }

            return candidatesOfIndexesAndDigits;
        }
    }
}