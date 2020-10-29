using System.Collections.Generic;
using NUnit.Framework;

namespace ICD.Connect.Cameras.Vaddio.Tests
{
	[TestFixture]
	public sealed class VaddioRoboshotSerialBufferTest
	{
		[Test]
		public void EnqueueTest()
		{
			int loginPrompts = 0;
			int passwordPrompts = 0;
			List<string> completed = new List<string>();
			List<string> telnetHeader = new List<string>();

			VaddioRoboshotSerialBuffer buffer = new VaddioRoboshotSerialBuffer();
			buffer.OnUsernamePrompt += (sender, args) => loginPrompts++;
			buffer.OnPasswordPrompt += (sender, args) => passwordPrompts++;
			buffer.OnCompletedSerial += (sender, args) => completed.Add(args.Data);
			buffer.OnSerialTelnetHeader += (sender, args) => telnetHeader.Add(args.Data);

			// Telnet negotiation
			const string telnet = "ÿý\u0001ÿý\u001fÿû\u0001ÿû\u0003";
			buffer.Enqueue(telnet);

			Assert.AreEqual(0, loginPrompts);
			Assert.AreEqual(0, passwordPrompts);
			Assert.AreEqual(0, completed.Count);
			Assert.AreEqual(4, telnetHeader.Count);

			// Welcome message + login prompt
			const string welcome =
				"\r\r\n\r\n\r /\\\\\\        /\\\\\\   /\\\\\\\\\\     /\\\\\\      /\\\\\\\\\\\\\\\\\\\\\\\\         \r\n\r  \\/\\\\\\       \\/\\\\\\  \\/\\\\\\\\\\\\   \\/\\\\\\    /\\\\\\//////////         \r\n\r   \\//\\\\\\      /\\\\\\   \\/\\\\\\/\\\\\\  \\/\\\\\\   /\\\\\\                   \r\n\r     \\//\\\\\\    /\\\\\\    \\/\\\\\\//\\\\\\ \\/\\\\\\  \\/\\\\\\    /\\\\\\\\\\\\\\      \r\n\r       \\//\\\\\\  /\\\\\\     \\/\\\\\\\\//\\\\\\\\/\\\\\\  \\/\\\\\\   \\/////\\\\\\     \r\n\r         \\//\\\\\\/\\\\\\      \\/\\\\\\ \\//\\\\\\/\\\\\\  \\/\\\\\\       \\/\\\\\\    \r\n\r           \\//\\\\\\\\\\       \\/\\\\\\  \\//\\\\\\\\\\\\  \\/\\\\\\       \\/\\\\\\   \r\n\r             \\//\\\\\\        \\/\\\\\\   \\//\\\\\\\\\\  \\//\\\\\\\\\\\\\\\\\\\\\\\\/   \r\n\r               \\///         \\///     \\/////    \\////////////    \r\n\r                                                                                                      \r\n\rVaddio LLC http://www.vaddio.com\r\n\r\r\n\rVaddio VNG 1.6+snapshot-20171129 vaddio-conferenceshot-54-10-EC-A8-63-E9\r\n\r\r\n\r\r\nvaddio-conferenceshot-54-10-EC-A8-63-E9 login: ";
			buffer.Enqueue(welcome);

			Assert.AreEqual(1, loginPrompts);
			Assert.AreEqual(0, passwordPrompts);
			Assert.AreEqual(0, completed.Count);
			Assert.AreEqual(4, telnetHeader.Count);
		}
	}
}
