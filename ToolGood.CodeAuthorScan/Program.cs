using System;
using System.Collections.Generic;
using System.IO;
using ToolGood.CodeAuthorScan.Codes;
using ToolGood.CodeAuthorScan.Datas;
using Newtonsoft.Json;

namespace ToolGood.CodeAuthorScan
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = string.Join(" ", args);
            if (string.IsNullOrWhiteSpace(path)) {
                path = Directory.GetCurrentDirectory();
            }
            var pdbFiles = PdbFileHelper.GetPdbFiles(path);
            var gitFolder = GitFileHelper.GetFolderPath(path);
            foreach (var pdbFile in pdbFiles) {
                var pdbFileInfos = PdbFileHelper.GetPdbInfos(pdbFile);
                var files = PdbFileHelper.GetFiles(pdbFileInfos);
                List<GitFileInfo> gitInfos = new List<GitFileInfo>();
                foreach (var file in files) {
                    var ifs = GitFileHelper.GetFileInfo(gitFolder, file);
                    gitInfos.AddRange(ifs);
                }
                CodeAuthorInfo authorInfo = new CodeAuthorInfo();
                foreach (var pdbInfo in pdbFileInfos) {
                    CodeAuthorHelper.BuildCodeAuthor(authorInfo, pdbInfo, gitInfos);
                }
                var outFile = CodeAuthorHelper.GetOutFile(pdbFile);
                File.WriteAllText(outFile, JsonConvert.SerializeObject(authorInfo));
            }
        }
    }
}
