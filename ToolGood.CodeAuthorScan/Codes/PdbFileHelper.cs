using SharpPdb.Managed.Windows;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using ToolGood.CodeAuthorScan.Datas;
using System.Linq;
using SharpPdb.Managed;
namespace ToolGood.CodeAuthorScan.Codes
{
    public class PdbFileHelper
    {
        public static List<string> GetPdbFiles(string folderPath)
        {
            return Directory.GetFiles(folderPath, "*.pdb").ToList();
        }

        public static List<PdbFileInfo> GetPdbInfos(string file)
        {
            List<PdbFileInfo> infos = new List<PdbFileInfo>();
            //Microsoft.DiaSymReader.SymUnmanagedReaderFactory.CreateReader()
            //Microsoft.DiaSymReader.PortablePdb.SymBinder 

            var reader = SharpPdb.Managed.PdbFileReader.OpenPdb(file);
            var funcs = reader.Functions;
            if (funcs.Count > 0) {
                var sps = funcs[0].SequencePoints;
                GetPdbInfos(infos, sps);
            }

            return infos;
        }
        private static void GetPdbInfos(List<PdbFileInfo> infos, IReadOnlyList<SharpPdb.Managed.IPdbSequencePoint> sequencePoints)
        {
            foreach (IPdbSequencePoint sp in sequencePoints) {
                if (sp.EndLine == sp.StartLine && sp.StartColumn == sp.EndColumn) {
                    continue;
                }
                var startLine = sp.StartLine;
                var endLine = sp.EndLine;
                SharpPdb.Managed.Windows.PdbSequencePoint psp = sp as SharpPdb.Managed.Windows.PdbSequencePoint;
                if (psp != null) {
                    var file = psp.Function.DbiModule.Files[0];
                    var cname = psp.Function.DbiModule.ModuleName.String;
                    var @namespace = cname.Substring(0, cname.LastIndexOf('.'));
                    var className = cname.Substring(cname.LastIndexOf('.') + 1);
                    var mname = psp.Function.Procedure.Name.String;
                    NewMethod(infos, file, @namespace, className, mname, startLine, endLine);
                } else {
                    SharpPdb.Managed.Portable.PdbSequencePoint psp2 = sp as SharpPdb.Managed.Portable.PdbSequencePoint;
                    var file = psp2.Source.Name;
                  var md=  psp2.Function.MethodDebugInformation;
                    //var file = psp2.Function.DbiModule.Files[0];
                    //var cname = psp2.Function.DbiModule.ModuleName.String;
                    //var @namespace = cname.Substring(0, cname.LastIndexOf('.'));
                    //var className = cname.Substring(cname.LastIndexOf('.') + 1);
                    //var mname = psp2.Function.Procedure.Name.String;
                    //NewMethod(infos, file, @namespace, className, mname, startLine, endLine);
                }

            }
        }
        private static void NewMethod(List<PdbFileInfo> infos, string file, string @namespace, string className, string mname, int startLine, int endLine)
        {
            var uqname = $"{file}.{@namespace}.{className}.{mname}";
            var info = infos.Where(q => q.GetFullName() == uqname).FirstOrDefault();
            if (info == null) {
                info = new PdbFileInfo() {
                    Namespace = @namespace,
                    Class = className,
                    Method = mname,
                    File = file,
                    LineStart = startLine,
                    LineEnd = endLine
                };
                infos.Add(info);
            } else {
                if (info.LineStart > startLine) {
                    info.LineStart = startLine;
                }
                if (info.LineEnd < endLine) {
                    info.LineEnd = endLine;
                }
            }
        }
        public static List<string> GetFiles(List<PdbFileInfo> infos)
        {
            return infos.Select(q => q.File).Distinct().ToList();
        }

        //PdbFile reader = SharpPdb.Managed.PdbFileReader.OpenPdb(@"D:\git\NGit\ConsoleApp2\bin\Debug\ConsoleApp2.pdb") as PdbFile;
        //var funcs = reader.Functions;
        //        if (funcs.Count>0) {
        //            var sps = funcs[0].SequencePoints;
        //            foreach (PdbSequencePoint sp in sps) {
        //                //sp.Source
        //            }
        //        }

        //        foreach (PdbFunction item in funcs) {
        //            Console.WriteLine("文件：");
        //            Console.WriteLine(item.DbiModule.Files[0]);
        //            Console.WriteLine("类名：");
        //            Console.WriteLine(item.DbiModule.ModuleName.String);
        //            Console.WriteLine("方法名：");
        //            Console.WriteLine(item.Procedure.Name);
        //            Console.WriteLine(JsonConvert.SerializeObject(item.Procedure.CodeOffset));
        //            Console.WriteLine(JsonConvert.SerializeObject(item.Procedure.CodeSize));
        //            Console.WriteLine(JsonConvert.SerializeObject(item.Procedure.SymbolStreamIndex));

        //            //Console.WriteLine($"{item.Procedure.Next},{item.Procedure.End}");
        //            foreach (var sp in item.SequencePoints) {
        //                Console.WriteLine($"{sp.StartLine},{sp.StartColumn},{sp.EndLine},{sp.EndColumn}");
        //            }


        //            Console.WriteLine("");
        //            Console.WriteLine("");
        //            //foreach (var localScope in item.LocalScopes) {
        //            //    var name = localScope.Variables[0].Name;
        //            //}
        //        }
    }
}
