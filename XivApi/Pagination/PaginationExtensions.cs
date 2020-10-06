using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace XivApi.Pagination
{
    public static class PaginationExtensions
    {
        public static async IAsyncEnumerable<T> GetPaginatedAsync<T>(this XivApiClient apiClient,
            Func<IRestRequest> baseRequestBuilder, string? cacheKey = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default) {
            bool lastPage = false;
            for (int page = 0; !lastPage; page++) {
                IRestRequest request = baseRequestBuilder();
                request.AddQueryParameter("page", page.ToString());
                string? pageKey = cacheKey != null ? $"paged:{cacheKey}:{page}" : null;
                PaginatedResponse<T> response =
                    await apiClient.ApiGetAsync<PaginatedResponse<T>>(request, pageKey, cancellationToken);
                if (response.Pagination.PageNext == null) {
                    lastPage = true;
                }

                foreach (T result in response.Results) {
                    yield return result;
                }

                // Avoid rate issues with this big a response, 99% of usecases enumeration will be canceled before here
                await Task.Delay(500, cancellationToken);
            }
        }
    }
}