using System;
using System.Collections.Generic;
using System.IO;

namespace DuplicateDrugIdentifier
{
    class Program
    {
        enum Index : int
        {
            ItemCode = 0,
            Description = 1,
            PrimarySupplier = 2,
            PrimarySupplierProductCode = 3,
            Barcode = 4,
            ProductGroup = 5,
            ProductGroupName = 6,
            RetailPrice = 7,
            QtyOnHand = 8,
            QtyOnOrder = 9,
            CreationDate = 10,
            Status = 11,
            LastSellPriceChange = 12
        }

        static void Main()
        {
            const char delimiter = '\t';

            List<List<string>> entries;
            string filePath;

            filePath = GetFilePath();

            entries = GetEntries(filePath, delimiter);

            foreach (List<string> entry in entries)
            {
                Console.WriteLine(entry[(int)Index.Description]);
            }

            Console.ReadLine();

            // TODO - String matching (split description into name, quantity etc.)

            // TODO - Put description into data structure

            // TODO - String matching (find drugs w/ same name & quantity)
        }

        private static string GetFilePath()
        {
            Console.WriteLine("Please drag and drop file into command window & press Enter: ");
            return Console.ReadLine().Replace("\"", string.Empty);
        }

        private static List<List<string>> GetEntries(string filePath, char delimiter)
        {
            List<List<string>> entries;

            entries = new List<List<string>>();

            using (StreamReader streamReader = new StreamReader(filePath))
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    string[] values = line.Split(delimiter);

                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = values[i].Replace("\"", "");
                    }

                    List<string> entry = new List<string>(values);

                    entries.Add(entry);
                }
            }

            return entries;
        }
    }
}
