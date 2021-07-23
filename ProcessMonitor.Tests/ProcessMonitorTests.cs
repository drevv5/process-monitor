using System;
using System.Diagnostics;
using NUnit.Framework;

namespace ProcessMonitoring.Tests
{
    public class ProcessMonitorTests
    {
        ProcessMonitor pm;

        [SetUp]
        public void Setup() =>
            pm = new ProcessMonitor();

        #region ArgsAreCorrect_Tests
        [Test]
        public void ArgsAreCorrect_ArgumentNullException()
        {
            string[] nullOrEmpty = { "", string.Empty };
            string[] args = { null, "4", "2" };
            foreach (var noe in nullOrEmpty)
            {
                args[0] = noe;
                Assert.That(() => pm.ArgsAreCorrect(args),
                    Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void ArgsAreCorrect_WrongAmmountOfArgumentsException()
        {
            string[] args = { "4", "2" };
            Assert.That(() => pm.ArgsAreCorrect(args),
                Throws.TypeOf<WrongAmmountOfArgumentsException>());
        }

        [Test]
        public void ArgsAreCorrect_ArithmeticException()
        {
            string[] args = { "notepad", "1", "4" };
            Assert.That(() => pm.ArgsAreCorrect(args),
                Throws.TypeOf<ArithmeticException>());
        }
        [Test]
        public void ArgsAreCorrect_CorrectProcessNameArgumentException()
        {
            string[] args = { "&7jdf9aa(&*F", "19", "2" };
            Assert.That(() => pm.ArgsAreCorrect(args),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ArgsAreCorrect_Correct2ndAnd3rdArgumentException()
        {
            string[] args = { "notepad", "a5", "6f" };
            Assert.That(() => pm.ArgsAreCorrect(args),
                Throws.TypeOf<ArgumentException>());
        }
        #endregion

        #region ParseArgs_Tests
        [Test]
        public void ParseArgs_CorrectParse()
        {
            var args = new string[] { "notepad", "5", "1" };
            var expected = ("notepad", 5, 1);

            var actual = pm.ParseArgs(args);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseArgs_TryParseFormatException()
        {
            var args = new string[] { "notepad", "5a", "1f" };
            Assert.That(() => pm.ParseArgs(args),
                Throws.TypeOf<FormatException>());
        }

        #endregion
    }
}