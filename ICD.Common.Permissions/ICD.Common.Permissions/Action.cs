namespace ICD.Common.Permissions
{
	public class Action : IAction
	{
		/// <summary>
		/// The name of the action
		/// </summary>
		public string Value { get; set; }

		protected Action(string value)
		{
			Value = value;
		}

		/// <summary>
		/// Returns the string representation of the object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Value;
		}

		/// <summary>
		/// Parses an xml &lt;Action&gt; element into an Action.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		public static Action FromString(string action)
		{
			return new Action(action);
		}
	}
}