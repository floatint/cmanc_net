using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTVariableNode : ASTNode
    {
        public string Name { set; get; }

        public ASTVariableNode(ASTNode parent) : base(parent)
        {

        }
    }
}
