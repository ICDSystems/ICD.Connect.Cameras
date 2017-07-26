using System;
using System.Collections.Generic;
using ICD.Connect.Analytics.FusionPro;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed class MetlifeFusionViewFactory : IFusionViewFactory
	{
		private delegate IFusionView FactoryMethod(IFusionRoom fusionRoom);

		private readonly IFusionRoom m_FusionRoom;

		private readonly Dictionary<Type, FactoryMethod> m_ViewFactories = new Dictionary<Type, FactoryMethod>
		{
			{typeof(ICodecFusionView), room => new CodecFusionView(room)},
			{typeof(IDisplaysFusionView), room => new DisplaysFusionView(room)},
			{typeof(IDspFusionView), room => new DspFusionView(room)},
			{typeof(ILightingFusionView), room => new LightingFusionView(room)},
			{typeof(ILoggingFusionView), room => new LoggingFusionView(room)},
			{typeof(IPanelFusionView), room => new PanelFusionView(room)},
			{typeof(IProgramFusionView), room => new ProgramFusionView(room)},
			{typeof(IRackFusionView), room => new RackFusionView(room)},
			{typeof(IRoomFusionView), room => new RoomFusionView(room)},
			{typeof(IRoutingFusionView), room => new RoutingFusionView(room)},
			{typeof(ISettingsFusionView), room => new SettingsFusionView(room)}
		};

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fusionRoom"></param>
		public MetlifeFusionViewFactory(IFusionRoom fusionRoom)
		{
			m_FusionRoom = fusionRoom;
		}

		/// <summary>
		/// Instantiates a new view of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetNewView<T>()
			where T : class, IFusionView
		{
			if (!m_ViewFactories.ContainsKey(typeof(T)))
			{
				string message = string.Format("{0} does not contain factory for {1}", GetType().Name, typeof(T).Name);
				throw new KeyNotFoundException(message);
			}

			FactoryMethod factory = m_ViewFactories[typeof(T)];
			IFusionView output = factory(m_FusionRoom);

			if (output as T == null)
				throw new Exception(string.Format("FusionView {0} is not of type {1}", output, typeof(T).Name));

			return output as T;
		}
	}
}
