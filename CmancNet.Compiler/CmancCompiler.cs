using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using Antlr4.Runtime.Tree;
using CmancNet.Compiler.ASTParser;
using CmancNet.Compiler.ASTProcessors;
using CmancNet.Compiler.Codegen;
using CmancNet.Compiler.Utils.Logging;

namespace CmancNet.Compiler
{
    /// <summary>
    /// Base compiler implementation
    /// </summary>
    public class CmancCompiler : ICompiler
    {
        public CmancCompiler()
        {
            Messages = new List<MessageRecord>();
        }

        public AssemblyBuilder Compile(string sourcePath)
        {
            AssemblyBuilder builtAssembly = null;
            try
            {
                //get source reader
                var sourceCodeStream = Utils.SourceProvider.FromFile(sourcePath);
                //get token source
                ITokenSource tokenSource = new CmanLexer(sourceCodeStream);
                //get token stream
                ITokenStream tokenStream = new CommonTokenStream(tokenSource);
                //get parser
                CmanParser parser = new CmanParser(tokenStream);
                // 

                //TODO: add parser error handler

                //
                IParseTree parseTree = parser.compileUnit();
                ParseTreeWalker walker = new ParseTreeWalker();
                //ast builder
                var astBuilder = new ASTBuilderListener();
                //build ast tree
                walker.Walk(astBuilder, parseTree);
                var ast = astBuilder.CompilationUnit;
                //store ast builder messages
                Messages = Messages.Concat(astBuilder.Messages);
                //build symbol table 
                if (!astBuilder.Error)
                {
                    var symbolTableBuilder = new ASTSymbolTableBuilder(ast);
                    bool stBuilt = symbolTableBuilder.Build();
                    Messages = Messages.Concat(symbolTableBuilder.Messages);
                    //semantic check
                    if (stBuilt)
                    {
                        var symbolTable = symbolTableBuilder.Symbols;
                        var semanticChecker = new ASTSemanticChecker(ast, symbolTable);
                        bool valid = semanticChecker.IsValid();
                        Messages = Messages.Concat(semanticChecker.Messages);
                        //compilation
                        if (valid)
                        {
                            var codeBuilder = new CodeBuilder(ast, symbolTable);
                            try
                            {
                                builtAssembly = codeBuilder.Build();
                            }
                            catch(Exception ex)
                            {
                                Messages = Messages.Concat(new MessageRecord[] {
                                    new MessageRecord(
                                            MsgCode.CompilerError,
                                            sourcePath,
                                            null,
                                            null,
                                            ex.ToString()
                                        )
                                });
                            }
                        }
                    }
                }
            } catch(Exception ex)
            {
                Messages = Messages.Concat(new MessageRecord[] { new MessageRecord(
                    MsgCode.CompilerError,
                    sourcePath,
                    null,
                    null,
                    ex.ToString()
                    )});
            } 
            //TODO: add fail compilation msg
            if (builtAssembly == null)
            {
                Messages = Messages.Concat(new MessageRecord[] {
                    new MessageRecord(
                        MsgCode.CompilationFailed,
                        sourcePath,
                        null,
                        null,
                        Messages.Where(x => x.Message.Type == MsgType.Error).Count(),
                        Messages.Where(x => x.Message.Type == MsgType.Warning).Count()
                        )
                });
            }
            else
            {
                Messages = Messages.Concat(new MessageRecord[] {
                    new MessageRecord(
                        MsgCode.CompilationSuccessful,
                        sourcePath,
                        null,
                        null,
                        Messages.Where(x => x.Message.Type == MsgType.Error).Count(),
                        Messages.Where(x => x.Message.Type == MsgType.Warning).Count()
                        )
                });
            }
            return builtAssembly;
        }

        public IEnumerable<MessageRecord> Messages { private set; get; }
        public bool Error => Messages.Any(x => x.Message.Type == MsgType.Error);
    }
}
