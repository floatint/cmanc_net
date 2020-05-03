using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmancNet.Compiler.ASTParser.AST.Expressions;
using CmancNet.Compiler.ASTParser.AST.Expressions.Binary;
using CmancNet.Compiler.ASTParser.AST.Expressions.Unary;
using CmancNet.Compiler.ASTParser.AST.Statements;

namespace CmancNet.Compiler.ASTProcessors.Analysis
{
    class ASTExprHelper
    {
        public static Type GetExpressionType(IASTExprNode expr)
        {
            switch (expr)
            {
                case IASTLiteral litNode:
                    return GetLiteralType(litNode);
                case IASTBinOpNode binOpNode:
                    return GetBinOpType(binOpNode);
                case IASTUnarOpNode unarOpNode:
                    return GetUnarOpType(unarOpNode);
                case ASTCallStatementNode callNode:
                    return typeof(object);
                case ASTVariableNode varNode:
                    return typeof(object);

            }
            return null;
        }

        private static Type GetLiteralType(IASTLiteral litNode)
        {
            switch (litNode)
            {
                case ASTNumberLiteralNode numNode:
                    return typeof(decimal);
                case ASTStringLiteralNode strNode:
                    return typeof(string);
                case ASTBoolLiteralNode boolNode:
                    return typeof(bool);
                case ASTNullLiteralNode nullNode:
                    return typeof(void);
            }
            return null;
        }

        private static Type GetBinOpType(IASTBinOpNode binOp)
        {
            if (binOp is IASTArithmOpNode)
                return typeof(decimal);
            else
                return typeof(bool); //logical operations
        }

        private static Type GetUnarOpType(IASTUnarOpNode unarOp)
        {
            if (unarOp is ASTIndexOpNode)
                return typeof(object);
            if (unarOp is ASTMinusOpNode)
                return typeof(decimal);
            if (unarOp is ASTNotOpNode)
                return typeof(bool);
            return null;
        }

        public static bool IsValuable(IASTExprNode expr)
        {
            switch (expr)
            {
                case IASTLiteral litNode:
                    return true;
                case ASTNotOpNode notOpNode:
                    return IsValuable(notOpNode.Expression);
            }
            return false;
        }

        public static object GetValue(IASTExprNode expr)
        {
            switch (expr)
            {
                case ASTNotOpNode notOpNode:
                    return !Convert.ToBoolean(GetValue(notOpNode.Expression));
                case ASTMinusOpNode minusOpNode:
                    return decimal.Negate((decimal)GetValue(minusOpNode.Expression));
                case ASTNumberLiteralNode numNode:
                    return Convert.ToDecimal(numNode.Value);
                case ASTStringLiteralNode strNode:
                    return strNode.Value;
                case ASTNullLiteralNode nullNode:
                    return null;
                case ASTBoolLiteralNode boolNode:
                    return boolNode.Value;
            }
            return null;
        }
    }
}
