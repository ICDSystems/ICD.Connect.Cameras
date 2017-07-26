using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Presenters
{
	public sealed class ProgramFusionPresenter : AbstractFusionPresenter<IProgramFusionView>, IProgramFusionPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="presenters"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public ProgramFusionPresenter(int roomId, IFusionPresenterFactory presenters, IFusionViewFactory views, ICore core)
			: base(roomId, presenters, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();

			string compiledDate = ProgramUtils.CompiledDate;
			string programFile = ProgramUtils.ProgramFile;
			string programSlot = ProgramUtils.ProgramNumberFormatted;
			string programVersion = ProgramUtils.CompilerRevision.ToString();
			string systemName = ProgramUtils.ApplicationName;
			string model = CrestronUtils.ModelName;
			string version = CrestronUtils.ModelVersion.ToString();
			string address = IcdEnvironment.NetworkAddresses.FirstOrDefault();

			GetView().SetCompiledDate(compiledDate);
			GetView().SetProgramFile(programFile);
			GetView().SetProgramSlot(programSlot);
			GetView().SetProgramVersion(programVersion);
			GetView().SetSystemName(systemName);
			GetView().SetProcessorModel(model);
			GetView().SetProcessorFirmwareVersion(version);
			GetView().SetProcessorAddress(address);
		}
	}
}
