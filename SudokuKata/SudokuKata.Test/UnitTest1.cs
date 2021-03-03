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
            Program.Play();
            Approvals.Verify(output);
        }
    }
}
