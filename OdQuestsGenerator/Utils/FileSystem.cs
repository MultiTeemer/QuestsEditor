using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace OdQuestsGenerator.Utils
{
	static class FileSystem
	{
		public static string ReadFileContents(string filePath)
		{
			using (var fileStream = File.OpenRead(filePath)) {
				using (var reader = new StreamReader(fileStream)) {
					return reader.ReadToEnd();
				}
			}
		}

		public static SyntaxTree ReadCodeFromFile(string filePath)
		{
			var content = ReadFileContents(filePath);
			return CSharpSyntaxTree.ParseText(content);
		}
	}
}
