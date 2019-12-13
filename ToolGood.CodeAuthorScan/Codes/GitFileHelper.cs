using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ToolGood.CodeAuthorScan.Datas;
namespace ToolGood.CodeAuthorScan.Codes
{
    public class GitFileHelper
    {
        public static string GetFolderPath(string filePath)
        {
            var path = Path.Combine(filePath, ".git");
            if (Directory.Exists(path)) {
                return filePath;
            }
            if (filePath.EndsWith(":\\")) {
                return null;
            }
            return GetFolderPath(Path.GetDirectoryName(filePath));
        }

        public static List<GitFileInfo> GetFileInfo(string repositoryFolder, string file)
        {
            List<GitFileInfo> infos = new List<GitFileInfo>();
            var repository = new LibGit2Sharp.Repository(repositoryFolder);
            if (repositoryFolder.StartsWith("\\")==false) {
                repositoryFolder = repositoryFolder + "\\";
            }
            var rFile = file.Replace(repositoryFolder, "").Replace("\\","/");
            var t = repository.Blame(rFile);
            foreach (var item in t) {
                GitFileInfo info = new GitFileInfo() {
                    File = file,
                    Line = item.FinalStartLineNumber,
                    Author = item.FinalCommit.Author.Name,
                    CommitTime = item.FinalCommit.Author.When.DateTime
                };
                infos.Add(info);
            }
            return infos;
        }
        public static List<GitFileInfo> GetFileInfo(string RepositoryFolder, string file, int startLine, int endLine)
        {
            List<GitFileInfo> infos = new List<GitFileInfo>();
            var repository = new LibGit2Sharp.Repository(RepositoryFolder);
            var t = repository.Blame(file, new BlameOptions() { MinLine = startLine + 1, MaxLine = endLine + 1 });
            foreach (var item in t) {
                GitFileInfo info = new GitFileInfo() {
                    File = file,
                    Line = item.FinalStartLineNumber,
                    Author = item.FinalCommit.Author.Name,
                    CommitTime = item.FinalCommit.Author.When.DateTime
                };
                infos.Add(info);
            }
            return infos;
        }

    }
}
