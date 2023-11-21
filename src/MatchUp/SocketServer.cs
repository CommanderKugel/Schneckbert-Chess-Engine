using System.Net;
using System.Net.Sockets;
using System.Text;


public class SocketServer
{
    const string connectionIP = "127.0.0.1";
    const int connectionPort = 25001;
    IPAddress localAddress;
    TcpListener listener;
    TcpClient client;

    bool running;


    public void StartServer()
    {
        localAddress = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        client = listener.AcceptTcpClient();

        running = true;
        while (running)
        {
            sendData();
        }
    }

    private void sendData()
    {
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];

        // here implement receiving data
        //int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        //string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        //if (dataReceived != null)
        //{
        //      handle received Data here
        //}

        // send data to host
        byte[] myWriteBuffer = Encoding.ASCII.GetBytes("Hello World!");
        nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length);


    }

}
