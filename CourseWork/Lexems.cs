using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;

namespace CourseWork
{
    public class Lexems:Dictionaries
    {
        public List <List<Element>> ReadFile()
        {
            List<Element> row = new List<Element> { };
            int number;
            string line = "", lower, type = "", labelPattern = @"[\.,A-z,\?,\@,\\_,\$][\w,\?,\$,\@,\\_]+:$";
            char[] separator = { ' ', '\t', '\n' };
            Console.OutputEncoding = Encoding.UTF8;
            using (StreamReader sr = new StreamReader("test.txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "") continue;
                    if (Regex.IsMatch(line.TrimStart(' ', '\t'), labelPattern, RegexOptions.IgnoreCase))
                        labels.Add(line.TrimStart(' ', '\t').Substring(0, line.TrimStart(' ', '\t').Length - 1));
                    line = line.Replace(":", " : ");
                    line = line.Replace("+", " + ");
                    line = line.Replace(",", " , ");
                    line = line.Replace("/", " / ");
                    line = line.Replace("*", " * ");
                    line = line.Replace("[", " [ ");
                    line = line.Replace("]", " ] ");
                    String[] strlist = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    number = 1;
                    row = new List<Element> { };
                    foreach (String s in strlist)
                    {
                        lower = s;
                        type = CheckFails(lower, type);
                        if (type != "Text const")
                        {
                            lower = lower.ToLower();
                        }
                        else
                            lower = lower.Trim('"');
                        Element temp = new Element(number, lower, lower.Length, type);
                        row.Add(temp);
                        number++;
                    }
                    AllElements.Add(row);
                }
            }
            LabelCheck();
            DeleteZeroStrs();
            return AllElements;
        }

        public void DeleteZeroStrs()
        {
            for(int i = 0; i < AllElements.Count; i++)
            {
                if (AllElements[i].Count < 1)
                    AllElements.RemoveAt(i);
            }
            return;
        }
        public string  CheckFails(string line, string type)
        {
            if (line[0] == '"')
            {
                type = "Text const";
                return type;
            }
            int numb;
            string pattern = @"(\d+[\d,a-f]+h)|(\d+h)";
            if (Regex.IsMatch(line, pattern, RegexOptions.IgnoreCase))
            {
                type = "hexadecimal const"; return type;
            }
            if (commands.ContainsValue(line.ToLower()))
            {
                type = "Mnemocode identificator"; return type;
            }
            if (registers.ContainsValue(line.ToLower()))
            {
                if (line.Length == 3) type = "32-bit data register";
                else type = "8-bit data register";
                return type;
            }
            if (data_directives.ContainsValue(line.ToLower()))
            {
                type = "Data type ";
                for (numb = 1; numb <= data_directives.Count; numb++)
                {
                    if (data_directives[numb] == line.ToLower())
                        type += numb;
                }
                return type;
            }
            if (conditional_directives.ContainsValue(line.ToLower()))
            {
                type = "Conditional directive type ";
                for (numb = 1; numb <= conditional_directives.Count; numb++)
                {
                    if (conditional_directives[numb] == line)
                        type += numb;
                }
                return type;
            }
            if (segments.ContainsValue(line.ToLower()))
            {
                type = "Segment register identifier type ";
                for (numb = 1; numb <= segments.Count; numb++)
                {
                    if (segments[numb] == line)
                        type += numb;
                }
                return type;
            }
            if (directives.ContainsValue(line.ToLower()))
            {
                type = "Segment directive"; return type;
            }
            pattern = @"^\d+";
            if (Regex.IsMatch(line, pattern, RegexOptions.IgnoreCase))
            {
                type = "decimal const"; return type;
            }
            pattern = "+-*/[]:,";

            foreach (char ch in pattern)
            {
                if (line[0] == ch)
                {
                    type = "one symbol"; return type;
                }
            }
            string str = new string(line).ToLower();
            if(!identifiactors.Contains(str))
                identifiactors.Add(str);
            type = "User defined";
            return type;
        }
        public void LabelCheck()
        {
            foreach(string s in labels)
            {
                if (identifiactors.Contains(s))
                    identifiactors.Remove(s);
            }
        }
    }
}
