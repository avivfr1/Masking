using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Masking
{
    public class MaskingEntity
    {
        private static readonly List<int> numberList = Enumerable.Range(0, 256).ToList();

        public string Mask(string textFromFile)
        {
            Dictionary<string, string> oldIPAndNewIP = new Dictionary<string, string>();
            Dictionary<string, string> oldClassAndNewClass = new Dictionary<string, string>();
            Dictionary<string, List<int>> newClassAndListOfMembers = new Dictionary<string, List<int>>();
            string newText = textFromFile;
            string ipRegex = @"\b(((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3})(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
            MatchCollection matches = Regex.Matches(textFromFile, ipRegex);

            foreach (Match matchIP in matches)
            {
                string oldIP = matchIP.Value;
                string oldClass = matchIP.Groups[1].ToString().Remove(matchIP.Groups[1].ToString().Length - 1);
                string lastOctet = matchIP.Groups[4].ToString();

                if (!oldIPAndNewIP.ContainsKey(oldIP))
                {
                    if (oldClassAndNewClass.ContainsKey(oldClass))
                    {
                        string newClass = oldClassAndNewClass[oldClass];
                        int newOctet = getOctet(newClassAndListOfMembers[newClass]);
                        newClassAndListOfMembers[newClass].Add(newOctet);
                        string newIP = oldClassAndNewClass[oldClass] + "." + newOctet.ToString();
                        oldIPAndNewIP.Add(oldIP, newIP);
                    }

                    else
                    {
                        string newClass = getNewClass(oldClassAndNewClass);
                        Random random = new Random();
                        int newOctet = random.Next(0, 256);
                        string newIP = newClass + "." + newOctet.ToString();
                        oldIPAndNewIP.Add(oldIP, newIP);
                        oldClassAndNewClass.Add(oldClass, newClass);
                        newClassAndListOfMembers.Add(newClass, new List<int>() { newOctet });
                    }
                }
            }

            foreach (KeyValuePair<string, string> kvp in oldIPAndNewIP)
            {
                newText = newText.Replace(kvp.Key, kvp.Value);
            }

            return newText;
        }

        private static int getOctet(List<int> octets)
        {
            List<int> takenOctets = octets;
            Random random = new Random();
            List<int> listOfOctets = numberList;
            int countOfOctets = listOfOctets.Count;
            int index = random.Next(0, countOfOctets);

            while (takenOctets.Contains(listOfOctets[index]))
            {
                listOfOctets.RemoveAt(index);
                countOfOctets--;
                index = random.Next(0, countOfOctets);
            }

            return listOfOctets[index];
        }

        private static string getNewClass(Dictionary<string, string> oldClassAndNewClass)
        {
            Random random = new Random();
            string newClass = "";

            while (newClass.Equals("") || oldClassAndNewClass.Values.Contains(newClass) || oldClassAndNewClass.Keys.Contains(newClass))
            {
                newClass = "";

                for (int i = 0; i < 3; i++)
                {
                    newClass += random.Next(0, 256).ToString() + ".";
                }

                newClass = newClass.Remove(newClass.Length - 1);
            }

            return newClass;
        }
    }
}
