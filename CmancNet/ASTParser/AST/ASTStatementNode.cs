using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    //TODO: сделать интерфейсом
    abstract class ASTStatementNode : ASTExpressionNode
    {
        public ASTStatementNode(ASTNode parent) : base(parent)
        {
        }
    }
}
