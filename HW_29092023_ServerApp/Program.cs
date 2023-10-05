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

            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(ipPoint);
                socket.Listen();
                Console.WriteLine("Server started. Waiting for connections...");

                while (true)
                {
                    Socket client = await socket.AcceptAsync();
                    IPEndPoint clientEndPoint = (IPEndPoint)client.RemoteEndPoint;
                    Console.WriteLine("Client connected. Waiting for data...");

                    Task.Run(async () =>
                    {
                        byte[] data = new byte[256];
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        try
                        {
                            do
                            {
                                bytes = await client.ReceiveAsync(data, SocketFlags.None);
                                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                                Console.WriteLine($"At {DateTime.Now.ToShortTimeString()} a line was received from {clientEndPoint.Address}: {builder.ToString()}");

                                string message = "Hello client!";
                                byte[] messagedata = Encoding.UTF8.GetBytes(message);
                                await client.SendAsync(messagedata, SocketFlags.None);

                            } while (client.Connected && client.Available > 0);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Client {clientEndPoint.Address} disconnected: {e.Message}");
                        }
                        finally
                        {
                            client.Close();
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
    }
}