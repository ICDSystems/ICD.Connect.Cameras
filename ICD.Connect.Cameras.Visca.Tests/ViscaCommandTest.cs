using ICD.Common.Utils;
using NUnit.Framework;

namespace ICD.Connect.Cameras.Visca.Tests
{
	[TestFixture]
	public sealed class ViscaCommandTest
	{
		[Test]
		public static void GetPanCommandTest()
		{
			string expectedPanLeftCommand = StringUtils.ToString(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x08, 0x08, 0x01, 0x03, 0xFF });
			string expectedPanRightCommand = StringUtils.ToString(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x08, 0x08, 0x02, 0x03, 0xFF });

			ViscaCommand panLeftCommand = ViscaCommand.GetPanCommand(0, eCameraPanAction.Left);
			ViscaCommand panRightCommand = ViscaCommand.GetPanCommand(0, eCameraPanAction.Right);

			Assert.AreEqual(expectedPanLeftCommand, panLeftCommand.Serialize());
			Assert.AreEqual(expectedPanRightCommand, panRightCommand.Serialize());
		}

		[Test]
		public static void GetTiltCommandTest()
		{
			string expectedTiltUpCommand = StringUtils.ToString(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x08, 0x08, 0x03, 0x01, 0xFF });
			string expectedTiltDownCommand = StringUtils.ToString(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x08, 0x08, 0x03, 0x02, 0xFF });

			ViscaCommand tiltUpCommand = ViscaCommand.GetTiltCommand(0, eCameraTiltAction.Up);
			ViscaCommand tiltDownCommand = ViscaCommand.GetTiltCommand(0, eCameraTiltAction.Down);

			Assert.AreEqual(expectedTiltUpCommand, tiltUpCommand.Serialize());
			Assert.AreEqual(expectedTiltDownCommand, tiltDownCommand.Serialize());
		}

		[Test]
		public static void GetZoomCommandTest()
		{
			string expectedZoomInCommand = StringUtils.ToString(new byte[] { 0x81, 0x01, 0x04, 0x07, 0x21, 0xFF });
			string expectedZoomOutCommand = StringUtils.ToString(new byte[] { 0x81, 0x01, 0x04, 0x07, 0x31, 0xFF });

			ViscaCommand zoomInCommand = ViscaCommand.GetZoomCommand(0, eCameraZoomAction.ZoomIn, 1);
			ViscaCommand zoomOutCommand = ViscaCommand.GetZoomCommand(0, eCameraZoomAction.ZoomOut, 1);

			Assert.AreEqual(expectedZoomInCommand, zoomInCommand.Serialize());
			Assert.AreEqual(expectedZoomOutCommand, zoomOutCommand.Serialize());
		}

		[Test]
		public static void GetSetAddressCommandTest()
		{
			string expectedGetAddressCommand = StringUtils.ToString(new byte[] { 0x88, 0x30, 0x01, 0xFF });
			ViscaCommand getAddressCommand = ViscaCommand.GetSetAddressCommand();

			Assert.AreEqual(expectedGetAddressCommand, getAddressCommand.Serialize());
		}

		[Test]
		public static void GetClearCommandTest()
		{
			string expectedClearCommand = StringUtils.ToString(new byte[] { 0x88, 0x01, 0x00, 0x01, 0xFF });
			ViscaCommand clearCommand = ViscaCommand.GetClearCommand();

			Assert.AreEqual(expectedClearCommand, clearCommand.Serialize());
		}

		[Test]
		public static void GetPowerOnCommandTest()
		{
			string expectedPowerOnCommand = StringUtils.ToString(new byte[] {0x81, 0x01, 0x04, 0x00, 0x02, 0xFF});
			ViscaCommand powerOnCommand = ViscaCommand.GetPowerOnCommand(1);

			Assert.AreEqual(expectedPowerOnCommand, powerOnCommand.Serialize());
		}

		[Test]
		public static void GetPowerOffCommandTest()
		{
			string expectedPowerOffCommand = StringUtils.ToString(new byte[] { 0x81, 0x01, 0x04, 0x00, 0x03, 0xFF });
			ViscaCommand powerOffCommand = ViscaCommand.GetPowerOffCommand(1);

			Assert.AreEqual(expectedPowerOffCommand, powerOffCommand.Serialize());
		}
	}
}
