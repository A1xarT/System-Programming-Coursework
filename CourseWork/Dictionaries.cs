using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork
{
    public class Dictionaries
    {
       
        public Dictionary<int, string> commands = new Dictionary<int, string>(9);
        public Dictionary<int, string> registers = new Dictionary<int, string>(8);
        public Dictionary<int, string> data_directives = new Dictionary<int, string> { };
        public Dictionary<int, string> conditional_directives = new Dictionary<int, string>(3);
        public Dictionary<int, string> segments = new Dictionary<int, string>(6);
        public Dictionary<int, string> directives = new Dictionary<int, string>(3);
        public Dictionary<int, char> oneSymb = new Dictionary<int, char>(8);
        public List<List<Element>> AllElements = new List<List<Element>> { };
        public List<string> identifiactors = new List<string> { };
        public List<string> _8bitRegisters = new List<string> { "al", "ah", "bl", "bh", "cl","ch", "dl", "dh" };
        public List<string> labelmnemoniks = new List<string> { "db", "dw", "dd", "equ", "segment", "ends" };
        public List<string> labels = new List<string> { };
        public List<mnems> codes = new List<mnems> { };

        public Dictionaries()
        {
            string str = "cld,imul,inc,and,lea,cmp,mov,add,jnz"; int iterator = 1;
            string[] mas = str.Split(",", StringSplitOptions.RemoveEmptyEntries);
            foreach (string st in mas)
                commands.Add(iterator++, st);
            str = "eax,ebx,ecx,edx,esi,edi,ebp,esp,ah,al,bh,bl,ch,cl,dh,dl"; iterator = 1;
            mas = str.Split(",", StringSplitOptions.RemoveEmptyEntries);
            foreach (string st in mas)
                registers.Add(iterator++, st);
            str = "*+[]:/-,"; iterator = 1;
            foreach (char ch in str)
                oneSymb.Add(iterator++, ch);
            data_directives.Add(1, "db");
            data_directives.Add(2, "dw");
            data_directives.Add(3, "dd");
            data_directives.Add(4, "equ");
            conditional_directives.Add(1, "if");
            conditional_directives.Add(2, "else");
            conditional_directives.Add(3, "endif");
            segments.Add(1, "cs");
            segments.Add(2, "ss");
            segments.Add(3, "ds");
            segments.Add(4, "es");
            segments.Add(5, "fs");
            segments.Add(6, "gs");
            directives.Add(1, "end");
            directives.Add(2, "ends");
            directives.Add(3, "segment");
        }
        public void makemnems()
        {
            mnems mov = new mnems(2);
            mnems cld = new mnems(1);
            mnems imul = new mnems(2);
            mnems inc = new mnems(2);
            mnems lea = new mnems(2);
            mnems cmp = new mnems(2);
            mnems add = new mnems(2);
            mnems and = new mnems(2);
            mnems jnz = new mnems(2);
        }
    }

    public class mnems
    {
        public mnems(int a) { code_bytes = a; }
        public int code_bytes = 0;
    }
    public class Element
    {
        public int number;
        public string name;
        public int length;
        public string type;
        public Element(int _number, string _name, int _length, string _type)
        {
            number = _number;
            name = _name;
            length = _length;
            type = _type;
        }
    }
    public class Row
    {
        public int numberOfLexeme;
        public int m_first;
        public int qtyOfLexemes;
        public int op1_first;
        public int qtyOfOp1;
        public int op2_first;
        public int qtyOfOp2;
        public int quantity;
        public void def()
        {
            numberOfLexeme = 0;
            m_first = 0;
            qtyOfLexemes = 0;
            op1_first = 0;
            qtyOfOp1 = 0;
            op2_first = 0;
            qtyOfOp2 = 0;
            quantity = 0;
        }
    }
}
