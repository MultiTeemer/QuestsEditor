using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataTransformers;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff
{
	struct Flow
	{
		public readonly Graph Graph;
		public readonly List<Sector> Sectors;

		public Flow(Graph graph, List<Sector> sectors)
		{
			Graph = graph;
			Sectors = sectors;
		}
	}

	class SectorsLoader
	{
		private readonly Code code;

		public SectorsLoader(Code code)
		{
			this.code = code;
		}

		public Flow Load(string folder)
		{
			var gameDir = Directory.EnumerateDirectories(folder).FirstOrDefault(p => p.EndsWith(".Game"));
			if (gameDir == null) {
				throw new Exception("Couldn't find \"*.Game\" directory");
			}

			var sectorsDir = Path.Combine(gameDir, "Sectors");
			if (!Directory.Exists(sectorsDir)) {
				throw new Exception($"Couldn't find \"{sectorsDir}\" directory");
			}

			var graph = new Graph();

			var secsDirs = Directory.EnumerateDirectories(sectorsDir)
				.Where(p => new DirectoryInfo(p).Name.StartsWith("Sector"));

			var sectors = secsDirs.Select(LoadSector).ToList();
			var links = secsDirs.SelectMany(LoadQuestLinks);

			BuildGraph(graph, sectors, links);

			LoadConfig(gameDir);

			return new Flow(graph, sectors);
		}

		public Sector LoadSector(string dirPath)
		{
			var sectorFile = Directory.EnumerateFiles(dirPath)
				.FirstOrDefault(p => new FileInfo(p).Name.StartsWith("Sector"));
			if (sectorFile == null) {
				throw new System.Exception("Cannot find file describing sector");
			}

			var codeBulk = code.ReadFromFile(Path.Combine(dirPath, sectorFile), CodeBulkType.Sector);
			var sector = new Sector {
				Name = FromCodeTransformer.FetchSectorName(codeBulk.Tree),
				Quests = new List<Quest>(),
			};

			var questsDir = Path.Combine(dirPath, "Quests");
			if (Directory.Exists(questsDir)) {
				var files = Directory.EnumerateFiles(questsDir);
				foreach (var filePath in files) {
					sector.Quests.Add(LoadQuest(filePath));
				}
			}

			code.RegisterSectorForCodeBulk(sector, codeBulk);

			return sector;
		}

		public Quest LoadQuest(string path)
		{
			var codeBulk = code.ReadFromFile(path, CodeBulkType.Quest);
			var quest = FromCodeTransformer.ReadQuest(codeBulk.Tree);

			code.RegisterQuestforCodeBulk(quest, codeBulk);

			return quest;
		}

		private List<Tuple<string, string>> LoadQuestLinks(string dirPath)
		{
			var sectorFile = Directory.EnumerateFiles(dirPath)
				.FirstOrDefault(p => new FileInfo(p).Name.StartsWith("Sector"));
			var tree = FileSystem.ReadCodeFromFile(Path.Combine(dirPath, sectorFile));
			return FromCodeTransformer.FetchQuestToQuestLink(tree);
		}

		private void LoadConfig(string gameDirectory)
		{
			var configsDir = Path.Combine(gameDirectory, "Configs");
			if (!Directory.Exists(configsDir)) {
				throw new Exception($"Couldn't find \"{configsDir}\" directory");
			}
			var configFilePath = Path.Combine(configsDir, "QuestsConfig.cs");
			code.ReadFromFile(configFilePath, CodeBulkType.Config);
		}

		private void BuildGraph(Graph graph, List<Sector> sectors, IEnumerable<Tuple<string, string>> links)
		{
			foreach (var sector in sectors) {
				foreach (var quest in sector.Quests) {
					graph.AddNode(quest);
				}
			}

			foreach (var link in links) {
				var n1 = graph.FindNodeForQuest(link.Item1);
				var n2 = graph.FindNodeForQuest(link.Item2);

				if (n1 == null) {
					throw new Exception($"Cannot find node for {link.Item1}");
				}

				if (n2 == null) {
					throw new Exception($"Cannot find node for {link.Item2}");
				}

				graph.AddLink(n1, n2);
			}
		}
	}
}
