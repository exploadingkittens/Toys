using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;

namespace Toys.ViewComponents
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
