using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppreciationApp.Web.Models;
using AppreciationApp.Web.Services.Interfaces;
using AppreciationApp.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppreciationApp.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppreciationController : ControllerBase
    {
        private readonly IAppreciationService appreciationService;
        public AppreciationController(IAppreciationService appreciationService)
        {
            this.appreciationService = appreciationService;
        }
        public async Task<List<AppreciationViewModel>> Get15FiveAppreciations([FromQuery]string postcode)
        {
            var highfives = appreciationService.GetAppreciations();
            var AppreciationViewModel = new List<AppreciationViewModel>();
            int indexCount = 0;
            foreach (var item in highfives)
            {
                indexCount += 1;
                AppreciationViewModel.Add(new AppreciationViewModel()
                {
                    Index = indexCount,
                    Message = item.Message,
                    Username = item.AppreciatedUser
                });
            }

            return AppreciationViewModel;
        }
    }
}