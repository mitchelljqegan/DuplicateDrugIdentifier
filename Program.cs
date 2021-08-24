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
            //****Update Accordingly****//
            const char csvDelimiter = ',';
            const string drugNameColumnName = "Description";
            //*************************//

            List<List<string>> entries;
            int drugNameIndex;
            string filePath;

            filePath = GetFilePath();

            entries = filePath.Contains(".csv") ? GetCSVEntries(filePath, csvDelimiter) : GetExcelEntries(filePath);

            drugNameIndex = entries[0].FindIndex(entry => entry.Equals(drugNameColumnName));

            // TODO - String matching (split description into name, quantity etc.)

            // TODO - Put description into data structure

            // TODO - String matching (find drugs w/ same name & quantity)
        }

        private static string GetFilePath()
        {
            Console.WriteLine("Please drag and drop file into command window & press Enter: ");
            return Console.ReadLine().Replace("\"", string.Empty);
        }

        private static List<List<string>> GetCSVEntries(string filePath, char csvDelimiter)
        {
            List<List<string>> entries;

            entries = new List<List<string>>();

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

        private static List<List<string>> GetExcelEntries(string filePath)
        {
            List<List<string>> entries;
            Application excel;
            Workbook workbook;
            Worksheet worksheet;
            int numRows;
            int numColumns;

            entries = new List<List<string>>();

            excel = new Application();
            workbook = excel.Workbooks.Open(filePath);
            worksheet = workbook.ActiveSheet;

            numRows = worksheet.UsedRange.Rows.Count;
            numColumns = worksheet.UsedRange.Columns.Count;

            for (int i = 1; i <= numRows; i++)
            {
                List<string> entry = new List<string>();

                for (int j = 1; j <= numColumns; j++)
                {
                    string value;

                    value = worksheet.Cells[i, j].Value != null ? worksheet.Cells[i, j].Value.ToString() : string.Empty;

                    entry.Add(value);
                }

                entries.Add(entry);
            }

            workbook.Close();
            excel.Quit();

            return entries;
        }
    }
}
