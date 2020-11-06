using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Slave
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
        Thread Threadas;
        Thread Threadas1;
        Thread Th;
        BackEnd.Client client;
        private void btnWait_Click(object sender, RoutedEventArgs e)
        {
             
                Threadas1 = new Thread(delegate ()
               {
                   client = new BackEnd.Client();
                   client.Initialize("127.0.0.1", 2020);
                   client.BeginRead();

               });
               Threadas1.Start();

               /* Th = new Thread(delegate ()
               {
                   BackEnd.Tesseract.getTextFromPhotos();

               });
               Th.Start();*/
           
         

        }
   
       
        private static string getDestinationPath()
        {
            var enviroment = System.Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(enviroment).Parent.FullName;

            return projectDirectory;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BackEnd.Client.Executing = false;

            if (Threadas!=null)
            {
                Threadas.Abort();
            }
            if (Threadas1!=null)
            {
                Threadas1.Abort();
            }
            if (Th!=null)
            {
                Th.Abort();
            }
            if (BackEnd.Client.sock != null)
            {
                BackEnd.Client.sock.Close();
                BackEnd.Client.sock.Dispose();
            }
            if (client.tcpClient != null)
            {

                client.tcpClient.Close();
                client.tcpClient.Dispose();
            }
           

        }
    }
}
