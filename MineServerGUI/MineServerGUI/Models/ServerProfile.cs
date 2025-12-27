using System;

namespace MineServerGUI.Models
{
    public class ServerProfile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string Version { get; set; } = "";
        public string ServerDirectory { get; set; } = "";
        public string ServerJarPath { get; set; } = "";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;
        public string? Description { get; set; }
        
        // Server settings
        public string MinMemory { get; set; } = "2G";
        public string MaxMemory { get; set; } = "4G";
    }
}

