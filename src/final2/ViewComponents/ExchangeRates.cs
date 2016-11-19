using toysRus.DAL;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace toysRus.ViewComponents
{
    public class ExchangeRates : ViewComponent
    {
        public ExchangeRates()
        {  }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.Run(() => (IViewComponentResult)View());
        }
    }
}
