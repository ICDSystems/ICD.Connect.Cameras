using System;
using System.Collections.Generic;
using ICD.Common.Properties;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters
{
	public interface IFusionPresenterFactory : IDisposable
	{
		/// <summary>
		/// Lazy loads all of the presenters.
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		IEnumerable<IFusionPresenter> GetPresenters();
	}
}
