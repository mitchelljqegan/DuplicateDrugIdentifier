using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            List<string> Descriptions;
            string filePath;

            filePath = GetFilePath();

            entries = GetEntries(filePath, delimiter);

            //Print all Descriptions
            //Descriptions = GetDescriptions(entries);
            //Descriptions.ForEach(Console.WriteLine);

            List<int[]> indexes;
            List<string[]> matches;
            (indexes,matches) = FindPermutations(entries);
            for (int i = 0; i < indexes.Count(); i++)
            {
                Console.WriteLine(indexes[i][0].ToString() + "," + matches[i][0] + "," + indexes[i][1].ToString() + "," + matches[i][1]);
            }


            Console.ReadLine();

            // TODO - String matching (split description into name, quantity etc.)

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

        private static List<string> GetDescriptions(List<List<string>> entries)
        {
            List<string> Descriptions = new List<string>();
            foreach (List<string> entry in entries)
            {
                Descriptions.Add(entry[(int)Index.Description]);
            }
            return Descriptions;
        }
        private static (List<int[]>, List<string[]>) FindPermutations(List<List<string>> entries)
        {
            List<string[]> output = new List<string[]>();
            List<int[]> index = new List<int[]>();
            List<string> Descriptions;
            Descriptions = GetDescriptions(entries);
            for (int i = 0; i < Descriptions.Count; i++)
            {
                string[] words = Descriptions[i].Split(' ');
                words = words.Where(val => val != " ").ToArray();
                List<string> wordsL = words.ToList<string>();
                for (int j = i + 1; j < Descriptions.Count; j++)
                {
                    string[] other_words = Descriptions[j].Split(' ');
                    other_words = other_words.Where(val => val != "").ToArray();
                    List<string> other_wordsL = other_words.ToList<string>();
                    if (words[0] == other_words[0])
                    {
                        bool found = false;
                        if (ScrambledEquals(wordsL, other_wordsL)){found = true;}
                        wordsL.Add("NEW");
                        wordsL.Add("TABLET");
                        wordsL.Add("TAB");
                        wordsL.Add("TABS");
                        wordsL.Add("BL");
                        wordsL.Add("BT");
                        wordsL.Add("BP");
                        wordsL.Add("BLS");
                        wordsL.Add("BLST");
                        wordsL.Add("BLSTR");
                        wordsL.Add("BLISTER");
                        wordsL.Add("PACK");
                        wordsL.Add("CAP");
                        wordsL.Add("CAPS");
                        wordsL.Add("CAPSULE");
                        wordsL.Add("CAPSULES");
                        if (!other_wordsL.Except(wordsL).Any()) {found = true;}
                        if (found)
                        {
                            Console.WriteLine(string.Join(" ", words) + " = " + string.Join(" ", other_words));
                            index.Add(new int[] {i, j});
                            output.Add(new string[]{ string.Join(" ", words), string.Join(" ", other_words) });
                        }
                    }
                }
            }
            return (index,output);
        }
        public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (T s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (T s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }
    }
}
