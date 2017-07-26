using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ICD.Common.Permissions.Test_SimplSharp
{
	[TestFixture]
	public class PermissionsManagerTests
	{
		private const string ACTION = "TestAction";
		private PermissionsManager manager;
		private Object testObj;

		[SetUp]
		public void Init()
		{
			testObj = new object();
			manager = new PermissionsManager();
			manager.SetDefaultPermissions(CreateDefaultPermissions());
			manager.SetObjectPermissions(testObj, CreateObjectPermissions());
			manager.DefaultRoles = new [] {"DefaultRole"};
		}

		private static IEnumerable<Permission> CreateDefaultPermissions()
		{
			yield return new Permission() {Action = Action.FromString(ACTION), Roles = new [] {"TestRole"}};
			yield return new Permission() {Action = Action.FromString("FallbackAction"), Roles = new[] {"FallbackRole"}};
		}

		private static IEnumerable<Permission> CreateObjectPermissions()
		{
			yield return new Permission() {Action = Action.FromString(ACTION), Roles = new [] {"TestObjectRole"}};
		}

		[Test]
		public void GetRoles_DefaultPermissions()
		{
			var roles = manager.GetRoles(Action.FromString(ACTION)).ToList();
			Assert.Contains("TestRole", roles);
		}

		[Test]
		public void GetRoles_WithObject_ObjectPermissions()
		{
			var roles = manager.GetRoles(Action.FromString(ACTION), testObj).ToList();
			Assert.Contains("TestObjectRole", roles);
		}

		[Test]
		public void GetRoles_FallbackToDefaultRoles()
		{
			var roles = manager.GetRoles(Action.FromString("DifferentAction")).ToList();
			Assert.Contains("DefaultRole", roles);
		}

		[Test]
		public void GetRoles_WithObject_FallbackToDefaultPermissions()
		{
			var roles = manager.GetRoles(Action.FromString("FallbackAction"), testObj).ToList();
			Assert.Contains("FallbackRole", roles);
		}
	}
}