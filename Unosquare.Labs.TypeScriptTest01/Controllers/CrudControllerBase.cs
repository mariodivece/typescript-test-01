using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

namespace Unosquare.Labs.TypeScriptTest01.Controllers
{
    /// <summary>
    /// Represents a CRUD controller used to implement basic functionality
    /// Only works with keys that have a long datatype and tables with 1 key only.
    /// Not tested with navigation properties.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <seealso cref="Unosquare.Labs.EmbedIO.Modules.WebApiController" />
    public abstract class CrudControllerBase<T, C> : WebApiController
        where T : class, new()
        where C : DbContext, new()
    {

        /// <summary>
        /// Creates an EF context. Override to customize the creation of the context
        /// </summary>
        /// <returns></returns>
        protected virtual C CreateContext()
        {
            return new C();
        }

        /// <summary>
        /// Finds the specified item given the identifier.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <returns></returns>
        protected abstract Func<T, bool> Find(long itemId);

        /// <summary>
        /// Retrieves the entities
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        protected virtual async Task<bool> Retrieve(WebServer server, HttpListenerContext context)
        {
            try
            {
                var lastSegment = context.Request.Url.Segments.Last();


                if (lastSegment.EndsWith("/"))
                {
                    using (var db = CreateContext())
                    {
                        var entity = await db.Set<T>().ToAsyncEnumerable().ToArray();
                        return context.JsonResponse(entity);
                    }
                }

                long entityId = 0;

                if (long.TryParse(lastSegment, out entityId))
                {
                    using (var db = CreateContext())
                    {                        
                        var data = await db.Set<T>().ToAsyncEnumerable().FirstOrDefault(Find(entityId));
                        if (data != null)
                        {
                            return context.JsonResponse(new[] { data });
                        }
                    }
                }

                throw new KeyNotFoundException($"Key Not Found: {lastSegment}");

            }
            catch (KeyNotFoundException knfex)
            {
                return HandleError(context, knfex, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return HandleError(context, ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Deletes the specified entity, given its id.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        protected virtual async Task<bool> Delete(WebServer server, HttpListenerContext context)
        {
            try
            {
                var lastSegment = context.Request.Url.Segments.Last();
                long entityId = 0;

                if (long.TryParse(lastSegment, out entityId))
                {
                    using (var db = CreateContext())
                    {
                        var entity = await db.Set<T>().ToAsyncEnumerable().FirstOrDefault(Find(entityId));
                        db.Set<T>().Remove(entity);
                        await db.SaveChangesAsync();
                        if (entity != null)
                        {
                            return context.JsonResponse(new[] { entity });
                        }
                    }
                }

                throw new KeyNotFoundException($"Key Not Found: {lastSegment}");

            }
            catch (KeyNotFoundException knfex)
            {
                return HandleError(context, knfex, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return HandleError(context, ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Updates the specified entity given in the body of the request in JSON format
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        protected virtual async Task<bool> Update(WebServer server, HttpListenerContext context)
        {
            try
            {
                var sourceItem = context.ParseJson<T>();
                var dynamicSourceItem = sourceItem as dynamic;
                using (var db = CreateContext())
                {

                    var keyName = db.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties
                        .Select(x => x.Name).Single();

                    var propertyNames = db.Model.FindEntityType(typeof(T)).GetProperties().ToArray();

                    var targetItem = await db.Set<T>().ToAsyncEnumerable().FirstOrDefault(Find((long)dynamicSourceItem[keyName]));
                    if (targetItem == null)
                        throw new KeyNotFoundException($"Key Not Found: '{dynamicSourceItem[keyName]}'");


                    var dynamicTargetItem = targetItem as dynamic;
                    foreach (var properyName in propertyNames)
                    {
                        dynamicTargetItem[properyName] = dynamicSourceItem[properyName];
                    }

                    var result = await db.SaveChangesAsync();

                    return context.JsonResponse(targetItem);
                }
            }
            catch (KeyNotFoundException knfex)
            {
                return HandleError(context, knfex, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return HandleError(context, ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Creates a new entity and returns it. The entity must be provided in the request body in JSON format.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual async Task<bool> Create(WebServer server, HttpListenerContext context)
        {
            try
            {
                var sourceItem = context.ParseJson<T>();
                using (var db = CreateContext())
                {
                    db.Set<T>().Add(sourceItem);
                    var result = await db.SaveChangesAsync();
                    return context.JsonResponse(sourceItem);
                }
            }
            catch (Exception ex)
            {
                return HandleError(context, ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Handles the error returning an error status code and json-encoded body.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <returns></returns>
        protected bool HandleError(HttpListenerContext context, Exception ex, int statusCode = 500)
        {
            var errorResponse = new
            {
                Title = "Unexpected Error",
                ErrorCode = ex.GetType().Name,
                Description = ex.ExceptionMessage(),
            };

            context.Response.StatusCode = statusCode;
            return context.JsonResponse(errorResponse);
        }
    }
}
