using System;
using System.Collections.Generic;
using System.IO;
using ToolGood.CodeAuthorScan.Codes;
using ToolGood.CodeAuthorScan.Datas;
using Newtonsoft.Json;
using System.Threading.Tasks;

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

            CodeAuthorInfo authorInfo = new CodeAuthorInfo();
            foreach (var pdbFile in pdbFiles) {
                var pdbFileInfos = PdbFileHelper.GetPdbInfos(pdbFile);

                //foreach (var pdbFileInfo in pdbFileInfos) {
                //    if (authorInfo.HasFile(pdbFileInfo.File) ==false) {
                //        authorInfo.AddFile(pdbFileInfo.File);
                //    }
                //    List<GitFileInfo> gitInfos = new List<GitFileInfo>();
                //    var ifs = GitFileHelper.GetFileInfo(gitFolder, pdbFileInfo.File,pdbFileInfo.LineStart,pdbFileInfo.LineEnd);
                //    authorInfo.AddGitUpdate(pdbFileInfo.File, ifs);
                //}


                var files = PdbFileHelper.GetFiles(pdbFileInfos);
                foreach (var file in files) {
                    if (authorInfo.HasFile(file)) { continue; }
                    authorInfo.AddFile(file);
                }
                //System.Threading.Tasks.Parallel.ForEach(files, file => {
                //    List<GitFileInfo> gitInfos = new List<GitFileInfo>();
                //    var ifs = GitFileHelper.GetFileInfo(gitFolder, file);
                //    authorInfo.AddGitUpdate(file, ifs);
                //});

                foreach (var file in files) {
                    List<GitFileInfo> gitInfos = new List<GitFileInfo>();
                    var ifs = GitFileHelper.GetFileInfo(gitFolder, file);
                    authorInfo.AddGitUpdate(file, ifs);
                }

                foreach (var pdbFileInfo in pdbFileInfos) {
                    authorInfo.AddMethod(pdbFileInfo.Namespace, pdbFileInfo.Class, pdbFileInfo.Method
                            , pdbFileInfo.File, pdbFileInfo.LineStart, pdbFileInfo.LineEnd);
                }

                //List<GitFileInfo> gitInfos = new List<GitFileInfo>();
                //foreach (var file in files) {
                //    var ifs = GitFileHelper.GetFileInfo(gitFolder, file);
                //    gitInfos.AddRange(ifs);
                //}

                //CodeAuthorInfo authorInfo = new CodeAuthorInfo();
                //foreach (var pdbInfo in pdbFileInfos) {
                //    CodeAuthorHelper.BuildCodeAuthor(authorInfo, pdbInfo, gitInfos);
                //}
                //var outFile = CodeAuthorHelper.GetOutFile(pdbFile);

                //File.WriteAllText(outFile, JsonConvert.SerializeObject(authorInfo));
            }
            File.WriteAllText("1.json", JsonConvert.SerializeObject(authorInfo));

        }


    }
}
