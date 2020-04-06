using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST.Expressions
{
    class ASTVariableNode : ASTNode, IASTExprNode
    {
        public string Name { set; get; }

        public ASTVariableNode(CmanParser.VarContext context, ASTNode parent) : base(parent)
        {
            SetLocation(context);
            Name = context.children.First(x => x is CmanParser.NameContext).GetText();
        }

        public override IList<ASTNode> Children => new List<ASTNode>();

        public override string ToString()
        {
            return string.Format("{0} ({1})", GetType().Name, Name);
        }
    }
}
