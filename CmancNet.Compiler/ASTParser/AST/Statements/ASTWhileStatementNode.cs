using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.Compiler.ASTParser.AST.Expressions;

namespace CmancNet.Compiler.ASTParser.AST.Statements
{
    class ASTWhileStatementNode : ASTNode, IASTStatementNode
    {
        private IASTExprNode _condition;
        private ASTBodyStatementNode _body;

        public IASTExprNode Condition
        {
            set
            {
                ((ASTNode)value).Parent = this;
                _condition = value;
            }
            get => _condition;
        }

        public ASTBodyStatementNode Body
        {
            set
            {
                value.Parent = this;
                _body = value;
            }
            get => _body;
        }

        public ASTWhileStatementNode(CmanParser.WhileStatementContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }

        public override IList<ASTNode> Children
        {
            get
            {
                var list = new List<ASTNode> { (ASTNode)Condition, Body };
                list.RemoveAll(x => x is null);
                return list;
            }
        }

    }
}
