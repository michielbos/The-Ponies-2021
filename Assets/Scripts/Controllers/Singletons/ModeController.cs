using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
	public class ModeController : SingletonMonoBehaviour<ModeController>
	{
		public GuiButtonController LiveModeGuiButtonController;
		public GuiButtonController BuyModeGuiButtonController;
		public GuiButtonController BuildModeGuiButtonController;
		public GuiButtonController CameraModeGuiButtonController;
		public GuiButtonController OptionsModeGuiButtonController;

		private HudPanel currentMode;

		public void SwitchMode(HudPanel mode)
		{
			if (mode == currentMode)
				mode = HudPanel.None;
			currentMode = mode;

			LiveModeGuiButtonController.enabled = mode == HudPanel.Live;
			BuyModeGuiButtonController.enabled = mode == HudPanel.Buy;
			BuildModeGuiButtonController.enabled = mode == HudPanel.Build;
			CameraModeGuiButtonController.enabled = mode == HudPanel.Camera;
			OptionsModeGuiButtonController.enabled = mode == HudPanel.Options;

			// Only pause for Buy Mode and up, excluding Live mode and whenever no mode is selected.
			// No pannel = -1
			//      Live = 0
			SpeedController.Instance.ForcePause(mode > 0); 
			HUDController.Instance.UpdateSpeed();
		}
	}
}