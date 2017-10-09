using OdQuestsGenerator.CodeEditing;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Commands
{
	class EditingContext
	{
		public readonly CommandsHistory History;
		public readonly Code Code;
		public readonly CodeEditor CodeEditor;

		public Flow Flow;

		public EditingContext(
			Flow flow,
			CommandsHistory history,
			Code code,
			CodeEditor codeEditor
		)
		{
			Flow = flow;
			History = history;
			Code = code;
			CodeEditor = codeEditor;
		}
	}
}
