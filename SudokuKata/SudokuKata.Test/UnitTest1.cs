using System;
using System.IO;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Combinations;
using ApprovalUtilities.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuKata.Board;

namespace SudokuKata.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            for (var i = 0; i < 10; i++)
            {
                Program.Play(new Random(i));
            }

            Approvals.Verify(output);
        }

        [TestMethod]
        public void TestEmptyBoard()
        {
            Approvals.Verify(new SudokuBoard());
        }

        [TestMethod]
        public void GetRows()
        {
            var result = SudokuBoard.GetRows();
            Approvals.VerifyAll("rows", result, list => list.ToReadableString());
        }

        [TestMethod]
        public void GetColumns()
        {
            var result = SudokuBoard.GetColumns();
            Approvals.VerifyAll("columns", result, list => list.ToReadableString());
        }

        [TestMethod]
        public void GetBlocks()
        {
            var result = SudokuBoard.GetBlocks();
            Approvals.VerifyAll("blocks", result, list => list.ToReadableString());
        }

        [TestMethod]
        public void BuildCellGroupsTest()
        {
            var result = SudokuBoard.BuildCellGroups();
            Approvals.VerifyAll(result, "results", _ => _.ToReadableString());
        }

        [TestMethod]
        public void GetDigitsForMaskTest()
        {
            var digits = Enumerable.Range(1, 9);
            CombinationApprovals.VerifyAllCombinations(
                (a, b, c) =>
                {
                    var mask = SudokuBoard.GetMaskForDigits(a, b, c);
                    return SudokuBoard
                        .GetDigitsForMask(mask).ToReadableString();
                },
                digits, digits, digits
            );
        }

        [TestMethod]
        public void GetDigitsForMasks_orSomething_LockingTest()
        {
            var result = RemoveDigitsWhenConstrainedToAGroupOfNCells.GetAllCombinationsOfNumbersFromOneToNine();
            Approvals.VerifyAll("", result, r => r.ToReadableString());
        }
    }
}