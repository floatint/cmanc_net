using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTProcNode : ASTNode
    {
        public string Name { set; get; }
        public ASTBodyNode Body { set; get; }
        public IList<ASTVariableNode> Arguments { set; get; }

        public ASTProcNode(ASTCompilationUnitNode parent) : base(parent)
        {
            
        }
    }
}
