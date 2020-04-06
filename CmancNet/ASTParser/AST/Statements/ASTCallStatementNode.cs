using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.ASTParser.AST.Expressions;

namespace CmancNet.ASTParser.AST.Statements
{
    class ASTCallStatementNode : ASTNode, IASTStatementNode, IASTExprNode
    {
        private ASTExprListNode _args;

        public string ProcedureName { set; get; }
        public ASTExprListNode Arguments
        {
            set
            {
                value.Parent = this;
                _args = value;
            }
            get => _args;
        }

        public ASTCallStatementNode(CmanParser.SubCallStatementContext context, ASTNode parent) 
            : base(parent)
        {
            SetLocation(context);
            ProcedureName = context.children.First(x => x is CmanParser.NameContext).GetText();
        }

        public override IList<ASTNode> Children 
            => Arguments == null ? new List<ASTNode>() : new List<ASTNode> { Arguments };

        public override string ToString()
        {
            return string.Format("{0}: [{1}]", base.ToString(), ProcedureName); ;
        }
    }
}
