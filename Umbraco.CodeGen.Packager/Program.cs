using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace Umbraco.CodeGen.Packager
{
    class Program
    {
        private const string PackageGuid = "3648B0E2-7E9D-4A2B-AAA7-FD68C23218A9";

        static void Main(string[] args)
        {
            var solutionDir = args.Length == 1 ? args[0] : Path.Combine(Environment.CurrentDirectory, @"..\..\..\");
            solutionDir = solutionDir.Trim();
            if (solutionDir.EndsWith("\""))
                solutionDir = solutionDir.Replace("\"", "\\");
            var packagePath = Path.Combine(solutionDir, @"Packaging\Package");
            var packageSourcePath = Path.Combine(packagePath, "Source");
            var guidPath = Path.Combine(packageSourcePath, PackageGuid);
#if DEBUG
			var packagerPath = Path.Combine(solutionDir, @"Umbraco.CodeGen.Packager\bin\debug");
#else
            var packagerPath = Path.Combine(solutionDir, @"Umbraco.CodeGen.Packager\bin\release");
#endif
            var buildPath = Path.Combine(solutionDir, "Umbraco.CodeGen.Tests");
            var packageDocPath = Path.Combine(solutionDir, @"Packaging\Umbraco.CodeGen.Package.xml");
            var packageDocTargetPath = Path.Combine(guidPath, "package.xml");
            var versionNumber = typeof(CodeGenerator).Assembly.GetName().Version.ToString();
            var zipPath = Path.Combine(packagePath, String.Format("Umbraco_CodeGen_{0}.zip", versionNumber));
            var waited = false;

            Console.WriteLine("-- Creating package --");
            Console.WriteLine("Using folder '{0}'", packagePath);

            if (Directory.Exists(packagePath))
            {
                Console.WriteLine("Deleting folder '{0}'", packagePath);
                Directory.Delete(packagePath, true);
                while (Directory.Exists(packagePath))
                {
                    Console.Write(".");
                    waited = true;
                }
                if (waited)
                    Console.WriteLine();
            }

            Console.WriteLine("Creating folder '{0}'", packagePath);
            Directory.CreateDirectory(packagePath);

            Console.WriteLine("Creating folder '{0}'", packageSourcePath);
            Directory.CreateDirectory(packageSourcePath);

            Console.WriteLine("Creating folder '{0}'", guidPath);
            Directory.CreateDirectory(guidPath);

            var doc = XDocument.Load(packageDocPath);
            foreach (var fileNode in Descendants(doc, "file"))
            {
                var orgPath = Descendants(fileNode, "orgPath").Single().Value.Substring(1).Replace("/", "\\");
                var orgName = Descendants(fileNode, "orgName").Single().Value;
                var parentPath = orgPath.Contains("bin") ? packagerPath : Path.Combine(buildPath, orgPath);
                var sourcePath = Path.Combine(parentPath, orgName);
                var targetPath = Path.Combine(guidPath, orgName);
                Console.WriteLine("Copying file '{0}'", sourcePath);
                File.Copy(sourcePath, targetPath);
                File.SetAttributes(targetPath, FileAttributes.Normal);
            }
            Descendants(doc, "version").Single().Value = versionNumber;

            Console.WriteLine("Saving package manifest to '{0}", packageDocTargetPath);
            doc.Save(packageDocTargetPath);

            Console.WriteLine("Zipping to {0}", zipPath);

            var fastZip = new FastZip();
            fastZip.CreateZip(zipPath, packageSourcePath, true, null);

            Console.ReadKey();
        }

        private static IEnumerable<XElement> Descendants(XContainer doc, string nodeName)
        {
            return doc.DescendantNodes().OfType<XElement>().Where(n => n.Name == nodeName);
        }
    }
}
