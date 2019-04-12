using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using AppreciationApp.Web.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Regex;

namespace AppreciationApp.Web.Repository
{
    public class FifteenFiveAppreciationRepository : IFifteenFiveAppreciationRepository
    {
        public List<HighFives> GetWeeklyHighFives()
        {
            var APIKey = ""; //API Key Goes Here. Don't add it to the repo
            var url = @"https://theleadagency.15five.com/api/public/high-five/";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKey);
            
            UriBuilder builder = new UriBuilder(url);
            var dayFrom = StartOfWeek(DateTime.Today, DayOfWeek.Monday);
            builder.Query = "created_on_start=" + dayFrom.ToString("yyyy-MM-dd");
            
            var response = client.GetAsync(builder.Uri).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseResults = response.Content.ReadAsStringAsync().Result;
                dynamic result = JsonConvert.DeserializeObject(responseResults);
                //TODo : Make a service and move this into the service
                var highFives = new List<HighFives>();

                //Initialise list of recipients
                List<string> recipientsList = new List<string>();
                

                foreach (var highFive in result.results)
                {
                    var highFiveMessage = highFive.text.ToString();
                    dynamic textSplit = "";
                    var recipient = "";

                    var copyOfHighFive = highFiveMessage;

                    while (highFiveMessage.Contains("@"))
                    {
                        //Split text to find recipients - 
                        var splitHighFive = SplitHighFive(textSplit, highFiveMessage, recipient);
                        highFiveMessage = splitHighFive.Item1; //"For doing a great job at x, y, z"
                        recipient = splitHighFive.Item2; //"BobRoss"
                        //Space
                        recipient = SpaceApartName(recipient); //"Bob Ross"
                        //Populate recipientsList
                        recipientsList = PopulateList(recipientsList, recipient);     
                    }

                    string taggedUserPattern = @"(^[@]\w+)";
                    foreach(var receiver in recipientsList)
                    {                        
                        highFiveMessage = Regex.Replace(copyOfHighFive, taggedUserPattern, receiver);
                    }

                    recipientsList.Clear();
                    highFives.Add(new HighFives()
                    {
                     Message   = highFiveMessage
                    });
                }
                return highFives;
            }

            throw new Exception("Request to 15Five was unsuccessful");
        }

        private List<string> PopulateList(List<string> recipientsList, string recipient)
        {
            if (!string.IsNullOrEmpty(recipient))
            {
                recipientsList.Add(recipient);
            }

            return recipientsList;
        }

        private string SpaceApartName(string recipient)
        {
            var r = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z]) |
                         (?<=[^A-Z])(?=[A-Z]) |
                         (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            return recipient = r.Replace(recipient, " ");
        }

        private Tuple<string, object> SplitHighFive(dynamic textSplit, dynamic highFiveMessage, object recipient)
        {
            textSplit = highFiveMessage.Split(new[] { '@', ' ' }, 3);
            highFiveMessage = textSplit[2];
            recipient = textSplit[1].ToString();

            return new Tuple<string, object>(highFiveMessage, recipient);
        }

        public static DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }

}
