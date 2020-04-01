using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CmancNet.ASTParser.AST
{
    class ASTCompilationUnitNode : ASTNode
    {
        public IList<ASTProcNode> Procedures { set; get; }
        public string Name { set; get; }


        public ASTCompilationUnitNode(CmanParser.CompileUnitContext context) : base(null)
        {
            Name = Path.GetFileNameWithoutExtension(context.start.InputStream.SourceName);
            SetLocation(context);
            Procedures = new List<ASTProcNode>();
        }
    }
}
