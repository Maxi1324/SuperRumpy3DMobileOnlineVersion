﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    [Serializable]
    public class UIPlayerInfo
    {
        public string PlayerName { get; set; }
        public int PlayerNum { get; set; }
        public PlayerSkin Skin { get; set; }
        public int ControllerNum { get; set; }
        public long RegisZeit { get; private set; }
        public bool Editable { get; set; } = true;

        public bool WasUpStick { get; set; }

        public UIPlayerInfo()
        {
            RegisZeit = Stopwatch.GetTimestamp();
        }

        public override bool Equals(object obj)
        {
            var info = obj as UIPlayerInfo;
            return PlayerNum == info.PlayerNum;
        }

        public override int GetHashCode()
        {
            var hashCode = 977716907;
            hashCode = hashCode * -1521134295 + PlayerNum.GetHashCode();
            return hashCode;
        }
    }
    [Serializable]
    public enum PlayerSkin
    {
        Rumpy,
        Rumpina,
        Joe,
        Mama
    }
}
