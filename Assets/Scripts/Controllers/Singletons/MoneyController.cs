﻿using Assets.Scripts.Util;

namespace Assets.Scripts.Controllers
{
    public class MoneyController : SingletonMonoBehaviour<MoneyController>
    {
        public int Funds { get; private set; }

        private void Start()
        {
            //TODO: Allocate the moneys in the proper place when we have one.
            Funds = 50000; 
            HUDController.Instance.UpdateFunds();
        }

        public void ChangeFunds(int change)
        {
            Funds += change;
            HUDController.Instance.UpdateFunds();
        }

        public void SetFunds(int amount)
        {
            Funds = amount;
            HUDController.Instance.UpdateFunds();
        }
    }
}