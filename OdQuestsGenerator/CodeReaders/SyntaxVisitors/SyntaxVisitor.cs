using Microsoft.CodeAnalysis.CSharp;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.CodeReaders.SyntaxVisitors
{
	abstract class SyntaxWalker : CSharpSyntaxWalker
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

		protected SyntaxWalker(Code code)
		{
			Code = code;
		}
	}
}
