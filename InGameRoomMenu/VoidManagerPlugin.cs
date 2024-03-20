using VoidManager.MPModChecks;

namespace InGameRoomMenu
{
    public class VoidManagerPlugin : VoidManager.VoidPlugin
    {
        public override MultiplayerType MPType => MultiplayerType.Client;

        public override string Author => "Dragon";

        public override string Description => "Allows host to manage room settings during gameplay. Accessed with F5 menu.";
    }
}
