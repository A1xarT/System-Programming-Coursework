using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace CourseWork
{
    class UserForTables
    {
        public string name = "";
        public string type = "";
        public string segment = "";
        public bool isSegment = false;
        public string offset = "";
    }
    class pairs
    {
        public int segm = 0;
        public string value = "";
    }
    class _1st_pass_through : Dictionaries
    {
        public List<int> stringsToIgnore = new List<int> { };
        public List<string> tabletki;
        public List<string> toWrite = new List<string> { };
        public List<bool> error = new List<bool> { };
        public List<string> new_Identies = new List<string> { };
        public List<int> offsets = new List<int> { };
        public List<Row> rows;
        public List<string> SetInCode = new List<string> { };
        public List<string> DW_boys = new List<string> { };
        public List<string> DB_boys = new List<string> { };

        //public void getCodeOnes()
        //{
        //    foreach(List <Element> ListEl in AllElements)
        //    {
        //        if(ListEl.Count == 3)
        //    }
        //}
        public void DoFirst()
        {
            makeIgnoreList();
            GetDW();
            GetSetInCode();
            write();
            Scheme();
        }
        public void GetDW()
        {
            foreach(List <Element> ListEl in AllElements)
            {
                if (ListEl.Count > 2)
                {
                    if (ListEl[1].name == "dw")
                        DW_boys.Add(ListEl[0].name);
                    if (ListEl[1].name == "db")
                        DB_boys.Add(ListEl[0].name);
                }
            }
        }

        public void GetSetInCode()
        {
            bool CodeSeg = false;
            foreach(List <Element> ListEl in AllElements)
            {
                if(CodeSeg)
                {
                    if(ListEl.Count == 3)
                    {
                        if (data_directives.ContainsValue(ListEl[1].name))
                            SetInCode.Add(ListEl[0].name);
                    }
                }
                if (ListEl.Count == 2)
                    if (ListEl[1].name == "ends")
                        CodeSeg = true;

            }
        }
        public void write()
        {
            string cur_seg = "";
            for (int ko = 0; ko < AllElements.Count; ko++)
                error.Add(false);

            string pattern = @"^(\d+[\d,a-f]+h$)|(^\d+h$)", pattern2 = @".+";
            for (int i = 0; i < AllElements.Count; i++)
            {
                if (((AllElements[i].Count > 2) && (data_directives.ContainsValue(AllElements[i][1].name)) && (AllElements[i][2].name != null)) || (labels.Contains(AllElements[i][0].name)))
                {
                    if (new_Identies.Contains(AllElements[i][0].name))
                    {
                        error[i] = true;
                        continue;
                    }
                    else
                        new_Identies.Add(AllElements[i][0].name);
                }
                if ((AllElements[i].Count == 2) && (AllElements[i][1].name == "segment"))
                {
                    cur_seg = AllElements[i][0].name;
                }
                if ((AllElements[i].Count == 2) && (AllElements[i][1].name == "ends"))
                {
                    if (cur_seg != AllElements[i][0].name)
                    {
                        error[i] = true;
                        continue;
                    }
                }
                if (rows[i].numberOfLexeme == 1)
                {
                    if ((AllElements[i].Count > 1) && (!labelmnemoniks.Contains(AllElements[i][1].name)))
                    {
                        if (AllElements[i][1].name != ":")
                        {
                            error[i] = true;

                            continue;
                        }
                    }
                    else
                    {
                        if ((AllElements[i].Count == 3) && (data_directives.ContainsValue(AllElements[i][1].name)))
                        {
                            if ((!Regex.IsMatch(AllElements[i][2].name, pattern, RegexOptions.IgnoreCase)) || (AllElements[i][2].type != "hexadecimal const"))
                            {
                                if ((!Regex.IsMatch(AllElements[i][2].name, pattern2, RegexOptions.IgnoreCase)) || (AllElements[i][2].type != "Text const"))
                                {
                                    if ((AllElements[i][1].name != "equ") || (!new_Identies.Contains(AllElements[i][2].name)))
                                    {
                                        error[i] = true;

                                        continue;
                                    }
                                }
                            }
                        }
                    }
                    if ((!labels.Contains(AllElements[i][0].name)) && (!new_Identies.Contains(AllElements[1][0].name)))
                        new_Identies.Add(AllElements[i][0].name);
                }
                else
                {
                    if (AllElements[i][0].name == "cld")
                    {
                        if (AllElements[i].Count > 1)
                        {
                            error[i] = true;

                        }
                    }
                    if (AllElements[i][0].name == "imul")
                    {
                        if (AllElements[i].Count < 2)
                        {
                            error[i] = true;
                        }
                        else
                        {
                            if ((AllElements[i].Count == 2)&&(!registers.ContainsValue(AllElements[i][1].name))&&(!identifiactors.Contains(AllElements[i][1].name)))
                            {
                                error[i] = true;
                            }
                        }
                    }
                    if (AllElements[i][0].name == "inc")
                    {
                        if (AllElements[i].Count < 2)
                        {
                            error[i] = true;

                            continue;
                        }
                        string memich = "";
                        for (int k = 1; k < AllElements[i].Count; k++)
                            memich += AllElements[i][k].name;
                        if (!memcheck(memich))
                        {
                            error[i] = true;
                            continue;
                        }
                        if (DW_boys.Contains(findVar(memich)))
                        {
                            error[i] = true;
                            continue;
                        }
                        //if (DW_boys.Contains(findVar(memich)))
                        //{
                        //    error[i] = true;
                        //    continue;
                        //}
                    }
                    if (AllElements[i][0].name == "and")
                    {
                        if (AllElements[i].Count != 4)
                        {
                            error[i] = true;

                            continue;
                        }
                        if ((!registers.ContainsValue(AllElements[i][1].name)) || (!registers.ContainsValue(AllElements[i][3].name)))
                        {
                            error[i] = true;

                        }
                    }
                    if (AllElements[i][0].name == "mov")
                    {
                        if (AllElements[i].Count != 4)
                        {
                            error[i] = true;

                            continue;
                        }
                        if (!registers.ContainsValue(AllElements[i][1].name))
                        {
                            error[i] = true;

                        }
                        else
                        {
                            if (!Regex.IsMatch(AllElements[i][3].name, pattern, RegexOptions.IgnoreCase))
                            {
                                error[i] = true;

                            }
                        }
                    }
                    if (AllElements[i][0].name == "add")
                    {
                        if (AllElements[i].Count < 4)
                        {
                            error[i] = true;

                            continue;
                        }
                        string memich = "";
                        for (int k = 1; k < AllElements[i].Count - 2; k++)
                            memich += AllElements[i][k].name;
                        if (DW_boys.Contains(findVar(memich)))
                        {
                            error[i] = true;
                            continue;
                        }
                        if ((!memcheck(memich)) || (!Regex.IsMatch(AllElements[i][AllElements[i].Count - 1].name, pattern, RegexOptions.IgnoreCase)))
                        {
                            error[i] = true;

                            continue;
                        }
                    }
                    if (AllElements[i][0].name == "lea")
                    {
                        if (AllElements[i].Count < 4)
                        {
                            error[i] = true;
                            continue;
                        }
                        if (!registers.ContainsValue(AllElements[i][1].name))
                        {
                            error[i] = true;

                            continue;
                        }
                        string memich = "";
                        for (int k = 3; k < AllElements[i].Count; k++)
                            memich += AllElements[i][k].name;
                        if (!memcheck(memich))
                        {
                            error[i] = true;

                            continue;
                        }
                    }
                    if (AllElements[i][0].name == "cmp")
                    {
                        if (AllElements[i].Count < 4)
                        {
                            error[i] = true;

                            continue;
                        }
                        string memich = "";
                        for (int k = 1; k < AllElements[i].Count - 2; k++)
                            memich += AllElements[i][k].name;
                        if (DW_boys.Contains(findVar(memich)))
                        {
                            error[i] = true;
                            continue;
                        }
                        if ((!memcheck(memich)) || (!registers.ContainsValue(AllElements[i][AllElements[i].Count - 1].name)))
                        {
                            error[i] = true;

                            continue;
                        }
                    }
                    if (AllElements[i][0].name == "jnz")
                    {
                        if (AllElements[i].Count != 2)
                        {
                            error[i] = true;

                            continue;
                        }
                        if (!labels.Contains(AllElements[i][1].name))
                        {
                            error[i] = true;

                            continue;
                        }
                    }
                }
            }

        }
        public bool memcheck(string str)
        {
            List<string> iDNTS = checkDefined();
            if (new_Identies.Contains(str)) return true;
            string pattern = @"^\[(eax|ebx|ecx|edx|edi|esi|ebp|esp)\+(eax|ebx|ecx|edx|edi|esi|ebp)\*(1|2|4|8)\]$";
            string seg = str.Substring(0, 2);
            if (segments.ContainsValue(seg))
            {
                str = str.Substring(3);
                foreach (string s in iDNTS)
                {
                    if (str.Contains(s))
                    {
                        if (str == s) return true;
                        string str2 = str.Substring(s.Length);
                        if (Regex.IsMatch(str2, pattern, RegexOptions.IgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                foreach (string s in iDNTS)
                {
                    if (str.Contains(s))
                    {
                        if (str == s) return true;
                        string str2 = str.Substring(s.Length);
                        if (Regex.IsMatch(str2, pattern, RegexOptions.IgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public List<string> checkDefined()
        {
            List<string> defined = new List<string> { };
            for (int i = 0; i < AllElements.Count; i++)
            {
                if ((AllElements[i].Count > 2) && (!defined.Contains(AllElements[i][0].name)) && (AllElements[i][0].type == "User defined"))
                    defined.Add(AllElements[i][0].name);
            }

            return defined;
        }
        public Dictionary<int, string> ErrorstoWrite = new Dictionary<int, string> { };
        List<pairs> Lbls = new List<pairs> { };
        public List<string> AvailavleIdenties = new List<string> { };
        public void makeIgnoreList()
        {
            List<string> definedBoys = new List<string> { };
            int flagIF = 0;
            for (int i = 0; i < AllElements.Count; i++)
            {
                if ((AllElements[i].Count == 3) && (AllElements[i][0].type == "User defined") && (!definedBoys.Contains(AllElements[i][0].name)))
                    definedBoys.Add(AllElements[i][0].name);
                if ((AllElements[i].Count > 1) && (AllElements[i][0].name == "if") && ((definedBoys.Contains(AllElements[i][1].name)) || (AllElements[i][1].type == "hexadecimal const")))
                {
                    stringsToIgnore.Add(i);
                    if (AllElements[i][1].type == "hexadecimal const")
                    {
                        flagIF = 1;
                        string zerostr = "0h";
                        for (int z = 0; z < 10; z++)
                        {
                            if (AllElements[i][1].name == zerostr)
                            {
                                flagIF = 0;
                                break;
                            }
                            zerostr = zerostr.Insert(0, "0");
                        }
                    }
                    if (definedBoys.Contains(AllElements[i][1].name))
                    {
                        foreach (List<Element> le in AllElements)
                        {
                            if ((le.Count > 2) && (le[0].name == AllElements[i][1].name) && (le[1].name == "equ"))
                            {
                                if (le[2].type == "Text const")
                                {
                                    if (le[2].name.Length > 0)
                                    {
                                        flagIF = 1;
                                        break;
                                    }
                                }
                                if (le[2].type == "hexadecimal const")
                                {
                                    flagIF = 1;
                                    string zerostr = "0h";
                                    for (int z = 0; z < 10; z++)
                                    {
                                        if (le[2].name == zerostr)
                                        {
                                            flagIF = 0;
                                            break;
                                        }
                                        zerostr = zerostr.Insert(0, "0");
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (flagIF == 1)
                    {
                        int j = i;
                        while (AllElements[j][0].name != "else")
                        {
                            j++;
                        }
                        while (AllElements[j][0].name != "endif")
                        {
                            stringsToIgnore.Add(j);
                            j++;
                        }
                        stringsToIgnore.Add(j);
                    }
                    else
                    {
                        int j = i + 1;
                        while (AllElements[j][0].name != "else")
                        {
                            stringsToIgnore.Add(j);
                            j++;
                        }
                        stringsToIgnore.Add(j);
                        while (AllElements[j][0].name != "endif")
                        {
                            j++;
                        }
                        stringsToIgnore.Add(j);
                    }
                }
            }
        }
        public void Scheme()
        {
            var result = identifiactors;
            foreach (string st in labels)
                result.Add(st);
            string line, segment = "";
            int counter = 0, segflag = 0, key = 0, flagIF = 0, eraseC = 0, i = 0, rowCounter = 1;
            string s_counter = "0000";
            Console.OutputEncoding = Encoding.UTF8;
            List<string> newLabels = new List<string> { };
            using (StreamReader sr = new StreamReader("test.txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "") continue;
                    if (line.Trim(' ', '\n', '\t') == "") continue;
                    if (stringsToIgnore.Contains(i))
                    {
                        i++; continue;
                    }
                    if (error[i])
                    {
                        ErrorstoWrite.Add(rowCounter, "Line " + rowCounter + " —  Error");
                    }
                    if ((AllElements[i].Count>1)&&(AllElements[i][1].name == "segment"))
                    {
                        segment = AllElements[i][0].name;
                        UserForTables UF = new UserForTables()
                        {
                            isSegment = true,
                            name = AllElements[i][0].name,
                            offset = s_counter,
                        };
                        Identies.Add(UF);
                        result.Remove(AllElements[i][0].name);
                        if (segflag == 0)
                            segflag = 1;
                        else
                        {
                            key = 1;
                        }
                    }
                    //if (i < 9) Console.Write("  " + rowCounter + "  ");
                    //else
                    //{
                    //    if (i < 99) Console.Write(" " + rowCounter + "  ");
                    //    else Console.Write(rowCounter + "  ");
                    //}
                    counter += eraseC;
                    offsets.Add(counter);
                    eraseC = 0;
                    s_counter = hex(counter);
                    //line = line.ToLower();
                    string toRemember = "";
                    foreach (Element elem in AllElements[i])
                    {
                        if(elem.type == "Text const")
                        {
                            toRemember = elem.name;
                        }
                    }
                    if(toRemember.Length!= 0)
                    {
                        //Console.WriteLine(toRemember);
                        line = line.ToLower().Substring(0, line.Length - toRemember.Length-2);
                        toRemember = toRemember.Insert(0, "\"");
                        toRemember = toRemember.Insert(toRemember.Length, "\"");
                        line = line.Insert(line.Length, toRemember);
                    }
                    if (rows[i].numberOfLexeme == 1)
                    {
                        pairs A = new pairs() { segm = key, value = AllElements[i][0].name ,};
                        Lbls.Add(A);
                        newLabels.Add(AllElements[i][0].name);
                    }
                    if (AllElements[i].Count == 2)
                    {
                        if ((AllElements[i][1].name == "segment"))
                        {
                            eraseC = 0;
                        }
                        if (AllElements[i][1].name == "ends")
                        {
                            foreach(UserForTables uf in Identies)
                            {
                                if(uf.name == segment)
                                {
                                    uf.offset = s_counter;
                                }
                            }
                            segment = "";
                            counter = 0;
                            eraseC = 0;
                        }
                    }
                    if (AllElements[i].Count == 3)
                    {
                        
                        if (AllElements[i][1].name == "db")
                        {
                            if (AllElements[i][2].type == "Text const")
                                eraseC = AllElements[i][2].name.Length;
                            else
                                eraseC = 1;
                        }
                        if (AllElements[i][1].name == "dd")
                        {
                            eraseC = 4;
                        }
                        if (AllElements[i][1].name == "dw")
                        {
                            eraseC = 2;
                        }
                    }
                    if (AllElements[i][0].name == "cld")
                    {
                        eraseC = 1;
                    }
                    if ((AllElements[i][0].name == "jnz") && (AllElements[i].Count > 1))
                    {
                        int raznica = counter - HexToInt(GetOffset(AllElements[i][1].name));
                        if ((newLabels.Contains(AllElements[i][1].name)) && (raznica < 127))
                            eraseC = 2;
                        else
                            eraseC = 6;
                    }
                    if (AllElements[i][0].name == "mov")
                    {
                        eraseC = 5;
                    }
                    if (AllElements[i][0].name == "imul")
                    {
                        if(registers.ContainsValue(AllElements[i][1].name))
                            eraseC = 2;
                        else
                        {
                            eraseC = 6;
                            string memich = "";
                            for (int k = 1; k < AllElements[i].Count; k++)
                                memich += AllElements[i][k].name;
                            if (memich.Contains("["))
                                eraseC += 1;
                            if (segments.ContainsValue(memich.Substring(0, 2)))
                                eraseC += 1;
                            else
                            {
                                if (SetInCode.Contains(findVar(memich)))
                                    eraseC += 1;
                                else if ((memich.Contains("[")) && (memich.Contains("ebp")) || (memich.Contains("esp")))
                                    eraseC += 1;
                            }
                        }
                    }
                    if (AllElements[i][0].name == "and")
                    {
                        eraseC = 2;
                    }
                    if ((AllElements[i][0].name == "inc") && (AllElements[i].Count > 1))
                    {
                        eraseC = 6;
                        //if (DW_boys.Contains(AllElements[i][1].name))
                        //    eraseC += 1;
                        string memich = "";
                        for (int k = 1; k < AllElements[i].Count; k++)
                            memich += AllElements[i][k].name;
                        if (memich.Contains("["))
                            eraseC += 1;
                        if (segments.ContainsValue(memich.Substring(0, 2)))
                            eraseC += 1;
                        else
                        {
                            if (SetInCode.Contains(findVar(memich)))
                                eraseC += 1;
                            else if ((memich.Contains("[")) && (memich.Contains("ebp")) || (memich.Contains("esp")))
                                eraseC += 1;
                        }
                    }
                    if ((AllElements[i][0].name == "add") && (AllElements[i].Count > 3))
                    {
                        eraseC = 7;
                        string memich = "";
                        for (int k = 1; k < AllElements[i].Count - 2; k++)
                            memich += AllElements[i][k].name;
                        if (memich.Contains("["))
                            eraseC += 1;
                        if (segments.ContainsValue(memich.Substring(0, 2)))
                            eraseC += 1;
                        else
                        {
                            if (SetInCode.Contains(findVar(memich)))
                                eraseC += 1;
                            else if ((memich.Contains("[")) && (memich.Contains("ebp")) || (memich.Contains("esp")))
                                eraseC += 1;
                        }
                    }
                    if ((AllElements[i][0].name == "cmp") && (AllElements[i].Count > 3))
                    {
                        eraseC = 6;
                        string memich = "";
                        for (int k = 1; k < AllElements[i].Count - 2; k++)
                            memich += AllElements[i][k].name;
                        if (memich.Contains("["))
                            eraseC += 1;
                        if (segments.ContainsValue(memich.Substring(0, 2)))
                            eraseC += 1;
                        else
                        {
                            if (SetInCode.Contains(findVar(memich)))
                                eraseC += 1;
                            else if ((memich.Contains("[")) && (memich.Contains("ebp")) || (memich.Contains("esp")))
                                eraseC += 1;
                        }
                    }
                    if ((AllElements[i][0].name == "lea") && (AllElements[i].Count > 3))
                    {
                        eraseC = 5;
                        string memich = "";
                        for (int k = 3; k < AllElements[i].Count; k++)
                            memich += AllElements[i][k].name;
                        if (memich.Contains("["))
                            eraseC += 2;
                    }
                    toWrite.Add(s_counter + "|\t" + line);
                    if (result.Contains(AllElements[i][0].name))
                    {
                        UserForTables UF = new UserForTables()
                        {
                            isSegment = false,
                            name = AllElements[i][0].name,
                            offset = s_counter,
                            type = AllElements[i][1].type,
                            segment = segment,
                        };
                        if (labels.Contains(AllElements[i][0].name))
                            UF.type = "label";
                        Identies.Add(UF);
                    }
                    if (error[i]) eraseC = 0;
                    i++;
                    rowCounter++;
                }
            }
            string pathToWrite = @"D:\Docs\CourseWork Asy\CourseWork\Listing.lst";
            using (StreamWriter sw = new StreamWriter(pathToWrite))
            {
                for(int q = 0; q < toWrite.Count; q++)
                {
                    int qRow = q + 1;
                    if (qRow < 10) sw.Write("  " + qRow + "  ");
                    else
                    {
                        if (qRow < 100) sw.Write(" " + qRow + "  ");
                        else sw.Write(qRow + "  ");
                    }
                    sw.WriteLine(toWrite[q]);
                    if (ErrorstoWrite.ContainsKey(q+1))
                        sw.WriteLine(ErrorstoWrite[q+1]);
                }
                makeTables();
                tabletki = GetTables();
                foreach (string tab in tabletki)
                    sw.WriteLine(tab);
            }
        }
        public string findVar(string name)
        {
            string varib = "";
            while (true)
            {
                if (findSeg(name) != "")
                    name = name.Substring(3);
                else break;
            }
            if (name.Contains("["))
            {
                foreach (char ch in name)
                {
                    if (ch == '[') break;
                    varib += ch;
                }
            }
            else
                varib = name;
            return varib;
        }
        public string findSeg(string name)
        {
            string seg = "";
            if (name.Length < 4) return seg;
            if (segments.ContainsValue(name.Substring(0, 2)))
            {
                seg = name.Substring(0, 2);
                if (findSeg(name.Substring(3)) != "")
                    seg = findSeg(name.Substring(3));
            }
            return seg;
        }
        public string GetOffset(string name)
        {
            int bad_counter = 0;
            for (int i = 0; i < offsets.Count; i++)
            {
                if (stringsToIgnore.Contains(i)) bad_counter++;
                if (AllElements[i + bad_counter][0].name == name)
                    return AddZeroes(hex(offsets[i]), 8);
            }
            return "00000000";
        }
        public string AddZeroes(string name, int result_count)
        {
            while (name.Length != result_count)
                name = name.Insert(0, "0");
            return name;
        }
        public int HexToInt(string name)
        {
            name = name.ToUpper();
            int result = 0, multi = 1, index = 0;
            while (name.Length != 0)
            {
                if (Char.IsLetter(name[name.Length - 1]))
                {
                    if (name[name.Length - 1] == 'A') index = 10;
                    if (name[name.Length - 1] == 'B') index = 11;
                    if (name[name.Length - 1] == 'C') index = 12;
                    if (name[name.Length - 1] == 'D') index = 13;
                    if (name[name.Length - 1] == 'E') index = 14;
                    if (name[name.Length - 1] == 'F') index = 15;
                }
                else index = Int32.Parse(name[name.Length - 1].ToString());
                result += index * multi;
                name = name.Remove(name.Length - 1);
                multi *= 16;
            }
            return result;
        }
        public string hex(int n)
        {
            string s_counter = Convert.ToString(n, 16).ToLower();
            while (s_counter.Length < 8)
            {
                s_counter = s_counter.Insert(0, "0");
            }
            return s_counter;
        }
        public List<UserForTables> Identies = new List<UserForTables> { };
        public void makeTables()
        {
            string path = @"D:\Docs\CourseWork Asy\CourseWork\bin\Debug\netcoreapp3.1\Tables.txt";
            string defaultstr = "         ", defaultstrforstring = "              ", normalizedstr, normalizedstr2, normalizedstr3;
            //int defaultlen = 5; 
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("\n");
                sw.WriteLine("\t\t\t\t\tSegments table\n");
                sw.WriteLine("| Name of segment | Default bit depth | Current offset |");
                foreach (UserForTables uf in Identies)
                {
                    if (uf.isSegment)
                    {
                        normalizedstr = defaultstr.Substring(0,defaultstr.Length - uf.name.Length).Insert(0,uf.name);
                        sw.WriteLine("|       " +  normalizedstr + " | " + "      32 bit     " + " |     " + uf.offset + "       |");
                    }
                }
                sw.WriteLine(" ------------------------------------------------------");
                sw.WriteLine("\t\t\tSegment register table\n");
                sw.WriteLine("| \tSegment register \t|\t Assignment \t|");
                sw.WriteLine("| \t       cs		   \t|\t	code	\t|");
                sw.WriteLine("| \t       ds		   \t|\t	data1	\t|");
                sw.WriteLine("| \t       ss		   \t|\t	nothing	\t|");
                sw.WriteLine("| \t       es		   \t|\t	nothing	\t|");
                sw.WriteLine("| \t       gs		   \t|\t	nothing	\t|");
                sw.WriteLine("| \t       fs		   \t|\t	nothing	\t|");
                sw.WriteLine(" -------------------------------------------");
                sw.WriteLine("\n \t\t\t\t\tUser idindificators table\n");
                sw.WriteLine("|       Name      |    Type        |   Segment    | Offset      |");
                foreach(UserForTables uf in Identies)
                {
                    if (!uf.isSegment)
                    {
                        normalizedstr = defaultstr.Substring(0, defaultstr.Length - uf.name.Length).Insert(0, uf.name);
                        normalizedstr2 = defaultstrforstring.Substring(0, defaultstrforstring.Length - uf.type.Length).Insert(0, uf.type);
                        normalizedstr3 = defaultstr.Substring(0, defaultstr.Length - uf.segment.Length).Insert(0, uf.segment);
                        sw.WriteLine("|       " + normalizedstr + " | " + normalizedstr2 + " |   " + normalizedstr3 + "  |  " + uf.offset + "       |");
                    }
                }
            }
        }
        public List<string> GetTables()
        {
            string line;
            List<string> tables = new List<string> { };
            Console.OutputEncoding = Encoding.UTF8;
            using (StreamReader sr = new StreamReader("Tables.txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    tables.Add(line);
                }
            }
            return tables;
        }
    }

}
