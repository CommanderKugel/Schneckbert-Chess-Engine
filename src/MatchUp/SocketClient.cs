
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SocketClient
{
    const string connectionIP = "127.0.0.1";
    const int connectionPort = 25001;

    IPHostEntry host;
    IPAddress ipAddress;
    IPEndPoint remoteEP;

    Socket sender;

    public void bootClient()
    {
        try
        {
            host = Dns.GetHostEntry("localhost");
            ipAddress = IPAddress.Parse(connectionIP);
            remoteEP = new IPEndPoint(ipAddress, connectionPort);

            sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        catch
        {
            Console.WriteLine("ERROR booting Client");
        }
    }

    public bool sendData(ushort data)
    {
        try
        {
            sender.Connect(remoteEP);

            byte msg2 = (byte) (data >> 8);
            byte msg1 = (byte) data;
            byte[] msg = { msg1, msg2 };
            int bytesSent = sender.Send(msg);

            sender.Shutdown(SocketShutdown.Both);
            sender.Close();

            return true;
        }
        catch
        {
            Console.WriteLine("ERROR sending data");
            return false;
        }
    }

    public void StartClient()
    {
        try {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = IPAddress.Parse(connectionIP);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, connectionPort);

            Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sender.Connect(remoteEP);

                byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");
                int bytesSent = sender.Send(msg);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

            }
            catch
            {
                Console.WriteLine("ERROR sending data 1");
            }
        }
        catch
        {
            Console.WriteLine("ERROR sending data 2");
        }
    }
    
}
