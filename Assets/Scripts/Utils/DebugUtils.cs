using System;

namespace Utils
{
    public sealed class DebugUtils
    {

        public static void Assert(bool condition)
        {
            if (!condition) throw new Exception();
        }

    }
}