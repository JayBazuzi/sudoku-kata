﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuKata
{
    public class Program
    {
        public static void Play()
        {
            var rng = new Random();

            Play(rng);
        }


        public static void Play(Random rng)
        {
            var solvedBoard = SudokoBoardGenerator.ConstructFullySolvedBoard(rng);
            var puzzle = GeneratePuzzleFromCompletelySolvedBoard(rng, solvedBoard);
            LogStartOfSolution(puzzle);

            SolvePuzzle(rng, puzzle, solvedBoard);
        }

        private static void LogStartOfSolution(SudokuBoard puzzle)
        {
            Console.WriteLine();
            Console.WriteLine("Starting look of the board to solve:");
            Console.WriteLine(puzzle.ToString());


            PrintLineOfEquals();
        }


        private static void SolvePuzzle(Random rng, SudokuBoard puzzle,
            SudokuBoard solvedBoard)
        {
            ChangesMadeStates changesMadeStates ;

            do
            {
                var candidates2 = puzzle.GetCandidates(true);

                do
                {
                    changesMadeStates = ChangesMadeStates.None;
                    foreach (var step in GetSudokuSolverSteps())
                    {
                        changesMadeStates = changesMadeStates.DoIfUnchanged(step, rng, puzzle);
                    }
                } while (changesMadeStates.CandidateChanged);

                changesMadeStates = LookIfBoardHasMultipleSolutions(rng, changesMadeStates.CellChanged,
                    candidates2.Board,
                    solvedBoard.GetBoardAsNumbers(), puzzle);

                PrintBoardChange(changesMadeStates.CellChanged, puzzle);
            } while (changesMadeStates.CellChanged);
        }

        private static ISudokuSolverStep[] GetSudokuSolverSteps()
        {
            return new ISudokuSolverStep[]
            {
                new PickCellsWithOnlyOneCandidateRemaining(),
                new TryToFindANumberWhichCanOnlyAppearInOnePlaceInARowColumnBlock(),
                new TryToFindPairsOfDigitsInTheSameRowColumnBlockAndRemoveThemFromOtherCollidingCells(),
                new RemoveDigitsWhenConstrainedToAGroupOfNCells()
            };
        }


        public static LookupStructures PrepareLookupStructures()
        {
            #region Prepare lookup structures that will be used in further execution

            var maskToOnesCount = new Dictionary<int, int>();
            maskToOnesCount[0] = 0;
            for (var i = 1; i < 1 << 9; i++)
            {
                var smaller = i >> 1;
                var increment = i & 1;
                maskToOnesCount[i] = maskToOnesCount[smaller] + increment;
            }

            var singleBitToIndex = new Dictionary<int, int>();
            for (var i = 0; i < 9; i++)
            {
                singleBitToIndex[1 << i] = i;
            }

            #endregion

            return new LookupStructures(singleBitToIndex, maskToOnesCount);
        }

        private static void PrintLineOfEquals()
        {
            Console.WriteLine();
            Console.WriteLine(new string('=', 80));
            Console.WriteLine();
        }

        private static SudokuBoard GeneratePuzzleFromCompletelySolvedBoard(Random rng,
            SudokuBoard solvedBoard
        )
        {
            var puzzle = solvedBoard.Clone();

            // Board is solved at this point.
            // Now pick subset of digits as the starting position.
            var remainingDigits = 30;
            var maxRemovedPerBlock = 6;
            var removedPerBlock = new int[3, 3];
            var positions = Enumerable.Range(0, 9 * 9).ToArray();

            var removedPosition = 0;

            while (removedPosition < 9 * 9 - remainingDigits)
            {
                var curRemainingDigits = positions.Length - removedPosition;
                var indexToPick = removedPosition + rng.Next(curRemainingDigits);

                var row = positions[indexToPick] / 9;
                var col = positions[indexToPick] % 9;

                var blockRowToRemove = row / 3;
                var blockColToRemove = col / 3;

                if (removedPerBlock[blockRowToRemove, blockColToRemove] >= maxRemovedPerBlock)
                {
                    continue;
                }

                removedPerBlock[blockRowToRemove, blockColToRemove] += 1;

                var temp = positions[removedPosition];
                positions[removedPosition] = positions[indexToPick];
                positions[indexToPick] = temp;

                puzzle.SetValue(row, col, SudokuBoard.Unknown);

                removedPosition += 1;
            }

            return puzzle;
        }

        private static ChangesMadeStates LookIfBoardHasMultipleSolutions(Random rng, bool changeMade, int[] candidateMasks,
            int[] finalState,
            SudokuBoard sudokuBoard)
        {
            var state = sudokuBoard.GetBoardAsNumbers();
            var maskToOnesCount = PrepareLookupStructures()._maskToOnesCount;
            Stack<int[]> stateStack;
            Stack<int> rowIndexStack;
            Stack<int> colIndexStack;
            Stack<bool[]> usedDigitsStack;
            Stack<int> lastDigitStack;
            Command command;

            #region Final attempt - look if the board has multiple solutions

            if (!changeMade)
            {
                // This is the last chance to do something in this iteration:
                // If this attempt fails, board will not be entirely solved.

                // Try to see if there are pairs of values that can be exchanged arbitrarily
                // This happens when board has more than one valid solution

                var candidateIndex1 = new Queue<int>();
                var candidateIndex2 = new Queue<int>();
                var candidateDigit1 = new Queue<int>();
                var candidateDigit2 = new Queue<int>();

                for (var i = 0; i < candidateMasks.Length - 1; i++)
                {
                    if (maskToOnesCount[candidateMasks[i]] == 2)
                    {
                        var row = i / 9;
                        var col = i % 9;
                        var blockIndex = 3 * (row / 3) + col / 3;

                        var temp = candidateMasks[i];
                        var lower = 0;
                        var upper = 0;
                        for (var digit = 1; temp > 0; digit++)
                        {
                            if ((temp & 1) != 0)
                            {
                                lower = upper;
                                upper = digit;
                            }

                            temp = temp >> 1;
                        }

                        for (var j = i + 1; j < candidateMasks.Length; j++)
                        {
                            if (candidateMasks[j] == candidateMasks[i])
                            {
                                var row1 = j / 9;
                                var col1 = j % 9;
                                var blockIndex1 = 3 * (row1 / 3) + col1 / 3;

                                if (row == row1 || col == col1 || blockIndex == blockIndex1)
                                {
                                    candidateIndex1.Enqueue(i);
                                    candidateIndex2.Enqueue(j);
                                    candidateDigit1.Enqueue(lower);
                                    candidateDigit2.Enqueue(upper);
                                }
                            }
                        }
                    }
                }

                // At this point we have the lists with pairs of cells that might pick one of two digits each
                // Now we have to check whether that is really true - does the board have two solutions?

                var stateIndex1 = new List<int>();
                var stateIndex2 = new List<int>();
                var value1 = new List<int>();
                var value2 = new List<int>();

                while (candidateIndex1.Any())
                {
                    var index1 = candidateIndex1.Dequeue();
                    var index2 = candidateIndex2.Dequeue();
                    var digit1 = candidateDigit1.Dequeue();
                    var digit2 = candidateDigit2.Dequeue();

                    var alternateState = new int[finalState.Length];
                    Array.Copy(state, alternateState, alternateState.Length);

                    if (finalState[index1] == digit1)
                    {
                        alternateState[index1] = digit2;
                        alternateState[index2] = digit1;
                    }
                    else
                    {
                        alternateState[index1] = digit1;
                        alternateState[index2] = digit2;
                    }

                    // What follows below is a complete copy-paste of the solver which appears at the beginning of this method
                    // However, the algorithm couldn't be applied directly and it had to be modified.
                    // Implementation below assumes that the board might not have a solution.
                    stateStack = new Stack<int[]>();
                    rowIndexStack = new Stack<int>();
                    colIndexStack = new Stack<int>();
                    usedDigitsStack = new Stack<bool[]>();
                    lastDigitStack = new Stack<int>();

                    command = Command.Expand;
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
                        stateIndex1.Add(index1);
                        stateIndex2.Add(index2);
                        value1.Add(digit1);
                        value2.Add(digit2);
                    }
                } // while (candidateIndex1.Any())

                if (stateIndex1.Any())
                {
                    var pos = rng.Next(stateIndex1.Count());
                    var index1 = stateIndex1.ElementAt(pos);
                    var index2 = stateIndex2.ElementAt(pos);
                    var digit1 = value1.ElementAt(pos);
                    var digit2 = value2.ElementAt(pos);
                    var row1 = index1 / 9;
                    var col1 = index1 % 9;
                    var row2 = index2 / 9;
                    var col2 = index2 % 9;

                    var description = string.Empty;

                    if (index1 / 9 == index2 / 9)
                    {
                        description = $"row #{index1 / 9 + 1}";
                    }
                    else if (index1 % 9 == index2 % 9)
                    {
                        description = $"column #{index1 % 9 + 1}";
                    }
                    else
                    {
                        description = $"block ({row1 / 3 + 1}, {col1 / 3 + 1})";
                    }

                    state[index1] = finalState[index1];
                    state[index2] = finalState[index2];
                    candidateMasks[index1] = 0;
                    candidateMasks[index2] = 0;
                    changeMade = true;

                    for (var i = 0; i < state.Length; i++)
                    {
                        var tempRow = i / 9;
                        var tempCol = i % 9;

                        sudokuBoard.SetValue(tempRow, tempCol, SudokuBoard.Unknown);
                        if (state[i] > 0)
                        {
                            sudokuBoard.SetValue(tempRow, tempCol, state[i]);
                        }
                    }

                    Console.WriteLine(
                        $"Guessing that {digit1} and {digit2} are arbitrary in {description} (multiple solutions): Pick {finalState[index1]}->({row1 + 1}, {col1 + 1}), {finalState[index2]}->({row2 + 1}, {col2 + 1}).");
                }
            }

            #endregion

            return new ChangesMadeStates {CellChanged = changeMade};
        }

        private static void PrintBoardChange(bool changeMade, SudokuBoard sudokuBoard)
        {
            if (changeMade)
            {
                #region Print the board as it looks after one change was made to it

                Console.WriteLine(sudokuBoard);

                Console.WriteLine("Code: {0}", sudokuBoard.ToCodeString());
                Console.WriteLine();

                #endregion
            }
        }

        public static void Main(string[] args)
        {
            Play();

            Console.WriteLine();
            Console.Write("Press ENTER to exit... ");
            Console.ReadLine();
        }
    }
}