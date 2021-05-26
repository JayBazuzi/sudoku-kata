﻿using System;
using System.Collections.Generic;
using System.Linq;

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
        private Candidates candidates;

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

        public int[] GetBoardAsNumbers()
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
            return FromNumbers(GetBoardAsNumbers());
        }

        public Candidates GetCandidates(bool forceRecalculation = false)
        {
            if (forceRecalculation || candidates == null)
            {
                candidates = CalculateCandidatesForCurrentStateOfTheBoard();
            }


            return candidates;
        }

        private Candidates CalculateCandidatesForCurrentStateOfTheBoard()
        {
            var boardAsNumbers = GetBoardAsNumbers();

            var candidateMasks = new int[boardAsNumbers.Length];
            var candidates = new Candidates(candidateMasks);

            for (var i = 0; i < boardAsNumbers.Length; i++)
            {
                if (boardAsNumbers[i] == 0)
                {
                    var row = i / 9;
                    var col = i % 9;
                    var blockRow = row / 3;
                    var blockCol = col / 3;

                    var colidingNumbers = 0;
                    for (var j = 0; j < 9; j++)
                    {
                        var rowSiblingIndex = 9 * row + j;
                        var colSiblingIndex = 9 * j + col;
                        var blockSiblingIndex = 9 * (blockRow * 3 + j / 3) + blockCol * 3 + j % 3;

                        var rowSiblingMask = 1 << (boardAsNumbers[rowSiblingIndex] - 1);
                        var colSiblingMask = 1 << (boardAsNumbers[colSiblingIndex] - 1);
                        var blockSiblingMask = 1 << (boardAsNumbers[blockSiblingIndex] - 1);

                        colidingNumbers = colidingNumbers | rowSiblingMask | colSiblingMask | blockSiblingMask;
                    }

                    var allOnes = (1 << 9) - 1;
                    candidateMasks[i] = allOnes & ~colidingNumbers;
                }
            }

            return candidates;
        }

        public static List<IGrouping<int, SudokuConstraints_OrSomething>> BuildCellGroups()
        {
            var indexes = Enumerable.Range(0, 81);

            #region Build a collection (named cellGroups) which maps cell indices into distinct groups (rows/columns/blocks)

            var rowsIndices = indexes
                .Select(index => new SudokuConstraints_OrSomething(c => $"row #{c.Row + 1}", Cell.FromIndex(index, 0)))
                .GroupBy(tuple => tuple.Index / 9);

            var columnIndices = indexes
                .Select(index => new SudokuConstraints_OrSomething(c => $"column #{c.Column + 1}", Cell.FromIndex(index, 0)))
                .GroupBy(tuple => 9 + tuple.Index % 9);

            var blockIndices = indexes
                .Select(index => new SudokuConstraints_OrSomething(tuple => $"block ({tuple.Row / 3 + 1}, {tuple.Column / 3 + 1})", Cell.FromIndex(index, 0)))
                .GroupBy(c => 18 + 3 * (c.Row / 3) + c.Column / 3);

            var cellGroups = rowsIndices.Concat(columnIndices).Concat(blockIndices).ToList();

            #endregion

            return cellGroups;
        }

        public SudokuBoard GeneratePuzzleFromCompletelySolvedBoard(Random rng
        )
        {
            var puzzle = Clone();

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

                puzzle.SetValue(row, col, Unknown);

                removedPosition += 1;
            }

            return puzzle;
        }

        public void SetValue(Cell cell)
        {
            SetValue(cell.Row, cell.Column, cell.Value);
        }

        public static IEnumerable<IEnumerable<Cell>> GetRows()
        {
            for (var row = 0; row < 9; row++)
            {
                var result = new List<Cell>();
                for (var column = 0; column < 9; column++)
                {
                    result.Add(new Cell(row, column, 0));
                }

                yield return result;
            }
        }

        public static IEnumerable<IEnumerable<Cell>> GetColumns()
        {
            for (var column = 0; column < 9; column++)
            {
                var result = new List<Cell>();
                for (var row = 0; row < 9; row++)
                {
                    result.Add(new Cell(row, column, 0));
                }

                yield return result;
            }
        }

        public static IEnumerable<IEnumerable<Cell>> GetBlocks()
        {
            for (var cellGroup = 0; cellGroup < 9; cellGroup++)
            {
                var result = new List<Cell>();
                for (var indexInGroup = 0; indexInGroup < 9; indexInGroup++)
                {
                    var blockRowIndex = cellGroup / 3 * 3 + indexInGroup / 3;
                    var blockColIndex = cellGroup % 3 * 3 + indexInGroup % 3;
                    var blockStateIndex = blockRowIndex * 9 + blockColIndex;
                    result.Add(Cell.FromIndex(blockStateIndex, 0));
                }

                yield return result;
            }
        }

        public bool IsDigitPossible(int digit, Cell cell)
        {
            var candidateMasks = GetCandidates().Board;

            var mask = 1 << (digit - 1);
            var isDigitPossible = (candidateMasks[cell.ToIndex()] & mask) != 0;
            return isDigitPossible;
        }
    }
}