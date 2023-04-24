using System.Text;
// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using CoreFoundation;
using CoreSmartCard;
using CoreSmartCard.Cards;
Console.WriteLine("Hello, World!");

string ipAddress = "192.168.1.8";
int port = 4162;

var _tcpClient= new TcpClient(ipAddress, port);
        var context = new CardContext();
        var terminals = context.ListReaders();

        // Connect to remote reader
        var remoteTerminal = terminals.FirstOrDefault(t => t.Name.Contains("RemoteReader"));
        if (remoteTerminal == null)
        {
            throw new Exception("Remote reader not found.");
        }

        // Open network stream
        var stream = _tcpClient.GetStream();

        // Connect to remote card
        var remoteCard = new RemoteCard(remoteTerminal, new NetworkStreamConnection(stream));

        // Get tag UID
        var uidBytes = remoteCard.GetAttrib(CardAttrib.Uid);
        var uidString = BitConverter.ToString(uidBytes).Replace("-", "");

        return uidString;


// while (true)
// {
//     try
//     {
//         // Connect to the ATR 210 Reader over TCP/IP
//         TcpClient client = new TcpClient(ipAddress, port);
//         var cmdBytes = new byte[] {  0xFF, 0xCA, 0x00, 0x00, 0x00 };
//         var stringBytes = "FF CA 00 00 00";
//         // [0x6f][0x02][0x00][0x00][0x00][0x00][0x00][0x00][0x00][0x00] 
//         // cmdBytes = new byte[] { 0x6F, 0x02, 0x00, 0x00 ,0x00, 0x00 ,0x00, 0x00 ,0x00, 0x00, 0x00, 0x00  };
//         var cmdb= Encoding.ASCII.GetBytes(stringBytes);
//         NetworkStream stream = client.GetStream();

//         var responseData = SendUtli(stream, cmdBytes);

//         var uid = string.Concat(responseData).Substring(0, 16).Trim();

//         Console.WriteLine(uid);

//         // Close the TCP/IP connection
//         stream.Close();
//         client.Close();
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine("Error: " + ex.Message);
//     }
//     Thread.Sleep(100);
// }
// byte[] ReadUt(NetworkStream stream, byte block, byte keyType, byte keyNum, byte requestBytes)
// {
//     var authBytes = new byte[] { 0xFF, 0x88, 0x00, block, keyType, keyNum };
//     var cmdBytes = new byte[] { 0xFF, 0xB0, 0x00, block, requestBytes };

//     SendUtli(stream, authBytes);
//     return SendUtli(stream, cmdBytes);
// }

// byte[] SendUtli(NetworkStream stream, byte[] request)
// {
//     byte[] recivedBuffer = new byte[1024];


//     stream.Write(request, 0, request.Length);

//     var byteRead = stream.Read(recivedBuffer, 0, recivedBuffer.Length);

//     return recivedBuffer;
// }
