using System;
using System.Collections.Generic;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Panels
{
	/// <summary>
	/// Simple class for registering/unregistering callbacks for sig changes.
	/// </summary>
	public sealed class SigCallbackManager
	{
		/// <summary>
		/// Raised when any sig callback is called.
		/// Useful for determining activity when a user interacts with a touchpanel.
		/// </summary>
		public event EventHandler OnAnyCallback;

		/// <summary>
		/// Maps sigs to registered callbacks.
		/// </summary>
		private readonly Dictionary<uint, Dictionary<eSigType, IcdHashSet<Action<SigCallbackManager, SigAdapterEventArgs>>>> m_SigToCallback;

		private readonly SafeCriticalSection m_RegistrationSection;

		private DateTime? m_LastOutput;

		#region Properties

		/// <summary>
		/// Gets the time that a callback was called.
		/// </summary>
		public DateTime? LastOutput { get { return m_LastOutput; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public SigCallbackManager()
		{
			m_SigToCallback = new Dictionary<uint, Dictionary<eSigType, IcdHashSet<Action<SigCallbackManager, SigAdapterEventArgs>>>>();
			m_RegistrationSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Raises the callbacks registered with the signature.
		/// </summary>
		/// <param name="sig"></param>
		public void RaiseSigChangeCallback(ISig sig)
		{
			m_LastOutput = IcdEnvironment.GetLocalTime();
			OnAnyCallback.Raise(this);

			IcdHashSet<Action<SigCallbackManager, SigAdapterEventArgs>> callbacks;

			m_RegistrationSection.Enter();

			try
			{
				if (!m_SigToCallback.ContainsKey(sig.Number))
					return;

				if (!m_SigToCallback[sig.Number].ContainsKey(sig.Type))
					return;

				callbacks = m_SigToCallback[sig.Number][sig.Type];
			}
			finally
			{
				m_RegistrationSection.Leave();
			}

			foreach (Action<SigCallbackManager, SigAdapterEventArgs> callback in callbacks)
			{
				try
				{
					callback(this, new SigAdapterEventArgs(sig));
				}
				catch (Exception e)
				{
					ServiceProvider.TryGetService<ILoggerService>()
					               .AddEntry(eSeverity.Error, e, "Exception in callback - {0}", e.Message);
				}
			}
		}

		/// <summary>
		/// Registers the callback for sig change events.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		public void RegisterSigChangeCallback(uint number, eSigType type, Action<SigCallbackManager, SigAdapterEventArgs> callback)
		{
			m_RegistrationSection.Execute(() => RegisterCallback(m_SigToCallback, number, type, callback));
		}

		/// <summary>
		/// Unregisters the callback for sig change events.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		public void UnregisterSigChangeCallback(uint number, eSigType type, Action<SigCallbackManager, SigAdapterEventArgs> callback)
		{
			m_RegistrationSection.Execute(() => UnregisterCallback(m_SigToCallback, number, type, callback));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Adds the callback to the dictionary.
		/// </summary>
		/// <param name="callbacks"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		private static void RegisterCallback(IDictionary<eSigType, IcdHashSet<Action<SigCallbackManager, SigAdapterEventArgs>>> callbacks, eSigType type,
											 Action<SigCallbackManager, SigAdapterEventArgs> callback)
		{
			if (!callbacks.ContainsKey(type))
				callbacks[type] = new IcdHashSet<Action<SigCallbackManager, SigAdapterEventArgs>>();
			callbacks[type].Add(callback);
		}

		/// <summary>
		/// Adds the callback to the dictionary.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="callbacks"></param>
		/// <param name="key"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		private static void RegisterCallback<T>(IDictionary<T, Dictionary<eSigType, IcdHashSet<Action<SigCallbackManager, SigAdapterEventArgs>>>> callbacks,
												T key, eSigType type, Action<SigCallbackManager, SigAdapterEventArgs> callback)
		{
			if (!callbacks.ContainsKey(key))
				callbacks[key] = new Dictionary<eSigType, IcdHashSet<Action<SigCallbackManager, SigAdapterEventArgs>>>();
			RegisterCallback(callbacks[key], type, callback);
		}

		/// <summary>
		/// Removes the callback from the dictionary.
		/// </summary>
		/// <param name="callbacks"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		private static void UnregisterCallback(IDictionary<eSigType, IcdHashSet<Action<SigCallbackManager, SigAdapterEventArgs>>> callbacks,
											   eSigType type, Action<SigCallbackManager, SigAdapterEventArgs> callback)
		{
			if (callbacks.ContainsKey(type))
				callbacks[type].Remove(callback);
		}

		/// <summary>
		/// Removes the callback from the dictionary.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="callbacks"></param>
		/// <param name="key"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		private static void UnregisterCallback<T>(
			IDictionary<T, Dictionary<eSigType, IcdHashSet<Action<SigCallbackManager, SigAdapterEventArgs>>>> callbacks,
			T key, eSigType type, Action<SigCallbackManager, SigAdapterEventArgs> callback)
		{
			if (callbacks.ContainsKey(key))
				UnregisterCallback(callbacks[key], type, callback);
		}

		#endregion
	}
}
