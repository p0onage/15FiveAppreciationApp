using AppreciationApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppreciationApp.Web.Clients.Interfaces
{
    public interface IAppreciationAPIClient
    {
        dynamic GetWeeklyHighFives();
    }
}
