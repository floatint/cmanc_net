using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.Compiler.ASTParser.AST.Expressions;

namespace CmancNet.Compiler.ASTParser.AST.Statements
{
    class ASTForStatementNode : ASTNode, IASTStatementNode
    {
        private ASTNode _counter;
        private IASTExprNode _condition;
        private IASTExprNode _step;
        private ASTBodyStatementNode _body;

        public ASTNode Counter { set; get; }
        public IASTExprNode Condition { set; get; }
        public IASTExprNode Step { set; get; }
        public ASTBodyStatementNode Body { set; get; }

        public ASTForStatementNode(CmanParser.ForStatementContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }

        public override IList<ASTNode> Children
        {
            get
            {
                var list = new List<ASTNode> { Counter, (ASTNode)Condition, (ASTNode)Step };
                list.RemoveAll(x => x is null);
                return list;
            }
        }
    }
}
