using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;

namespace CourseWork
{
    class SyntaxAnalyzer : Dictionaries
    {
        public List<Element> row = new List<Element> { };
        public List<Row> Rows = new List<Row> { };
        public void Syntax()
        {
            int flag, flag1;
            foreach (List<Element> t in AllElements)
            {
                Row temp = new Row();
                flag = 0;
                flag1 = -1;
                temp.def();
                foreach (Element tn in t)
                {
                    if (tn.name[0] == ',')
                    {
                        continue;
                    }
                    if ((flag == 0) && ((conditional_directives.ContainsValue(tn.name)) || (directives.ContainsValue(tn.name)) || (data_directives.ContainsValue(tn.name)) || (commands.ContainsValue(tn.name))))
                    {
                        flag = 1;
                        temp.m_first = tn.number;
                        temp.qtyOfLexemes = 1;
                        continue;
                    }
                    else if(flag == 0)
                    {
                        temp.numberOfLexeme = 1;
                        continue;
                    }
                    if (flag == 1)
                    {
                        if (flag1 == 1)
                        {
                            if (oneSymb.ContainsValue(tn.name[0]))
                            {
                                flag1 = 0;
                                temp.qtyOfOp1 += 1;
                                continue;
                            }
                            else
                                flag = 2;
                        }
                        else
                        {
                            if (flag1 == -1) temp.op1_first = tn.number;
                            flag1 = 1;
                            temp.qtyOfOp1 += 1;
                            continue;
                        }
                    }
                    if (flag == 2)
                    {
                        flag = 3;
                        temp.op2_first = tn.number;
                        temp.qtyOfOp2 = 1;
                        continue;
                    }
                    if (flag == 3)
                    {
                        temp.qtyOfOp2++;
                        continue;
                    }
                }
                Rows.Add(temp);
            }
        }

        public void SyntaxOutput(List <Row> rows)
        {
            string line;
            StreamReader sr = new StreamReader("test.txt");
            for (int i = 0; i < AllElements.Count; i++)
            {
                line = sr.ReadLine();
                if (line == "") line = sr.ReadLine();
                Console.WriteLine("\n" + "    " + line);
                foreach (Element tet in AllElements[i])
                        Console.WriteLine("\n    " + tet.number + "\t" + tet.name + "\t" + tet.length + "\t" + tet.type);
                Console.WriteLine("  _________________________________________________________________");
                Console.WriteLine(" | Поле  |                  |                   |                  |");
                Console.WriteLine(" | міток | Поле мнемокоду   |    1й операнд     |   2й операнд     |");
                Console.WriteLine(" |(імені)|                  |                   |                  |");
                Console.WriteLine(" |_______|__________________|___________________|__________________|");
                Console.WriteLine(" |   " + rows[i].numberOfLexeme + "   |  " + rows[i].m_first + "     |    "
                    + rows[i].qtyOfLexemes + "    | " + rows[i].op1_first + "       |  " + rows[i].qtyOfOp1 +  "      |\t" + rows[i].op2_first + " | " + rows[i].qtyOfOp2 + "      |");
                Console.WriteLine(" |_______|________|_________|_________|_________|_________|________|");

            }
        }
    }
}
