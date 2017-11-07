using System.Drawing;
using Dataweb.NShape;
using Dataweb.NShape.Controllers;
using Dataweb.NShape.WinFormsUI;

namespace OdQuestsGenerator.Forms.BaseUIStuff.DiagramEditing
{
	abstract class DiagramWrapper
	{
		public readonly Display Display;

		protected readonly DiagramSetController Controller;
		protected readonly Project Project;

		protected Diagram Diagram;

		public bool DiagramEdited { get; protected set; }

		public DiagramWrapper(
			DiagramSetController controller,
			Project project,
			Display display
		)
		{
			Controller = controller;
			Project = project;
			Display = display;
		}

		public abstract void Clear();

		protected void ClearDiagram()
		{
			DiagramEdited = true;
			Project.Repository.DeleteAll(Diagram);
			Diagram.Shapes.Clear();
			DiagramEdited = false;
		}

		protected void InitDiagram(Size size)
		{
			Diagram = new Diagram("Foo") {
				Size = size,
			};
			Display.Diagram = Diagram;

			Project.Repository.InsertAll(Diagram);
		}

		protected void AddShape(Shape shape)
		{
			Diagram.Shapes.Add(shape);
			DiagramEdited = true;
			Project.Repository.Insert(shape, Diagram);
			DiagramEdited = false;
		}

		protected void RemoveShape(Shape shape)
		{
			if (Diagram.Shapes.Contains(shape)) {
				Diagram.Shapes.Remove(shape);
				DiagramEdited = true;
				Project.Repository.Delete(shape);
				DiagramEdited = false;
			}
		}
	}
}
