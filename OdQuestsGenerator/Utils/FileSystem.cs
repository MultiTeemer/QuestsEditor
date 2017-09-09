using System.IO;

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
	}
}
