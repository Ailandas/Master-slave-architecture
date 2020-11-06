using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_sender_Master.BackEnd
{
    public static class IPAdress
    {
        public static List<int> ID = new List<int>();
        public static List<TcpClient> Users = new List<TcpClient>();

        public static void AddNewUser(TcpClient client, int IDas)
        {
            ID.Add(IDas);
            Users.Add(client);

        }
        public static void Print()
        {
            try
            {
                for (int i = 0; i < Users.Count; i++)
                {
                    Console.WriteLine(ID[i]);
                }
            }
            catch (Exception exc)
            {

            }
        }
        public static int GetLastIDClient(int id)
        {
            int index = 0;
            for (int i = 0; i < ID.Count; i++)
            {
                if (ID[i] == id)
                {
                    index =i;
                }
            }
            return index;
        }
        public static bool CheckIfAliveByID(int id)
        {
            bool connected = false;
            for (int i = 0; i < Users.Count; i++)
            {
                if (ID[i] == id)
                {
                    if (Users[i].Connected)
                    {
                        connected = true;
                    }
                }
            }
            return connected;
        }
    }
}