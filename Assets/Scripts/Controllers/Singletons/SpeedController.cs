using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
	public enum Speed
	{
		Paused = 0,
		Normal = 1,
		Fast = 2,
		Ultra = 3
	}

	public class SpeedController : SingletonMonoBehaviour<SpeedController>
	{
		public Speed CurrentSpeed { get; private set; }
		public bool Paused { get { return CurrentSpeed == Speed.Paused || _forcePaused; } }
		private float _pauseTimer;
		private bool _forcePaused;

		void Start()
		{
			CurrentSpeed = Speed.Normal;
		}

		public void Update()
		{
			if (Paused)
			{
				_pauseTimer += Time.deltaTime * 2;
				HUDController.Instance.UpdatePauseBlink(_pauseTimer);
			}
		}

		public void SetSpeed(Speed speed)
		{
			if (_forcePaused)
				return;

			int from = Paused ? 0 : (int) CurrentSpeed;
			int to = (int) speed;

			if (from != to)
			{
				PlaySpeedSound(from, to);
				CurrentSpeed = speed;
				_pauseTimer = 0;
				HUDController.Instance.UpdateSpeed();
			}
		}

		private void PlaySpeedSound(int from, int to)
		{
			string ff = from.ToString();
			string tf = to.ToString();
			if (ff == "0") ff = "P";
			if (tf == "0") tf = "P";
			string clipName = "UI_SPEED_" + ff + "TO" + tf;
			SoundController.Instance.PlaySound(clipName);
		}

		public void ForcePause(bool paused)
		{
			_forcePaused = paused;
		}
	}
}