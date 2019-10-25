using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Eff.Core;

namespace Eff.Effects
{
	public interface IEffCtx
	{
		T Resolve<T>();
	}
	
	public class DoEffect<T> : Effect<T>
	{

		public Func<IEffCtx, Task<T>> Func { get; }
		public DoEffect(Func<IEffCtx, Task<T>> func, string memberName, string sourceFilePath, int sourceLineNumber) : base(memberName, sourceFilePath, sourceLineNumber)
		{
			this.Func = func;
		}
	}


	public static class IO
	{

		public static DoEffect<T> Do<T>(Func<IEffCtx, Task<T>> func,
			[CallerMemberName] string memberName = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0)
		{
			return new DoEffect<T>(ctx => func(ctx), memberName, sourceFilePath, sourceLineNumber);
		}

	}
}
