namespace ICD.MetLife.RoomOS.Rooms
{
	public sealed class MetlifeRoomOwner
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }

		public override string ToString()
		{
			return string.Format("{0}(Name={1}, Email={2}, Phone={3})", GetType().Name, Name, Email, Phone);
		}
	}
}
