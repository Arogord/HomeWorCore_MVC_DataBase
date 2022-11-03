using HomeWorCore_MVC_DataBase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace HomeWorCore_MVC_DataBase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public string GetSensorReadings()
        {
            string _connectionString;
            SqlConnectionStringBuilder stringBuilder = new SqlConnectionStringBuilder();
            stringBuilder.DataSource = "localhost\\SQLEXPRESS";
            stringBuilder.InitialCatalog = "CourseDB";
            stringBuilder.IntegratedSecurity = true;
            stringBuilder.TrustServerCertificate = true;
            _connectionString = stringBuilder.ConnectionString;

            StringBuilder result = new StringBuilder();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                conn.Open();
                //Получим все данные из таблицы Sensors
                string sqlExpression = "Select * From Sensors";
                cmd.CommandText = sqlExpression;
                cmd.Connection = conn;
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    string columnName1 = reader.GetName(0);
                    string columnName2 = reader.GetName(1);
                    string columnName3 = reader.GetName(2);
                    string columnName4 = reader.GetName(3);
                    string columnName5 = reader.GetName(4);
                    string columnName6 = reader.GetName(5);
                    string columnName7 = reader.GetName(6);
                    result.Append($"{columnName1}\t{columnName2}\t{columnName3}\t{columnName4}\t{columnName5}\t{columnName6}\t{columnName7}\n");
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        decimal temp = reader.GetDecimal(1);
                        object humid = reader.GetDecimal(2);
                        object motion = reader.GetValue(3);
                        object light = reader.GetValue(4);
                        object co = reader[5];
                        DateTime datetime = reader.GetDateTime(6);

                        result.Append($"{id} \t{temp} \t\t{humid} \t\t{motion} \t\t{light} \t\t{co} \t\t{datetime}\n");
                    }

                }

                return result.ToString();
            }
        }

        [HttpGet]
        public IActionResult AddSensorData()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddSensorData(DataSensors data)
        {
            string _connectionString;
            SqlConnectionStringBuilder stringBuilder = new SqlConnectionStringBuilder();
            stringBuilder.DataSource = "localhost\\SQLEXPRESS";
            stringBuilder.InitialCatalog = "CourseDB";
            stringBuilder.IntegratedSecurity = true;
            stringBuilder.TrustServerCertificate = true;
            _connectionString = stringBuilder.ConnectionString;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                conn.Open();

                //Параметризация запросов

                string sqlExpression = "Insert Into Sensors Values (@id, @temp,@humid,@motion,@light,@co,@datetime)";
                cmd.CommandText = sqlExpression;
                cmd.Connection = conn;
                // создаем параметр
                SqlParameter idParam = new SqlParameter("@id", data.ID);
                // добавляем параметр к команде
                cmd.Parameters.Add(idParam);
                SqlParameter tempParam = new SqlParameter("@temp", data.Temp_Sensor);
                cmd.Parameters.Add(tempParam);
                SqlParameter humidParam = new SqlParameter("@humid", data.Humid_Sensor);
                cmd.Parameters.Add(humidParam);
                SqlParameter motionParam = new SqlParameter("@motion", data.Motion_Sensor);
                cmd.Parameters.Add(motionParam);
                SqlParameter lightParam = new SqlParameter("@light", data.Light_sensor);
                cmd.Parameters.Add(lightParam);
                SqlParameter coParam = new SqlParameter("@co", data.CO_Sensor);
                cmd.Parameters.Add(coParam);
                SqlParameter datetimeParam = new SqlParameter("@datetime", data.Data_Time);
                cmd.Parameters.Add(datetimeParam);

                int number = cmd.ExecuteNonQuery();
                return View();
            }
        }


    }
}