using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OdQuestsGenerator.CodeReaders;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff
{
	class Loader
	{
		private static readonly Type[] readersOrder = {
			typeof(SectorReader),
			typeof(QuestReader),
			typeof(ConfigReader),
			typeof(ReachedConditionReader),
			typeof(QuestInitializationReader),
			typeof(QuestActionsReader),
		};

		private readonly Code code;

		public Loader(Code code)
		{
			this.code = code;
		}

		public Flow Load(string folder)
		{
			var gameDir = Directory.EnumerateDirectories(folder).FirstOrDefault(p => p.EndsWith(".Game"));

			code.PathToProject = gameDir ?? throw new Exception("Couldn't find \"*.Game\" directory");

			var sectorsDir = Path.Combine(gameDir, "Sectors");
			if (!Directory.Exists(sectorsDir)) {
				throw new Exception($"Couldn't find \"{sectorsDir}\" directory");
			}

			var secsDirs = Directory.EnumerateDirectories(sectorsDir)
				.Where(p => new DirectoryInfo(p).Name.StartsWith("Sector"));

			foreach (var secDir in secsDirs) {
				LoadSectorCode(secDir);
			}

			LoadConfigsCode(gameDir);
			LoadProjectFiles(gameDir);

			var flow = new Flow(new Graph(), new List<Sector>());

			foreach (var readerType in readersOrder) {
				var reader = CodeReadersRepo.GetReader(readerType);
				var codeBulks = code.CodeBulksOfTypes(reader.AcceptedTypes);
				foreach (var codeBulk in codeBulks) {
					reader.Read(codeBulk, code, ref flow);
				}
			}

			return flow;
		}

		public void LoadSectorCode(string dirPath)
		{
			var sectorFile = Directory.EnumerateFiles(dirPath)
				.FirstOrDefault(p => new FileInfo(p).Name.StartsWith("Sector"));
			if (sectorFile == null) {
				throw new Exception("Cannot find file describing sector");
			}

			code.ReadFromFile(Path.Combine(dirPath, sectorFile), CodeBulkType.Sector);

			var questsDir = Path.Combine(dirPath, "Quests");
			if (Directory.Exists(questsDir)) {
				var files = Directory.EnumerateFiles(questsDir);
				foreach (var filePath in files) {
					code.ReadFromFile(filePath, CodeBulkType.Quest);
				}
			}
		}

		private void LoadConfigsCode(string gameDirectory)
		{
			var configsDir = Path.Combine(gameDirectory, "Configs");
			if (!Directory.Exists(configsDir)) {
				throw new Exception($"Couldn't find \"{configsDir}\" directory");
			}
			var configFilePath = Path.Combine(configsDir, "QuestsConfig.cs");
			code.ReadFromFile(configFilePath, CodeBulkType.Config);
		}

		private void LoadProjectFiles(string gameDirectory)
		{
			foreach (var file in Directory.EnumerateFiles(gameDirectory, "*.csproj")) {
				code.AddPathToProjectFile(file);
			}
		}
	}
}
