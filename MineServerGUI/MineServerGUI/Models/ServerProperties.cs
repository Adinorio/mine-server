namespace MineServerGUI.Models
{
    public class ServerProperties
    {
        public int MaxPlayers { get; set; } = 8;
        public string Difficulty { get; set; } = "normal";
        public string Gamemode { get; set; } = "survival";
        public string MOTD { get; set; } = "A Minecraft Server";
        public bool WhitelistEnabled { get; set; } = true;
        public bool EnforceWhitelist { get; set; } = true;
    }
}

