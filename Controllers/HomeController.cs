using MawdyAsistenciaApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using System.IO;
using System.Collections.Generic;
using ExcelDataReader;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;


using System.Text;
namespace MawdyAsistenciaApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public List<Titular> listaTitulares = new List<Titular>();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string searchString)
        {

            if (string.IsNullOrWhiteSpace(searchString))
            {
                Console.WriteLine("No se proporcionaron parámetros de búsqueda.");
            }
            else
            {
                // Eliminar espacios en blanco al principio y al final
                searchString = searchString.Trim();

                

                searchString = CleanSearchString(searchString);

                Console.WriteLine($"Búsqueda realizada: {searchString}");

                Titular nuevo = new Titular();
                List<Titular> listaTitulares = nuevo.BuscarTitulares(searchString);

                if (listaTitulares.Count == 0)
                {
                    Console.WriteLine("La lista de titulares está vacía.");
                }
                else
                {
                    Console.WriteLine($"La lista de titulares contiene {listaTitulares.Count} elementos.");

                    return View(listaTitulares);
                }


            }


            return View(listaTitulares);
        }
       
        public IActionResult Datos(DateTime? fechaInicio, DateTime? fechaFin)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var encoding1252 = System.Text.Encoding.GetEncoding(1252);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/data", "Amazon 2_Raw.xlsx");
            
            List<Order> orders = new List<Order>();
            List<Order> ordersConFechas = new List<Order>();
            var shippingTimes = new List<TimeSpan>();
            var ventasPorDia = new Dictionary<DateTime, (decimal TotalVentas, int TotalOrdenes)>();

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    
                    while (reader.Read()) // Leer fila por fila
                    {
                        //Console.WriteLine($"{reader.GetValue(0).ToString()}");
                        //If debido a que la primera fila contiene los encabezados y las siguientes las datos

                        if (reader.Depth > 0) //Validamos que no sea erl encabezado
                        {
                            var order = new Order
                            {

                                OrderID = reader.GetValue(0).ToString(),
                                OrderDate = Convert.ToDateTime(reader.GetValue(1)),
                                ShipDate = Convert.ToDateTime(reader.GetValue(2)),
                                EmailID = reader.GetValue(3).ToString(),
                                Geography = reader.GetValue(4).ToString(),
                                Category = reader.GetValue(5).ToString(),
                                ProductName = reader.GetValue(6).ToString(),
                                Sales = Convert.ToDecimal(reader.GetValue(7)),
                                Quantity = Convert.ToInt32(reader.GetValue(8)),
                                Profit = Convert.ToDecimal(reader.GetValue(9))

                            };

                            // Calcular el tiempo de envío y agregarlo a la lista
                            var shippingTime = order.ShipDate - order.OrderDate;
                            shippingTimes.Add(shippingTime);

                            orders.Add(order);

                        }
                    }
                }
            }

            // Calcular el tiempo medio de envío en días
            var averageShippingTimeDays = shippingTimes.Average(time => time.TotalDays);

            // Pasar el tiempo medio de envío en días a la vista
            ViewData["AverageShippingTimeDays"] = averageShippingTimeDays;

            //3er inciso para realizar calculos con las ventas netas:

            Dictionary<string, decimal> totalProfitsByCategory = new Dictionary<string, decimal>();

            foreach (var order in orders)
            {
                // Si la categoría ya existe en el diccionario, suma las ganancias
                if (totalProfitsByCategory.ContainsKey(order.Category))
                {
                    totalProfitsByCategory[order.Category] += order.Profit;
                }
                else // Si no, inicializa la categoría con las ganancias actuales
                {
                    totalProfitsByCategory.Add(order.Category, order.Profit);
                }
            }

            // Pasar el diccionario de ganancias totales por categoría a la vista
            ViewData["TotalProfitsByCategory"] = totalProfitsByCategory;

            //Ordenamos por fecha pedido
            //orders.Sort((x, y) => DateTime.Compare(x.OrderDate, y.OrderDate));


            //4. Ultima validación si recibimos fechas hacer el grafico o algun analisis:

            if (fechaInicio.HasValue || fechaFin.HasValue)
            {
                //seleccionar solo las ordenes que esten dentro de esas fechas:
                ordersConFechas = orders.Where(order => order.OrderDate >= fechaInicio.Value && order.OrderDate <= fechaFin.Value).ToList();


                ViewData["MostrarModal"] = true;

                ViewData["TotalRegistros"] = ordersConFechas.Count();
                var totalVentas = ordersConFechas.Sum(order => order.Sales);
                ViewData["TotalVentas"] = totalVentas;


                return View(ordersConFechas);
            }
            else
            {
                ViewData["MostrarModal"] = false;
            }


            // Pasar la lista de órdenes a la vista
            return View(orders);
        }

        public IActionResult MVC()
        {
            return View();
        }
        public IActionResult Capas()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }



        private string CleanSearchString(string input)
        {
            input = input.Replace("'", "");

            input = input.Replace(".", "");

            input = input.Replace(";", "");

            input = input.Replace("--", "");

            input = input.Replace(",", "");

            input = input.Replace("/*", "").Replace("*/", "");


            return input;
        }

        public async Task<IActionResult> LogOut()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Access");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}
