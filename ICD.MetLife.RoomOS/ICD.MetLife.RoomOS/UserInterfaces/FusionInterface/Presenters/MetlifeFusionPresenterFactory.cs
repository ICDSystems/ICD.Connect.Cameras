using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Presenters
{
	public sealed class MetlifeFusionPresenterFactory : IFusionPresenterFactory
	{
		private delegate IFusionPresenter PresenterFactory(
			int roomId, IFusionPresenterFactory presenters, IFusionViewFactory views, ICore core);

		private readonly Dictionary<Type, PresenterFactory> m_PresenterFactories = new Dictionary<Type, PresenterFactory>
		{
			{
				typeof(ICodecFusionPresenter),
				(room, presenters, views, core) => new CodecFusionPresenter(room, presenters, views, core)
			},
			{
				typeof(IDisplaysFusionPresenter),
				(room, presenters, views, core) => new DisplaysFusionPresenter(room, presenters, views, core)
			},
			{
				typeof(IDspFusionPresenter),
				(room, presenters, views, core) => new DspFusionPresenter(room, presenters, views, core)
			},
			{
				typeof(ILightingFusionPresenter),
				(room, presenters, views, core) => new LightingFusionPresenter(room, presenters, views, core)
			},
			{
				typeof(ILoggingFusionPresenter),
				(room, presenters, views, core) => new LoggingFusionPresenter(room, presenters, views, core)
			},
			{
				typeof(IPanelFusionPresenter),
				(room, presenters, views, core) => new PanelFusionPresenter(room, presenters, views, core)
			},
			{
				typeof(IProgramFusionPresenter),
				(room, presenters, views, core) => new ProgramFusionPresenter(room, presenters, views, core)
			},
			{
				typeof(IRackFusionPresenter),
				(room, presenters, views, core) => new RackFusionPresenter(room, presenters, views, core)
			},
			{
				typeof(IRoomFusionPresenter),
				(room, presenters, views, core) => new RoomFusionPresenter(room, presenters, views, core)
			},
			{
				typeof(IRoutingFusionPresenter),
				(room, presenters, views, core) => new RoutingFusionPresenter(room, presenters, views, core)
			},
			{
				typeof(ISettingsFusionPresenter),
				(room, presenters, views, core) => new SettingsFusionPresenter(room, presenters, views, core)
			}
		};

		private readonly Dictionary<Type, IFusionPresenter> m_Cache;
		private readonly SafeCriticalSection m_CacheSection;
		private readonly MetlifeRoom m_Room;
		private readonly IFusionViewFactory m_ViewFactory;
		private readonly ICore m_Core;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="viewFactory"></param>
		/// <param name="core"></param>
		public MetlifeFusionPresenterFactory(MetlifeRoom room, IFusionViewFactory viewFactory, ICore core)
		{
			m_Cache = new Dictionary<Type, IFusionPresenter>();
			m_CacheSection = new SafeCriticalSection();

			m_Room = room;
			m_ViewFactory = viewFactory;
			m_Core = core;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			m_Cache.Values.ForEach(p => p.Dispose());
			m_Cache.Clear();
		}

		/// <summary>
		/// Instantiates or returns an existing presenter of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private IFusionPresenter LazyLoadPresenter(Type type)
		{
			IFusionPresenter output;

			m_CacheSection.Enter();

			try
			{
				if (!m_Cache.ContainsKey(type))
					m_Cache[type] = GetNewPresenter(type);
				output = m_Cache[type];
			}
			finally
			{
				m_CacheSection.Leave();
			}

			return output;
		}

		/// <summary>
		/// Instantiates a new presenter of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private IFusionPresenter GetNewPresenter(Type type)
		{
			if (!m_PresenterFactories.ContainsKey(type))
			{
				string message = string.Format("{0} does not contain factory for {1}", GetType().Name, type.Name);
				throw new KeyNotFoundException(message);
			}

			PresenterFactory factory = m_PresenterFactories[type];
			IFusionPresenter output = factory(m_Room.Id, this, m_ViewFactory, m_Core);

			if (!type.IsInstanceOfType(output))
				throw new Exception(string.Format("Presenter {0} is not of type {1}", output, type.Name));

			return output;
		}

		/// <summary>
		/// Lazy loads all of the presenters.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IFusionPresenter> GetPresenters()
		{
			return m_PresenterFactories.Keys.Select(type => LazyLoadPresenter(type));
		}
	}
}
