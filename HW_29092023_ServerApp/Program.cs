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
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.ConnectAsync(endPoint);
                Console.WriteLine("Connected to the server");
                Console.WriteLine("Enter your choice: \n1.Date\n2.Time");
                int choice;
                bool ok = int.TryParse(Console.ReadLine(), out choice);

                string message = "";

                if (ok)
                {
                    switch (choice)
                    {
                        case 1:
                            message = "date";
                            break;
                        case 2:
                            message = "time";
                            break;
                        default:
                            break;
                    }
                }

                byte[] data = Encoding.UTF8.GetBytes(message);
                await socket.SendAsync(data, SocketFlags.None);


                data = new byte[256];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = await socket.ReceiveAsync(data, SocketFlags.None);
                    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    Console.WriteLine($"At {message} was received from {endPoint.Address}: {builder.ToString()}");

                } while (socket.Available > 0);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}