using System;

namespace CourseWork
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexems A = new Lexems();
            SyntaxAnalyzer B = new SyntaxAnalyzer()
            {
                AllElements = A.ReadFile()
            };
            B.Syntax();
            _1st_pass_through first = new _1st_pass_through()
            {
                AllElements = B.AllElements,
                rows = B.Rows,
                identifiactors = A.identifiactors,
                segments = A.segments,
                labels = A.labels,
            };
            first.DoFirst();
            _2nd_pass_through second = new _2nd_pass_through()
            {
                AllElements = first.AllElements,
                rows = first.rows,
                identifiactors = first.identifiactors,
                segments = first.segments,
                labels = first.labels,
                error = first.error,
                toWrite = first.toWrite,
                tabletki = first.tabletki,
                ErrorstoWrite = first.ErrorstoWrite,
                stringsToIgnore = first.stringsToIgnore,
                offsets = first.offsets,
                SetInCode = first.SetInCode,
                DW_boys = first.DW_boys,
            };
            second.DoSecond();
            B.SyntaxOutput(B.Rows);
            //Console.ReadKey(true);
        }
    }
}