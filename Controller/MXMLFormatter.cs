using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Controller
{
    public static class MXMLFormatter
    {
        public static void FormatQuestionbankToMXML(List<Question> inputQuestions, string SaveDirectory)
        {
            string formattedMXML = "";
            foreach (Question question in inputQuestions)
            {
                if(question.QuestionType == "Multiple Choice" || question.QuestionType == "Multiple Response") formattedMXML += FormatMultipleChoice(question);
                if (question.QuestionType == "Text Input") formattedMXML += FormatTextInput(question);
                if (question.QuestionType == "True / False") formattedMXML += FormatTrueFalse(question);
                if (question.QuestionType == "Number Input") formattedMXML += FormatNumerical(question);
            }
            CreateXMLFile(formattedMXML, SaveDirectory);
            //return formattedMXML;
        }

       static async void CreateXMLFile(string mainXMLBody, string SaveDirectory)
        {
            string xmlStart = @"<?xml version=""1.0"" encoding=""UTF-8""?><quiz>";
            string xmlEnd = @"</quiz>";
           
            await File.WriteAllTextAsync(SaveDirectory + "//Questionbank.xml", xmlStart + mainXMLBody + xmlEnd);
        }

        static string FormatMultipleChoice(Question McQuestion)//this handles both multiple choice and multiple response questions
        {
            string singleAnswer = "false";
            if (McQuestion.QuestionType == "MultipleChoice") singleAnswer = "true"; // can we do ifsstatements like this ? 

            string template = $@"
                <question type=""multichoice"">
                <name>
                    <text> { McQuestion.QuestionID} </text>
                </name>
                <questiontext format = ""html"">
                    <text><![CDATA[<p>{ McQuestion.QuestionText }</p>]]></text>
                </questiontext>
                <generalfeedback format = ""html"">
                    <text></text>
                </generalfeedback>
                <defaultgrade> 1.0000000 </defaultgrade>
                <penalty> 0.3333333 </penalty>
                <hidden> 0 </hidden>
                <idnumber>{ McQuestion.QuestionID }</idnumber>
                <single>{ singleAnswer }</single>
                <shuffleanswers>true</shuffleanswers>
                <answernumbering>abc</answernumbering>
                <correctfeedback format = ""html"">
                    <text>Your answer is correct.</text>
                </correctfeedback>
                <partiallycorrectfeedback format = ""html"">
                    <text>Your answer is partially correct.</text>
                </partiallycorrectfeedback>
                <incorrectfeedback format = ""html"">
                    <text>Your answer is incorrect.</text>
                </incorrectfeedback>
                      {formatAnswers(McQuestion.AnswerAlternatives, McQuestion.QuestionType, McQuestion.indexOfCorrectAnswers, McQuestion.FractionPercent)}
                <tags>             
                     {FormatTags(McQuestion.Tags)}
                    <tag><text>autoimported</text></tag>
                </tags>
            </question>";

            return template;

        }

        static string FormatTrueFalse(Question TFquestion)
        {

            string template = $@"
                <question type=""truefalse"">
                <name>
                    <text> { TFquestion.QuestionID} </text>
                </name>
                <questiontext format = ""html"">
                    <text><![CDATA[<p>{ TFquestion.QuestionText }</p>]]></text>
                </questiontext>
                <generalfeedback format = ""html"">
                    <text></text>
                </generalfeedback>
                <defaultgrade> 1.0000000 </defaultgrade>
                <penalty> 1.0000000 </penalty>
                <hidden> 0 </hidden>
                <idnumber>{ TFquestion.QuestionID }</idnumber>
                     {formatAnswers(TFquestion.AnswerAlternatives, TFquestion.QuestionType, TFquestion.indexOfCorrectAnswers, TFquestion.FractionPercent)}
                <tags>             
                     {FormatTags(TFquestion.Tags)}
                </tags>
            </question>";

            return template;

        }

        static string FormatTextInput(Question TxtQuestion)
        {

            string template = $@"
                <question type=""shortanswer"">
                <name>
                    <text> { TxtQuestion.QuestionID} </text>
                </name>
                <questiontext format = ""html"">
                    <text><![CDATA[<p>{ TxtQuestion.QuestionText }</p>]]></text>
                </questiontext>
                <generalfeedback format = ""html"">
                    <text></text>
                </generalfeedback>
                <defaultgrade> 1.0000000 </defaultgrade>
                <penalty> 0.3333333 </penalty>
                <hidden> 0 </hidden>
                <idnumber>{ TxtQuestion.QuestionID }</idnumber>
                <usecase>0</usecase>
                     {formatAnswers(TxtQuestion.AnswerAlternatives, TxtQuestion.QuestionType, TxtQuestion.indexOfCorrectAnswers, TxtQuestion.FractionPercent)}
                <tags>             
                     {FormatTags(TxtQuestion.Tags)}
                </tags>
            </question>";

            return template;

        }

        static string FormatNumerical(Question NumQuestion)
        {

            string template = $@"
                <question type=""numerical"">
                <name>
                    <text> { NumQuestion.QuestionID} </text>
                </name>
                <questiontext format = ""html"">
                    <text><![CDATA[<p dir=""ltr"" style=""text - align: left;"">{ NumQuestion.QuestionText }</p>]]></text>
                   </questiontext>
                <generalfeedback format = ""html"">
                    <text></text>
                </generalfeedback>
                <defaultgrade> 1.0000000 </defaultgrade>
                <penalty> 0.3333333 </penalty>
                <hidden> 0 </hidden>
                <idnumber>{ NumQuestion.QuestionID }</idnumber>
                {formatAnswers(NumQuestion.AnswerAlternatives, NumQuestion.QuestionType, NumQuestion.indexOfCorrectAnswers, NumQuestion.FractionPercent)}
                <unitgradingtype>0</unitgradingtype>
                <unitpenalty>0.1000000</unitpenalty>
                <showunits>3</showunits>
                <unitsleft>0</unitsleft>
                <tags>             
                     {FormatTags(NumQuestion.Tags)}
                </tags>
            </question>";

            return template;

        }
        static string FormatTags(List<string> Tags)
        {
            //Tags List
            string mxmlTagString = "<tag><text>Autoimported</text></tag>";
            foreach (string tag in Tags)
            {
                mxmlTagString += $@"<tag><text>{tag}</text></tag>";
            }
            return mxmlTagString;
        }

        static string formatAnswers(List<string> answerAlternatives, string questionType, List<int> indexOfCorrectAnswers, double fp)
        {
            //Create Answer List
            string mxmlAnswerString = "";
            for (int i = 0; i < answerAlternatives.Count; i++)
            {
                var answer = answerAlternatives[i];
                double fractionPercent = 0;

                //loops index of correct answers and check if this answer is coorect, then sets fraction percent thereafter
                foreach (var index in indexOfCorrectAnswers)
                {
                    if (index == i)//check if this iteration of the forloop handles a correct answer
                    {
                        //fraction from question object is either 100 or percentage depending on amount of correct answers
                        fractionPercent = fp;
                    }
                }

                if(questionType == "number")
                {
                    mxmlAnswerString += $@"<answer fraction=""{fractionPercent}
                                        ""format=""moodle_auto_format"">
                                        <text>{answer}</text>
                                        <feedback format=""html"">
                                        <text><![CDATA[<p></p>]]></text>
                                        </feedback>
                                        <tolerance> 0 </tolerance>
                                        </answer>";
                }
                else
                {
                    //concats all the answers into answersting 
                    mxmlAnswerString += $@"<answer fraction=""{fractionPercent}"" 
                                        format=""moodle_auto_format"">
                                        <text>{answer}</text>
                                        <feedback format = ""html"">
                                        <text><![CDATA[<p></p>]]></text>
                                        </feedback>
                                        </answer>";
                }
               
            }
            return mxmlAnswerString;
        }
    }
}
