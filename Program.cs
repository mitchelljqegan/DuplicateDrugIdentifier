using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;

namespace DuplicateDrugIdentifier
{
    class Program
    {
        static void Main()
        {
            char csvDelimiter = '\'';
            string filePath;

            filePath = GetFilePath();

            if (filePath.EndsWith(".csv"))
            {
                List<List<string>> csvEntries;

                csvEntries = GetCSVEntries(filePath, csvDelimiter);
            }
            else
            {
                Application excel;
                Workbook workbook;
                Worksheet worksheet;

                excel = new Application();
                workbook = excel.Workbooks.Open(filePath);
                worksheet = workbook.ActiveSheet();

                // Excel scraper
            }

            // String matching etc.
        }

        private static string GetFilePath()
        {
            Console.WriteLine("Please drag and drop file into command window & press Enter: ");
            return Console.ReadLine();
        }

        private static List<List<string>> GetCSVEntries(string filePath, char csvDelimiter)
        {
            List<List<string>> entries = new List<List<string>>();

            using (StreamReader streamReader = new StreamReader(filePath))
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    string[] values = line.Split(csvDelimiter);
                    List<string> entry = new List<string>(values);

                    entries.Add(entry);
                }
            }

            return entries;
        }
    }
}
