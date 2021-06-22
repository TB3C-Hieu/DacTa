using Microsoft.Win32;
using System;
using System.IO;

namespace DacTaHinhThuc
{
    class File
    {
        public string OpenFile()
        {
            string str_file = "";
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            bool? result = openFileDialog1.ShowDialog();
            if (result == true)
            {
                using (var sr = new StreamReader(openFileDialog1.FileName))
                {
                    str_file = sr.ReadToEnd();
                }
                str_file = openFileDialog1.FileName;
            }
            return str_file;
        }
        public string ReadFile(string str_file)
        {
            string str_read = "";

            try
            {
                if (str_file!=null)
                using (var sr = new StreamReader(str_file))
                {
                    str_read = sr.ReadToEnd();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return str_read;
        }

        public void SaveFile(string text)
        {

            SaveFileDialog save = new SaveFileDialog()
            {
                Title = "Save your file",
                Filter = "Text Documents (*.txt) | *.txt",
                FileName = " "
            };

            if (save.ShowDialog() == true)
            {
                StreamWriter sw = new StreamWriter(System.IO.File.Create(save.FileName));
                sw.Write(text);
                sw.Dispose();
            }
        }
    }
}

