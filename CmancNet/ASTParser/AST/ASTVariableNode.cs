using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTVariableNode : ASTExpressionNode
    {
        public string Name { set; get; }

        public ASTVariableNode(CmanParser.VarContext context, ASTNode parent) : base(parent)
        {
            Name = context.children.First(x => x is CmanParser.NameContext).GetText();
            SetLocation(context);
        }
    }
}
