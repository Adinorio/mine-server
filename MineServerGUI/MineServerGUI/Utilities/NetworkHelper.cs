using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MineServerGUI.Utilities
{
    public static class NetworkHelper
    {
        public static string? GetLocalIPAddress()
        {
            try
            {
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                                 ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                foreach (var ni in networkInterfaces)
                {
                    var ipProps = ni.GetIPProperties();
                    var ipv4Address = ipProps.UnicastAddresses
                        .FirstOrDefault(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork &&
                                               !IPAddress.IsLoopback(addr.Address) &&
                                               (addr.Address.ToString().StartsWith("192.168.") ||
                                                addr.Address.ToString().StartsWith("10.")));

                    if (ipv4Address != null)
                    {
                        return ipv4Address.Address.ToString();
                    }
                }
            }
            catch
            {
                // Ignore errors
            }

            return null;
        }

        public static string? GetPublicIPAddress()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "MineServerGUI");
                    var response = client.GetStringAsync("https://api.ipify.org").GetAwaiter().GetResult();
                    return response.Trim();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}

