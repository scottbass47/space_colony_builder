using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public sealed class StringUtils
    {

        public static string AddIndentation(object o, int numSpaces)
        {
            var builder = new StringBuilder();

            for(int i = 0; i < numSpaces; i++)
            {
                builder.Append(' ');
            }
            var spaces = builder.ToString();
            builder.Clear();

            var str = o.ToString();
            var lines = str.Split('\n');

            int idx = 0;
            foreach(var line in lines)
            {
                idx++;
                builder.Append($"{spaces}{line}{(idx == lines.Length ? "" : "\n")}");
            }
            return builder.ToString();
        }

    }
}
