using System;
using System.IO;
using ApprovalTests;
using ApprovalUtilities.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}