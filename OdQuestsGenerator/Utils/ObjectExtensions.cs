namespace OdQuestsGenerator.Utils
{
	public static class ObjectExtensions
	{
		public static TType As<TType>(this object obj)
			where TType : class
		{
			return obj as TType;
		}
	}
}
