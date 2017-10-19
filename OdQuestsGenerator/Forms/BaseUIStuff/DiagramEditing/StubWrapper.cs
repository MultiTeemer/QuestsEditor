using Dataweb.NShape;
using OdQuestsGenerator.Commands;

namespace OdQuestsGenerator.Forms.BaseUIStuff.DiagramEditing
{
	class StubWrapper : ToolWrapper<Tool, DiagramWrapper>
	{
		public StubWrapper(EditingContext context, Tool tool, DiagramWrapper diagramWrapper)
			: base(context, tool, diagramWrapper)
		{}
	}
}
