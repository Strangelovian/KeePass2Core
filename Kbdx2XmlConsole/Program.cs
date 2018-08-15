using System;
using System.Diagnostics;
using System.IO;
using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using Microsoft.Extensions.CommandLineUtils;

namespace Kdbx2XmlConsole
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var cmd = new CommandLineApplication();
            var kdbxArg = cmd.Option("-k | --kdbx <value>", "Kdbx file path", CommandOptionType.SingleValue);
            var pwdArg = cmd.Option("-p | --password <value>", "Kdbx password", CommandOptionType.SingleValue);
            var modeArg = cmd.Option("-m | --mode <value>", "Either xml or dup, defaults to xml. xml exports database to plain text xml file. dup loads and saves the database with a file name suffixed by .dup.kdbx", CommandOptionType.SingleValue);

            cmd.OnExecute(() =>
            {
                if (!kdbxArg.HasValue())
                {
                    Console.WriteLine($"{kdbxArg.Description} is mandatory");
                    return -1;
                }

                if (!pwdArg.HasValue())
                {
                    Console.WriteLine($"{pwdArg.Description} is mandatory");
                    return -1;
                }

                if (!modeArg.HasValue() || modeArg.Value() == "xml")
                {
                    ExportKdbx2Xml(kdbxArg.Value(), pwdArg.Value());
                }
                else
                {
                    ResaveKdbx(kdbxArg.Value(), pwdArg.Value());
                }

                return 0;
            });

            cmd.HelpOption("-? | -h | --help");
            cmd.Execute(args);  
        }

        private static void ExportKdbx2Xml(string kdbx2ExportPath, string kdbx2ExportPassword)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var pwbDatabase = new PwDatabase();

            var cmpKey = new CompositeKey();
            cmpKey.AddUserKey(new KcpPassword(kdbx2ExportPassword));
            
            var ioc = IOConnectionInfo.FromPath(kdbx2ExportPath);
            
            pwbDatabase.Open(ioc, cmpKey, null);
            
            var xmlExportFile = new KdbxFile(pwbDatabase);
            var xmlPath = Path.GetDirectoryName(kdbx2ExportPath) + Path.DirectorySeparatorChar
                          + Path.GetFileNameWithoutExtension(kdbx2ExportPath) + ".xml";
            var fileStream = new FileStream(xmlPath, FileMode.Create);
            
            xmlExportFile.Save(fileStream, null, KdbxFormat.PlainXml, null);
            var elapsed = stopwatch.Elapsed;

            Console.WriteLine($"{elapsed.TotalSeconds}s to export [{kdbx2ExportPath}] to [{xmlPath}]");
        }

        private static void ResaveKdbx(string kdbx2ExportPath, string kdbx2ExportPassword)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var pwbDatabase = new PwDatabase();

            var cmpKey = new CompositeKey();
            cmpKey.AddUserKey(new KcpPassword(kdbx2ExportPassword));
            
            var ioc = IOConnectionInfo.FromPath(kdbx2ExportPath);
            
            pwbDatabase.Open(ioc, cmpKey, null);

            var elapsed = stopwatch.Elapsed;
            Console.WriteLine($"{elapsed.TotalSeconds}s to load [{kdbx2ExportPath}]");

            stopwatch.Reset();

            var duplicate = new KdbxFile(pwbDatabase);
            var dupPath = Path.GetDirectoryName(kdbx2ExportPath)
                          + Path.DirectorySeparatorChar
                          + Path.GetFileNameWithoutExtension(kdbx2ExportPath)
                          + ".dup"
                          + Path.GetExtension(kdbx2ExportPath);
            var fileStream = new FileStream(dupPath, FileMode.Create);
            
            duplicate.Save(fileStream, null, KdbxFormat.Default, null);
            
            Console.WriteLine($"{elapsed.TotalSeconds}s to save [{kdbx2ExportPath}] to [{dupPath}]");
        }
    }
}