using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using AppreciationApp.Web.Models;
using Newtonsoft.Json;
using System.Linq;

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
            var Saturday = StartOfWeek(DateTime.Today, DayOfWeek.Saturday);
            builder.Query = "created_on_start=" + Saturday.ToString("yyyy-MM-dd");
            
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
                    var text = highFive.text.ToString();
                    dynamic textSplit;
                    while (text.Contains("@"))
                    {
                        //Split text to find recipient
                        textSplit = text.Split(new[] { '@', ' ' }, 3);
                        text = textSplit[2];
                        var recipient = textSplit[1].ToString();
                        //Add recipient to list
                        if (!string.IsNullOrEmpty(recipient))
                        {
                            recipientsList.Add(recipient);
                        }                        
                    }

                    var receivers = "Welldone to: ";
                    foreach (var receiver in recipientsList)
                    {
                        receivers = receivers + receiver + ", ";
                    }
                    receivers = receivers.TrimEnd(',', ' ');
                    highFives.Add(new HighFives()
                    {
                     AppreciatedUser = receivers,
                     Message   = text
                    });
                }
                return highFives;
            }

            throw new Exception("Request to 15Five was unsuccessful");
        }

        public static DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }

}
