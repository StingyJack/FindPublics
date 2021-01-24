namespace FindPublics
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("Specify the file path to analyze as the parameter");
            }

            var filePath = args[0].Trim();
            if (File.Exists(filePath) == false)
            {
                throw new FileNotFoundException("Need a real file to open", filePath);
            }

            Assembly targetAssembly;
            try
            {
                targetAssembly = Assembly.LoadFile(filePath);
            }
            catch (Exception e)
            {
                throw new BadImageFormatException($"Could not load {filePath} as a .net assembly", e);
            }

            var output = new StringBuilder();
            foreach (var asmPubType in targetAssembly.GetTypes().Where(t => t.IsPublic).OrderBy(t => t.FullName))
            {
                var hasOneExposedMember = false;

                var memberDetail = new StringBuilder();
                foreach (var pubMember in asmPubType.GetMembers().OrderBy(m => m.Name))
                {
                    if (IsExtraNoiseMember(pubMember))
                    {
                        continue;
                    }

                    hasOneExposedMember = true;
                    var pubMemberDescriptor = pubMember.ToString();
                    var shorterName = ReplaceClrTypesWithCsKeywords(pubMemberDescriptor);
                    memberDetail.AppendLine($"\t Member: {shorterName}");
                }

                if (hasOneExposedMember)
                {
                    output.AppendLine($"Public type: {asmPubType.FullName}");
                    output.AppendLine(memberDetail.ToString());
                }

            }
            Console.WriteLine(output.ToString());
            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press ENTER to close");
                Console.ReadLine();
            }
            
        }


        private static bool IsExtraNoiseMember(MemberInfo memberInfo)
        {
  
            if (memberInfo.MemberType != MemberTypes.Method)
            {
                return true;
            }

            if (memberInfo.Name.StartsWith("get_", StringComparison.Ordinal)
                || memberInfo.Name.StartsWith("set_", StringComparison.Ordinal))
            {
                return true;
            }

            if (memberInfo.Name.Equals("ToString", StringComparison.Ordinal)
                || memberInfo.Name.Equals("HasFlag", StringComparison.Ordinal)
                || memberInfo.Name.Equals("CompareTo", StringComparison.Ordinal)
                || memberInfo.Name.Equals("Equals", StringComparison.Ordinal)
                || memberInfo.Name.Equals("GetHashCode", StringComparison.Ordinal)
                || memberInfo.Name.Equals("GetTypeCode", StringComparison.Ordinal)
                || memberInfo.Name.Equals("GetType", StringComparison.Ordinal)
            )
            {
                return true;
            }

            return false;
        }


        private static string ReplaceClrTypesWithCsKeywords(string memberDescr)
        {
            var returnValue = memberDescr.Trim();
            returnValue = returnValue.Replace("System.String", "string");
            returnValue = returnValue.Replace("Int32", "int");
            returnValue = returnValue.Replace("Boolean", "bool");
            returnValue = returnValue.Replace("System.Object", "object");
            returnValue = returnValue.Replace("ByRef", "ref");



            return returnValue;

        }
    }
}