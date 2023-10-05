using System.Net.Sockets;
using System.Net;
using System.Text;

namespace HW_29092023_ServerApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await App();
        }

        static async Task App()
        {
            int port = 8080;
            string ip = "127.0.0.1";
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Bind(ipPoint);
                socket.Listen();
                Console.WriteLine("Server started. Waiting for connections...");
                Socket client = await socket.AcceptAsync();
                IPEndPoint clientEndPoint = (IPEndPoint)client.RemoteEndPoint;
                Console.WriteLine("Client connected. Waiting for data...");
                do
                {
                    byte[] data = new byte[256];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = await client.ReceiveAsync(data, SocketFlags.None);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));

                        string message = "";
                        switch (builder.ToString())
                        {
                            case "date":
                                message = $"{DateTime.Now.ToShortDateString()}";
                                break;
                            case "time":
                                message = $"{DateTime.Now.ToShortTimeString()}";
                                break;
                            default:
                                message = $"Invalid request!";
                                break;
                        }
                        Console.WriteLine($"At {DateTime.Now.ToShortTimeString()} was received from {clientEndPoint.Address}: {message}");
                        byte[] messagedata = Encoding.UTF8.GetBytes(message);
                        await client.SendAsync(messagedata, SocketFlags.None);

                    } while (client.Available > 0);
                }while (true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            
        }

    }
}