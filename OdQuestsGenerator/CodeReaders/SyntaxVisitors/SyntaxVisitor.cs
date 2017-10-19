using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.CodeReaders.SyntaxVisitors
{
	abstract class SyntaxVisitor : CSharpSyntaxVisitor
	{
		protected readonly Code Code;

		protected CodeBulk CurrentCodeBulk;

		public void Visit(CodeBulk codeBulk)
		{
			CurrentCodeBulk = codeBulk;
			var doc = Code.GetMappedCode(codeBulk);
			Visit(doc.GetSyntaxRootAsync().Result);
			CurrentCodeBulk = null;
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
