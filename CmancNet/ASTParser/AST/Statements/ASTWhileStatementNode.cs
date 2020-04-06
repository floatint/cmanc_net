using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.ASTParser.AST.Expressions;

namespace CmancNet.ASTParser.AST.Statements
{
    class ASTWhileStatementNode : ASTNode, IASTStatementNode
    {
        public IASTExprNode Condition { set; get; }
        public ASTBodyStatementNode Body { set; get; }

        public ASTWhileStatementNode(CmanParser.WhileStatementContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }

        public override IList<ASTNode> Children => 
            new List<ASTNode> { (ASTNode)Condition, Body };

    }
}
