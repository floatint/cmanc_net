using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CmancNet.ASTParser.AST.Statements
{
    class ASTCompileUnitNode : ASTNode, IASTStatementNode
    {
        public IList<ASTProcStatementNode> Procedures { get; private set; }

        public string Name { set; get; }

        public ASTCompileUnitNode(CmanParser.CompileUnitContext context) : base(null)
        {
            Name = Path.GetFileNameWithoutExtension(context.start.InputStream.SourceName);
            SetLocation(context);
        }

        public void AddProcedure(ASTProcStatementNode p)
        {
            if (Procedures == null)
                Procedures = new List<ASTProcStatementNode>();
            Procedures.Add(p);

        }

        public override IList<ASTNode> Children
        {
            get
            {
                return Procedures.ToList<ASTNode>();
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", GetType().Name, Name);
        }
    }
}
