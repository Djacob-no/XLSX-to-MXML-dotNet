using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Controller
{
    public class Question
    {
        public string QuestionID { get; set; }
        public string Level { get; set; }
        public List<string> Tags = new List<string>();
        public string QuestionType { get; set; }
        public string QuestionText { get; set; }
        public string Resources { get; set; }
        public string CorrectAnswer { get; set; }
        public List<int> indexOfCorrectAnswers = new List<int>(); //incase of multiple response questions store the indexes here
        public double FractionPercent; // answers divided by correct answers
        public List<string> AnswerAlternatives = new List<string>();
        public bool IsValid { get; private set; }
        public string ErrorMessage { get; private set; }

        public bool Validate()
        {
            bool valid = false;
            int CorrectAnswerInt;
            int AnswerAlternativesCount;

            QuestionText = replaceSpecialChars(QuestionText);
            for (int i = 0; i < AnswerAlternatives.Count; i++)
            {
                AnswerAlternatives[i] = replaceSpecialChars(AnswerAlternatives[i]);
            }
            for (int i = 0; i < Tags.Count; i++)
            {
                Tags[i] = replaceSpecialChars(Tags[i]);
            }


            //check various things depending on question type
            switch (QuestionType.ToLower())
            {
               case "multiple choice":
                    //check that there is only 1 correct answer and its readable as an integer
                    Regex rx = new Regex(@"\W|\sg");//regex to remove whitespace and non word characters like ) and , 
                    CorrectAnswer = rx.Replace(CorrectAnswer, "");
                    try
                    {
                        CorrectAnswerInt = Convert.ToInt32(CorrectAnswer); //correct answer should be an int at this point
                        if(CorrectAnswerInt > 7 || CorrectAnswerInt == 0)
                        {
                            ErrorMessage = "Correct answer is greater than 7 or equal to 0";
                            return valid;
                        }
                        indexOfCorrectAnswers.Add(CorrectAnswerInt);
                    }
                    catch (Exception)
                    {
                        ErrorMessage = "Correct answer is not an integer";
                        return valid; //if its not an int Fail the validate
                    }

                    //Check that there are answer alternatives
                    AnswerAlternativesCount = AnswerAlternatives.Count;
                    if (CorrectAnswerInt > AnswerAlternativesCount) 
                    {
                        ErrorMessage = "correct answer is greater than number of answer alternatives";
                        return valid; 
                    }
                    break;

                case "number input":
                    //check that the correct answer is a number 
                    try
                    {
                        double.Parse(CorrectAnswer, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        ErrorMessage = "Correct answer is not a number";
                        return valid;
                    }
                    break;

                case "true / false":
                    //Check that the correct answer is either true or false
                    try
                    {
                        bool.Parse(CorrectAnswer);
                    }
                    catch (Exception)
                    {
                        ErrorMessage = "Correct Answer is not correctly formatted (true or false)";
                        return valid;
                    }
                    break;

                case "text input":
                    //check that the correct answer exist 
                    if(CorrectAnswer == null || CorrectAnswer == " ")
                    {
                        ErrorMessage = "Correct answer Missing";
                        return valid;
                    }
                    break;

                case "multiple response":
                    //check that there is multiple correct answers and its readable format
                    CorrectAnswer = Regex.Replace(CorrectAnswer, @"\s","");//regex to remove whitespace
                    string[] CorrectAnswerArray = CorrectAnswer.Split(")");
                    int LargestCorrectAnswer = 0; 
                    if(CorrectAnswerArray.Length <= 1)
                    {
                        ErrorMessage = "Too few correct answers for this question type";
                        return valid;
                    }
                    foreach (var answer in CorrectAnswerArray)
                    {
                        if(answer == "") { continue; }
                        try
                        {
                            CorrectAnswerInt = Convert.ToInt32(answer); //correct answer should be an int at this point
                            if(LargestCorrectAnswer < CorrectAnswerInt)
                            {
                                LargestCorrectAnswer = CorrectAnswerInt; //save the highest correct answer 
                            }
                            if (CorrectAnswerInt > 7 || CorrectAnswerInt == 0) //check if Correct answer alternative is higher than the supported 7 or 0
                            {
                                ErrorMessage = "Correct answer is greater than 7 or equal to 0";
                                return valid;
                            }
                            indexOfCorrectAnswers.Add(CorrectAnswerInt);
                        }
                        catch (Exception)
                        {
                            ErrorMessage = "Correct answer is not an integer";
                            return valid; //if its not an int Fail the validate
                        }
                    }

                    //Check that there are answer alternatives
                    AnswerAlternativesCount = AnswerAlternatives.Count;
                    if (AnswerAlternativesCount <= 1)
                    {
                        ErrorMessage = "Too few answer alternatives for this question type";
                        return valid;
                    }

                    //check that the correct answers refrenced exists
                    var (number, index) = CorrectAnswerArray.Select((n, i) => (n, i)).Max();
               
                    if (LargestCorrectAnswer > AnswerAlternativesCount)
                    {
                        ErrorMessage = "correct answer is greater than number of answer alternatives";
                        return valid;
                    }
                    FractionPercent = AnswerAlternatives.Count / indexOfCorrectAnswers.Count;
                    break;

                default:
                    ErrorMessage = "Unrecognized Question Type";
                    return valid;
            }

            //check the resource column
            if(Resources.Length > 1)
            {
                if (Resources.IndexOf(".pdf") == -1 && Resources.IndexOf(".jpg") == -1 && Resources.IndexOf(".png") == -1)
                {
                    ErrorMessage = "Resource column missing file extension";
                    return IsValid;
                }
               
            }
            
            IsValid = true;
            return valid;
        }

        private string replaceSpecialChars(string inputstring)
        {
            inputstring = Regex.Replace(inputstring, "&", "&amp;");
            inputstring = Regex.Replace(inputstring, "<", "&lt;");
            inputstring = Regex.Replace(inputstring, ">", "&gt;");
            inputstring = Regex.Replace(inputstring, "\"", "&quot;");
            inputstring = Regex.Replace(inputstring, "'", "&#39;");
            return inputstring;
        }



    }
}
