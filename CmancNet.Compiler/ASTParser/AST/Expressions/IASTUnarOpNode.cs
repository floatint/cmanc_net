using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTParser.AST.Expressions
{
    /// <summary>
    /// Interface for all unary operators
    /// </summary>
    interface IASTUnarOpNode : IASTExprNode
    {
        IASTExprNode Expression { set; get; }
    }
}
