using Photon.Pun;
using System;
using UnityEngine;
using VoidManager.CustomGUI;
using VoidManager.Utilities;
using static UnityEngine.GUILayout;

namespace InGameRoomMenu
{
    internal class UGRMGUI : ModSettingsMenu
    {
        public override string Name()
        {
            return "In-Game Room Menu";
        }

        string ErrorMessage;
        string RoomName;
        string PlayerLimit;

        public override void Draw()
        {
            if(!Game.InGame || !PhotonNetwork.InRoom) //Blank menu when not in game.
            {
                Label("Must be in game");
                return;
            }
            if(RoomName == null) //Menu was open when game started.
            {
                OnOpen();
            }

            Label("Usage: While in game and host, change values below and hit 'apply'.");

            //Room Name TextField + Label
            SettingGroup($"Room Name - Current Value: {PhotonService.Instance.GetCurrentRoomName()}", ref RoomName, new Action(SetRooomName));

            //Private Room Button
            bool privateRoom = PhotonService.Instance.GetCurrentRoomPrivate();
            if (Button($"Room Publicity: {(privateRoom ? "Private" : "Public")}"))
            {
                if(PhotonNetwork.IsMasterClient)
                {
                    PhotonService.Instance.SetCurrentRoomPrivate(!privateRoom);
                }
            }

            //Player limit
            SettingGroup($"Player Limit - Current Value: {PhotonNetwork.CurrentRoom.MaxPlayers}", ref PlayerLimit, new Action(SetPlayerLimit));

            if (!string.IsNullOrWhiteSpace(ErrorMessage))
            {
                GUI.color = Color.red;
                Label(ErrorMessage);
                GUI.color = Color.white;
            }
        }

        void SetRooomName()
        {
            if (RoomName != PhotonService.Instance.GetCurrentRoomName())
            {
                PhotonService.Instance.SetCurrentRoomName(RoomName);
                RoomName = PhotonService.Instance.GetCurrentRoomName();
            }
        }

        void SetPlayerLimit()
        {
            byte MaxPlayerLimit = 4;
            var things = BepInEx.Bootstrap.Chainloader.PluginInfos;
            if (things.ContainsKey("MaxPlayers"))
            {
                MaxPlayerLimit = 255;
            }
            else if (things.ContainsKey("Space.HobosIn.Voider_Crew"))
            {
                MaxPlayerLimit = 8;
            }

            if (byte.TryParse(PlayerLimit, out byte pl) && pl != PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                if (pl <= MaxPlayerLimit && pl > 0)
                {
                    ErrorMessage = null;
                    PhotonNetwork.CurrentRoom.MaxPlayers = pl;
                    
                }
                else
                {
                    ErrorMessage = $"Player limit must be a number between 1 and {MaxPlayerLimit}";
                }
            }
            else
            {
                ErrorMessage = $"Player limit must be a number between 1 and {MaxPlayerLimit}";
            }
        }

        static GUIStyle MinSizeStyle;

        static void SettingGroup(string label, ref string settingvalue, Action func)
        {
            Label(label);
            BeginHorizontal();
            settingvalue = TextField(settingvalue);
            if(PhotonNetwork.IsMasterClient && Button("Apply", MinSizeStyle)) //Block apply button, but clients can still read current lobby settings
            {
                func?.Invoke();
            }
            EndHorizontal();
        }

        public override void OnOpen()
        {
            ErrorMessage = null;
            if (Game.InGame)
            {
                RoomName = PhotonService.Instance.GetCurrentRoomName();
                PlayerLimit = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
            }

            if(MinSizeStyle == null)
            {
                MinSizeStyle = new GUIStyle(GUI.skin.button);
                MinSizeStyle.stretchWidth = false;
            }
        }
    }
}
