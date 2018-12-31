using System;

namespace FeatureSelect
{
	public enum MaybeType { Just, Nothing }

	public abstract class Maybe<T>
	{
		public MaybeType Tag { get; }

		protected Maybe(MaybeType tag)
		{
			Tag = tag;
		}

		public TResult Match<TResult>(Func<T, TResult> just, Func<TResult> nothing)
		{
			return Tag == MaybeType.Just
				? just(((Just<T>) this).Value)
				: nothing();
		}
	}

	public sealed class Nothing<T> : Maybe<T>
	{
		public Nothing() : base(MaybeType.Nothing) { }
	}

	public sealed class Just<T> : Maybe<T>
	{
		public T Value { get; }

		public Just(T value) : base(MaybeType.Just) => Value = value;
	}

	public static class Maybe
	{
		public static Maybe<T> Nothing<T>() => new Nothing<T>();

		public static Just<T> Just<T>(T value) => new Just<T>(value);
	}
}