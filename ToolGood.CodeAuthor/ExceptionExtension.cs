using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using ToolGood.CodeAuthor.Datas;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace ToolGood.CodeAuthor
{
    public static class ExceptionExtension
    {
        private static CodeAuthorInfo CodeAuthor = new CodeAuthorInfo();

        public static string GetCodeAuthor(this Exception ex)
        {
            StringBuilder builder = new StringBuilder();
            {
                var list = new List<Exception>() { ex };
                var e = ex;
                while (e.InnerException!=null) {
                    e = e.InnerException;
                    list.Add(e);
                }

                foreach (var item in list) {
                    var type = item.TargetSite.DeclaringType;
                    builder.Append($"{type.Namespace}.{type.Name}.{item.TargetSite.Name}:");
                    var authors = CodeAuthor.GetCodeAuthor(type.Namespace, type.Name, item.TargetSite.Name);
                    for (int i = 0; i < authors.Count; i++) {
                        var author = authors[i];
                        builder.Append($"({i + 1}){author.Author}[{author.CommitTime.ToString("yyyy-MM-dd HH:mm:ss")}]");
                    }
                    builder.Append("\r\n");
                }
            }

 

            StackTrace stackTrace = new StackTrace(true);
            var frameList = stackTrace.GetFrames();
            foreach (var item in frameList) {
                var method = item.GetMethod();
                if (method == null) { continue; }
                var type = method.DeclaringType;
                if (type == null) { continue; }

                var authors = CodeAuthor.GetCodeAuthor(type.Namespace, type.Name, method.Name);
                if (authors.Count > 0) {
                    builder.Append($"{type.Namespace}.{type.Name}.{method.Name}:");
                    for (int i = 0; i < authors.Count; i++) {
                        var author = authors[i];
                        builder.Append($"({i + 1}){author.Author}[{author.CommitTime.ToString("yyyy-MM-dd HH:mm:ss")}]");
                    }
                    builder.Append("\r\n");
                }
            }
            return builder.ToString();
        }
    }
}
