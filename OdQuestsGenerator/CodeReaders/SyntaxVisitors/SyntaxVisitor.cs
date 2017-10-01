using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OdQuestsGenerator.Forms.QuestsViewerStuff;

namespace OdQuestsGenerator.CodeReaders.SyntaxVisitors
{
	abstract class SyntaxVisitor : CSharpSyntaxVisitor
	{
		protected readonly Code Code;

		protected CodeBulk currentCodeBulk;

		public void Visit(CodeBulk codeBulk)
		{
			currentCodeBulk = codeBulk;
			var doc = Code.GetMappedCode(codeBulk);
			Visit(doc.GetSyntaxRootAsync().Result);
			currentCodeBulk = null;
		}

		public override void Visit(SyntaxNode node)
		{
			foreach (var n in node.DescendantNodesAndSelf()) {
				base.Visit(n);
			}
		}

		protected SyntaxVisitor(Code code)
		{
			this.Code = code;
		}
	}
}
