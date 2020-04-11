using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST.Expressions.Binary
{
    class ASTLessCmpOpNode : ASTAbstractBinOpNode, IASTCmpOpNode
    {
        public ASTLessCmpOpNode(CmanParser.CompOpContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
        }
    }
}
