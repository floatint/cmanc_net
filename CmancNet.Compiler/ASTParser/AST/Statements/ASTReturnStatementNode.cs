using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.Compiler.ASTParser.AST.Expressions;

namespace CmancNet.Compiler.ASTParser.AST.Statements
{
    class ASTReturnStatementNode : ASTNode, IASTStatementNode
    {
        private IASTExprNode _expr;

        public IASTExprNode Expression
        {
            set
            {
                ((ASTNode)value).Parent = this;
                _expr = value;
            }
            get
            {
                return _expr;
            }
        }

        public override IList<ASTNode> Children => 
            Expression == null ? new List<ASTNode>() : new List<ASTNode> { (ASTNode)Expression};

        public ASTReturnStatementNode(CmanParser.ReturnStatementContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }
    }
}
