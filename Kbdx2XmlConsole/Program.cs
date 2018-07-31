namespace Kbdx2XmlConsole
{
    using System;
    using System.IO;
    using KeePassLib;
    using KeePassLib.Keys;
    using KeePassLib.Serialization;
    using Microsoft.Extensions.CommandLineUtils;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var cmd = new CommandLineApplication();
            var kdbxArg = cmd.Option("-k | --kdbx <value>", "Kdbx file path", CommandOptionType.SingleValue);
            var pwdArg = cmd.Option("-p | --password <value>", "Kdbx password", CommandOptionType.SingleValue);

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

                ExportKbdx2Xml(kdbxArg.Value(), pwdArg.Value());
                return 0;
            });

            cmd.HelpOption("-? | -h | --help");
            cmd.Execute(args);  
        }

        private static void ExportKbdx2Xml(string kdbx2ExportPath, string kdbx2ExportPassword)
        {            
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
                 
            Console.WriteLine($"looks like {kdbx2ExportPath} was successfully exported to {xmlPath}");
        }
    }
}