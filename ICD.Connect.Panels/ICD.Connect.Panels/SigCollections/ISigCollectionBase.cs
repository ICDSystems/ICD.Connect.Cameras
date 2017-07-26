using System.Collections.Generic;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Panels.SigCollections
{
	public interface ISigCollectionBase<T> : IEnumerable<T>
		where T : ISig
	{
		/// <summary>
		/// Get the sig with the specified number.
		/// 
		/// </summary>
		/// <param name="sigNumber">Number of the sig to return.</param>
		/// <returns/>
		/// <exception cref="T:System.IndexOutOfRangeException">Invalid Sig Number specified.</exception>
		T this[uint sigNumber] { get; }
	}
}
