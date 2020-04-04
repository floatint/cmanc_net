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
        public IList<ASTProcNode> Procedures { get; private set; }

        public override IList<ASTNode> Children {
            get
            {
                return Procedures.ToList<ASTNode>();
            }
        }

        public string Name { set; get; }

        public void AddProcedure(ASTProcNode p)
        {
            if (Procedures == null)
                Procedures = new List<ASTProcNode>();
            Procedures.Add(p);
            
        }

        public ASTCompilationUnitNode(CmanParser.CompileUnitContext context) : base(null)
        {
            Name = Path.GetFileNameWithoutExtension(context.start.InputStream.SourceName);
            SetLocation(context);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", GetType().Name, Name);
        }
    }
}
