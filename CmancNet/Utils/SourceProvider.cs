using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace CmancNet.Utils
{
    class SourceProvider
    {
        public static AntlrInputStream FromFile(string filePath)
        {
            return new AntlrFileStream(filePath, Encoding.UTF8);
        }
    }
}
