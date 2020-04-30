using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CmancNet.Compiler.ASTParser.AST.Statements
{
    class ASTCompileUnitNode : ASTNode, IASTStatementNode
    {
        public IList<ASTSubStatementNode> Procedures { get; private set; }

        public string Name { set; get; }

        public ASTCompileUnitNode(CmanParser.CompileUnitContext context) : base(null)
        {
            Name = Path.GetFileNameWithoutExtension(context.start.InputStream.SourceName);
            SetLocation(context);
        }

        public void AddProcedure(ASTSubStatementNode p)
        {
            if (Procedures == null)
                Procedures = new List<ASTSubStatementNode>();
            Procedures.Add(p);

        }

        public override IList<ASTNode> Children =>
            Procedures == null ? new List<ASTNode>() : Procedures.ToList<ASTNode>();

        public override string ToString()
        {
            return string.Format("{0}: [{1}]", base.ToString(), Name);
        }
    }
}
