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
		public Flow Load(string folder)
		{
			var gameDir = Directory.EnumerateDirectories(folder).FirstOrDefault(p => p.EndsWith(".Game"));
			if (gameDir == null) {
				throw new System.Exception("Couldn't find \"*.Game\" directory");
			}

			var sectorsDir = Path.Combine(gameDir, "Sectors");
			if (!Directory.Exists(sectorsDir)) {
				throw new System.Exception($"Couldn't find \"{sectorsDir}\" directory");
			}

			var graph = new Graph();

			var secsDirs = Directory.EnumerateDirectories(sectorsDir)
				.Where(p => new DirectoryInfo(p).Name.StartsWith("Sector"));

			var sectors = secsDirs.Select(LoadSector).ToList();
			var links = secsDirs.SelectMany(LoadQuestLinks);

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

			return new Flow(graph, sectors);
		}

		public Sector LoadSector(string dirPath)
		{
			var sectorFile = Directory.EnumerateFiles(dirPath)
				.FirstOrDefault(p => new FileInfo(p).Name.StartsWith("Sector"));
			if (sectorFile == null) {
				throw new System.Exception("Cannot find file describing sector");
			}

			var tree = FileSystem.ReadCodeFromFile(Path.Combine(dirPath, sectorFile));
			var sector = new Sector {
				Name = FromCodeTransformer.FetchSectorName(tree),
				Quests = new List<Quest>(),
			};

			var questsDir = Path.Combine(dirPath, "Quests");
			if (Directory.Exists(questsDir)) {
				var files = Directory.EnumerateFiles(questsDir);
				foreach (var filePath in files) {
					sector.Quests.Add(LoadQuest(filePath));
				}
			}

			return sector;
		}

		public Quest LoadQuest(string path) => FromCodeTransformer.ReadQuest(FileSystem.ReadCodeFromFile(path));

		private List<Tuple<string, string>> LoadQuestLinks(string dirPath)
		{
			var sectorFile = Directory.EnumerateFiles(dirPath)
				.FirstOrDefault(p => new FileInfo(p).Name.StartsWith("Sector"));
			var tree = FileSystem.ReadCodeFromFile(Path.Combine(dirPath, sectorFile));
			return FromCodeTransformer.FetchQuestToQuestLink(tree);
		}
	}
}
