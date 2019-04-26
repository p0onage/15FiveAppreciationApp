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
        public string taggedUserPattern = @"(^[@]\w+)";

        public List<HighFives> GetWeeklyHighFives()
        {
            var APIKey = "9255ef32f33648c3b5cf84e111fbe53a"; //API Key Goes Here. Don't add it to the repo
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

                    while (MoreUneditedRecipients(highFiveMessage))
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

                    //Replace '@UserName' with 'User Name'
                    foreach (var receiver in recipientsList)
                    {
                        var test = Regex.Matches(copyOfHighFive, taggedUserPattern);
                        //highFiveMessage = Regex.Replace(copyOfHighFive, taggedUserPattern, receiver);
                        MatchEvaluator evaluator = delegate (Match match)
                           {
                               return receiver;
                           };
                        highFiveMessage = System.Regex.Regex.Replace(copyOfHighFive, taggedUserPattern, evaluator);
                        bool stoptest = true;
                    }

                    recipientsList.Clear();
                    highFives.Add(new HighFives()
                    {
                        Message = highFiveMessage
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
            return recipient = Regex.Replace(recipient, "([a-z])([A-Z])", "$1 $2");
        }

        private Tuple<string, object> SplitHighFive(dynamic textSplit, dynamic highFiveMessage, object recipient)
        {
            var testsplit = Regex.Split(highFiveMessage, taggedUserPattern);
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



        public bool MoreUneditedRecipients(dynamic highFiveMessage)
        {
            Match match = Regex.Match(highFiveMessage, @"(^@\w+)");
            if (match.Success)
            {
                return true;
            }

            return false;
        }
    }

}
