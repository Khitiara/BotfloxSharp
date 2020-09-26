using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using RestSharp;

namespace XivApi.Pagination
{
    public static class PaginationExtensions
    {
        public static async IAsyncEnumerable<T> GetPaginatedAsync<T>(this XivApi api,
            Func<IRestRequest> baseRequestBuilder,
            [EnumeratorCancellation] CancellationToken cancellationToken = default) {
            bool lastPage = false;
            for (int page = 0; !lastPage; page++) {
                IRestRequest request = baseRequestBuilder();
                request.AddQueryParameter("page", page.ToString());
                PaginatedResponse<T> response = await api.ApiGetAsync<PaginatedResponse<T>>(request, cancellationToken);
                if (response.Pagination.PageNext == null) {
                    lastPage = true;
                }

                foreach (T result in response.Results) {
                    yield return result;
                }
            }
        }
    }
}