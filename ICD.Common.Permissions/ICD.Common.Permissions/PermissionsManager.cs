using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;

namespace ICD.Common.Permissions
{
	public class PermissionsManager
	{
		private List<Permission> DefaultPermissions { get; set; }

		private Dictionary<object, List<Permission>> ObjectPermissions { get; set; }

		private SafeCriticalSection DefaultPermissionsSection { get; set; }
		private SafeCriticalSection ObjectPermissionsSection { get; set; }

		public IEnumerable<string> DefaultRoles { get; set; }
		
		/// <summary>
		/// Constructor
		/// </summary>
		public PermissionsManager()
		{
			DefaultPermissions = new List<Permission>();
			ObjectPermissions = new Dictionary<object, List<Permission>>();
			DefaultPermissionsSection = new SafeCriticalSection();
			ObjectPermissionsSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Sets the default set of permissions
		/// </summary>
		/// <param name="permissions"></param>
		[PublicAPI]
		public void SetDefaultPermissions(IEnumerable<Permission> permissions)
		{
			DefaultPermissions = RemoveDuplicateActions(permissions).ToList();
		}

		/// <summary>
		/// Sets the object-specific permissions for the given object
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="permissions"></param>
		[PublicAPI]
		public void SetObjectPermissions(object obj, IEnumerable<Permission> permissions)
		{
			ObjectPermissions[obj] = RemoveDuplicateActions(permissions).ToList();
		}

		/// <summary>
		/// Removes the object-specific permissions for the given object
		/// </summary>
		/// <param name="obj"></param>
		[PublicAPI]
		public void RemoveObjectPermissions(object obj)
		{
			ObjectPermissions.Remove(obj);
		}

		/// <summary>
		/// Gets the set of roles required for an action. Uses default permissions as a lookup
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		[PublicAPI]
		public IEnumerable<string> GetRoles(IAction action)
		{
			var permission = DefaultPermissions.SingleOrDefault(p => p.Action.Value.Equals(action.Value));
			if (permission == null)
				return (DefaultRoles ?? Enumerable.Empty<string>()).ToList();
			return permission.Roles.ToList();
		}

		/// <summary>
		/// Gets the set of roles required for an action on specific object. Falls back to default permissions
		/// if no object-specific permissions are found
		/// </summary>
		/// <param name="action"></param>
		/// <param name="obj"></param>
		/// <returns>null if no permissions were found</returns>
		[PublicAPI]
		public IEnumerable<string> GetRoles(IAction action, object obj)
		{
			if (ObjectPermissions.ContainsKey(obj))
			{
				var permission = ObjectPermissions[obj].SingleOrDefault(p => p.Action.Value.Equals(action.Value));
				if (permission == null)
					return GetRoles(action);
				return permission.Roles.ToList();
			}
			return GetRoles(action);
		}

		/// <summary>
		/// Removes permissions with duplicate actions
		/// </summary>
		/// <param name="permissions"></param>
		/// <returns></returns>
		private IEnumerable<Permission> RemoveDuplicateActions(IEnumerable<Permission> permissions)
		{
			//Remove permissions with duplicate actions by using GroupBy -> Select First
			return permissions.GroupBy(p => p.Action).Select(g => g.First());
		}
	}
}