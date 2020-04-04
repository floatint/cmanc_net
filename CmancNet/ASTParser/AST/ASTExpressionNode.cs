using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace CmancNet.ASTParser.AST
{
    //TODO: сделать интерфейсом
    abstract class ASTExpressionNode : ASTNode
    {
        public ASTExpressionNode(ASTNode parent) : base(parent)
        {
        }
    }
}
