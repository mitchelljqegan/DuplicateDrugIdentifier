﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateDrugIdentifier
{
    class Program
    {
        // Use Release configuaration for best performance
        static void Main()
        {
            List<Entry> entries;
            string filePath;
            StringBuilder csv;

            filePath = GetFilePath();
            entries = GetEntries(filePath);
            csv = new StringBuilder();

            //FindOutputClosestMatches(csv, entries);
            List<int[]> index;
            List<string[]> output;
            (index, output) =FindPermutations(entries);
            for (int i = 0; i < index.Count(); i++)
            {
                Console.WriteLine(index[i][0] + "," + output[i][0] + "," + index[i][1] + "," + output[i][1] + "," + output[i][2]);
            }
            Console.ReadLine();
        }

        private static string GetFilePath()
        {
            Console.WriteLine("Please drag and drop file into command window and press Enter: ");
            return Console.ReadLine().Replace("\"", string.Empty);
        }

        private static List<Entry> GetEntries(string filePath)
        {
            List<Entry> entries = new List<Entry>();

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

                    entries.Add(entry);
                }
            }

            return entries;
        }

        private static void FindOutputClosestMatches(StringBuilder csv, List<Entry> entries)
        {
            csv.AppendLine("ItemCode, Description, PrimarySupplier, PrimarySupplierProductCode, Barcode, ProductGroup, ProductGroupName, RetailPrice, QtyOnHand, QtyOnOrder, CreationDate, Status, LastSellPriceChange, No. Differences");

            Parallel.ForEach(entries.Cast<Entry>(), subject =>
            {
                int minDistance;
                List<Entry> comparands;
                List<Entry> matches;
                string closest;

                comparands = new List<Entry>(entries);
                comparands.Remove(subject);

                minDistance = int.MaxValue;
                closest = "";

                Parallel.ForEach(comparands.Cast<Entry>(), comparand =>
                {
                    int distance;

                    distance = LevenshteinDistance(subject.Description, comparand.Description);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closest = comparand.Description;
                    }
                });

                matches = comparands.FindAll(comparand => comparand.Description.Equals(closest));

                csv.AppendLine(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}", subject.ItemCode, subject.Description, subject.PrimarySupplier, subject.PrimarySupplierProductCode, subject.Barcode, subject.ProductGroup, subject.ProductGroupName, subject.RetailPrice, subject.QtyOnHand, subject.QtyOnOrder, subject.CreationDate, subject.Status, subject.LastSellPriceChange, minDistance));

                Parallel.ForEach(matches.Cast<Entry>(), match =>
                {
                    csv.AppendLine(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}", match.ItemCode, match.Description, match.PrimarySupplier, match.PrimarySupplierProductCode, match.Barcode, match.ProductGroup, match.ProductGroupName, match.RetailPrice, match.QtyOnHand, match.QtyOnOrder, match.CreationDate, match.Status, match.LastSellPriceChange, minDistance));
                });
            });

            File.WriteAllText("result.csv", csv.ToString()); // File output to bin\*Configuration*\result.csv
        }

        // Code adapted from https://rosettacode.org/wiki/Levenshtein_distance#C.23
        private static int LevenshteinDistance(string s, string t)
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

        private static (List<int[]>, List<string[]>) FindPermutations(List<Entry> entries)
        {
            List<string[]> output = new List<string[]>();
            List<int[]> index = new List<int[]>();

            for (int i = 0; i < entries.Count; i++)
            {
                string[] words = entries[i].Description.Split(' ');
                words = words.Where(val => val != "").ToArray();
                List<string> wordsL = words.ToList();

                for (int j = i + 1; j < entries.Count; j++)
                {
                    string[] other_words = entries[j].Description.Split(' ');
                    other_words = other_words.Where(val => val != "").ToArray();
                    List<string> other_wordsL = other_words.ToList();
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
                        if (!other_wordsL.Except(wordsL).Any()) { found = true; }
                        wordsL.Remove("NEW");
                        wordsL.Remove("TABLET");
                        wordsL.Remove("TAB");
                        wordsL.Remove("TABS");
                        wordsL.Remove("BL");
                        wordsL.Remove("BT");
                        wordsL.Remove("BP");
                        wordsL.Remove("BLS");
                        wordsL.Remove("BLST");
                        wordsL.Remove("BLSTR");
                        wordsL.Remove("BLISTER");
                        wordsL.Remove("PACK");
                        wordsL.Remove("CAP");
                        wordsL.Remove("CAPS");
                        wordsL.Remove("CAPSULE");
                        wordsL.Remove("CAPSULES");
                        other_wordsL.Add("NEW");
                        other_wordsL.Add("TABLET");
                        other_wordsL.Add("TAB");
                        other_wordsL.Add("TABS");
                        other_wordsL.Add("BL");
                        other_wordsL.Add("BT");
                        other_wordsL.Add("BP");
                        other_wordsL.Add("BLS");
                        other_wordsL.Add("BLST");
                        other_wordsL.Add("BLSTR");
                        other_wordsL.Add("BLISTER");
                        other_wordsL.Add("PACK");
                        other_wordsL.Add("CAP");
                        other_wordsL.Add("CAPS");
                        other_wordsL.Add("CAPSULE");
                        other_wordsL.Add("CAPSULES");
                        if (!wordsL.Except(other_wordsL).Any()) { found = true; }
                        other_wordsL.Remove("NEW");
                        other_wordsL.Remove("TABLET");
                        other_wordsL.Remove("TAB");
                        other_wordsL.Remove("TABS");
                        other_wordsL.Remove("BL");
                        other_wordsL.Remove("BT");
                        other_wordsL.Remove("BP");
                        other_wordsL.Remove("BLS");
                        other_wordsL.Remove("BLST");
                        other_wordsL.Remove("BLSTR");
                        other_wordsL.Remove("BLISTER");
                        other_wordsL.Remove("PACK");
                        other_wordsL.Remove("CAP");
                        other_wordsL.Remove("CAPS");
                        other_wordsL.Remove("CAPSULE");
                        other_wordsL.Remove("CAPSULES");
                        if (found)
                        {
                            IEnumerable<string> a = wordsL.Intersect(other_wordsL);
                            int b = a.Count();
                            int c = wordsL.Count();
                            int d = other_wordsL.Count();

                            double comp = (double)b/((double)(c+d)/2)*100;

                            Console.WriteLine(string.Join(" ", words) + " = " + string.Join(" ", other_words)+", %"+comp);
                            index.Add(new int[] { i, j });
                            output.Add(new string[] { string.Join(" ", entries[i].Description), string.Join(" ", entries[j].Description), comp.ToString() });
                        }
                    }
                }
            }
            return (index, output);
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
