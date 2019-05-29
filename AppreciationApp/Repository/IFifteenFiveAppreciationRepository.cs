using System.Collections.Generic;
using AppreciationApp.Web.Models;

namespace AppreciationApp.Web.Repository
{
    public interface IFifteenFiveAppreciationRepository
    {
        /// <summary>
        /// Gets the weekly high fives.
        /// </summary>
        /// <returns>List of weekly high fives given out this week</returns>
        dynamic GetWeeklyHighFives();
    }
}