using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters
{
	/// <summary>
	/// Generates the given number of views.
	/// </summary>
	/// <typeparam name="TView"></typeparam>
	/// <param name="count"></param>
	/// <returns></returns>
	public delegate IEnumerable<TView> ListItemFactory<TView>(ushort count);

	/// <summary>
	/// Base class for list item factories.
	/// Takes a sequence of model items and generates the views and presenters, using a callback
	/// to bind the MVP triad.
	/// </summary>
	public abstract class AbstractListItemFactory<TModel, TPresenter, TView> : IEnumerable<TPresenter>, IDisposable
		where TPresenter : class, IPresenter
	{
		private readonly Dictionary<Type, List<TPresenter>> m_PresenterCache;
		private readonly SafeCriticalSection m_CacheSection;
		private readonly SafeCriticalSection m_BuildViewsSection;

		private readonly INavigationController m_NavigationController;
		private readonly ListItemFactory<TView> m_ViewFactory;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		protected AbstractListItemFactory(INavigationController navigationController, ListItemFactory<TView> viewFactory)
		{
			m_PresenterCache = new Dictionary<Type, List<TPresenter>>();
			m_CacheSection = new SafeCriticalSection();
			m_BuildViewsSection = new SafeCriticalSection();

			m_NavigationController = navigationController;
			m_ViewFactory = viewFactory;
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			foreach (TPresenter presenter in this)
				presenter.Dispose();
			m_PresenterCache.Clear();
		}

		/// <summary>
		/// Generates the presenters and views for the given sequence of models.
		/// </summary>
		/// <param name="models"></param>
		[PublicAPI]
		public TPresenter[] BuildChildren(IEnumerable<TModel> models)
		{
			List<TPresenter> output = new List<TPresenter>();
			Dictionary<Type, int> cacheIndices = new Dictionary<Type, int>();

			m_BuildViewsSection.Enter();

			try
			{
				// Clear the existing views
				ClearChildViews();

				// Gather all of the models
				TModel[] modelsArray = models as TModel[] ?? models.ToArray();

				// Build the views (may be fewer than models due to list max size)
				TView[] views = m_ViewFactory((ushort)modelsArray.Length).ToArray();

				// Build the presenters
				for (int index = 0; index < views.Length; index++)
				{
					// Get the view
					TView view = views[index];

					// Get the model
					TModel model = modelsArray[index];
					Type key = GetPresenterTypeForModel(model);

					// Get the presenter
					int cacheIndex = cacheIndices.GetDefault(key, 0);
					cacheIndices[key] = cacheIndex + 1;
					TPresenter presenter = LazyLoadPresenterFromCache(key, cacheIndex);

					// Bind
					BindMvpTriad(model, presenter, view);

					output.Add(presenter);
				}
			}
			finally
			{
				m_BuildViewsSection.Leave();
			}

			return output.ToArray();
		}

		/// <summary>
		/// Loops over the child presenters and sets the views to null.
		/// </summary>
		[PublicAPI]
		public void ClearChildViews()
		{
			foreach (TPresenter presenter in this)
				presenter.ClearView();
		}

		#endregion

		#region Virtual Methods

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected abstract void BindMvpTriad(TModel model, TPresenter presenter, TView view);

		/// <summary>
		/// Gets the presenter type for the given model instance.
		/// Override to fill lists with different presenters.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[PublicAPI]
		protected virtual Type GetPresenterTypeForModel(TModel model)
		{
			return typeof(TPresenter);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Retrieves or generates a presenter from the cache.
		/// This is the object pooling mechanism for the list.
		/// </summary>
		/// <param name="presenterType"></param>
		/// <param name="cacheIndex"></param>
		/// <returns></returns>
		private TPresenter LazyLoadPresenterFromCache(Type presenterType, int cacheIndex)
		{
			List<TPresenter> keyPresenterCache;

			m_CacheSection.Enter();

			try
			{
				if (!m_PresenterCache.ContainsKey(presenterType))
					m_PresenterCache[presenterType] = new List<TPresenter>();
				keyPresenterCache = m_PresenterCache[presenterType];
			}
			finally
			{
				m_CacheSection.Leave();
			}

			return GetNewPresenter(keyPresenterCache, presenterType, cacheIndex);
		}

		/// <summary>
		/// Gets the presenter at the given index in the cache. Generates presenters up to the index if needed.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cache"></param>
		/// <param name="type"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Type does not fit generic</exception>
		private T GetNewPresenter<T>(IList<T> cache, Type type, int index)
			where T : class, IPresenter
		{
			if (!typeof(T).IsAssignableFrom(type))
				throw new InvalidOperationException(typeof(T).Name + " not assignable from " + type.Name);

			for (int cacheIndex = cache.Count; cacheIndex <= index; cacheIndex++)
				cache.Add(m_NavigationController.GetNewPresenter(type) as T);

			return cache[index];
		}

		#endregion

		public IEnumerator<TPresenter> GetEnumerator()
		{
			return m_CacheSection.Execute(() => m_PresenterCache.SelectMany(kvp => kvp.Value).ToList()).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
