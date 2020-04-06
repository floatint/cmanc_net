using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST.Expressions
{
    /// <summary>
    /// Interface for all binary operators
    /// </summary>
    interface IASTBinOpNode : IASTExprNode
    {
        IASTExprNode Left { set; get; }
        IASTExprNode Right { set; get; }
    }
}
