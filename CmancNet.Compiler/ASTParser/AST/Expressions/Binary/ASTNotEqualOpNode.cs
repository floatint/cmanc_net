using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTParser.AST.Expressions.Binary
{
    class ASTNotEqualOpNode : ASTAbstractBinOpNode, IASTLogicOpNode
    {
        public ASTNotEqualOpNode(CmanParser.EqualsOpContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }
    }

}
