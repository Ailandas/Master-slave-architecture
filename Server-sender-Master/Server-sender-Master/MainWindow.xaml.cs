using Server_sender_Master.BackEnd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tesseract;
using System.Runtime.InteropServices;

namespace Server_sender_Master
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {

            InitializeComponent();

        }
        int SentItems = 0;

        public delegate void Send(int id, string Apdorotas);

        public List<UserControl1> userControlai = new List<UserControl1>();
        public object LockObject = new object();
        public bool end = false;
        public static List<string> NotCompleted = new List<string>();
        public static List<string> NotCompletedFullPath = new List<string>();
        public static List<string> LastCall = new List<string>();

        public static bool StopProgram = false;
        public static int TotalFiles = 0;
        public static int TotalFilesProcessed = 0;
        string[] arr;
        public int ClientCount = 0;
        public bool Triggerred = false;

        Thread Threadas1;
        Thread Threadas;
        Thread FailSafe;
        Thread MainProgress;

        public bool Started = false;
        public string Path;
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (Started == false)
            {

                if (txtPath.Text != "")
                {
                   
                    
                    Started = true;
                    Path = txtPath.Text;
                    listboxNotCompleted.Items.Clear();
                    listboxCompleted.Items.Clear();
                    Send DelegatePause = this.UpdateUI;
                    Threadas1 = new Thread(delegate ()
                   {
                       BackEnd.Server.BeginRecieve(DelegatePause);
                   });
                    Threadas1.Start();

                    Threadas = new Thread(delegate ()
                    {
                        BackEnd.Server.Start(2020, "127.0.0.1");


                    });
                    Threadas.Start();
                    int fileCount = BackEnd.FileOperations.getSize(Path);
                    TotalFiles = fileCount;
                    arr = BackEnd.FileOperations.getAllPhotosNames(fileCount, Path);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        listboxNotCompleted.Items.Add(arr[i]);
                        NotCompleted.Add(arr[i]);
                    }

                    FailSafe = new Thread(delegate ()
                   {

                       CheckIfDead();

                   });
                    FailSafe.Start();


                    MainProgress = new Thread(delegate ()
                   {

                       ExecuteMainProgress();

                   });
                    MainProgress.Start();
                }
            }

        }
        public void ExecuteMainProgress()
        {
            int OldProgress = 0;
            bool circle = true;
            while (circle == true)
            {
                if (TotalFiles != 0)
                {

                    if (OldProgress != TotalFilesProcessed)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            MainProgressBar.Maximum = TotalFiles;
                            MainProgressBar.Value = TotalFilesProcessed;
                            if (MainProgressBar.Value == MainProgressBar.Maximum)
                            {
                                circle = false;
                                

                            }
                        });
                        OldProgress++;
                    }

                }
            }
        }
      
        public void UpdateUI(int id, string Apdorotas)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {

                    listboxCompleted.Items.Add(Apdorotas);
                    listboxNotCompleted.Items.Remove(Apdorotas);
                    lblApdoroti.Content = "Apdoroti: " + listboxCompleted.Items.Count;
                    lblNeapdoroti.Content = "Neapdoroti: " + listboxNotCompleted.Items.Count;
                    for (int i = 0; i < userControlai.Count; i++)
                    {


                        if (userControlai[i].lblClientName.Content.ToString() == id.ToString())
                        {


                            TotalFilesProcessed++;

                            userControlai[i].ResetTimer();
                            userControlai[i].ClientProgressBar.Value = userControlai[i].ClientProgressBar.Value + 1;
                            for (int j = 0; j < NotCompleted.Count; j++)
                            {
                                if (NotCompleted[j].Contains(Apdorotas))
                                {
                                    NotCompleted.RemoveAt(j);
                                }
                            }



                            if (userControlai[i].ClientProgressBar.Value == userControlai[i].ClientProgressBar.Maximum)
                            {
                                if (end == true && listboxNotCompleted.Items.Count > 0 && LastCall.Count > 0)
                                {
                                   if(Size<LastCall.Count)
                                    {
                                        if (listboxNotCompleted.Items.Count > 5)
                                        {
                                            Thread Threadas12 = new Thread(delegate ()
                                            {
                                                SendExtra(id);
                                            });
                                            Threadas12.Start();

                                        }
                                        else
                                        {

                                            Thread Threadas13 = new Thread(delegate ()
                                            {
                                                SendLast(id);
                                            });
                                            Threadas13.Start();

                                        }
                                    }
                                   
                                        
                                    

                                }

                                int idas = Convert.ToInt32(userControlai[i].lblClientName.Content);
                                bool alive = BackEnd.IPAdress.CheckIfAliveByID(idas);
                                while (alive == false)
                                {
                                    Console.WriteLine("Dead");
                                    alive = BackEnd.IPAdress.CheckIfAliveByID(idas);
                                }


                                int newID = BackEnd.IPAdress.GetLastIDClient(idas);
                                TcpClient tcpKlientas = BackEnd.IPAdress.Users[newID];
                                //System.Windows.MessageBox.Show(newID.ToString());
                                userControlai[i].ClientProgressBar.Value = 0;
                                string content = userControlai[i].lblProgress.Content.ToString();
                                int cont = Convert.ToInt32(content);
                                cont++;
                                string newContent = cont.ToString();
                                userControlai[i].lblProgress.Content = newContent;
                                int ClientCount = BackEnd.Server.clients.Count;

                                if (SentItems < TotalFiles || listboxNotCompleted.Items.Count > 0)
                                {
                                    if (SentItems < TotalFiles)
                                    {
                                        Console.WriteLine("SentItems: " + SentItems);
                                        Console.WriteLine("TotalFiles: " + TotalFiles);
                                        lock (LockObject)
                                        {
                                            Thread Threadas1 = new Thread(delegate ()
                                            {

                                                SendFiveMore(tcpKlientas, idas);

                                            });
                                            Threadas1.Start();

                                        }
                                    }
                                    else
                                    {
                                        userControlai[i].Finished = true;
                                        userControlai[i].StopTimer();
                                    }


                                }




                            }
                            if (listboxNotCompleted.Items.Count <= 0)
                            {
                                userControlai[i].StopTimer();
                                userControlai[i].lblTimer.Content = "Finished";
                                userControlai[i].Finished = true;
                                StopProgram = true;
                            }
                            if (end == false)
                            {
                                lock (LockObject)
                                {
                                    userControlai[i].RemoveFileToProcess(Apdorotas);
                                }
                            }
                            

                        }
                    }
                });
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }




        private void btnSendTo1_Click(object sender, RoutedEventArgs e)
        {
            if (txtPath.Text != "")
            {
                if (Started == true)
                {
                    int clientCount = BackEnd.Server.clients.Count;
                    ClientCount = clientCount;
                    int Progress = 0;
                    for (int i=0; i<clientCount; i++)
                    {
                        UserControl1 userControl = new UserControl1(i, 5, Progress.ToString());
                        userControlai.Add(userControl);
                        StackPanelClients.Children.Add(userControl);
                    }
                    
                    Thread Threadas1 = new Thread(delegate ()
                    {

                        StartProcess();

                    });
                    Threadas1.Start();
                }
                else
                {
                    System.Windows.MessageBox.Show("Paleiskite serveri");
                }
            }

        }
        private void StartProcess()
        {
            string destination = BackEnd.FileOperations.getDestinationPath();

            int clientCount = BackEnd.Server.clients.Count;
            ClientCount = clientCount;
            int fileCount = BackEnd.FileOperations.getSize(Path);
            string[] arr = BackEnd.FileOperations.getAllPhotos(fileCount, Path);
            List<TcpClient> clients = BackEnd.Server.clients;


            string FileDestination = destination + @"\bin\Debug\";
            for (int i = 0; i < clientCount; i++)
            {
                lock (LockObject)
                {
                    string DateAndTime = DateTime.Now.Ticks.ToString();
                    destination = DateAndTime + ".txt";
                    File.WriteAllText(destination, i.ToString() + "*");
                    int Progress = 0;
                    //BackEnd.IPAdress.AddNewUser(clients[i]);
                   
                    //System.Windows.MessageBox.Show("iteracija: " + i + "clientCount: " + clientCount);
                    send(arr, SentItems, SentItems + 5, clients[i], destination, i);
                    SentItems = SentItems + 5;
                    while (finished == false)
                    {

                    }
                }
            }
        }
      
        bool finished = false;
        public void send(string[] arr, int start, int end, TcpClient tcpClient, string FileWithID, int ID)
        {
            BackEnd.Server.ClientID = ID;
            finished = false;
            Thread Threadas = new Thread(delegate ()
            {
                int StartCountas = BackEnd.Server.clients.Count;
                BackEnd.Server.BeginSend(FileWithID, tcpClient);
                StartCountas++;
                while (BackEnd.Server.clients.Count != StartCountas)
                {
                    if (BackEnd.Server.clients.Count == StartCountas)
                    {
                        break;

                    }
                }
                File.Delete(FileWithID);

                for (int i = start; i < end; i++)
                {


                    for (int j = 0; j < userControlai.Count; j++)
                    {
                        if (userControlai[j].ID == ID)
                        {
                            userControlai[j].AddFileToProcess(arr[i]);
                        }
                    }
                    int StartCount = BackEnd.Server.clients.Count;
                    BackEnd.Server.BeginSend(arr[i], BackEnd.Server.clients[StartCount - 1]);

                    StartCount++;
                    while (BackEnd.Server.clients.Count != StartCount)
                    {
                        if (BackEnd.Server.clients.Count == StartCount)
                        {
                            break;
                        }
                    }


                }
                finished = true;

            });
            Threadas.Start();
        }





        private void SendFiveMore(TcpClient klientas, int ID)
        {
            if (klientas.Connected)
            {
                int fileCount = BackEnd.FileOperations.getSize(Path);
                string[] arr = BackEnd.FileOperations.getAllPhotos(fileCount, Path);

                for (int i = SentItems; i < SentItems + 5; i++)
                {
                    BackEnd.Server.ClientID = ID;
                    if (i == SentItems)
                    {
                        for (int j = 0; j < userControlai.Count; j++)
                        {
                            if (userControlai[j].ID == ID)
                            {
                                userControlai[j].AddFileToProcess(arr[i]);
                            }
                        }

                        BackEnd.Server.BeginSend(arr[i], klientas);
                        bool isAlive = BackEnd.IPAdress.CheckIfAliveByID(ID);
                        while (isAlive == false)
                        {
                            //Console.WriteLine("Dead 1");
                            isAlive = BackEnd.IPAdress.CheckIfAliveByID(ID);
                        }

                    }
                    else
                    {
                        for (int j = 0; j < userControlai.Count; j++)
                        {
                            if (userControlai[j].ID == ID)
                            {
                                userControlai[j].AddFileToProcess(arr[i]);
                            }
                        }
                        bool isAlive = BackEnd.IPAdress.CheckIfAliveByID(ID);
                        while (isAlive == false)
                        {
                            //Console.WriteLine("Dead 2");
                            isAlive = BackEnd.IPAdress.CheckIfAliveByID(ID);
                        }
                        int newID = BackEnd.IPAdress.GetLastIDClient(ID);
                        TcpClient client = BackEnd.IPAdress.Users[newID];
                        BackEnd.Server.BeginSend(arr[i], client);


                    }

                }
            }
            


            SentItems = SentItems + 5;
        }
        int LeftFilesToSend = 0;
        private void CheckIfDead()
        {
            bool Round = true;
            while (Round == true)
            {
                int End = 0;

                this.Dispatcher.Invoke(() =>
                {
                    if (listboxNotCompleted.Items.Count > 0)
                    {

                        for (int i = 0; i < userControlai.Count; i++)
                        {
                            if (userControlai[i].increment > 3600)
                            {
                                userControlai[i].ClientProgressBar.Value = 0;
                                userControlai[i].lblTimer.Content = "Dead";
                                userControlai[i].StopTimer();
                                userControlai[i].Finished = true;
                                LeftFilesToSend = userControlai[i].getFilesToProcessSize();
                            }



                        }

                    }
                });
                for (int i = 0; i < userControlai.Count; i++)
                {
                    if (userControlai[i].Finished == true)
                    {
                        End++;

                        if (End == ClientCount)
                        {
                            if (end == false)
                            {


                                FillLeftOvers();

                            }

                        }

                    }
                }
                if (StopProgram == true)
                {
                    Round = false;
                }


            }
        }

        private void FillLeftOvers()
        {
            end = true;
            Console.WriteLine("Triggered");
            for (int i = 0; i < userControlai.Count; i++)
            {
                if (userControlai[i].getFilesToProcessSize() > 0)
                {
                    List<string> tempList = userControlai[i].getFiles();
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        LastCall.Add(tempList[j]);
                    }

                }
            }
            List<int> NotDeadIDS = new List<int>();
            foreach(string a in LastCall)
            {
                Console.WriteLine(a);
            }
            this.Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < userControlai.Count; i++)
                {
                    if (userControlai[i].lblTimer.Content.ToString() != "Dead")
                    {
                        NotDeadIDS.Add(userControlai[i].ID);
                    }
                }
            });
            if (LastCall.Count < 6)
            {
                
                SendLast(NotDeadIDS[0]);
            }
            else if (LastCall.Count*5 > NotDeadIDS.Count )
            {
                System.Windows.MessageBox.Show("1");

                for(int i=0; i<NotDeadIDS.Count; i++)
                {
                    lock (LockObject)
                    {
                        SendExtra(NotDeadIDS[i]);
                    }
                    
                }
            }
            else
            {
                System.Windows.MessageBox.Show("2");
                int count = 0;
               int RemainingFileCount = LastCall.Count;
                int Times = RemainingFileCount % 5;
                if (NotDeadIDS.Count <= Times)
                {
                    for(int i=0; i<NotDeadIDS.Count; i++)
                    {
                        lock (LockObject)
                        {
                            System.Windows.MessageBox.Show("3");
                            SendExtra(NotDeadIDS[i]);
                        }
                    }
                }
                else if (NotDeadIDS.Count >= Times)
                {
                    System.Windows.MessageBox.Show("4");
                    for (int i=0; i<Times; i++)
                    {
                        SendExtra(NotDeadIDS[i]);
                    }
                    SendLast(NotDeadIDS[Times + 1]);
                }
            }

        }
        public static int Size = 0;
        private void SendExtra(int ID)
        {

            int Saizas = LastCall.Count;
            
            
            if (Saizas - Size > 5)
            {

                for (int j = Size; j < Size + 5; j++)
                {
                    BackEnd.Server.ClientID = ID;
                    bool isAlive = BackEnd.IPAdress.CheckIfAliveByID(ID);
                    while (isAlive == false)
                    {

                        isAlive = BackEnd.IPAdress.CheckIfAliveByID(ID);
                    }
                    int newID = BackEnd.IPAdress.GetLastIDClient(ID);
                    TcpClient client = BackEnd.IPAdress.Users[newID];
                    BackEnd.Server.BeginSend(LastCall[j], client);
                }
                Size = Size + 5;



            }
            
        }
        private void SendLast(int ID)
        {
            lock (LockObject)
            {
                int Saizas = LastCall.Count;
                int newSize = Saizas - Size;

                for (int j = Size; j < LastCall.Count; j++)
                {
                    if (Threadas.IsAlive == true)
                    {
                        Console.WriteLine("ALIVE");
                    }

                    bool isAlive = BackEnd.IPAdress.CheckIfAliveByID(ID);
                    while (isAlive == false)
                    {

                        isAlive = BackEnd.IPAdress.CheckIfAliveByID(ID);
                    }
                    int newID = BackEnd.IPAdress.GetLastIDClient(ID);
                    TcpClient client = BackEnd.IPAdress.Users[newID];
                    BackEnd.Server.BeginSend(LastCall[j], client);
                    BackEnd.Server.ClientID = ID;
                }
                Size = Size + newSize;
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog Dialog = new FolderBrowserDialog();
            DialogResult result = Dialog.ShowDialog();
            if (result.ToString() == "OK")
            {
                txtPath.Text = Dialog.SelectedPath;

            }
        }
    }

}
