﻿using CppAst;
using CppAst.CodeGen.Common;
using CppAst.CodeGen.CSharp;
using System;
using System.Collections.Generic;
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
				AcceptHeaderDirectory(new[] { "headers", "headers/webgpu-headers" }, "Wgpu", "WGPU.NET");
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

		public static void AcceptHeaderDirectory(IEnumerable<string> headerDirectories, string outputClass, string outputNamespace)
		{
			Console.WriteLine("Searching in:");

			foreach (string directory in headerDirectories)
				Console.WriteLine($"- {directory}");

			CSharpConverterOptions options = new CSharpConverterOptions()
			{
				DefaultNamespace = outputNamespace,
				DefaultClassLib = outputClass,
				DefaultOutputFilePath = outputClass + ".cs",
				GenerateEnumItemAsFields = false,
				TypedefCodeGenKind = CppTypedefCodeGenKind.NoWrap,
				DefaultDllImportNameAndArguments = "\"libwgpu_native\"",

				MappingRules =
				{
					r => r.MapAll<CppEnumItem>().CSharpAction((converter, element) =>
					{
						if (element is not CSharpEnumItem item)
							return;

						// replace literals like unchecked((int)0) with just 0
						if (item.Value.StartsWith("unchecked((int)"))
							item.Value = item.Value[15..^1];

						CSharpEnum parent = item.Parent as CSharpEnum;

						// remove redundancy in enum members names
						string enumName = parent.Name;

						if (item.Name.StartsWith(enumName))
							item.Name = item.Name.Substring(enumName.Length);

						// this line is only really needed specifically for the WGPUInstanceBackend enum
						if (item.Value.Contains("WGPU"))
							item.Value = item.Value.Replace("WGPU" + parent.Name + "_", "");

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

			options.IncludeFolders.AddRange(headerDirectories);

			IEnumerable<string> headerFiles = headerDirectories.SelectMany(dir => Directory.EnumerateFiles(dir, "*.h"));
			CSharpCompilation compilation = CSharpConverter.Convert(headerFiles.ToList(), options);

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
