using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Server_sender_Master
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        System.Windows.Threading.DispatcherTimer Timer;
        public int increment = 0;
        public int ID;
        public bool Finished = false;
        public List<string> PhotosToProcess = new List<string>();
        public UserControl1(int ClientName, int ProgressBarMaxValue, string progress)
        {
            InitializeComponent();
            lblClientName.Content = ClientName;
            ID = ClientName;
            ClientProgressBar.Maximum = ProgressBarMaxValue;
            lblProgress.Content = progress;
            

            Timer = new System.Windows.Threading.DispatcherTimer(DispatcherPriority.Send);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            Timer.Tick += TimerTicker;
            Timer.Start();
            
        }
        private void TimerTicker(object sender, EventArgs e)
        {
            increment++;
            TimeSpan elapsed = TimeSpan.FromSeconds(increment);
            
            lblTimer.Content = elapsed.ToString();
        }
        public void ResetTimer()
        {

            Timer.Start();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            increment = 0;
        }
        public void StopTimer()
        {
            Timer.Stop();
        }
        public void AddFileToProcess(string file)
        {
            PhotosToProcess.Add(file);
        }
        public void RemoveFileToProcess(string file)
        {
            for(int i=0; i < PhotosToProcess.Count; i++)
            {
                string[] arr=PhotosToProcess[i].Split('\\');
                if (arr[arr.Length-1] == file)
                {

                    PhotosToProcess.Remove(PhotosToProcess[i]);
                    Console.WriteLine("removed");
                }
            }
        }
        public void PrintFileToProcess()
        {
            foreach(string i in PhotosToProcess)
            {
                Console.WriteLine("Printing"+i);
            }
        }
        public int getFilesToProcessSize()
        {
            return PhotosToProcess.Count;
        }
        public List<string> getFiles()
        {
            return PhotosToProcess;
        }
        public void ClearList()
        {
            PhotosToProcess.Clear();
        }
    }
}
