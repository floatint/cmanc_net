using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    interface IASTBinaryOpNode
    {
        ASTExpressionNode Left { set; get; }
        ASTExpressionNode Right { set; get; }
    }
}
