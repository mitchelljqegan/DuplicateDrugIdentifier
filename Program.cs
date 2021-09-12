using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace DuplicateDrugIdentifier
{
    public class Entry
    {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string PrimarySupplier { get; set; }
        public string PrimarySupplierProductCode { get; set; }
        public string Barcode { get; set; }
        public string ProductGroup { get; set; }
        public string ProductGroupName { get; set; }
        public double? RetailPrice { get; set; }
        public double QtyOnHand { get; set; }
        public int QtyOnOrder { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Status { get; set; }
        public DateTime? LastSellPriceChange { get; set; }

        public Entry(string[] values)
        {

            ItemCode = values[0];
            Description = values[1];
            PrimarySupplier = values[2];
            PrimarySupplierProductCode = values[3];
            Barcode = values[4];
            ProductGroup = values[5];
            ProductGroupName = values[6];

            if (double.TryParse(values[7], out double retailPrice))
            {
                RetailPrice = retailPrice;
            }
            else
            {
                RetailPrice = null;
            }

            QtyOnHand = double.Parse(values[8]);
            QtyOnOrder = int.Parse(values[9]);

            if (DateTime.TryParse(values[10], out DateTime creationDate))
            {
                CreationDate = creationDate;
            }
            else
            {
                CreationDate = null;
            }

            Status = values[11];

            if (DateTime.TryParse(values[12], out DateTime lastSellPriceChange))
            {
                LastSellPriceChange = lastSellPriceChange;
            }
            else
            {
                LastSellPriceChange = null;
            }
        }
    }

    class Program
    {
        const int apiqIndex = 0;
        const int sigmaIndex = 1;

        static void Main()
        {
            List<List<Entry>> entries;
            string filePath;
            StringBuilder csv;

            filePath = GetFilePath();
            entries = GetEntries(filePath);
            csv = new StringBuilder();

            csv.AppendLine("SIGMA, APIQ, Similarity (Less = Closer)");

            foreach (Entry sigmaEntry in entries[sigmaIndex])
            {
                int minDistance = int.MaxValue;
                string closest = "";

                foreach (Entry apiqEntry in entries[apiqIndex])
                {
                    int distance = LevenshteinDistance(sigmaEntry.Description, apiqEntry.Description);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closest = apiqEntry.Description;
                    }
                }

                csv.AppendLine(string.Format("{0}, {1}, {2}", sigmaEntry.Description, closest, minDistance));
            }

            File.WriteAllText("result.csv", csv.ToString());
        }

        private static string GetFilePath()
        {
            Console.WriteLine("Please drag and drop file into command window & press Enter: ");
            return Console.ReadLine().Replace("\"", string.Empty);
        }

        private static List<List<Entry>> GetEntries(string filePath)
        {
            List<List<Entry>> entries = new List<List<Entry>>();

            for (int i = 0; i < 2; i++)
            {
                entries.Add(new List<Entry>());
            }

            using (StreamReader streamReader = new StreamReader(filePath))
            {
                streamReader.ReadLine();

                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    string[] values = line.Split('\t');

                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = values[i].Replace("\"", "");
                    }

                    Entry entry = new Entry(values);

                    if (entry.PrimarySupplier.Contains("APIQ"))
                    {
                        entries[apiqIndex].Add(entry);
                    }
                    else
                    {
                        entries[sigmaIndex].Add(entry);
                    }
                }
            }

            return entries;
        }

        static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (int i = 0; i <= n; i++)
                d[i, 0] = i;
            for (int j = 0; j <= m; j++)
                d[0, j] = j;

            for (int j = 1; j <= m; j++)
                for (int i = 1; i <= n; i++)
                    if (s[i - 1] == t[j - 1])
                        d[i, j] = d[i - 1, j - 1];  //no operation
                    else
                        d[i, j] = Math.Min(Math.Min(
                            d[i - 1, j] + 1,    //a deletion
                            d[i, j - 1] + 1),   //an insertion
                            d[i - 1, j - 1] + 1 //a substitution
                            );
            return d[n, m];
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
                        //if (ScrambledEquals(wordsL, other_wordsL)){found = true;}
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
