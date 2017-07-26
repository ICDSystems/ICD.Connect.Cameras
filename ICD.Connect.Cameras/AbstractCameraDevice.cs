﻿using ICD.Connect.Conferencing.Cameras;
using ICD.Connect.Devices;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.Extensions;
using ICD.Common.EventArguments;
using ICD.Connect.Settings.Core;
using ICD.Common.Services.Logging;
using ICD.Connect.API.Commands;
using System.Collections.Generic;

namespace ICD.Connect.Cameras
{
    public abstract class AbstractCameraDevice<TSettings> : AbstractDevice<TSettings>, ICameraDevice
        where TSettings : ICameraDeviceSettings, new()
    {
        #region Methods

        public abstract void Move(eCameraAction action);

        public abstract void Stop();

                public override void BuildConsoleStatus(ICD.Connect.API.Nodes.AddStatusRowDelegate addRow)
        {
            base.BuildConsoleStatus(addRow);
        }

        /// <summary>
        /// Gets the child console commands.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IConsoleCommand> GetConsoleCommands()
        {
            foreach (IConsoleCommand command in GetBaseConsoleCommands())
                yield return command;

            yield return new GenericConsoleCommand<eCameraAction>("Move", "Moves or zooms the camera device", v => this.Move(v));
            yield return new ConsoleCommand("Stop", "Stops the camera", () => this.Stop());
        }

        /// <summary>
        /// Workaround for the "unverifiable code" warning.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
        {
            return base.GetConsoleCommands();
        }
        #endregion
    }

}