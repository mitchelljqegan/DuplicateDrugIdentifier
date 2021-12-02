using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateItemIdentifier
{
    class Program
    {
        private static void Main()
        {
            try
            {
                string filePath = GetFilePath();
                List<Entry> entries = GetEntries(filePath);
                List<string[]> output = FindPermutations(entries);
                SaveResults(output);
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An unexpected error occurred. Please start the program again.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Prompts user for, and gets the file path of database file.
        /// </summary>
        /// <returns>Database file path as string.</returns>
        private static string GetFilePath()
        {
            Console.WriteLine("Please drag and drop file into command window and press Enter: ");
            return Console.ReadLine().Replace("\"", string.Empty);
        }

        /// <summary>
        /// Extracts the database entries from the database file. 
        /// </summary>
        /// <param name="filePath">File path of database file as string.</param>
        /// <returns>List of database entries.</returns>
        private static List<Entry> GetEntries(string filePath)
        {
            List<Entry> entries = new List<Entry>();

            try
            {
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
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The database entries could not be retrieved. Please start the program again and ensure the correct file is provided and that it is in the correct format.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
                Environment.Exit(1);
            }

            return entries;
        }

        /// <summary>
        /// Finds permutations of database entries. Additional common words found in database entries are used to also find entries that are similar, but not exact permutations.
        /// </summary>
        /// <param name="entries">Database entries.</param>
        /// <returns>List of string arrays, which contain the matching entries' item codes, descriptions and their percentage similarity.</returns>
        private static List<string[]> FindPermutations(List<Entry> entries)
        {
            int iteration = 0;
            List<string[]> output = new List<string[]>();
            object progressLock = new object();
            string[] additionalWords = new string[]
            {
                "NEW",
                "TABLET",
                "TAB",
                "TABS",
                "BL",
                "BT",
                "BP",
                "BLS",
                "BLST",
                "BLSTR",
                "BLISTER",
                "PACK",
                "CAP",
                "CAPS",
                "CAPSULE",
                "CAPSULES"
            };

            Console.WriteLine();
            Parallel.For(0, entries.Count, i =>
            {
                List<string> entryWords = entries[i].Description.Split(' ').ToList();
                entryWords.RemoveAll(entryWord => entryWord.Equals(string.Empty));

                for (int j = i + 1; j < entries.Count; j++)
                {
                    bool found = false;
                    List<string> comparandWords;

                    comparandWords = entries[j].Description.Split(' ').ToList();
                    comparandWords.RemoveAll(comparandWord => comparandWord.Equals(string.Empty));

                    if (ScrambledEquals(entryWords, comparandWords))
                    {
                        found = true;
                    }
                    else
                    {
                        entryWords.AddRange(additionalWords);

                        if (!comparandWords.Except(entryWords).Any())
                        {
                            found = true;

                            foreach (string additionalWord in additionalWords)
                            {
                                entryWords.RemoveAll(entryWord => entryWord.Equals(additionalWord));
                            }
                        }
                        else
                        {
                            foreach (string additionalWord in additionalWords)
                            {
                                entryWords.RemoveAll(entryWord => entryWord.Equals(additionalWord));
                            }

                            comparandWords.AddRange(additionalWords);

                            if (!entryWords.Except(comparandWords).Any())
                            {
                                found = true;

                                foreach (string additionalWord in additionalWords)
                                {
                                    comparandWords.RemoveAll(comparandWord => comparandWord.Equals(additionalWord));
                                }
                            }

                            foreach (string additionalWord in additionalWords)
                            {
                                comparandWords.RemoveAll(comparandWord => comparandWord.Equals(additionalWord));
                            }
                        }
                    }

                    if (found)
                    {
                        int numMatchingWords = entryWords.Intersect(comparandWords).Count();
                        int totalWords = entryWords.Union(comparandWords).Count();

                        double similarity = (double)numMatchingWords / totalWords * 100;

                        output.Add(new string[] { entries[i].ItemCode, entries[i].Description, entries[j].ItemCode, entries[j].Description, similarity.ToString() });
                    }
                }

                lock (progressLock)
                {
                    Console.Write("\r{0:0.00}% complete...", (double)++iteration / entries.Count * 100);
                }
            });

            return output;
        }

        /// <summary>
        /// Compares two Lists for equality regardless of ordering. Adapted from https://stackoverflow.com/a/3670089.
        /// </summary>
        /// <param name="list1">The first List to be compared.</param>
        /// <param name="list2">The second List to be compared.</param>
        /// <returns>True if the Lists are equal, otherwise False.</returns>
        private static bool ScrambledEquals(List<string> list1, List<string> list2)
        {
            var cnt = new Dictionary<string, int>();
            foreach (string s in list1)
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
            foreach (string s in list2)
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

        /// <summary>
        /// Saves the permutations found to a .csv file.
        /// </summary>
        /// <param name="output">List of string arrays, which contain the matching entries' item codes, descriptions and their percentage similarity.</param>
        private static void SaveResults(List<string[]> output)
        {
            try
            {
                StringBuilder csv = new StringBuilder();

                string newLine = "ItemCode1, Description1, ItemCode2, Description2, Similarity (%)";
                csv.AppendLine(newLine);

                for (int i = 0; i < output.Count(); i++)
                {
                    newLine = string.Format("{0},{1},{2},{3},{4}", output[i][0], output[i][1], output[i][2], output[i][3], output[i][4]);
                    csv.AppendLine(newLine);
                }

                File.WriteAllText("Permutations.csv", csv.ToString());

                Console.WriteLine("\n\nResults written to file \"Permutations.csv\". Press any key to close.");
                Console.ReadKey();
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Program output could not be saved. Please start the program again and ensure a previous version of the file isn't open.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
                Environment.Exit(1);
            }
        }
    }
}
