using Assets.Scripts.Util;

namespace Assets.Scripts.Controllers
{
	public class MoneyController : SingletonMonoBehaviour<MoneyController>
	{
		public int Funds { get; private set; }

		public void ChangeFunds(int change)
		{
			Funds += change;
			HUDController.Instance.UpdateFunds();
		}

		public void SetFunds(int ammount)
		{
			Funds = ammount;
			HUDController.Instance.UpdateFunds();
		}
	}
}