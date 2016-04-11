using System;
using System.Net;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Labs.TypeScriptTest01.Models;

namespace Unosquare.Labs.TypeScriptTest01.Controllers
{
    public class ShoppingItemsController : CrudControllerBase<ShoppingItem, DatabaseContext>
    {
        const string ApiEndpointUrl = Constants.WebApiRelativePath + "shoppingitems/*";

        protected override Func<ShoppingItem, bool> Find(long itemId)
        {
            return (s => s.ItemId == itemId);
        }

        [WebApiHandler(EmbedIO.HttpVerbs.Post, ApiEndpointUrl)]
        public async Task<bool> PostEntity(WebServer server, HttpListenerContext context)
        {
            return await Create(server, context);
        }

        [WebApiHandler(EmbedIO.HttpVerbs.Get, ApiEndpointUrl)]
        public async Task<bool> GetEntity(WebServer server, HttpListenerContext context)
        {
            return await Retrieve(server, context);
        }

        [WebApiHandler(EmbedIO.HttpVerbs.Put, ApiEndpointUrl)]
        public async Task<bool> PutEntity(WebServer server, HttpListenerContext context)
        {
            return await Update(server, context);
        }

        [WebApiHandler(EmbedIO.HttpVerbs.Delete, ApiEndpointUrl)]
        public async Task<bool> DeleteEntity(WebServer server, HttpListenerContext context)
        {
            return await Delete(server, context);
        }


    }
}
