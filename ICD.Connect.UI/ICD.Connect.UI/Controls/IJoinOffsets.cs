namespace ICD.Connect.UI.Controls
{
	/// <summary>
	/// IJoinOffsets provides methods for getting the resulting join for a child
	/// control after offsets are applied.
	/// </summary>
	public interface IJoinOffsets
	{
		/// <summary>
		/// Gets the digital join offset for the given control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		ushort GetDigitalJoinOffset(IIndexed control);

		/// <summary>
		/// Gets the analog join offset for the given control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		ushort GetAnalogJoinOffset(IIndexed control);

		/// <summary>
		/// Gets the serial join offset for the given control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		ushort GetSerialJoinOffset(IIndexed control);
	}

	/// <summary>
	/// Extension methods for IJoinOffsets.
	/// </summary>
	public static class JoinOffsetsExtensions
	{
		/// <summary>
		/// Gets the resulting digital join for the given control.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="join"></param>
		/// <param name="control"></param>
		/// <returns></returns>
		public static ushort GetDigitalJoin(this IJoinOffsets extends, ushort join, IIndexed control)
		{
			return (ushort)(join + extends.GetDigitalJoinOffset(control));
		}

		/// <summary>
		/// Gets the resulting serial join for the given control.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="join"></param>
		/// <param name="control"></param>
		/// <returns></returns>
		public static ushort GetSerialJoin(this IJoinOffsets extends, ushort join, IIndexed control)
		{
			return (ushort)(join + extends.GetSerialJoinOffset(control));
		}

		/// <summary>
		/// Gets the resulting analog join for the given control.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="join"></param>
		/// <param name="control"></param>
		/// <returns></returns>
		public static ushort GetAnalogJoin(this IJoinOffsets extends, ushort join, IIndexed control)
		{
			return (ushort)(join + extends.GetAnalogJoinOffset(control));
		}
	}
}
