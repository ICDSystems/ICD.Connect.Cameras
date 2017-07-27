using ICD.Connect.Conferencing.Cameras;
using NUnit.Framework;

namespace ICD.Connect.Cameras.Visca.Tests_NetStandard
{
    [TestFixture]
    public sealed class ViscaCommandHandlerTest
    {
        [TestCase(0, 1, 0x81)]
        [TestCase(1, 0, 0x90)]
        public void GetIdsByte(int sender, int recipient, byte expected)
        {
            byte result = ViscaCommandHandler.GetIdsByte(sender, recipient);
            Assert.AreEqual(expected, result);
        }

        [TestCase("\x88\x30\x01\xFF")]
        public void SetAddress(string expected)
        {
            ViscaCommandHandler commandHandler = new ViscaCommandHandler();
            string result = commandHandler.SetAddress();
            Assert.AreEqual(expected, result);
            //return StringUtils.ToString(new byte[] { 0x88, 0x30, 0x01, 0xFF });

        }
        [TestCase(1, "\x81\x01\x00\x01\xFF")]
        [TestCase(2, "\x82\x01\x00\x01\xFF")]
        public void Clear(int Id, string expected)
        {
            ViscaCommandHandler commandHandler = new ViscaCommandHandler();
            string result = commandHandler.Clear(Id);
            //return StringUtils.ToString(new byte[] { GetIdsByte(0, Id), 0x01, 0x00, 0x01, 0xFF });
            Assert.AreEqual(expected, result);
        }
        [TestCase(1, "\x81\x01\x04\x07\x00\xFF\x81\x01\x06\x01\x01\x01\x03\x03\xFF")]
        public void Stop(int Id, string expected)
        {
            ViscaCommandHandler commandHandler = new ViscaCommandHandler();
            string result = commandHandler.Clear(Id);
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Moves the camera or zooms in, based on eCameraAction.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="action"></param>
        /// <param name="panSpeed"></param>
        /// <param name="tiltSpeed"></param>
        /// <param name="zoomSpeed"></param>
        /// <returns></returns>
        [TestCase(1, eCameraAction.Up, 8, 8, 3, "\x81\x01\x06\x01\x08\x08\x03\x03\x01\xFF")]
        [TestCase(1, eCameraAction.Down, 8, 8, 3, "\x81\x01\x06\x01\x08\x08\x03\x03\x02\xFF")]
        [TestCase(1, eCameraAction.Left, 8, 8, 3, "\x81\x01\x06\x01\x08\x08\x03\x01\x03\xFF")]
        [TestCase(1, eCameraAction.Down, 8, 8, 3, "\x81\x01\x06\x01\x08\x08\x03\x02\x03\xFF")]
        [TestCase(1, eCameraAction.ZoomIn, 8, 8, 3, "\x81\x01\x04\x07\x23\0xFF")]
        [TestCase(1, eCameraAction.ZoomOut, 8, 8, 3, "\x81\x01\x04\x07\x33\0xFF")]
        [TestCase(1, eCameraAction.Up, 24, 24, 7, "\x81\x01\x06\x01\x24\x24\x07\x03\x01\xFF")]
        [TestCase(1, eCameraAction.Down, 24, 24, 7, "\x81\x01\x06\x01\x24\x24\x07\x03\x02\xFF")]
        [TestCase(1, eCameraAction.Left, 24, 24, 7, "\x81\x01\x06\x01\x24\x24\x07\x01\x03\xFF")]
        [TestCase(1, eCameraAction.Down, 24, 24, 7, "\x81\x01\x06\x01\x24\x24\x07\x02\x03\xFF")]
        [TestCase(1, eCameraAction.ZoomIn, 24, 24, 7, "\x81\x01\x04\x07\x27\0xFF")]
        [TestCase(1, eCameraAction.ZoomOut,24, 24, 7, "\x81\x01\x04\x07\x37\0xFF")]
        public void Move(int Id, eCameraAction action, int panSpeed, int tiltSpeed, int zoomSpeed, string expected)
        {
            ViscaCommandHandler commandHandler = new ViscaCommandHandler();
            string result = commandHandler.Move(Id, action, panSpeed, tiltSpeed, zoomSpeed);
            Assert.AreEqual(expected, result);
        }
    }
}
