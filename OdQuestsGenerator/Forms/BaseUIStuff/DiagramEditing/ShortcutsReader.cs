using System.Windows.Forms;
using OdQuestsGenerator.Commands;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Forms.BaseUIStuff.DiagramEditing
{
	class ShortcutsReader
	{
		private readonly Code code;
		private readonly CommandsHistory history;

		public ShortcutsReader(Code code, CommandsHistory history)
		{
			this.code = code;
			this.history = history;
		}

		public bool? ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData.HasFlag(Keys.Control)) {
				if (keyData.HasFlag(Keys.S)) {
					code.Save();

					return true;
				}

				if (keyData.HasFlag(Keys.Z)) {
					history.Undo();

					return true;
				}

				if (keyData.HasFlag(Keys.Y)) {
					history.Do();

					return true;
				}
			}

			return null;
		}
	}
}
