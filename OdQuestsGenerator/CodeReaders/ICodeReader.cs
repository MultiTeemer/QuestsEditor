using System;
using System.Collections.Generic;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff;

namespace OdQuestsGenerator.CodeReaders
{
	interface ICodeReader
	{
		CodeBulkType[] AcceptedTypes { get; }

		void Read(CodeBulk codeBulk, Code code, ref Flow flow);
	}

	abstract class CodeReader : ICodeReader
	{
		public abstract CodeBulkType[] AcceptedTypes { get; }

		public abstract void Read(CodeBulk codeBulk, Code code, ref Flow flow);
	}

	static class CodeReadersRepo
	{
		private static Dictionary<Type, ICodeReader> readers = new Dictionary<Type, ICodeReader>();

		static CodeReadersRepo()
		{
			RegisterReaderType<SectorReader>();
			RegisterReaderType<QuestReader>();
			RegisterReaderType<ConfigReader>();
			RegisterReaderType<ReachedConditionReader>();
			RegisterReaderType<QuestInitializationReader>();
		}

		public static void RegisterReaderType(Type type)
		{
			readers.Add(type, (ICodeReader)type.GetConstructor(new Type[0]).Invoke(new object[0]));
		}

		public static void RegisterReaderType<TReaderType>()
			where TReaderType : ICodeReader, new()
		{
			readers.Add(typeof(TReaderType), new TReaderType());
		}

		public static TReaderType GetReader<TReaderType>()
			where TReaderType : ICodeReader
		{
			return (TReaderType)readers[typeof(TReaderType)];
		}

		public static ICodeReader GetReader(Type type)
		{
			return readers[type];
		}
	}
}
