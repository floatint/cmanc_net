using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.ASTParser.AST;

namespace CmancNet.Utils
{
    class ErrorFormatter
    {
        public static string Format(ASTNode token, string msg)
        {
            string res = "{0}({1},{2}): {3}";
            return string.Format(
                res,
                token.SourcePath,
                token.StartLine,
                token.StartPos,
                msg);
        }
       
    }
}
