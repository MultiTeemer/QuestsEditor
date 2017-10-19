using System.Windows.Forms;

namespace OdQuestsGenerator.Utils
{
	public static class ListBoxExtensions
	{
		public static void Fill(this ListBox listBox, string[] items)
		{
			listBox.Items.Clear();
			listBox.Items.AddRange(items);
		}
	}
}
