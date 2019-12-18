using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ToolGood.CodeAuthorScan.Datas
{
    public class CodeAuthorInfo
    {
        public class FileInfo
        {
            public int FileId { get; set; }
            public string File { get; set; }
        }
        public class GitUpdateInfo
        {
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
