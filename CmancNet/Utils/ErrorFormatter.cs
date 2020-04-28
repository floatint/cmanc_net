using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.ASTParser.AST;

namespace CmancNet.Utils
{
    //TODO: MessageFormatter, добавить тип сообщения
    //TODO: возможно стоит создать мапу сообщений и индексировать ее по коду
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
