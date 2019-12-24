using NDesk.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToolGood.CodeAuthorScan.Codes;
using ToolGood.CodeAuthorScan.Datas;

namespace ToolGood.CodeAuthorScan
{
    class Program
    {
        public class Options
        {
            public bool IsRunned;
            public string FolderPath;
            public string OutputFilePath = "CodeAuthor.json";
            public List<String> Arguments = new List<string>();
        }
        private static OptionSet optionSet;
        private static Options options = new Options();

        static void Main(string[] args)
        {
            optionSet = new OptionSet()
            {
                { "folder|f=", "必填，指定pdb所在文档",v=>{options.FolderPath=v;  } },
                { "out|o=", "输出文件名，默认“CodeAuthor.json”",v=>{ options.OutputFilePath =v; } },
                { "help|?|h", "显示帮助文档", v => {ShowHelp();options.IsRunned=true; } },
                { "version|v", "显示版本号", v => {ShowVersion();options.IsRunned=true; } },
            };
            if (args == null || args.Length == 0) { ShowHelp(); return; }
            try {
                options.Arguments = optionSet.Parse(args);
                if (options.IsRunned==false) {
                    if (options.FolderPath==null) {
                        ShowHelp();
                    } else {
                        Analysis();
                    }
                }
            } catch (OptionException err) {
                Console.Error.WriteLine("fatal: " + err.Message);
            }
        }

        private static void Analysis()
        {
            var path = options.FolderPath;
            var pdbFiles = PdbFileHelper.GetPdbFiles(path);

            CodeAuthorInfo authorInfo = new CodeAuthorInfo();
            foreach (var pdbFile in pdbFiles) {
                var pdbFileInfos = PdbFileHelper.GetPdbInfos(pdbFile);

                var files = PdbFileHelper.GetFiles(pdbFileInfos);
                foreach (var file in files) {
                    if (authorInfo.HasFile(file)) { continue; }
                    authorInfo.AddFile(file);
                }

                foreach (var file in files) {
                    List<GitFileInfo> gitInfos = new List<GitFileInfo>();
                    var ifs = GitFileHelper.GetFileInfo(file);
                    authorInfo.AddGitUpdate(file, ifs);
                }

                foreach (var pdbFileInfo in pdbFileInfos) {
                    authorInfo.AddMethod(pdbFileInfo.Namespace, pdbFileInfo.Class, pdbFileInfo.Method
                            , pdbFileInfo.File, pdbFileInfo.LineStart, pdbFileInfo.LineEnd);
                }

            }
            File.WriteAllText(options.OutputFilePath, JsonConvert.SerializeObject(authorInfo));
        }


        private static void ShowHelp()
        {
            Console.Write("usage: CodeAuthorScan ");
            Console.WriteLine(string.Join(" ", optionSet.Select(o => "[--" + string.Join("|-", o.Names) + "]").ToArray()));
            optionSet.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }
        private static void ShowVersion()
        {
            Console.WriteLine("version:1.0.0.0");
        }

    }
}
