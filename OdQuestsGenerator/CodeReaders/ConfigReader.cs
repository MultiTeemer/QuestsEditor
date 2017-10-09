using OdQuestsGenerator.CodeEditing;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff;

namespace OdQuestsGenerator.CodeReaders
{
	class ConfigReader : CodeReader
	{
		public override CodeBulkType[] AcceptedTypes => new[] { CodeBulkType.Config };

		public override void Read(CodeBulk codeBulk, Code code, ref Flow flow) {}
	}
}
