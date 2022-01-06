using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Data;
using Controller;
using Microsoft.Win32;

namespace View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        
        public string FinalXML = "";
        public string filePath;
        public string fileName;
        public string directory;
        public List<Question> ValidQuestions = new List<Question>();
        FileSystemWatcher watcher = new FileSystemWatcher();

        private void FileDropStackPanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                fileName = System.IO.Path.GetFileName(files[0]);
                filePath = files[0];
                directory = System.IO.Path.GetDirectoryName(files[0]);
                CreateFileWatcher(files[0]); // attaches fileWatcher to this file
                inputText.Text = @$"Watching changes to {fileName}";

                HandleFile();//reads file and prints table to UI if there are invalid questions.
              
                ConvertToMXML.IsEnabled = true;
                Grid.SetRow(FileDropStackPanel, 1);
            }
        }

        public void HandleFile()
        {

         List<Question> ProblemQuestionList = new List<Question>();
         
            var fileProcessesor = new FileProcessor(filePath); //process excel file'
            ; 
            try
            {
                List<Question> questions = fileProcessesor.Process();
                //checks the is valid property and displays list of failed 
                foreach (Question question in questions)
                {
                    if (!question.IsValid)
                    {
                        ProblemQuestionList.Add(question);
                    }
                    else
                    {
                        ValidQuestions.Add(question);
                    }
                }
            }
            catch
            {
                Dispatcher.Invoke(() => {
                    successMSG.Visibility = Visibility.Visible;
                    successTXT.Text = "error cannot read file";
                });//need to invoke to make sure this happens on the ui  thread
            }
            

            
            if (ProblemQuestionList.Count > 0)
            {
                Dispatcher.Invoke(() => {
                    ProblemQuestions.Visibility = Visibility.Visible;
                    successMSG.Visibility = Visibility.Hidden;
                    ProblemQuestions.DataContext = ProblemQuestionList;
                });//need to invoke to make sure this happens on the ui  thread
            }
            else
            {
                Dispatcher.Invoke(() => {
                    ProblemQuestions.Visibility = Visibility.Hidden;
                    successMSG.Visibility = Visibility.Visible;
                });//need to invoke to make sure this happens on the ui thread
            }
        }

        private void ConvertToMXML_Click(object sender, RoutedEventArgs e)
        {
            MXMLFormatter.FormatQuestionbankToMXML(ValidQuestions, directory);
            ConvertToMXML.Content = $" QuestionBank.xml file Created at {directory}";
            ConvertToMXML.FontSize = 10;
            ConvertToMXML.IsEnabled = false;
        }

        public void CreateFileWatcher(string fullpath)
        {
            // Create a new FileSystemWatcher and set its properties.
            watcher.Path = System.IO.Path.GetDirectoryName(fullpath);
            /* Watch for changes in LastAccess and LastWrite times, and 
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Attributes | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.CreationTime | NotifyFilters.Security;

            // Only watch text files.
            //watcher.Filter = "*.xlsx"; // WHY THIS NOT WORKING System.IO.Path.GetFileName(fullpath)
         
            watcher.Filters.Add("*.xlsx");

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnChanged);
            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            System.Threading.Thread.Sleep(1000);
            HandleFile();//reads file and prints table to UI if there are invalid questions.
            Dispatcher.Invoke(() => {
                ConvertToMXML.IsEnabled = true;
                ConvertToMXML.Content = "Convert To Moodle XML";
                ConvertToMXML.FontSize = 14;
            });
            
        }

        private void inputText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
