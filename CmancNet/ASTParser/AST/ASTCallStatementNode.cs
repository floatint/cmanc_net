using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTCallStatementNode : ASTStatementNode
    {
        public string ProcedureName { set; get; }
        public IList<ASTExpressionNode> Arguments { set; get; }

        public ASTCallStatementNode(CmanParser.ProcCallStatementContext context, ASTNode parent) 
            : base(parent)
        {
            SetLocation(context);
            ProcedureName = context.children.First(x => x is CmanParser.NameContext).GetText();
            //TODO: args parsing
        }
    }
}
