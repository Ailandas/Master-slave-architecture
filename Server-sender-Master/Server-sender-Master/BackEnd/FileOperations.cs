using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_sender_Master.BackEnd
{
    public static class FileOperations
    {
        public static string getDestinationPath()
        {
            var enviroment = System.Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(enviroment).Parent.FullName;

            return projectDirectory;
        }
        public static string[] getAllPhotos(int size, string path)
        {
            //string files = BackEnd.FileOperations.getDestinationPath() + @"\txt_images\";

            DirectoryInfo di = new DirectoryInfo(path);
            string[] array = new string[size];
            int index = 0;
            foreach (var i in di.EnumerateFiles())
            {
                array[index] = i.FullName;
                index++;
            }
            return array;
        }
        public static int getSize(string path)
        {
            //string files = BackEnd.FileOperations.getDestinationPath() + @"\txt_images\";
            DirectoryInfo di = new DirectoryInfo(path);

            int index = 0;
            foreach (var i in di.EnumerateFiles())
            {
                index++;
            }
            return index;
        }
        public static string[] getAllPhotosNames(int size, string path)
        {
           // string files = BackEnd.FileOperations.getDestinationPath() + @"\txt_images\";
            DirectoryInfo di = new DirectoryInfo(path);
            string[] array = new string[size];
            int index = 0;
            foreach (var i in di.EnumerateFiles())
            {
                array[index] = i.Name;
                index++;
            }
            return array;
        }
        public static string[] getAllPhotosFullNames(int size, string path)
        {
            //string files = BackEnd.FileOperations.getDestinationPath() + @"\txt_images\";
            DirectoryInfo di = new DirectoryInfo(path);
            string[] array = new string[size];
            int index = 0;
            foreach (var i in di.EnumerateFiles())
            {
                array[index] = i.FullName;
                index++;
            }
            return array;
        }
        public static void ToTxt(string fileName,string content)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    File.AppendAllText($"{fileName}", content);
                }
                else
                {
                    File.Create(fileName);
                    File.AppendAllText($"{fileName}", content);
                }
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }
        public static void DeleteIfExists()
        {
            if (File.Exists("Output.txt"))
            {
                File.Delete("Output.txt");
            }
        }
    }
}
