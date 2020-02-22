using System;
using System.IO;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace WeeklyReportCSharp
{
    public class WeeklyReport
    {
        private IWebDriver _driver = new ChromeDriver();
        private JObject _loginData;
        private JObject _elementsData;
        private readonly Constants c = new Constants();
        private static readonly Dictionary<string, Dictionary<string, object>> _inputs = new Dictionary<string, Dictionary<string, object>>
        {
            {
                 "(subject)",
                 new Dictionary<string, object>
                 {
                     { "bool", false },
                     { "text", "" }
                 }
            },
            {
                 "(work)",
                 new Dictionary<string, object>
                 {
                     { "bool", false },
                     { "text", "" }
                 }
            },
            {
                 "(problems)",
                 new Dictionary<string, object>
                 {
                     { "bool", false },
                     { "text", "" }
                 }
            },
            {
                 "(plans)",
                 new Dictionary<string, object>
                 {
                     { "bool", false },
                     { "text", "" }
                 }
            },
            {
                 "(info)",
                 new Dictionary<string, object>
                 {
                     { "bool", false },
                     { "text", "" }
                 }
            },
            {
                 "(impression)",
                 new Dictionary<string, object>
                 {
                     { "bool", false },
                     { "text", "" }
                 }
            },
        };

        public WeeklyReport(string url, string loginData, string elementsData, string weeklyReport)
        {
            _driver.Url = url;
            _loginData = GetJson(loginData);
            _elementsData = GetJson(elementsData);
            ReadWeeklyReport(weeklyReport);
        }

        public void ReadWeeklyReport(string file)
        {
            bool paraStart = false;
            string current = "";
            string line;
            if (File.Exists(file))
            {
                StreamReader content = new StreamReader(file);
                while ((line = content.ReadLine()) != null)
                {
                    int lineLength = line.IndexOf(")") + 1;
                    string _line = line.Substring(0, lineLength);
                    if (_line == "(end)")
                    {
                        paraStart = false;
                        _inputs[current][c.key1] = false;
                    }
                    if (paraStart)
                    {
                        if ((bool)_inputs[current][c.key1])
                        {
                            _inputs[current][c.key2] = (string)_inputs[current][c.key2] + line + "\n";
                        }
                    }
                    if (_inputs.ContainsKey(_line))
                    {
                        paraStart = true;
                        current = _line;
                        _inputs[_line][c.key1] = true;
                    }
                }
                content.Close();
            }
        }

        public string GetDate()
        {
            DateTime today = DateTime.Now;
            string endDay = today.ToString(c.day);
            string startDay = today.AddDays(-5).ToString(c.day);
            string month = today.ToString(c.month);
            string year = today.ToString(c.year);
            return $"({startDay}-{endDay} {month} {year})";
        }

        public JObject GetJson(string file)
        {
            return JObject.Parse(File.ReadAllText(file));
        }

        public IWebElement GetElement(string xPath)
        {
            return _driver.FindElement(By.XPath(xPath));
        }

        public void ClickAction(params string[] args)
        {
            bool isLoading;
            while (true)
            {
                isLoading = false;
                try
                {
                    GetElement((string)_elementsData[args[0]][args[1]][args[2]]).Click();
                }
                catch (Exception)
                {
                    isLoading = true;
                }
                if (!isLoading)
                    break;
            }
        }

        public void RunLoginTasks()
        {
            GetElement((string)_elementsData[c.site][c.section1][c.input1]).SendKeys((string)_loginData[c.site][c.user]);
            GetElement((string)_elementsData[c.site][c.section1][c.input2]).SendKeys((string)_loginData[c.site][c.pw]);
        }

        public void RunWeeklyReportTasks()
        {
            GetElement((string)_elementsData[c.site][c.section3][c.input3]).Clear();
            GetElement((string)_elementsData[c.site][c.section3][c.input3]).SendKeys($"{_inputs[c.key3][c.key2]} {GetDate()}");
            GetElement((string)_elementsData[c.site][c.section3][c.input4]).SendKeys((string)_inputs[c.key4][c.key2]);
            GetElement((string)_elementsData[c.site][c.section3][c.input5]).SendKeys((string)_inputs[c.key5][c.key2]);
            GetElement((string)_elementsData[c.site][c.section3][c.input6]).SendKeys((string)_inputs[c.key6][c.key2]);
            GetElement((string)_elementsData[c.site][c.section3][c.input7]).SendKeys((string)_inputs[c.key7][c.key2]);
            GetElement((string)_elementsData[c.site][c.section3][c.input8]).SendKeys((string)_inputs[c.key8][c.key2]);
        }

        public void Run()
        {
            RunLoginTasks();
            ClickAction(c.site, c.section1, c.button1);
            ClickAction(c.site, c.section2, c.href1);
            ClickAction(c.site, c.section2, c.href2);
            ClickAction(c.site, c.section2, c.href3);
            RunWeeklyReportTasks();
            ClickAction(c.site, c.section3, c.select1);
            ClickAction(c.site, c.section3, c.button2);
            ClickAction(c.site, c.section3, c.button3);
        }

        public void Submit()
        {
            ClickAction(c.site, c.section3, c.button4);
            Thread.Sleep(2000);
            _driver.Quit();
        }

        public void Quit()
        {
            _driver.Quit();
        }
    }
}
