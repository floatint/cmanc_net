using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.ASTParser.AST.Expressions;

namespace CmancNet.ASTParser.AST.Statements
{
    class ASTAssignStatementNode : ASTNode, IASTStatementNode
    {
        public IASTExprNode Left { set; get; }
        public IASTExprNode Right { set; get; }

        public override IList<ASTNode> Children => 
            new List<ASTNode> { (ASTNode)Left, (ASTNode)Right };

        public ASTAssignStatementNode(CmanParser.AssignStatementContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }

    }
}
