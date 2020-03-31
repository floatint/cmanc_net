using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTWhileStatementNode : ASTStatementNode
    {
        public ASTWhileStatementNode(ASTNode parent, ASTNode condition, ASTNode body) : base(parent)
        {

        }
    }
}
