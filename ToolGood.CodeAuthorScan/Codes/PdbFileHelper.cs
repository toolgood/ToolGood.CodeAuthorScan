using SharpPdb.Managed.Windows;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using ToolGood.CodeAuthorScan.Datas;
using System.Linq;
using SharpPdb.Managed;
using Microsoft.DiaSymReader.Tools;
using System.Reflection.PortableExecutable;
using System.Xml;

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
            var pr = reader as SharpPdb.Managed.Windows.PdbFile;
            if (pr != null) {
                var funcs = reader.Functions;
                if (funcs.Count > 0) {
                    var sps = funcs[0].SequencePoints;
                    GetPdbInfos(infos, sps);
                }
            } else {
                var dllFile = Path.ChangeExtension(file, ".dll");
                if (File.Exists(dllFile)) {
                    using (var peStream = new FileStream(dllFile, FileMode.Open, FileAccess.Read))
                    using (var pdbStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    using (var dstFileStream = new FileStream(file + ".xml", FileMode.Create, FileAccess.ReadWrite))
                    using (var sw = new StreamWriter(dstFileStream, Encoding.UTF8)) {
                        PdbToXmlConverter.ToXml(sw, pdbStream, peStream);
                    }
                    GetPdbInfo(infos, file + ".xml");


                    //var pdbStream = File.OpenRead(file);
                    //using (var peStream = new FileStream(dllFile, FileMode.Open, FileAccess.Read))
                    //using (var peReader = new PEReader(peStream, PEStreamOptions.LeaveOpen)) {
                    //    using (var ms = new MemoryStream()) {
                    //        PdbToXmlConverter.ToXml(ms, pdbStream, peStream);

                    //        PdbToXmlConverter.ToXml(fs, peStream)
                    //        PdbConverter.Default.ConvertPortableToWindows(peStream, fs, ms);
                    //        var bytes = ms.ToArray();
                    //        File.WriteAllBytes(file + "2", bytes);
                    //    }
                    //}
                    //reader = SharpPdb.Managed.PdbFileReader.OpenPdb(file + "2");
                    //pr = reader as SharpPdb.Managed.Windows.PdbFile;
                    //if (pr != null) {
                    //    var funcs = reader.Functions;
                    //    if (funcs.Count > 0) {
                    //        var sps = funcs[0].SequencePoints;
                    //        GetPdbInfos(infos, sps);
                    //    }
                    //}
                }

            }

            //var funcs = reader.Functions;
            //if (funcs.Count > 0) {
            //    var sps = funcs[0].SequencePoints;
            //    GetPdbInfos(infos, sps);
            //}

            return infos;
        }
        private static void ConvertPdbToXml(string dllFile, string file)
        {
            var fs = File.OpenRead(file);
            using (var peStream = new FileStream(dllFile, FileMode.Open, FileAccess.Read))
            using (var peReader = new PEReader(peStream, PEStreamOptions.LeaveOpen)) {
                using (var ms = new MemoryStream()) {
                    PdbConverter.Default.ConvertPortableToWindows(peStream, fs, ms);
                    var bytes = ms.ToArray();
                    File.WriteAllBytes(file + ".xml", bytes);
                }
            }
        }

        private static void GetPdbInfo(List<PdbFileInfo> infos, string xmlFile)
        {
            var xml = File.ReadAllText(xmlFile);
            XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.LoadXml(xml);

            var fileNodes = xmlDocument.SelectNodes("/symbols/files/file");
            Dictionary<string, string> srcFiles = new Dictionary<string, string>();
            foreach (XmlNode node in fileNodes) {
                var key = node.Attributes["id"].Value;
                var value = node.Attributes["name"].Value;
                srcFiles[key] = value;
            }
            var methodNodes = xmlDocument.SelectNodes("/symbols/methods/method");
            foreach (XmlNode node in methodNodes) {
                var cname = node.Attributes["containingType"].Value;
                var @namespace = cname.Substring(0, cname.LastIndexOf('.'));
                var className = cname.Substring(cname.LastIndexOf('.') + 1);
                var mname = node.Attributes["name"].Value;

                var entrys = node.SelectNodes("/sequencePoints/entry");
                int startLine = int.MaxValue;
                int endLine = 0;
                var file = "";
                foreach (XmlNode entry in entrys) {
                    if (entry.Attributes["hidden"].Value == "true") {
                        var fileId = entry.Attributes["document"].Value;
                        file = srcFiles[fileId];
                        var sl = int.Parse(entry.Attributes["startLine"].Value);
                        var el = int.Parse(entry.Attributes["endColumn"].Value);
                        if (startLine < sl) { startLine = sl; }
                        if (endLine > el) { endLine = el; }
                    }
                }
                GetPdbInfos(infos, file, @namespace, className, mname, startLine, endLine);
            }
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
                var file = psp.Function.DbiModule.Files[0];
                var cname = psp.Function.DbiModule.ModuleName.String;
                var @namespace = cname.Substring(0, cname.LastIndexOf('.'));
                var className = cname.Substring(cname.LastIndexOf('.') + 1);
                var mname = psp.Function.Procedure.Name.String;
                GetPdbInfos(infos, file, @namespace, className, mname, startLine, endLine);
            }
        }

        private static void GetPdbInfos(List<PdbFileInfo> infos, string file, string @namespace, string className, string mname, int startLine, int endLine)
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
