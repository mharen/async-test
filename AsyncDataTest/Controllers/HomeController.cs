using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AsyncDataTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _commandText;
        private readonly string _connectionString;

        public HomeController()
        {
            _commandText = "SELECT GETUTCDATE()";
            _connectionString = ConfigurationManager.ConnectionStrings["Sql"].ConnectionString;
        }

        public ActionResult Index()
        {

            // do some data access
            DateTime result;
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = _commandText;
                connection.Open();
                result = (DateTime) command.ExecuteScalar();
            }

            return View("Index", result);
        }


        public async Task<ActionResult> IndexAsync()
        {
            // do some data access
            var resultTask = Task.Factory.StartNew(async () =>
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = _commandText;
                    connection.Open();
                    return (DateTime)await command.ExecuteScalarAsync();
                }
            });

            var dateTask = await resultTask;

            return View("Index", dateTask.Result);
        }
    }
}
