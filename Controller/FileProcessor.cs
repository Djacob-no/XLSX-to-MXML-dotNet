using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.IO;
using System.Collections.Generic;

namespace Controller
{
    public class FileProcessor
    {

        public string path { get; }
        public FileProcessor (string inputFilePath )
        {
            path = inputFilePath;
        }
        public List<Question> Process()
        {
            Console.WriteLine($"Begin Processing of {path}");

            //check if file exist 
            if (!File.Exists(path))
            {
                throw new ArgumentException("Cannot find file");
            }
           

            var xlApp = new Excel.Application();
            var xlWorkBook = xlApp.Workbooks.Open(@path, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            var xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            //var range = (Excel.Range)xlWorkSheet.Cells[10, 10];
            var range = xlWorkSheet.UsedRange;
            int rw = range.Rows.Count;
            int cl = range.Columns.Count;

            string cell = (string)range.Cells[3, 3].Value;
            //string QuestionID = (string)range.Cells[1, 0].Value;

            var Questions = new List<Question>();
            //for each row, create a question object from this row
            for (int i = 2; i < rw; i++)//skip 1st row
            {
                Question question = new Question
                {
                    QuestionID = range.Cells[i, 1].Value?.ToString() ?? "",
                    Level = (string)range.Cells[i, 2].Value?.ToString() ?? "",
                    QuestionType = (string)range.Cells[i, 5].Value?.ToString() ?? "",
                    QuestionText = (string)range.Cells[i, 6].Value?.ToString() ?? "",
                    Resources = (string)range.Cells[i, 7].Value?.ToString() ?? "",
                    CorrectAnswer = (string)range.Cells[i, 8].Value?.ToString() ?? "",
                };
          

                //consider modifing template to allow tags in one column separated by comma
                question.Tags.Add(range.Cells[i, 3].Value);//tag 1 
                question.Tags.Add(range.Cells[i, 4].Value);//tag 2 Only two tags are supported with this template
                
                //for each answer alternative column in this row. Answers start from column 9
                for (int j = 9; j < cl; j++)
                {
                    if(range.Cells[i, j].Value != null)
                    {
                        question.AnswerAlternatives.Add(range.Cells[i, j].Value);
                    }
                }

                question.Validate(); //validate if this question has any known issues
                Questions.Add(question);
            }



            return Questions;



        }
    }
}
