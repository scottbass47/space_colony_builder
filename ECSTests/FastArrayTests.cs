using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECS;

namespace ECSTests
{
    // @TODO Add more test cases...
    [TestClass]
    public class FastMapTests
    {
        [TestMethod]
        public void TestAdd()
        {
            FastMap<string> arr = new FastMap<string>();
            arr.Put(1, "hey");
            arr.Put(1, "there");

            Assert.AreEqual(arr.Size, 1);
        }

    }
}
