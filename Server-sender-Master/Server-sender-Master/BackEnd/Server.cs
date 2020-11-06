using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_sender_Master.BackEnd
{
    public static class Server
    {
        public static List<TcpClient> clients = new List<TcpClient>(); //Listas klientu
        public static TcpClient ClientForProgressTracking; //Klientas, naudojamas atsisiusti progresa is slave'u;
        private static TcpListener listener;
        private static object lockObject = new object();
        
        public static int ClientID = 0;
        public static void Start(int port, string ip)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            listener.Start();
            Console.WriteLine("Listening...");
            StartAccept();

        }
        private static void StartAccept()
        {
            listener.BeginAcceptTcpClient(HandleAsyncConnection, listener);
        }
        private static void HandleAsyncConnection(IAsyncResult res)
        {
            try
            {
                StartAccept(); //listen for new connections again
                TcpClient client = listener.EndAcceptTcpClient(res);
                lock (lockObject)
                {
                    clients.Add(client);
                    IPAdress.AddNewUser(client,ClientID);
                }

                string endpoint = client.Client.RemoteEndPoint.ToString();
                Console.WriteLine("connected to port:  " + endpoint+ "withID: "+ ClientID);
                //proceed
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

        }
        public static void BeginSend(string fileName, TcpClient tcpClient)
        {
            try
            {
                Socket socket;
                socket = tcpClient.Client;

                byte[] fileNameBytes = Encoding.ASCII.GetBytes(Path.GetFileName(fileName));
                byte[] fileNameLen = BitConverter.GetBytes(Path.GetFileName(fileName).Length);
                byte[] preBuff = new byte[4 + fileNameBytes.Length];

                fileNameLen.CopyTo(preBuff, 0);
                fileNameBytes.CopyTo(preBuff, 4);

                socket.SendFile(fileName, preBuff, null, TransmitFileOptions.UseDefaultWorkerThread);
                Console.WriteLine("sent");
                // clients.Remove(tcpClient);
                socket.Close();
                socket.Dispose();
            }
            catch(Exception exc)
            {

            }
        }
      
        public static void BeginRecieve(Delegate Progress)
        {
            try
            {
                IPAddress localAdd = IPAddress.Parse("127.0.0.1");
                TcpListener listener = new TcpListener(localAdd, 2021);
                Console.WriteLine("Listening...");
                listener.Start();
                while (true)
                {
                    //---incoming client connected---
                    TcpClient client = listener.AcceptTcpClient();

                    //---get the incoming data through a network stream---
                    NetworkStream nwStream = client.GetStream();
                    byte[] buffer = new byte[client.ReceiveBufferSize];

                    //---read incoming stream---
                    int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

                    //---convert the data received into a string---
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    string[] splitted = dataReceived.Split('*');
                    

                    lock (lockObject)
                    {
                        BackEnd.FileOperations.ToTxt("Output.txt", splitted[2]);
                        Progress.DynamicInvoke(Convert.ToInt32(splitted[0]),splitted[1]);
                        
                    }

                }
            }
            catch(Exception exc)
            {
                listener.Stop();
                
            }
           

        }
      

    }
}

