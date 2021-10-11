using CppAst;
using CppAst.CodeGen.Common;
using CppAst.CodeGen.CSharp;
using System;
using System.IO;
using System.Linq;
using Zio.FileSystems;

namespace BindingsGenerator
{
	public static class Generator
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Generating bindings...");

			try
			{
				AcceptHeaderDirectory("headers", "Wgpu", "WGPU.NET");
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to generate bindings:");
				Console.WriteLine(e);
				Console.ReadKey();
			}

			Console.WriteLine("Done. Press any key to exit");
			Console.ReadKey();
		}

		public static void AcceptHeaderDirectory(string headerDirectory, string outputClass, string outputNamespace)
		{
			Console.WriteLine($"Reading from {headerDirectory}...");

			CSharpConverterOptions options = new CSharpConverterOptions()
			{
				DefaultNamespace = outputNamespace,
				DefaultClassLib = outputClass,
				DefaultOutputFilePath = outputClass + ".cs",
				GenerateEnumItemAsFields = false,
				TypedefCodeGenKind = CppTypedefCodeGenKind.NoWrap,
				DefaultDllImportNameAndArguments = "\"libwgpu\"",

				MappingRules =
				{
					r => r.MapAll<CppEnumItem>().CSharpAction((converter, element) =>
					{
						if (element is not CSharpEnumItem item)
							return;

						// replace literals like unchecked((int)0) with just 0
						if (item.Value.StartsWith("unchecked((int)"))
							item.Value = item.Value[15..^1];

						// remove redundancy in enum members
						string enumName = (item.Parent as CSharpEnum).Name;

						if (item.Name.StartsWith(enumName))
							item.Name = item.Name.Substring(enumName.Length);

						// at this point a few enums will be named 1D, 2D or 3D which is invalid, so we fix it
						if (item.Name.StartsWith("1D"))
							item.Name = "OneDimension";
						else if (item.Name.StartsWith("2DArray"))
							item.Name = "TwoDimensionalArray";
						else if (item.Name.StartsWith("2D"))
							item.Name = "TwoDimensions";
						else if (item.Name.StartsWith("3D"))
							item.Name = "ThreeDimensions";
					}),

					r => r.MapAll<CppElement>().CppAction((converter, element) =>
					{
						if (element is not ICppMember member)
							return;

						if (member.Name.StartsWith("WGPU"))
							member.Name = member.Name[4..];

						member.Name = member.Name.Replace("_", "");
					}),

					r => r.MapAll<CppFunction>().CSharpAction((converter, element) =>
					{
						if (element is not CSharpMethod method)
							return;

						if (method.Name.StartsWith("wgpu"))
						{
							(method.Attributes[0] as CSharpDllImportAttribute).EntryPoint = $"\"{method.Name}\""; ;
							method.Name = method.Name[4..];
						}
					})
				}
			};

			options.IncludeFolders.Add(headerDirectory);

			CSharpCompilation compilation = CSharpConverter.Convert(Directory.EnumerateFiles(headerDirectory, "*.h").ToList(), options);

			if (compilation.HasErrors)
			{
				Console.WriteLine("Failed to generate bindings due to:");

				foreach (CppDiagnosticMessage message in compilation.Diagnostics.Messages)
					if (message.Type == CppLogMessageType.Error)
						Console.WriteLine(message);

				Console.ReadKey();
				return;
			}

			using PhysicalFileSystem fileSystem = new PhysicalFileSystem();
			using SubFileSystem subFileSystem = new SubFileSystem(fileSystem, fileSystem.ConvertPathFromInternal("."));

			CodeWriter writer = new CodeWriter(new CodeWriterOptions(subFileSystem));

			compilation.DumpTo(writer);
		}
	}
}
