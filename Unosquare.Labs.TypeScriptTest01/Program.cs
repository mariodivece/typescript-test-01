using System;
using System.IO;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Log;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Labs.TypeScriptTest01.Models;

namespace Unosquare.Labs.TypeScriptTest01
{
    public class Program
    {



        static void Main(string[] args)
        {
            if (false)
            {
                using (var db = new DatabaseContext())
                {
                    db.ShoppingItems.Add(new ShoppingItem() { Name = "Red Roses (x12)", Description = "A beautiful bouquet of 12 roses for your loved one", LastModified = DateTime.Now, Price = 24.99M, Stock = 6 });
                    db.SaveChanges();
                }
            }



            using (var server = new WebServer(Constants.ServerUrl, new SimpleConsoleLog()))
            {
                server.RegisterModule(new LocalSessionModule());
                server.RegisterModule(new StaticFilesModule(Constants.HtmlRootPath));
                server.Module<StaticFilesModule>().UseRamCache = true;
                server.RegisterModule(new WebApiModule());
                server.Module<WebApiModule>().RegisterController<Controllers.ShoppingItemsController>();

                server.RunAsync();

                // Fire up the browser to show the content if we are debugging!
#if DEBUG
                var browser = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo(Constants.ServerUrl) { UseShellExecute = true }
                };
                browser.Start();
#endif

                Console.ReadKey(true);
            }

            
        }
    }
}
