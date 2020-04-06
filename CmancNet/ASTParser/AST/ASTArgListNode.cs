using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.ASTParser.AST.Expressions;

namespace CmancNet.ASTParser.AST
{
    class ASTArgListNode : ASTNode
    {
        public IList<ASTVariableNode> Arguments { get; private set; }

        public override IList<ASTNode> Children => Arguments.ToList<ASTNode>();

        public ASTArgListNode(CmanParser.ArgListDeclContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }

        public void AddArgument(ASTVariableNode v)
        {
            if (Arguments == null)
                Arguments = new List<ASTVariableNode>();
            v.Parent = this;
            Arguments.Insert(0, v);
        }
    }
}
