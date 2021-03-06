﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.Compiler.ASTParser.AST.Expressions
{
    class ASTStringLiteralNode : ASTNode, IASTExprNode, IASTLiteral
    {
        public string Value { set; get; }

        public ASTStringLiteralNode(CmanParser.StringLiteralContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
            Value = context.STRING().GetText().Trim(new char[] { '"' });
        }

        public override IList<ASTNode> Children => new List<ASTNode>();

        public override string ToString()
        {
            return string.Format("{0}: [{1}]", base.ToString(), Value);
        }
    }
}
