using System;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataTransformers;
using OdQuestsGenerator.Forms.QuestsViewerStuff;

namespace OdQuestsGenerator.CodeReaders
{
	class ReachedConditionReader : CodeReader
	{
		public override void Read(CodeBulk codeBulk, Code code, ref Flow flow)
		{
			var links = FromCodeTransformer.FetchQuestToQuestLink(codeBulk.Tree);
			foreach (var link in links) {
				var n1 = flow.Graph.FindNodeForQuest(link.Item1);
				var n2 = flow.Graph.FindNodeForQuest(link.Item2);

				if (n1 == null) {
					throw new Exception($"Cannot find node for {link.Item1}");
				}

				if (n2 == null) {
					throw new Exception($"Cannot find node for {link.Item2}");
				}

				flow.Graph.AddLink(n1, n2);
			}
		}
	}
}
