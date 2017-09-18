using System;
using System.Windows.Forms;
using OdQuestsGenerator.ApplicationData;
using OdQuestsGenerator.Forms;

namespace OdQuestsGenerator
{
	static class Program
	{
		public static Preferences Preferences = new Preferences();

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new QuestsViewer());
		}
	}
}
