using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.Compiler.ASTParser.AST.Expressions;

namespace CmancNet.Compiler.ASTParser.AST.Statements
{
    class ASTAssignStatementNode : ASTNode, IASTStatementNode
    {
        private IASTExprNode _left;
        private IASTExprNode _right;

        public IASTExprNode Left
        {
            set
            {
                ((ASTNode)value).Parent = this;
                _left = value;
            }
            get
            {
                return _left;
            }
        }

        public IASTExprNode Right
        {
            set
            {
                ((ASTNode)value).Parent = this;
                _right = value;
            }
            get
            {
                return _right;
            }
        }


        public override IList<ASTNode> Children => 
            new List<ASTNode> { (ASTNode)Left, (ASTNode)Right };

        public ASTAssignStatementNode(CmanParser.AssignStatementContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }

    }
}
