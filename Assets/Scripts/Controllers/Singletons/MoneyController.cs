using Assets.Scripts.Util;

namespace Assets.Scripts.Controllers
{
    public class MoneyController : SingletonMonoBehaviour<MoneyController>
    {
        public int Funds { get; private set; }

#if UNITY_EDITOR
        // Playing in edit mode, are we? 
        // You get infinite meney! Yay!
        private void Start()
        {
            Funds = int.MaxValue; 
            HUDController.Instance.UpdateFunds();
        }
#endif

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