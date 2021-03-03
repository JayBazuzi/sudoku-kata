using System;
using System.IO;
using ApprovalTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SudokuKata.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            StringWriter output = new StringWriter();
            Console.SetOut(output);
            for (int i = 0; i < 10; i++)
            {
                Program.Play(new Random(i));
            }
            Approvals.Verify(output);
        }

        [TestMethod]
        public void TestEmptyBoard()
        {
            Approvals.Verify(new SudokuBoardAndStackState());
        }
    }
}
