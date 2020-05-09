using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Reflection.Emit;
using CmancNet.Compiler;
using CmancNet.Compiler.Utils.Logging;
using CommandLine;

namespace CmancNet.Driver
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            IList<MessageRecord> messages = new List<MessageRecord>();
            var timer = new Stopwatch();
            //parse arguments
            Parser.Default.ParseArguments<Options>(args)
                    .WithParsed(o =>
                    {
                        options = o;
                    });
            //try compile
            if (options.SourceFileName != null)
            {
                //check file exists
                if (File.Exists(options.SourceFileName))
                {
                    //try compile
                    ICompiler compiler = new CmancCompiler();
                    timer.Start();
                    AssemblyBuilder assembly = compiler.Compile(options.SourceFileName);
                    timer.Stop();
                    //store compiler messages
                    messages = messages.Concat(compiler.Messages).ToList();
                    //try set entry point
                    if (!compiler.Error)
                    {
                        var entryPoint = assembly.GetType("Program").GetMethod("main");
                        if (entryPoint == null)
                        {
                            messages.Add(new MessageRecord(
                                MsgCode.EntryPointNotFound,
                                options.SourceFileName,
                                null,
                                null,
                                "main()"
                                ));
                        }
                        else
                        {
                            //try save assembly
                            try
                            {
                                assembly.SetEntryPoint(entryPoint);
                                string outName = options.OutputFileName == null ?
                                    Path.GetFileNameWithoutExtension(options.SourceFileName) :
                                    Path.GetFileNameWithoutExtension(options.OutputFileName);
                                assembly.Save(outName + ".exe");
                            }
                            catch (Exception ex)
                            {
                                messages.Add(new MessageRecord(
                                    MsgCode.CompilerError,
                                    options.SourceFileName,
                                    null,
                                    null,
                                    "couldn't save compiled assembly.\n" + ex.ToString()
                                    ));
                            }
                        }
                    }
                }
                else
                {
                    messages.Add(new MessageRecord(
                        MsgCode.CompilerError,
                        null,
                        null,
                        null,
                        string.Format("source file '{0}' not found", options.SourceFileName)
                        ));
                    options.SourceFileName = null;
                }
                int errorsCnt = messages.Where(x => x.Message.Type == MsgType.Error).Count();
                int warnCnt = messages.Where(x => x.Message.Type == MsgType.Warning).Count();
                if (errorsCnt != 0)
                {
                    messages.Add(new MessageRecord(
                            MsgCode.CompilationFailed,
                            options.SourceFileName,
                            null,
                            null,
                            errorsCnt,
                            warnCnt
                        ));
                }
                else
                {
                    messages.Add(new MessageRecord(
                        MsgCode.CompilationSuccessful,
                        options.SourceFileName,
                        null,
                        null,
                        errorsCnt,
                        warnCnt
                    ));
                }
                messages.Add(new MessageRecord(
                    MsgCode.CompilationTime,
                    options.SourceFileName,
                    null,
                    null,
                    timer.ElapsedMilliseconds / 1000d
                    ));
            }
            else
            {
                messages.Add(new MessageRecord(
                    MsgCode.CompilerError,
                    null,
                    null,
                    null,
                    "Source file for compilation not defined"
                    ));
            }

            foreach (var m in messages)
                MessageRecordWriter.WriteLine(m);
        }
    }
}
