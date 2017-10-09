using Dataweb.NShape;
using Dataweb.NShape.Controllers;
using Dataweb.NShape.WinFormsUI;

namespace OdQuestsGenerator.Forms.BaseUIStuff.DiagramEditing
{
	class DiagramWrapper
	{
		public readonly Display Display;

		protected readonly DiagramSetController controller;
		protected readonly Project project;

		protected Diagram diagram;

		public bool DiagramEdited { get; protected set; }

		public DiagramWrapper(
			DiagramSetController controller,
			Project project,
			Display display
		)
		{
			this.controller = controller;
			this.project = project;

			Display = display;
		}
	}
}
