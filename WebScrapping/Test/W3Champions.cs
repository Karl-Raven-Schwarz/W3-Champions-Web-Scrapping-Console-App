using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Text.RegularExpressions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebScrapping.Models;
using System.Globalization;

namespace WebScrapping.Test
{
    public class W3Champions
    {
        static readonly string Pattern = @"([^%]+)";
        static readonly string PlayerPattern = @"(?<=^)(.*?)(?=\s)";
        static readonly string MapPattern = @"(?<=vs\s\d\s)(\S+\s?\S*)";
        static readonly string DatePattern = @"\d{2}-\w{3}-\d{4}\s\d{2}:\d{2}";
        static readonly string DurationPattern = @"\d{2}:\d{2}(?=\s)";
        static readonly string MatchesNumberRegexPattern = @"(?<=\d+ of )\d+";

        public static List<string> GetKeys(List<string> players) 
        {
            var keys = new List<string>();

            foreach (var player in players) 
            {
                keys.Add(Regex.Match(player, Pattern).Groups[1].Value);
            }
            return keys;
        }

        public static Dictionary<string, List<string>> First(List<Player> players)
        {
            Dictionary<string, List<string>> Data = new();

            ChromeOptions options = new();
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--enable-javascript");

            IWebDriver driver = new ChromeDriver(options);
            //driver.Manage().Window.Minimize();

            var getAWonsText = string.Empty;

            var getTable = string.Empty;

            int failed = 0;

            foreach (var player in players) 
            {
                var data = new List<string>();

                driver.Navigate().GoToUrl($"https://www.w3champions.com/player/{player.Link}/matches/");
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("won")));

                string cssSelector = ".name-link";

                string playerName = player.Name;

                

                const string cssFilterButton = "//button[@class='v-btn v-btn--outlined theme--light v-size--default']";
                const string cssModeFilterButton = "#input-99";
                const string css1vs1Mode = "//div[contains(text(), '1 vs 1')]";

                driver.FindElement(By.XPath(cssFilterButton)).Click();

                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cssModeFilterButton)));

                driver.FindElement(By.CssSelector(cssModeFilterButton)).Click();

                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(css1vs1Mode)));

                driver.FindElement(By.XPath(css1vs1Mode)).Click();

                Thread.Sleep(2000);

                var getWons = driver.FindElements(By.CssSelector(cssSelector));
                
                getTable = driver.FindElement(By.ClassName("custom-table")).Text;

                /*Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Message);*/

                foreach (var w in getWons)
                {
                    try
                    {
                        if (w.Text != null)
                        {
                            if(!w.Text.Contains(playerName) && w.Text.Contains('+'))
                            {
                                string contenidoHtml = w.GetAttribute("innerHTML");
                                string getOponentName = contenidoHtml.Split('<')[0].Trim();

                                //getAWonsText += $"{getOponentName}\n";
                                data.Add(getOponentName);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        failed += 1;
                    }
                   
                }
                //getAWonsText += $"\n- {playerName} -\n\n";
                Data.Add(playerName, data);
            }

            driver.Quit();

            Console.WriteLine($"---\n| {failed}\n---");
            Console.WriteLine($"---\n| {getTable}\n---");
            return Data;
        }

        static string GetMatchId(string player1Name, string player2Name, bool winner)
        {
            if(winner)
            {
                return player1Name + player2Name;
            }
            else
            {
                return player2Name + player1Name;
            }
        }

        static DateTime TextToDateTime(string date)
        {
            string format = "dd-MMM-yyyy HH:mm";
            DateTime Date = DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);

            return Date;
        }

        static TimeSpan TextToTimeSpan(string time)
        {
            _ = new TimeSpan();
            TimeSpan Time;
            if (time.Length == 5)
            {
                _ = TimeSpan.TryParse($"00:{time}", out Time);
            }
            else
            {
                _ = TimeSpan.TryParse($"0{time}", out Time);
            }
            return Time;
        }

        static bool GetPlayerWinner(string player1Data, string player2Data)
        {
            if(player1Data.Contains('+') && player2Data.Contains('-'))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<List<Models.Match>> GetData1vs1(List<Player> players)
        {
            //Apply game mode filter
            const string cssFilterButton = "//button[@class='v-btn v-btn--outlined theme--light v-size--default']";
            const string cssModeFilterButton = "#input-99";
            const string css1vs1Mode = "//div[contains(text(), '1 vs 1')]";

            //
            const string pathFilterGamesCount = "//div[@class='text-center font-regular mt-2']";

            List<List<Models.Match>> Data = new();

            ChromeOptions options = new();
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--enable-javascript");

            IWebDriver driver = new ChromeDriver(options);

            var getAWonsText = string.Empty;

            foreach (var player in players)
            {
                var data = new List<Models.Match>();
                var getTable = string.Empty;

                #region Get data

                driver.Navigate().GoToUrl($"https://www.w3champions.com/player/{player.Link}/matches/");

                //Click filter button
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(5));
                wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("won")));

                string getMatchesNumber = driver.FindElement(By.XPath(pathFilterGamesCount)).Text;
                getMatchesNumber = Regex.Match(getMatchesNumber, MatchesNumberRegexPattern).Value;

                int matchesNumber = int.Parse(getMatchesNumber);

                driver.FindElement(By.XPath(cssFilterButton)).Click();

                //Click game mode filter button
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cssModeFilterButton)));

                driver.FindElement(By.CssSelector(cssModeFilterButton)).Click();

                //Click 1 vs 1 game mode button
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(css1vs1Mode)));

                driver.FindElement(By.XPath(css1vs1Mode)).Click();

                Thread.Sleep(1000);

                //Get table data
                getTable = driver.FindElement(By.ClassName("custom-table")).Text;
                getTable = getTable.Trim('\r');

                #endregion

                List<string> Code = getTable.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                for (int i = 1; i < Code.Count; i += 4)
                {
                    var col1 = Regex.Match(Code[i], PlayerPattern).Value;
                    var col2 = Regex.Match(Code[i + 2], PlayerPattern).Value;
                    var col3 = Regex.Match(Code[i + 3], MapPattern).Value;
                    var col4 = Regex.Match(Code[i + 3], DatePattern).Value;
                    var col5 = Regex.Match(Code[i + 3], DurationPattern).Value;

                    var winner = GetPlayerWinner(col1, col2);
                    var date = TextToDateTime(col4);
                    var time = TextToTimeSpan(col5);
                    var id = GetMatchId(col1, col2, winner);
                    var gameMode = "1vs1";

                    var newMatch = new Models.Match(id, col1, col2, winner, gameMode, col3, date, time);

                    data.Add(newMatch);
                }

                Data.Add(data);
            }

            driver.Quit();

            return Data;
        }
    }
}