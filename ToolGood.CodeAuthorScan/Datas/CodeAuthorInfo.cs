using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ToolGood.CodeAuthorScan.Datas
{
    public class CodeAuthorInfo
    {
        #region Class 
        public class FileInfo
        {
            public int FileId { get; set; }
            public string File { get; set; }
        }
        public class GitUpdateInfo
        {
            public int UpdateId { get; set; }
            public int FileId { get; set; }
            public int LineStart { get; set; }
            public int LineEnd { get; set; }
            public string Author { get; set; }
            public DateTime CommitTime { get; set; }
        }
        public class MethodInfo
        {
            public string Namespace { get; set; }
            public string Class { get; set; }
            public string Method { get; set; }

            public List<int> GitUpdateInfoIds { get; set; }
        }
        #endregion

        public List<FileInfo> Files { get; set; }

        public List<GitUpdateInfo> GitUpdates { get; set; }

        public List<MethodInfo> Methods { get; set; }

        public CodeAuthorInfo()
        {
            Files = new List<FileInfo>();
            GitUpdates = new List<GitUpdateInfo>();
            Methods = new List<MethodInfo>();
        }

        public bool HasFile(string file)
        {
            return Files.Any(q => q.File == file);
        }
        public void AddFile(string file)
        {
            var id = Files.Count + 1;
            Files.Add(new FileInfo() { File = file, FileId = id });
        }
        public void AddGitUpdate(string file, List<GitFileInfo> gitFiles)
        {
            var fileId = Files.Where(q => q.File == file).First().FileId;

            GitUpdateInfo lastInfo = null;
            foreach (var gitFile in gitFiles) {
                if (lastInfo == null || lastInfo.Author != gitFile.Author) {
                    lastInfo = new GitUpdateInfo();
                    GitUpdates.Add(lastInfo);
                    lastInfo.UpdateId = GitUpdates.Count + 1;
                    lastInfo.Author = gitFile.Author;
                    lastInfo.CommitTime = gitFile.CommitTime;
                    lastInfo.LineStart = gitFile.Line;
                }
                lastInfo.LineEnd = gitFile.Line;
            }
        }
        public void AddMethod(string namespaceName, string className, string methodName, string file, int startLine, int endLine)
        {
            var fileId = Files.Where(q => q.File == file).First().FileId;
            var ids = GitUpdates.Where(q => q.FileId == fileId && q.LineStart >= startLine && q.LineEnd <= endLine).Select(q => q.UpdateId).ToList();
            Methods.Add(new MethodInfo() {
                Namespace = namespaceName,
                Class = className,
                Method = methodName,
                GitUpdateInfoIds = ids
            });
        }



        public void AddAuthor(string namespaceName, string className, string methodName, string author, DateTime commitTime)
        {
            //CodeNamespaceInfo codeNamespaceInfo;
            //CodeClassInfo codeClassInfo;
            //CodeMethodInfo codeMethodInfo;
            //if (NamespaceInfos.TryGetValue(namespaceName, out codeNamespaceInfo) == false) {
            //    codeNamespaceInfo = new CodeNamespaceInfo();
            //    NamespaceInfos[namespaceName] = codeNamespaceInfo;
            //}
            //if (codeNamespaceInfo.ClassInfos.TryGetValue(className, out codeClassInfo) == false) {
            //    codeClassInfo = new CodeClassInfo();
            //    codeNamespaceInfo.ClassInfos[className] = codeClassInfo;
            //}
            //if (codeClassInfo.MethodInfos.TryGetValue(methodName, out codeMethodInfo) == false) {
            //    codeMethodInfo = new CodeMethodInfo();
            //    codeClassInfo.MethodInfos[className] = codeMethodInfo;
            //}

            //CodeAuthor codeAuthor = codeMethodInfo.Authors.Where(q => q.Author == author && q.CommitTime == commitTime).FirstOrDefault();
            //if (codeAuthor == null) {
            //    codeAuthor = new CodeAuthor() {
            //        Author = author,
            //        CommitTime = commitTime
            //    };
            //    codeMethodInfo.Authors.Add(codeAuthor);
            //}
        }


    }
}
