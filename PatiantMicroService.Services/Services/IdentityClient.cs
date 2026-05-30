using PatiantMicroService.ServicesAbstraction.Interfaces;
using PatiantMicroService.Shared.CommonResult;
using PatiantMicroService.Shared.DTOs.PatiantDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PatiantMicroService.Services.Services
{
    public class IdentityClient : IIdentityClient
    {
        private readonly HttpClient _httpClient;

        public IdentityClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Result<UpdateIdentityUserResponse>> UpdatePatientAsync(
             string userId,
             UpdateIdentityUserRequest request,
             string token)
                 {
                     var httpRequest = new HttpRequestMessage(
                         HttpMethod.Patch,
                         $"/Clinic/Authentication/UpdateUser/{userId}");
             
                     httpRequest.Headers.Authorization =
                         new AuthenticationHeaderValue(
                             "Bearer",
                             token.Replace("Bearer ", ""));
             
                     httpRequest.Content = JsonContent.Create(request);
             
                     var response = await _httpClient.SendAsync(httpRequest);
             
                     var content = await response.Content.ReadAsStringAsync();
             
                     if (!response.IsSuccessStatusCode)
                     {
                         return Result<UpdateIdentityUserResponse>.Fail(
                             Error.Failure(
                                 "Identity.UpdateFailed",
                                 content
                             )
                         );
                     }
             
                     var result =
                         JsonSerializer.Deserialize<UpdateIdentityUserResponse>(
                             content,
                             new JsonSerializerOptions
                             {
                                 PropertyNameCaseInsensitive = true
                             });
             
                     return Result<UpdateIdentityUserResponse>.Ok(result!);
                 }

        public async Task<Result<ReturnUserDataDto>> GetUserByIdAsync(string userId)
        {
            var response = await _httpClient.GetAsync(
                $"/Clinic/Authentication/UserById/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                return Result<ReturnUserDataDto>.Fail(
                    Error.NotFound(
                        "Identity.UserNotFound",
                        error
                    )
                );
            }

            var user =
                await response.Content.ReadFromJsonAsync<ReturnUserDataDto>();

            if (user is null)
            {
                return Result<ReturnUserDataDto>.Fail(
                    Error.Failure(
                        "Identity.InvalidResponse",
                        "Identity returned invalid data"
                    )
                );
            }

            return Result<ReturnUserDataDto>.Ok(user);
        }

        public async Task<Result<bool>> DeleteUserAsync(
            string email,
            string token)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/Clinic/Authentication/DeleteUser?email={email}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token.Replace("Bearer ", ""));

            var response =
                await _httpClient.SendAsync(request);

            var content =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return Result<bool>.Fail(
                    Error.Failure(
                        "Identity.DeleteFailed",
                        content));
            }

            return Result<bool>.Ok(true);
        }
    }
}

