using Microsoft.AspNetCore.Mvc;
using RoyalCode.SmartProblems.MvcResults;

namespace RoyalCode.SmartProblems.TestsApi.Controllers
{
    public class ProblemsController : Controller
    {
        public MatchActionResult NotFoundProblem()
        {
            return Problems.NotFound("Not Found");
        }

        public MatchActionResult<WeatherForecast> Get()
        {
            return new WeatherForecast()
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                TemperatureC = 20,
                Summary = "Sunny"
            };
        }
    }
}
