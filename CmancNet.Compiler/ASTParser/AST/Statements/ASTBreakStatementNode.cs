using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTParser.AST.Statements
{
    class ASTBreakStatementNode : ASTNode, IASTStatementNode
    {

        public override IList<ASTNode> Children => new List<ASTNode>();

        public ASTBreakStatementNode(CmanParser.BreakStatementContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }
    }
}
