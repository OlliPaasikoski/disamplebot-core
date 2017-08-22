using BasicSampleBot.DocumentDBCore.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BasicSampleBot.DocumentDBCore
{
    public static class DocumentDBRepository<T>
    {
        // A DocumentDB 'database' is a logical container for collections
        // In the sample dataset, that the container is called ToDoList
        private static readonly string DatabaseId = "ToDoList";
        private static readonly string CollectionId = "BetterItems";
        private static DocumentClient client;

        #region CRUD methods
        
        // generic method for creating any DocumentDB document
        public static async Task<Document> CreateTodoItemIfNotExistsAsync(TodoItem item)
        {
            // TODO: test how to break this
            try
            {
                return await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, item.Id.ToString())).ConfigureAwait(false);
            }
            catch (DocumentClientException dce)
            {
                if (dce.StatusCode == HttpStatusCode.NotFound)
                {
                    return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item).ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }

        }

        /// <summary>
        /// God damn
        /// </summary>
        /// <param name="documentLink"></param>
        /// <returns>Document</returns>
        public static async Task<Document> GetByDocumentLink(string documentLink)
        {
            return await client.ReadDocumentAsync(documentLink);
        }

        public static async Task DeleteDocument(Guid testGuid)
        {
            var response = await (client.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri(DatabaseId, CollectionId, testGuid.ToString())));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Document</returns>
        public static async Task<Document> GetTodoItemById(Guid id)
        {
            //  Best way if selfLink is not available...

            IDocumentQuery<Document> query = (
                    from doc in client.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                    where doc.Id.Equals(id.ToString())
                    select doc
                    ).AsDocumentQuery();

            Document documentToReturn = null;
            while (query.HasMoreResults)
            {
                FeedResponse<Document> res = await query.ExecuteNextAsync<Document>();
                if (res.Any())
                {
                    documentToReturn = res.Single();
                    break;
                }
            }

            return documentToReturn;

            //FeedOptions queryOptions = new FeedOptions { MaxItemCount = 1 };

            // Find by Id

            //IQueryable<TodoItem> todoQuery = client.CreateDocumentQuery<TodoItem>(
            //    UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), queryOptions)
            //    .Where(todo => todo.Id.Equals(id.ToString())).FirstOrDefault();

            //FeedResponse<TodoItem> feedResponse = 

            //IDocumentQuery<TodoItem> query = await client.CreateDocumentQuery<TodoItem>(
            //    UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), queryOptions)
            //    .Where(todo => todo.Id.Equals(id.ToString())).AsDocumentQuery();

            //using (var todoQuery = client.CreateDocumentQuery<TodoItem>(
            //    UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), queryOptions)
            //    .Where(todo => todo.Id.Equals(id.ToString())).FirstOrDefault()
            //    .AsDocumentQuery().ExecuteNextAsync();
            //{
            //    return await todoQuery.ExecuteNextAsync().Result;
            //}

            
        }

        #endregion

        #region Initiatialization methods

        public static void Initialize()
        {
            client = new DocumentClient(
                new Uri(ConfigurationManager.AppSettings["DocumentDBEndpointUri"]),
                ConfigurationManager.AppSettings["PrimaryKey"]
            );
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }
        private static async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion
    }
}