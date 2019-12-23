using NGit.Api;
using NGit.Diff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ToolGood.CodeAuthorScan.Datas;
namespace ToolGood.CodeAuthorScan.Codes
{
    public class GitFileHelper
    {
        private static Dictionary<string, string> gitRepositoryDict = new Dictionary<string, string>();
        private static Dictionary<string, Git> gitDict = new Dictionary<string, Git>();

        public static string GetFolderPath(string filePath)
        {
            var path = Path.Combine(filePath, ".git");
            if (Directory.Exists(path)) {
                Console.WriteLine($"获取git文件夹：{filePath} ");
                return filePath;
            }
            if (filePath.EndsWith(":\\")) {
                return null;
            }
            return GetFolderPath(Path.GetDirectoryName(filePath));
        }

        public static string GetRepositoryPath(string filePath)
        {
            var path = Path.Combine(filePath, ".git");
            if (Directory.Exists(path)) {
                gitRepositoryDict[filePath] = filePath;
                return filePath;
            }
            if (filePath.EndsWith(":\\")) {
                return null;
            }
            path = GetRepositoryPath(Path.GetDirectoryName(filePath));
            if (path != null) {
                gitRepositoryDict[filePath] = path;
            }
            return path;
        }

        public static Git GetRepository(string repositoryFolder)
        {
            Git git;
            if (gitDict.TryGetValue(repositoryFolder, out git) == false) {
                git = Git.Init().SetDirectory(repositoryFolder).Call();
                gitDict[repositoryFolder] = git;
            }
            return git;
        }

        public static List<GitFileInfo> GetFileInfo(string file)
        {
            List<GitFileInfo> list = new List<GitFileInfo>();
            if (File.Exists(file) == false) { return list; }
            Console.WriteLine("解析文件："+file);
            var repoPath = GetRepositoryPath(file);
            if (repoPath == null) { return list; }
            if (repoPath.StartsWith("\\") == false) {
                repoPath = repoPath + "\\";
            }
            var relativePath = file.Replace(repoPath, "").Replace("\\", "/");

            var git = GetRepository(repoPath);
            var blameHunks = git.Blame().SetFilePath(relativePath).SetTextComparator(RawTextComparator.WS_IGNORE_ALL).Call();
            blameHunks.ComputeAll();
            try {
                for (int i = 0; i < int.MaxValue; i++) {
                    var author = blameHunks.GetSourceAuthor(i);
                    //var author = blameHunks.GetSourceCommit(0);
                    GitFileInfo info = new GitFileInfo() {
                        File = file,
                        Line = i,
                        Author = author.GetName(),
                        CommitTime = author.GetWhen()
                    };
                    list.Add(info);
                }
            } catch (Exception) { }
            return list;
        }


        //public static List<GitFileInfo> GetFileInfo(string repositoryFolder, string file)
        //{
        //    List<GitFileInfo> infos = new List<GitFileInfo>();
        //    var git = Git.Init().SetDirectory(repositoryFolder).Call();


        //    if (repositoryFolder.StartsWith("\\") == false) {
        //        repositoryFolder = repositoryFolder + "\\";
        //    }
        //    var rFile = file.Replace(repositoryFolder, "").Replace("\\", "/");


        //    try {
        //        var t = repository.Blame(rFile,new BlameOptions { StartingAt = repository.Head,MaxLine=0 });
        //        //var t2 = repository.Blame(rFile);
        //        Console.WriteLine($"获取文件上传信息：{file} ");

        //        foreach (var item in t) {
        //            GitFileInfo info = new GitFileInfo() {
        //                File = file,
        //                Line = item.FinalStartLineNumber,
        //                Author = item.FinalCommit.Author.Name,
        //                CommitTime = item.FinalCommit.Author.When.DateTime
        //            };
        //            infos.Add(info);
        //        }
        //    } catch (Exception) { }
        //    return infos;
        //}
        //public static List<GitFileInfo> GetFileInfo(string repositoryFolder, string file, int startLine, int endLine)
        //{
        //    List<GitFileInfo> infos = new List<GitFileInfo>();
        //    var repository = new Repository(repositoryFolder);
        //    if (repositoryFolder.StartsWith("\\") == false) {
        //        repositoryFolder = repositoryFolder + "\\";
        //    }
        //    var rFile = file.Replace(repositoryFolder, "").Replace("\\", "/");
        //    try {
        //        var t = repository.Blame(rFile, new BlameOptions() { MinLine = startLine + 1, MaxLine = endLine + 1, Strategy = BlameStrategy.Default });
        //        Console.WriteLine($"获取文件上传信息：{file} ");

        //        foreach (var item in t) {
        //            GitFileInfo info = new GitFileInfo() {
        //                File = file,
        //                Line = item.FinalStartLineNumber,
        //                Author = item.FinalCommit.Author.Name,
        //                CommitTime = item.FinalCommit.Author.When.DateTime
        //            };
        //            infos.Add(info);
        //        }
        //    } catch (Exception) { }
        //    return infos;



        //    //List<GitFileInfo> infos = new List<GitFileInfo>();
        //    //var repository = new Repository(RepositoryFolder);
        //    //var t = repository.Blame(file, new BlameOptions() { MinLine = startLine + 1, MaxLine = endLine + 1 });
        //    //foreach (var item in t) {
        //    //    GitFileInfo info = new GitFileInfo() {
        //    //        File = file,
        //    //        Line = item.FinalStartLineNumber,
        //    //        Author = item.FinalCommit.Author.Name,
        //    //        CommitTime = item.FinalCommit.Author.When.DateTime
        //    //    };
        //    //    infos.Add(info);
        //    //}
        //    //return infos;
        //}

    }
}
