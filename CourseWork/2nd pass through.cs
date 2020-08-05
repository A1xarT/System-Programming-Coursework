using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Globalization;
namespace CourseWork
{
    class RowCode
    {
        public string Mnemonika;
        public string mod;
        public string reg;
        public string rm;
    }

    class RegTable
    {
        public string reg_val;
        public string _8bit_reg;
        public string _32bit_reg;
    }

    class _2nd_pass_through : _1st_pass_through
    {
        public List<string> knownLabels = new List<string> { };
        public List<string> codes = new List<string> { };
        public List<RegTable> RegisterTables = new List<RegTable> { };
        public void DoSecond()
        {
            RegisterTables.Add(new RegTable() { reg_val = "000", _8bit_reg = "al", _32bit_reg = "eax", });
            RegisterTables.Add(new RegTable() { reg_val = "001", _8bit_reg = "cl", _32bit_reg = "ecx", });
            RegisterTables.Add(new RegTable() { reg_val = "010", _8bit_reg = "dl", _32bit_reg = "edx", });
            RegisterTables.Add(new RegTable() { reg_val = "011", _8bit_reg = "bl", _32bit_reg = "ebx", });
            RegisterTables.Add(new RegTable() { reg_val = "100", _8bit_reg = "ah", _32bit_reg = "esp", });
            RegisterTables.Add(new RegTable() { reg_val = "101", _8bit_reg = "ch", _32bit_reg = "ebp", });
            RegisterTables.Add(new RegTable() { reg_val = "110", _8bit_reg = "dh", _32bit_reg = "esi", });
            RegisterTables.Add(new RegTable() { reg_val = "111", _8bit_reg = "bh", _32bit_reg = "edi", });
            string Code; string immunity = "";
            int bad_counter = 0;
            for(int i = 0; i < AllElements.Count; i++)
            {
                if (stringsToIgnore.Contains(i))
                {
                    bad_counter++; 
                    continue;
                }
                if (error[i])
                {
                    codes.Add("");
                    continue;
                }
                Code = "";
                if (AllElements[i][0].type == "User defined")      //All user defined identificators
                {
                    if((AllElements[i].Count == 2)&&(AllElements[i][1].name == ":")) 
                        knownLabels.Add(AllElements[i][0].name);
                    if (AllElements[i].Count == 3)
                    {
                        if(AllElements[i][2].type == "hexadecimal const")
                        {
                            Code = deletezeros(AllElements[i][2].name.Substring(0, AllElements[i][2].name.Length - 1));
                            if (Code == "0")
                            {
                                if (AllElements[i][1].name == "db")
                                    Code = "00";
                                if (AllElements[i][1].name == "dw")
                                    Code = "0000";
                                if (AllElements[i][1].name == "dd")
                                    Code = "00000000";
                            }
                            if(AllElements[i][1].name == "equ")
                            {
                                while ((Code.Length > 4) && (Code.Length < 8))
                                    Code = Code.Insert(0, "0");
                                while (Code.Length < 4)
                                    Code = Code.Insert(0, "0");
                            }
                            if (AllElements[i][1].name == "db")
                                while (Code.Length < 2)
                                    Code = Code.Insert(0, "0");
                            if (AllElements[i][1].name == "dw")
                                while (Code.Length < 4)
                                    Code = Code.Insert(0, "0");
                            if (AllElements[i][1].name == "dd")
                                while (Code.Length < 8)
                                    Code = Code.Insert(0, "0");
                        }
                        if(AllElements[i][2].type == "Text const")
                        {
                            byte[] str = Encoding.Default.GetBytes(AllElements[i][2].name);
                            foreach (byte b in str)
                            {
                                Code += " " + deletezeros(hex(Int32.Parse(b.ToString().TrimStart('0'))));
                            }
                            Code = Code.Remove(0, 1);
                        }
                        if (AllElements[i][1].name == "equ")
                        {
                            Code = Code.Insert(0, "=");
                        }
                    }
                }
                // INSTRUCTIONS //
                if (AllElements[i][0].name == "cld")
                    Code = "FC";

                if (AllElements[i].Count > 1)
                {
                    if(AllElements[i][0].name == "imul")
                    {
                        if(registers.ContainsValue(AllElements[i][1].name))
                        {
                            RowCode imulich = new RowCode();
                            if (_8bitRegisters.Contains(AllElements[i][1].name))
                            {
                                imulich.Mnemonika = "F6"; imulich.mod = "11"; imulich.reg = "001";
                            }
                            else
                            {
                                imulich.Mnemonika = "F7"; imulich.mod = "11"; imulich.reg = "101";
                            }
                            if (AllElements[i][1].name[1] == 'h')
                                imulich.reg = "101";
                            imulich.rm = RegTableSearch(AllElements[i][1].name);
                            Code = imulich.Mnemonika + " " + deletezeros(hex(ByteStrToInt(imulich.mod + imulich.reg + imulich.rm)));
                        }
                        else
                        {
                            RowCode imulich = new RowCode();
                            string mem1 = "", regmem_bye = "", firstBoy = "", secondBoy = "";
                            for (int k = 1; k < AllElements[i].Count; k++)
                                mem1 += AllElements[i][k].name;
                            string variba = findVar(mem1);
                            if (findType(variba) == "db")
                                imulich.Mnemonika = "F6";
                            else imulich.Mnemonika = "F7";
                            Code += imulich.Mnemonika;
                            if (!mem1.Contains('['))
                            {
                                regmem_bye = "2D";
                                Code += " " + regmem_bye;
                            }
                            else
                            {
                                regmem_bye = "AC";
                                if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "1") imulich.mod = "00";
                                if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "2") imulich.mod = "01";
                                if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "4") imulich.mod = "10";
                                if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "8") imulich.mod = "11";
                                firstBoy = mem1.Substring(mem1.IndexOf('[') + 1, 3);
                                secondBoy = mem1.Substring(mem1.IndexOf('+') + 1, 3);
                                foreach (RegTable regT in RegisterTables)
                                {
                                    if (regT._32bit_reg == firstBoy)
                                        imulich.rm = regT.reg_val;
                                    if (regT._32bit_reg == secondBoy)
                                        imulich.reg = regT.reg_val;
                                }
                                Code += " " + regmem_bye + " " + AddZeroes(deletezeros(hex(ByteStrToInt(imulich.mod + imulich.reg + imulich.rm))), 2);
                            }
                            if (findSeg(mem1) != "")
                                Code = Code.Insert(0, SegToHex(findSeg(mem1)) + ": ");
                            else
                            {
                                if (SetInCode.Contains(findVar(mem1)))
                                    Code = Code.Insert(0, "2E: ");
                                else if ((mem1.Contains('['))&&(firstBoy == "ebp") || (firstBoy == "esp") || (secondBoy == "ebp"))
                                {
                                    Code = Code.Insert(0, "3E: ");
                                }
                            }
                            Code += " " + GetOffset(findVar(mem1));
                        }
                    }
                    if(AllElements[i][0].name == "inc")
                    {
                        RowCode incich = new RowCode();
                        string mem1 = "";
                        string regmem_bye = "";
                        for (int k = 1; k < AllElements[i].Count; k++)
                            mem1 += AllElements[i][k].name;
                        //if (DW_boys.Contains(findVar(mem1)))
                        //    Code += "66| ";
                        string variba = findVar(mem1);
                        if (findType(variba) == "db")
                            incich.Mnemonika = "FE";
                        else incich.Mnemonika = "FF";
                        Code += incich.Mnemonika;
                        if (!mem1.Contains('[')) 
                        {
                            regmem_bye = "05";
                            Code += " " + regmem_bye;
                        }
                        else
                        {
                            regmem_bye = "84";
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "1") incich.mod = "00";
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "2") incich.mod = "01";
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "4") incich.mod = "10";
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "8") incich.mod = "11";
                            string firstBoy = mem1.Substring(mem1.IndexOf('[') + 1, 3);
                            string secondBoy = mem1.Substring(mem1.IndexOf('+') + 1, 3);
                            foreach (RegTable regT in RegisterTables)
                            {
                                if (regT._32bit_reg == firstBoy)
                                    incich.rm = regT.reg_val;
                                if (regT._32bit_reg == secondBoy)
                                    incich.reg = regT.reg_val;
                            }
                            if (findSeg(mem1) != "")
                                Code = SegToHex(findSeg(mem1)) + ": ";
                            else
                            {
                                if (SetInCode.Contains(findVar(mem1)))
                                    Code = "2E: ";
                                else if ((firstBoy == "ebp") || (firstBoy == "esp") || (secondBoy == "ebp"))
                                {
                                    Code = Code.Insert(0, "3E: ");
                                }
                            }
                            Code += regmem_bye + " " + AddZeroes(deletezeros(hex(ByteStrToInt(incich.mod + incich.reg + incich.rm))), 2);
                        }
                        Code += " " + GetOffset(findVar(mem1));
                    }
                    if (AllElements[i][0].name == "jnz")
                    {
                        string mnemonichka = "";
                        if(knownLabels.Contains(AllElements[i][1].name))
                        {
                            if (offsets[i - bad_counter] - HexToInt(GetOffset(AllElements[i][1].name)) < 127)
                            {

                                mnemonichka = "75 " + AddZeroes(deletezeros(hex(254 + HexToInt(GetOffset(AllElements[i][1].name)) - offsets[i - bad_counter])),2);
                            }
                            else
                            {
                                mnemonichka = "0F 85 ";
                                string dest = "FFFFFFFE";
                                int resultDest = HexToInt(dest) - offsets[i - bad_counter] + HexToInt(GetOffset(AllElements[i][1].name)) - 4;
                                mnemonichka += hex(resultDest);
                            }
                           
                        }
                        else
                        {
                            if(HexToInt(GetOffset(AllElements[i][1].name)) - offsets[i-bad_counter] < 130)
                            {
                                mnemonichka = "75 " + AddZeroes(deletezeros(hex(HexToInt(GetOffset(AllElements[i][1].name)) - offsets[i - bad_counter] - 2)), 2) + " 90 90 90 90";
                            }
                            else
                            {
                                mnemonichka = "0F 85 " + AddZeroes(deletezeros(hex(HexToInt(GetOffset(AllElements[i][1].name)) - offsets[i - bad_counter] - 6)), 8);
                            }
                        }
                        Code = mnemonichka;
                    }
                }
                if(AllElements[i].Count > 3)
                {
                    if (AllElements[i][0].name == "and")
                    {
                        RowCode Andich = new RowCode();
                        if (_8bitRegisters.Contains(AllElements[i][1].name))
                        {
                            Andich.Mnemonika = "22"; Andich.mod = "11";
                        }
                        else
                        {
                            Andich.Mnemonika = "23"; Andich.mod = "11";
                        }
                        if (AllElements[i][1].name[1] == 'h')
                            Andich.reg = "101";
                        Andich.reg = RegTableSearch(AllElements[i][1].name);
                        Andich.rm = RegTableSearch(AllElements[i][3].name);
                        Code = Andich.Mnemonika + " " + deletezeros(hex(ByteStrToInt(Andich.mod + Andich.reg + Andich.rm)));
                    }
                    if(AllElements[i][0].name == "mov")
                    {
                        RowCode muvich = new RowCode();
                        if (_8bitRegisters.Contains(AllElements[i][1].name))
                            muvich.Mnemonika = "176";
                        else
                            muvich.Mnemonika = "184";
                        int increase = 0;
                        for(int k = 0; k < RegisterTables.Count; k++)
                            if((RegisterTables[k]._8bit_reg == AllElements[i][1].name)||(RegisterTables[k]._32bit_reg == AllElements[i][1].name))
                            {
                                increase = k;
                                break;
                            }
                        string number = AllElements[i][3].name.Remove(AllElements[i][3].length - 1);
                        if (_8bitRegisters.Contains(AllElements[i][1].name))
                            number = AddZeroes(number, 2);
                        else
                            number = AddZeroes(number, 8);
                        Code = deletezeros(hex(Int32.Parse(muvich.Mnemonika) + increase)) + " " + number;
                    }
                    if(AllElements[i][0].name == "add")
                    {
                        RowCode addich = new RowCode();
                        string mem1 = "";
                        for (int k = 1; k < AllElements[i].Count; k++)
                        {
                            if (AllElements[i][k].name == ",") break;
                            mem1 += AllElements[i][k].name;
                        }
                        string imm = ""; bool flag = false; bool is_8byte = false, varIs8 = false;
                        for(int k = 2; k < AllElements[i].Count; k++)
                        {
                            if (flag) imm += AllElements[i][k].name;
                            if (AllElements[i][k].name == ",")
                                flag = true;
                        }
                        imm = imm.Remove(imm.Length - 1);
                        if (imm.Length < 3)
                            is_8byte = true;
                        if (findType(findVar(mem1)) == "db")
                            varIs8 = true;
                        if (varIs8)
                            addich.Mnemonika = "80";
                        else 
                        {
                            if (is_8byte)
                                addich.Mnemonika = "83";
                            else
                                addich.Mnemonika = "81";
                        }
                        string regmem_bye = "";
                        //else if (CheckBase(mem1))
                        //    Code = "3E: ";
                        Code += addich.Mnemonika;
                        if (!mem1.Contains('['))
                        {
                            regmem_bye = "05";
                            Code += " " + regmem_bye;
                        }
                        else
                        {
                            regmem_bye = "84";
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "1") addich.mod = "00";
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "2") addich.mod = "01";
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "4") addich.mod = "10";
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "8") addich.mod = "11";
                            string firstBoy = mem1.Substring(mem1.IndexOf('[') + 1, 3);
                            string secondBoy = mem1.Substring(mem1.IndexOf('+') + 1, 3);
                            foreach (RegTable regT in RegisterTables)
                            {
                                if (regT._32bit_reg == firstBoy)
                                    addich.rm = regT.reg_val;
                                if (regT._32bit_reg == secondBoy)
                                    addich.reg = regT.reg_val;
                            }
                            if (findSeg(mem1) != "")
                                Code = Code.Insert(0, SegToHex(findSeg(mem1)) + ": ");
                            else
                            {
                                if (SetInCode.Contains(findVar(mem1)))
                                    Code = Code.Insert(0, "2E: ");
                                else if ((mem1.Contains('[')) && (firstBoy == "ebp") || (firstBoy == "esp") || (secondBoy == "ebp"))
                                {
                                    Code = Code.Insert(0, "3E: ");
                                }
                            }
                            Code += " " + regmem_bye + " " + AddZeroes(deletezeros(hex(ByteStrToInt(addich.mod + addich.reg + addich.rm))), 2);
                        }
                        if (is_8byte) imm = AddZeroes(imm, 2);
                        else imm = AddZeroes(imm, 8);
                        Code += " " + GetOffset(findVar(mem1));
                        immunity = imm;
                            
                    }
                    if(AllElements[i][0].name == "cmp")
                    {
                        RowCode cmpshich = new RowCode();
                        string mem1 = "";
                        for (int k = 1; k < AllElements[i].Count; k++)
                        {
                            if (AllElements[i][k].name == ",") break;
                            mem1 += AllElements[i][k].name;
                        }
                        string regik = ""; bool flag = false;
                        for (int k = 2; k < AllElements[i].Count; k++)
                        {
                            if (flag) regik += AllElements[i][k].name;
                            if (AllElements[i][k].name == ",")
                                flag = true;
                        }
                        if (_8bitRegisters.Contains(regik))
                            cmpshich.Mnemonika = "38";
                        else cmpshich.Mnemonika = "39";
                        Code += cmpshich.Mnemonika;
                        if (mem1.Contains('['))
                        {
                            string sib = "10";
                            foreach(RegTable regT in RegisterTables)
                            {
                                if((regT._32bit_reg == regik)||(regT._8bit_reg == regik))
                                {
                                    sib += regT.reg_val;
                                    break;
                                }
                            }
                            sib += "100";
                            Code += " " + AddZeroes(deletezeros(hex(ByteStrToInt(sib))), 2);
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "1") cmpshich.mod = "00";
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "2") cmpshich.mod = "01";
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "4") cmpshich.mod = "10";
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "8") cmpshich.mod = "11";
                            string firstBoy = mem1.Substring(mem1.IndexOf('[') + 1, 3);
                            string secondBoy = mem1.Substring(mem1.IndexOf('+') + 1, 3);
                            if (findSeg(mem1) != "")
                                Code = Code.Insert(0, SegToHex(findSeg(mem1)) + ": ");
                            else
                            {
                                if (SetInCode.Contains(findVar(mem1)))
                                    Code = Code.Insert(0, "2E: ");
                                else if ((mem1.Contains('[')) && (firstBoy == "ebp") || (firstBoy == "esp") || (secondBoy == "ebp"))
                                {
                                    Code = Code.Insert(0, "3E: ");
                                }
                            }
                            foreach (RegTable regT in RegisterTables)
                            {
                                if (regT._32bit_reg == firstBoy)
                                    cmpshich.rm = regT.reg_val;
                                if (regT._32bit_reg == secondBoy)
                                    cmpshich.reg = regT.reg_val;
                            }
                            Code += " " + AddZeroes(deletezeros(hex(ByteStrToInt(cmpshich.mod + cmpshich.reg + cmpshich.rm))), 2);
                        }
                        else
                        {
                            cmpshich.mod = "00";
                            cmpshich.rm = "000";
                            for (int k = 0; k < RegisterTables.Count; k++)
                            {
                                if ((RegisterTables[k]._32bit_reg == regik) || (RegisterTables[k]._8bit_reg == regik))
                                {
                                    cmpshich.reg = RegisterTables[k].reg_val;
                                    break;
                                }
                            }
                            Code += " " + AddZeroes(deletezeros(hex(5 + ByteStrToInt(cmpshich.mod + cmpshich.reg + cmpshich.rm))), 2);
                        }
                        Code += " " + GetOffset(findVar(mem1));
                    }
                    if(AllElements[i][0].name == "lea")
                    {
                        RowCode leich = new RowCode();
                        leich.Mnemonika = "8D";
                        string mem1 = "", regik = AllElements[i][1].name;
                        for (int k = 3; k < AllElements[i].Count; k++)
                        {
                            if (AllElements[i][k].name == ",") break;
                            mem1 += AllElements[i][k].name;
                        }
                        if(!mem1.Contains('['))
                        {
                            for(int k = 0; k < RegisterTables.Count; k++)
                            {
                                if(RegisterTables[k]._32bit_reg == regik)
                                {
                                    leich.Mnemonika = deletezeros(hex(184 + k));
                                }
                            }
                            Code = leich.Mnemonika;
                        }
                        else
                        {
                            RowCode rm = new RowCode();
                            rm.mod = "10"; rm.rm = "100"; rm.reg = RegTableSearch(regik);
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "1") leich.mod = "00";
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "2") leich.mod = "01";
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "4") leich.mod = "10";
                            if (mem1.Substring(mem1.IndexOf('*') + 1, 1) == "8") leich.mod = "11";
                            string firstBoy = mem1.Substring(mem1.IndexOf('[') + 1, 3);
                            string secondBoy = mem1.Substring(mem1.IndexOf('+') + 1, 3);
                            foreach (RegTable regT in RegisterTables)
                            {
                                if (regT._32bit_reg == firstBoy)
                                    leich.rm = regT.reg_val;
                                if (regT._32bit_reg == secondBoy)
                                    leich.reg = regT.reg_val;
                            }
                            Code = leich.Mnemonika + " " + AddZeroes(deletezeros(hex(ByteStrToInt(rm.mod + rm.reg + rm.rm))), 2);
                            Code += " " + AddZeroes(deletezeros(hex(ByteStrToInt(leich.mod + leich.reg + leich.rm))), 2);
                        }
                        Code += " " + GetOffset(findVar(mem1));
                    }
                }

                Code = Code.ToUpper();
                if ((AllElements[i][0].name == "inc") || (AllElements[i][0].name == "lea") || (AllElements[i][0].name == "cmp")|| 
                    (AllElements[i][0].name == "add")||((AllElements[i][0].name == "imul")&&(!registers.ContainsValue(AllElements[i][1].name))))
                    Code += "r";
                if (AllElements[i][0].name == "add")
                    Code += " " + immunity;
                codes.Add(Code);
            }
            InfoOut();
        }

        public bool CheckBase(string name)
        {
            if (!name.Contains('[')) return false;
            if (name.Substring(name.IndexOf('[') + 1, 3) != "ebp")
                if (name.Substring(name.IndexOf('[') + 1, 3) != "esp")
                    return false;
            return true;
        }
        public string findType(string varName)
        {
            string type = "";
            foreach(List <Element> el in AllElements)
            {
                if (el[0].name == varName)
                    return el[1].name;
            }
            return type;
        }
        public string SegToHex(string name)
        {
            if (name == "cs") return "2E";
            if(name == "ss") return "36";
            if (name == "ds") return "3E";
            if (name == "es") return "26"; 
            if (name == "fs") return "64";
            return "65";
        }
        
        
        public string RegTableSearch(string name)
        {
            int kk = 0;
            foreach (RegTable regt in RegisterTables)
            {
                if ((regt._8bit_reg == name) || (regt._32bit_reg == name))
                    break;
                kk++;
            }
            if (kk == 8) return RegisterTables[7].reg_val;
            return RegisterTables[kk].reg_val;
        }
        public int ByteStrToInt(string str)
        {
            int Bytes = 0, multi = 1;
            for(int i = str.Length-1; i >= 0; i--)
            {
                if (str[i] == '1')
                    Bytes += multi;
                multi *= 2;
            }
            return Bytes;
        }
        public string deletezeros(string str)
        {
            while((str.Length > 1)&&(str[0] == '0'))
                str = str.Remove(0, 1);
            return str;
        }
        public string Get_Probeli(int n)
        {
            string probels = "";
            while (probels.Length != n)
                probels = probels.Insert(0," ");
            return probels;
        }
        public void InfoOut()
        {
            string pathToWrite = @"D:\Docs\CourseWork Asy\CourseWork\Listing.lst";
            using (StreamWriter sw = new StreamWriter(pathToWrite))
            {
                int rowC = 1;
                for (int q = 0; q < toWrite.Count; q++)
                {
                    if (rowC < 10) 
                    {
                        sw.Write("  " + rowC + "  ");
                        Console.Write("  " + rowC + "  ");
                    } 
                    else
                    {
                        if (rowC < 100)
                        {
                            sw.Write(" " + rowC + "  ");
                            Console.Write(" " + rowC + "  ");
                        }
                        else
                        {
                            sw.Write(rowC + "  ");
                            Console.Write(rowC + "  ");
                        }
                    }
                    sw.WriteLine(toWrite[q].Insert(9, codes[q] + Get_Probeli(25 - codes[q].Length)));
                    Console.WriteLine(toWrite[q].Insert(9, codes[q] + Get_Probeli(25 - codes[q].Length)));
                    if (ErrorstoWrite.ContainsKey(q + 1)) 
                    {
                        sw.WriteLine(ErrorstoWrite[q + 1]);
                        Console.WriteLine(ErrorstoWrite[q + 1]);
                    }
                    rowC++;
                }
                foreach (string tab in tabletki)
                    sw.WriteLine(tab);
            }
        }
    }
}
