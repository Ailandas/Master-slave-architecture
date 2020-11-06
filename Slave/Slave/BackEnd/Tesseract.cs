using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace Slave.BackEnd
{
   public static class Tesseract
    {
        public static object LockObject=new object();
        public static void getTextFromPhotos()
        {
              try
              {

               
                    
                while (true)
                {
                    var ENGLISH_LANGUAGE = "eng";
                    string Tessdata = @"C:\tessdata";
                    string Directory = getDirectory();
                    string FileDirectory = Directory + @"\bin\Debug\";
                    DirectoryInfo dinfo = new DirectoryInfo(FileDirectory);
                    
                    using (var ocrEngine = new TesseractEngine(Tessdata, ENGLISH_LANGUAGE, EngineMode.Default))
                    {

                        foreach (var i in dinfo.EnumerateFiles())
                        {
                            if (i.Extension == ".jpg")
                            {
                                Console.WriteLine("Tesseract");

                                using (var imageWithText = Pix.LoadFromFile(i.FullName))
                                {
                                    
                                    using (var page = ocrEngine.Process(imageWithText)) 
                                    {
                                        var text = page.GetText();
                                        //File.AppendAllText(DateTime.Now.Ticks.ToString()+".txt",text);
                                        
                                        string newString = $"{BackEnd.Client.ID}{i.Name}*{text}";
                                        BackEnd.Client.SendBackData(newString);


                                    }
                                }
                                File.Delete(i.FullName);

                                Console.WriteLine("end");
                            }
                        }
                    }
                }

                   
                

              }
              catch (Exception exc)
              {
                  Console.WriteLine(exc.Message);
              }
              
        }
        public static void getTextFromPhotosEach(string file)
        {
            try
            {



                
                    var ENGLISH_LANGUAGE = "eng";
                    string Tessdata = @"C:\tessdata";
                    string Directory = getDirectory();
                    string FileDirectory = Directory + @"\bin\Debug\" + file;
                    DirectoryInfo dinfo = new DirectoryInfo(FileDirectory);

                    using (var ocrEngine = new TesseractEngine(Tessdata, ENGLISH_LANGUAGE, EngineMode.Default))
                    {

                         
                                Console.WriteLine("Tesseract");

                                using (var imageWithText = Pix.LoadFromFile(FileDirectory))
                                {

                                    using (var page = ocrEngine.Process(imageWithText))
                                    {
                                        var text = page.GetText();

                                        string newString = $"{BackEnd.Client.ID}{file}*{text}";
                                        lock (LockObject)
                                        {
                                            BackEnd.Client.SendBackData(newString);
                                        }
                                        


                                    }
                                }
                                File.Delete(FileDirectory);

                                Console.WriteLine("end");
                            
                        
                    }
                




            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
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
