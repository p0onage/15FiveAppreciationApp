using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AppreciationApp.Web.Services.Interfaces;
using AppreciationApp.Web.Clients.Interfaces;
using AppreciationApp.Web.Models;

namespace AppreciationApp.Web.Services
{
    public class AppreciationService : IAppreciationService
    {
        private IAppreciationAPIClient appreciationAPIClient;

        public AppreciationService(IAppreciationAPIClient appreciationAPIClient)
        {
            this.appreciationAPIClient = appreciationAPIClient;
        }

        public List<HighFives> GetAppreciations()
        {
            var result = appreciationAPIClient.GetWeeklyHighFives();

            var highFives = new List<HighFives>();

            //Initialise list of recipients
            List<string> recipientsList = new List<string>();


            foreach (var highFive in result.results)
            {
                var highFiveMessage = highFive.text.ToString();
                var appreciator = highFive.creator_details.full_name.ToString();
                dynamic textSplit = "";

                var copyOfHighFive = highFiveMessage;

                while (UneditedRecipients(highFiveMessage))
                {
                    highFiveMessage = SpaceApartNames(highFiveMessage);
                }

                highFives.Add(new HighFives()
                {
                    Message = highFiveMessage,
                    AppreciatedUser = appreciator
                });
            }
            return highFives;
        }
        public List<string> PopulateList(List<string> recipientsList, string recipient)
        {
            if (!string.IsNullOrEmpty(recipient))
            {
                recipientsList.Add(recipient);
            }

            return recipientsList;
        }

        public string SpaceApartNames(string message)
        {
            message = Regex.Replace(message, "[@]", "");

            return message = Regex.Replace(message, "([a-z])([A-Z])", "$1 $2");
        }

        public DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public bool UneditedRecipients(dynamic highFiveMessage)
        {
            Match match = Regex.Match(highFiveMessage, @"(@\w+)");
            if (match.Success)
            {
                return true;
            }

            return false;
        }

    }
}
