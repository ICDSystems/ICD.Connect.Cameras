using System.Collections.Generic;
using System.Linq;
using ICD.Common.Services;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Analytics.FusionPro;
using ICD.Connect.Audio.ClockAudio;
using ICD.Connect.Audio.Shure;
using ICD.Connect.Panels;
using ICD.Connect.Rooms;
using ICD.Connect.Settings.Core;
using ICD.Connect.UI;
using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface;
using ICD.MetLife.RoomOS.UserInterfaces.MicrophoneInterfaces;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface;

namespace ICD.MetLife.RoomOS
{
	public sealed class MetlifeUserInterfaceFactory : AbstractUserInterfaceFactory<MetlifeUserInterfaceFactorySettings>
	{
		private readonly IcdHashSet<MetlifeUserInterface> m_UserInterfaces;
		private readonly IcdHashSet<MetlifeFusionInterface> m_FusionInterfaces;
		private readonly IcdHashSet<MetlifeClockAudioInterface> m_ClockAudioInterfaces;
		private readonly IcdHashSet<MetlifeShureInterface> m_ShureInterfaces; 

		private readonly SafeCriticalSection m_UiSection;

		private static ICore Core { get { return ServiceProvider.GetService<ICore>(); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		public MetlifeUserInterfaceFactory()
		{
			m_UserInterfaces = new IcdHashSet<MetlifeUserInterface>();
			m_FusionInterfaces = new IcdHashSet<MetlifeFusionInterface>();
			m_ClockAudioInterfaces = new IcdHashSet<MetlifeClockAudioInterface>();
			m_ShureInterfaces = new IcdHashSet<MetlifeShureInterface>();

			m_UiSection = new SafeCriticalSection();
		}

		#region Pulbic Methods

		/// <summary>
		/// Clears the instantiated user interfaces.
		/// </summary>
		public override void ClearUserInterfaces()
		{
			m_UiSection.Enter();

			try
			{
				m_UserInterfaces.ForEach(u => u.Dispose());
				m_UserInterfaces.Clear();

				m_FusionInterfaces.ForEach(f => f.Dispose());
				m_FusionInterfaces.Clear();

				m_ClockAudioInterfaces.ForEach(c => c.Dispose());
				m_ClockAudioInterfaces.Clear();

				m_ShureInterfaces.ForEach(s => s.Dispose());
				m_ShureInterfaces.Clear();
			}
			finally
			{
				m_UiSection.Leave();
			}
		}

		/// <summary>
		/// Clears and rebuilds the user interfaces.
		/// </summary>
		public override void BuildUserInterfaces()
		{
			m_UiSection.Enter();

			try
			{
				ClearUserInterfaces();

				IEnumerable<MetlifeUserInterface> uis = CreateUserInterfaces();
				IEnumerable<MetlifeFusionInterface> fis = CreateFusionInterfaces();
				IEnumerable<MetlifeClockAudioInterface> cis = CreateClockAudioInterfaces();
				IEnumerable<MetlifeShureInterface> sis = CreateShureInterfaces();

				m_UserInterfaces.AddRange(uis);
				m_FusionInterfaces.AddRange(fis);
				m_ClockAudioInterfaces.AddRange(cis);
				m_ShureInterfaces.AddRange(sis);
			}
			finally
			{
				m_UiSection.Leave();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the MetlifeRooms from the core.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<MetlifeRoom> GetMetlifeRooms()
		{
			return Core.Originators.OfType<MetlifeRoom>();
		}

		#region UIs

		/// <summary>
		/// Instantiates the user interfaces for the given core.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<MetlifeUserInterface> CreateUserInterfaces()
		{
			return GetMetlifeRooms().SelectMany(r => CreateUserInterfaces(r));
		}

		/// <summary>
		/// Instantiates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		private static IEnumerable<MetlifeUserInterface> CreateUserInterfaces(MetlifeRoom room)
		{
			return room.Panels.Select(panel => CreateUserInterface(room, panel));
		}

		/// <summary>
		/// Instantiates the UI for the given room and panel via reflection.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="panel"></param>
		/// <returns></returns>
		private static MetlifeUserInterface CreateUserInterface(MetlifeRoom room, IPanelDevice panel)
		{
			return new MetlifeUserInterface(room, panel, Core);
		}

		#endregion

		#region FIs

		/// <summary>
		/// Instantiates the user interfaces for the given core.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<MetlifeFusionInterface> CreateFusionInterfaces()
		{
			return GetMetlifeRooms().SelectMany(r => CreateFusionInterfaces(r));
		}

		/// <summary>
		/// Instantiates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		private static IEnumerable<MetlifeFusionInterface> CreateFusionInterfaces(MetlifeRoom room)
		{
			return room.GetDevices<IFusionRoom>()
					   .Select(fusionRoom => CreateFusionInterface(room, fusionRoom));
		}

		/// <summary>
		/// Instantiates the Fusion Interface for the given room and panel via reflection.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="fusionRoom"></param>
		/// <returns></returns>
		private static MetlifeFusionInterface CreateFusionInterface(MetlifeRoom room, IFusionRoom fusionRoom)
		{
			return new MetlifeFusionInterface(room, fusionRoom, Core);
		}

		#endregion

		#region ClockAudio Interfaces

		private IEnumerable<MetlifeClockAudioInterface> CreateClockAudioInterfaces()
		{
			return GetMetlifeRooms().SelectMany(r => CreateClockAudioInterfaces(r));
		}

		private IEnumerable<MetlifeClockAudioInterface> CreateClockAudioInterfaces(MetlifeRoom room)
		{
			return room.GetDevices<ClockAudioTs001Device>()
					   .Select(microphone => CreateClockAudioInterface(room, microphone));
		}

		private MetlifeClockAudioInterface CreateClockAudioInterface(MetlifeRoom room, ClockAudioTs001Device microphone)
		{
			return new MetlifeClockAudioInterface(room, microphone);
		}

		#endregion

		#region Shure Interfaces

		private IEnumerable<MetlifeShureInterface> CreateShureInterfaces()
		{
			return GetMetlifeRooms().SelectMany(r => CreateShureInterfaces(r));
		}

		private IEnumerable<MetlifeShureInterface> CreateShureInterfaces(MetlifeRoom room)
		{
			return room.GetDevices<IShureMxaDevice>()
					   .Select(microphone => CreateShureInterface(room, microphone));
		}

		private MetlifeShureInterface CreateShureInterface(MetlifeRoom room, IShureMxaDevice microphone)
		{
			return new MetlifeShureInterface(room, microphone);
		}

		#endregion

		#endregion
	}
}
