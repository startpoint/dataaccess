using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using ECommerce.Data.NoSql;
using Newtonsoft.Json;

namespace Ecommerce.Data.Http
{
    public class HttpBaseConnection<T> : INoSqlDbConnection<T> where T : class
    {
        private readonly NoSqlDbContextOptions _dbContextOptions;

        public HttpBaseConnection(NoSqlDbContextOptions dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task<IList<T>> Where(Func<T, bool> func)
        {
            var url = $"{_dbContextOptions.Connectionstring}/{ParseWhereToQueryString(func)}";

            var request = WebRequest.Create(url);

            using (var response = await request.GetResponseAsync() as HttpWebResponse)
            {
                if (response?.StatusCode != HttpStatusCode.OK) return null;

                var stream = response.GetResponseStream();

                if (stream == null) return null;

                var json = await new StreamReader(stream).ReadToEndAsync();
                return JsonConvert.DeserializeObject<IList<T>>(json);
            }
        }

        private static string ParseWhereToQueryString(Func<T, bool> func)
        {
            var expr = FuncToExpression(func);
            var expBody = expr.Body.ToString();

            foreach (var param in expr.Parameters)
            {
                expBody = expBody.Replace(param.Name + ".", param.Type.Name + ".")
                    .Replace("AndAlso", "&&");
            }

            return expBody;
        }

        private static Expression<Func<T, bool>> FuncToExpression(Func<T, bool> f)
        {
            return x => f(x);
        }

        public async Task AddAsync(T value)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create((string) _dbContextOptions.Connectionstring.Url);

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync()))
            {
                var json = JsonConvert.SerializeObject((object) value);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        public async Task UpdateAsync(T value)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create((string) _dbContextOptions.Connectionstring.Url);

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "PUT";

            using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync()))
            {
                var json = JsonConvert.SerializeObject((object) value);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        public async Task RemoveAsync(string id)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create((string) _dbContextOptions.Connectionstring.Url);

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "DELETE";

            using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync()))
            {
                var json = "{id:" + id + "}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        public void DropDatabase()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //
        }
    }
}