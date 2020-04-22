﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmancNet.ASTParser.AST.Expressions
{
    class ASTNumberLiteralNode : ASTNode, IASTExprNode, IASTLiteral
    {
        public decimal Value { set; get; }

        public ASTNumberLiteralNode(CmanParser.NumberLiteralContext context, ASTNode parent)
            : base(parent)
        {
            SetLocation(context);
            Value = decimal.Parse(context.GetText());
        }

        public override IList<ASTNode> Children => new List<ASTNode>();

        public override string ToString()
        {
            return string.Format("{0}: [{1}]", base.ToString(), Value);
        }
    }
}
