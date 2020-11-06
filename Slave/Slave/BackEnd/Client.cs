using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Slave.BackEnd
{

    public class Client
    {
        public static bool Executing = true; //Sustabdyti socketus kai isjungi programa
        public TcpClient tcpClient;
        public static string message = "";
        public static string ID = "";
        public Thread Th;
        public void Initialize(string ip, int port)
        {
            try
            {
                if (Executing == true)
                {
                    tcpClient = new TcpClient(ip, port);

                    if (tcpClient.Connected)
                    {
                        Console.WriteLine("Connected to: {0}:{1}", ip, port);

                    }

                }

            }
            catch (Exception ex)
            {
                if (Executing == true)
                {
                    Console.WriteLine(ex.Message);
                    Initialize(ip, port);
                }

            }
        }

        public void BeginRead()
        {
            try
            {

                if (Executing == true)
                {
                    using (var stream = tcpClient.GetStream())
                    {
                        byte[] fileNameLengthBytes = new byte[4];
                        stream.Read(fileNameLengthBytes, 0, 4);
                        int fileNameLength = BitConverter.ToInt32(fileNameLengthBytes, 0);
                        byte[] fileNameBytes = new byte[fileNameLength];
                        stream.Read(fileNameBytes, 0, fileNameLength);
                        string fileName = Encoding.ASCII.GetString(fileNameBytes, 0, fileNameLength);
                        if (fileNameLengthBytes[0] != 0)
                        {
                            using (var output = File.Create(fileName))
                            {
                                var buffer = new byte[1024];
                                int bytesRead;
                                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    output.Write(buffer, 0, bytesRead);
                                    Console.WriteLine(bytesRead.ToString());
                                    /*  if (bytesRead < 1024)
                                      {
                                          break;
                                      }*/
                                }
                            }
                            Console.WriteLine("recieved: " + fileName);
                            Console.WriteLine("id: " + ID);
                            string Direktorija = getDirectory() + @"\bin\Debug\" + fileName;
                            if (Direktorija.EndsWith(".txt"))
                            {
                                ID = File.ReadAllText(Direktorija);
                                File.Delete(Direktorija);
                            }
                               Th = new Thread(delegate ()
                              {
                                  BackEnd.Tesseract.getTextFromPhotosEach(fileName);

                              });
                              Th.Start();
                                            


                        }
                    }

                    Initialize("127.0.0.1", 2020);
                    BeginRead();
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }
        public static Socket sock;
        public static void SendBackData(string content)
        {
            if (Executing == true)
            {

                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress host = IPAddress.Parse("127.0.0.1");
                IPEndPoint hostep = new IPEndPoint(host, 2021);
               


                try
                {
                    sock.Connect(hostep);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Problem connecting to host");
                    Console.WriteLine(e.ToString());
                    sock.Close();
                    return;
                }

                try
                {
                    sock.Send(Encoding.ASCII.GetBytes(content));
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Problem sending data");
                    Console.WriteLine(e.ToString());
                    sock.Close();
                    return;
                }
                sock.Close();
            }
        }
       
      
        private static string getDirectory()
        {
            var enviroment = System.Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(enviroment).Parent.FullName;

            return projectDirectory;
        }




    }
}
