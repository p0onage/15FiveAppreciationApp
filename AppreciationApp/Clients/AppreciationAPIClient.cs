﻿using AppreciationApp.Web.Clients.Interfaces;
using AppreciationApp.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AppreciationApp.Web.Clients
{
    public class AppreciationAPIClient : IAppreciationAPIClient
    {
        public dynamic GetWeeklyHighFives()
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
                return result;
                
            }

            throw new Exception("Request to 15Five was unsuccessful");
        }

        public DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
