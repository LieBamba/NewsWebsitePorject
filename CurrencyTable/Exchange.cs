using CurrencyTable.Properties.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CurrencyTable
{
    public class Exchange
    {
        private readonly ILogger _logger;
        private readonly ICurrencyServices _currencyServices;
        public Exchange(ILoggerFactory loggerFactory, ICurrencyServices currencyServices)
        {
            _logger = loggerFactory.CreateLogger<Exchange>();
            _currencyServices = currencyServices;
        }

        [Function("Exchange")]
        public void Run([TimerTrigger("0 14 * * * *", RunOnStartup = true)] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            var newRates = _currencyServices.GetRateAsync().Result;
            
            if (myTimer.ScheduleStatus is not null) 
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }

            
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
