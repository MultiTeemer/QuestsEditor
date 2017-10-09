using Dataweb.NShape;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff.Commands;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	class EditingContext
	{
		public readonly Project Project;
		public readonly CommandsHistory History;
		public readonly FlowView FlowView;
		public readonly Code Code;
		public readonly CodeEditor CodeEditor;
		public readonly ToolsManager ToolsManager;

		public Flow Flow;

		public EditingContext(
			Flow flow,
			Project project,
			CommandsHistory history,
			FlowView view,
			Code code,
			CodeEditor codeEditor,
			ToolsManager toolsManager
		)
		{
			Flow = flow;
			Project = project;
			History = history;
			FlowView = view;
			Code = code;
			CodeEditor = codeEditor;
			ToolsManager = toolsManager;
		}
	}
}
