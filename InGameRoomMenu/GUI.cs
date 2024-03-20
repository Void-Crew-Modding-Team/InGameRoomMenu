using Photon.Pun;
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

        //string ErrorMessage;
        string RoomName;
        bool RoomIsPrivate = false;
        float PlayerLimit;
        //string PlayerLimit;
        //byte MaxPlayerLimit;

        public override void Draw()
        {
            if(!Game.InGame) //Blank menu when not in game.
            {
                Label("Must be in game");
                return;
            }
            if(RoomName == null) //Menu was open when game started.
            {
                OnOpen();
            }

            Label("Usage: While in game and host, change values below and hit 'apply'. Loaded values are reloaded on menu open.");

            //Room Name TextField + Label
            Label("Room Name:");
            RoomName = TextField(RoomName);

            //Private Room Button
            if (Button($"Private Room: {RoomIsPrivate}"))
            {
                if(PhotonNetwork.IsMasterClient)
                {
                    RoomIsPrivate = !RoomIsPrivate;
                }
            }

            Label($"Player Limit: {PlayerLimit.ToString()}/4");
            //PlayerLimit = TextField(PlayerLimit);
            PlayerLimit = HorizontalSlider(PlayerLimit, 0, 4);
            PlayerLimit = (int)PlayerLimit;

            if (!PhotonNetwork.IsMasterClient) //Block apply button, but clients can still read current lobby settings
            {
                Label("Must be host to change settings");
            }
            else if (Button("Apply"))
            {
                //ErrorMessage = null;

                //Apply Room Privacy
                if(PhotonService.Instance.GetCurrentRoomPrivate() != RoomIsPrivate)
                {
                    PhotonService.Instance.SetCurrentRoomPrivate(RoomIsPrivate);
                }
                if(RoomName != PhotonService.Instance.GetCurrentRoomName())
                {
                    PhotonService.Instance.SetCurrentRoomName(RoomName);
                }
                if((byte)PlayerLimit != PhotonNetwork.CurrentRoom.MaxPlayers)
                {
                    PhotonNetwork.CurrentRoom.MaxPlayers = (byte)PlayerLimit;
                }
                /*if(byte.TryParse(PlayerLimit, out byte pLimit) && pLimit != PhotonNetwork.CurrentRoom.MaxPlayers)
                {
                    if (pLimit >= 0 && pLimit <= 4)
                    {
                        PhotonNetwork.CurrentRoom.MaxPlayers = pLimit;
                    }
                    else
                    {
                        ErrorMessage = "Player limit must not go below 0 or exceed 4";
                    }
                }*/
            }
            //if(!ErrorMessage.IsNullOrWhiteSpace())
            //{
            //    GUI.color = Color.red;
            //    Label(ErrorMessage);
            //    GUI.color = Color.white;
            //}
        }

        public override void OnOpen()
        {
            //ErrorMessage = string.Empty;
            if (Game.InGame)
            {
                RoomName = PhotonService.Instance.GetCurrentRoomName();
                RoomIsPrivate = PhotonService.Instance.GetCurrentRoomPrivate();
                PlayerLimit = PhotonNetwork.CurrentRoom.MaxPlayers;
                //if(VoidManager.MPModChecks.MP)
            }
        }
    }
}
