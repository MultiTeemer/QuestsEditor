using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.CodeReaders
{
	class ConfigReader : CodeReader
	{
		public override CodeBulkType[] AcceptedTypes => new[] { CodeBulkType.Config };

		public override void Read(CodeBulk codeBulk, Code code, ref Flow flow) {}
	}
}
