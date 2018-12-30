using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECS;

namespace ECSTests
{
    // @TODO Add more test cases...
    [TestClass]
    public class BitsTest
    {
        [TestMethod]
        public void TestCreate()
        {
            Bits bits = new Bits(32 * 100);

            Assert.AreEqual(bits.Words.Length, 101);
        }

        [TestMethod]
        public void TestSet()
        {
            Bits bits = new Bits(10);
            bits.Set(0, true);
            bits.Set(100, true);

            Assert.AreEqual(1, bits.Words[0]);
            Assert.AreEqual(16, bits.Words[3]);
        }

        [TestMethod]
        public void TestGet()
        {
            Bits bits = new Bits(10);
            bits.Set(4, true);
            bits.Set(5, true);
            bits.Set(9, true);
            bits.Set(10, false);

            Assert.IsTrue(bits.Get(4));
            Assert.IsTrue(bits.Get(5));
            Assert.IsTrue(bits.Get(9));
            Assert.IsFalse(bits.Get(10));
            Assert.IsFalse(bits.Get(0));
        }
    }
}
