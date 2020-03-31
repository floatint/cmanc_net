using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST
{
    class ASTCompilationUnitNode : ASTNode
    {
        public IList<ASTProcNode> Procedures { set; get; }
        public string Name { set; get; }

        public ASTCompilationUnitNode(string name) : base(null)
        {
            Name = name;
            Procedures = new List<ASTProcNode>();
        }
    }
}
