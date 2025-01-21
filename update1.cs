using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyAssistant
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void processJSONAnalysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessAndDisplayJsonFromTextBox();
        }

        private void fastProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = Clipboard.GetText();
                ProcessAndDisplayJsonFromClipboard();
                Clipboard.SetText(textBox2.Text);
                toolStripStatusLabel1.Text = "Content Copied to Clipboard.";
            }
            catch (Exception)
            {
                HandleJsonError();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.V))
            {
                try
                {
                    textBox1.Text = Clipboard.GetText();
                    ProcessAndDisplayJsonFromClipboard();
                    Clipboard.SetText(textBox2.Text);
                    toolStripStatusLabel1.Text = "Content Copied to Clipboard.";
                }
                catch (Exception)
                {
                    HandleJsonError();
                }
            }
        }

        private void ProcessAndDisplayJsonFromTextBox()
        {
            try
            {
                string inputJson = textBox1.Text;
                string formattedJson = FormatJson(inputJson);
                string result = AnalyzeJson(inputJson);
                textBox1.Text = formattedJson;
                textBox2.Text = result;
            }
            catch (Exception)
            {
                toolStripStatusLabel1.Text = "Invalid Json";
            }
        }

        private void ProcessAndDisplayJsonFromClipboard()
        {
            try
            {
                string inputJson = Clipboard.GetText();
                string formattedJson = FormatJson(inputJson);
                string result = AnalyzeJson(inputJson);
                textBox1.Text = formattedJson;
                textBox2.Text = result;
            }
            catch (Exception)
            {
                toolStripStatusLabel1.Text = "Invalid Json";
            }
        }

        private static string FormatJson(string jsonText)
        {
            try
            {
                return JToken.Parse(jsonText).ToString(Formatting.Indented);
            }
            catch (JsonReaderException)
            {
                throw new ArgumentException("Invalid JSON format");
            }
        }

        private static string AnalyzeJson(string jsonText)
        {
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(jsonText);
            ReplaceNullValuesWithDate(obj, DateTime.UtcNow.AddDays(1));
            string modifiedJsonString = JsonConvert.SerializeObject(obj, Formatting.Indented);
            dynamic deserializedObj = JsonConvert.DeserializeObject<dynamic>(modifiedJsonString);

            int countOfContainers = deserializedObj?.count ?? 0;
            var listOfContainers = deserializedObj?.value ?? new List<string> { };
            var groupedByDate = new Dictionary<string, List<dynamic>>();

            foreach (var item in listOfContainers)
            {
                DateTime parsedDate = DateTime.Parse((string)item.StartDate);
                string date = parsedDate.ToString("yyyy-MM-dd");
                if (!groupedByDate.ContainsKey(date))
                {
                    groupedByDate[date] = new List<dynamic> { item };
                }
                else
                {
                    groupedByDate[date].Add(item);
                }
            }

            string result = "";
            foreach (var date in groupedByDate.Keys)
            {
                if (date.Equals(DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd")))
                {
                    result += ($"null => [{groupedByDate[date].Count}] " + $"{String.Join(", ", groupedByDate[date].ConvertAll(item => (string)item.Container))}" + Environment.NewLine);
                }
                else
                {
                    result += ($"{date} => [{groupedByDate[date].Count}] " + $"{String.Join(", ", groupedByDate[date].ConvertAll(item => (string)item.Container))}" + Environment.NewLine);
                }
            }

            return result;
        }

        private static void ReplaceNullValuesWithDate(JToken token, DateTime newDate)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (var property in token.Children<JProperty>())
                {
                    if (property.Value.Type == JTokenType.Null)
                    {
                        property.Value = newDate.ToString("o"); // ISO 8601 format
                    }
                    else
                    {
                        ReplaceNullValuesWithDate(property.Value, newDate);
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var item in token.Children())
                {
                    ReplaceNullValuesWithDate(item, newDate);
                }
            }
        }

        private void HandleJsonError()
        {
            toolStripStatusLabel1.Text = "JSON Error";
            textBox1.Clear();
            textBox2.Clear();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Made by Chandan Somani.");
        }
    }
}
