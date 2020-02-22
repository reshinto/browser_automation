using System;

namespace WeeklyReportCSharp
{
    class Program
    {

        static void Main(string[] args)
        {
            Constants c = new Constants();
            string url = c.url;
            string loginData = c.loginData;
            string elementsData = c.elementsData;
            string weeklyReport = c.weeklyReport;
            WeeklyReport report = new WeeklyReport(url, loginData, elementsData, weeklyReport);
            report.Run();
            Console.WriteLine(c.prompt1);
            string result = Console.ReadLine().ToLower();
            if (result == c.promptResult1)
            {
                Console.WriteLine(c.prompt2);
                report.Submit();
            }
            else
            {
                Console.WriteLine(c.prompt3);
                report.Quit();
            }
        }
    }
}
