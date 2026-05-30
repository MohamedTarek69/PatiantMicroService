using PatiantMicroService.Shared.CommonResult;
using PatiantMicroService.Shared.DTOs.PatiantDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.ServicesAbstraction.Interfaces
{
    public interface IIdentityClient
    {
        Task<Result<UpdateIdentityUserResponse>> UpdatePatientAsync(
            string userId,
            UpdateIdentityUserRequest request,
            string token);

        Task<Result<ReturnUserDataDto>> GetUserByIdAsync(string userId);

        Task<Result<bool>> DeleteUserAsync(
            string userId,
            string token);
    }
}
