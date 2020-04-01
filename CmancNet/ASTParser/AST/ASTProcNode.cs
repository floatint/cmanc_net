using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace CmancNet.ASTParser.AST
{
    class ASTProcNode : ASTNode
    {
        public string Name { set; get; }
        public ASTBodyNode Body { set; get; }
        public IList<ASTVariableNode> Arguments { set; get; }

        public ASTProcNode(CmanParser.ProcStatementContext context, ASTCompilationUnitNode compileUnit)
            : base(compileUnit)
        {
            Name = context.children.First(x => x is CmanParser.NameContext).GetText();
            SetLocation(context);
            var argsDecl = context.children.FirstOrDefault(x => x is CmanParser.ArgListDeclContext);
            if (argsDecl != null)
            {
                var argsList = ((ParserRuleContext)argsDecl).children.Where(x => x is CmanParser.VarContext).ToList();
                Arguments = new List<ASTVariableNode>();
                foreach (var a in argsList)
                {
                    Arguments.Add(new ASTVariableNode((CmanParser.VarContext)a, this));
                }
            } 
        }
    }
}
