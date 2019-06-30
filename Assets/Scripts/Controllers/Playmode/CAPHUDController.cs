using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.Util;
using Controllers.Singletons;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Controllers.Playmode
{

    public class CAPHUDController : SingletonMonoBehaviour<CAPHUDController>, IPointerEnterHandler, IPointerExitHandler
    {

        public Image cpanel;

        public Sprite noModeCpanel;
        public Sprite bodyModeCpanel;
        public Sprite headModeCpanel;
        public Sprite eyesModeCpanel;
        public Sprite maneModeCpanel;
        public Sprite tailModeCpanel;
        public Sprite outfitModeCpanel;
        public Sprite accesorryModeCpanel;
        public Sprite peronalityModeCpanel;

        private bool touchingGui;

     //   void Start()
     //   {
     //       MusicController.Instance.SwitchMusic(MusicType.NoMusic);
     //   }

        // Called from Unity GUI Button
        public void ActivatePanel(int index)
        {
            ActivatePanel((CapHudPanel)index);
        }

        private static void ActivatePanel(CapHudPanel panel)
        {
            CAPModeController.GetInstance().SwitchMode(panel);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            touchingGui = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            touchingGui = false;
        }

        public bool IsMouseOverGui()
        {
            //I doubt how reliable this is, but it's not like we have anything better at the moment.
            return touchingGui;
        }

        public void OnModeUpdate(CapHudPanel mode)
        {
            Debug.Log ("Swapping sprite, mode" + mode);
            if (mode == CapHudPanel.Body)
            {
                cpanel.sprite = bodyModeCpanel;
            }
            else if (mode == CapHudPanel.Head)
            {
                cpanel.sprite = headModeCpanel;
            }
            else if (mode == CapHudPanel.Eyes)
            {
                cpanel.sprite = eyesModeCpanel;
            }
            else if (mode == CapHudPanel.Mane)
            {
                cpanel.sprite = maneModeCpanel;
            }
            else if (mode == CapHudPanel.Tail)
            {
                cpanel.sprite = tailModeCpanel;
            }
            else if (mode == CapHudPanel.Outfit)
            {
                cpanel.sprite = outfitModeCpanel;
            }
            else if (mode == CapHudPanel.Accesorry)
            {
                cpanel.sprite = accesorryModeCpanel;
            }
            else if (mode == CapHudPanel.Personality)
            {
                cpanel.sprite = peronalityModeCpanel;
            }
            else
            {
                cpanel.sprite = noModeCpanel;
            }
        }
    }

    public enum CapHudPanel
    {
        None = -1,
        Body = 0,
        Head = 1,
        Eyes = 2,
        Mane = 3,
        Tail = 4,
        Outfit = 5,
        Accesorry = 6,
        Personality = 7,
    }



}