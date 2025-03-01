using Core_Layer.Data_Transfer_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Inetrfaces.Services
{
    public interface IAccountService
    {
        public Task<Response> RegisterAsync(RegisterDto UserDto);
        public Task<ReturnLoginDto> LoginAsync(LoginDto UserDto);
        public Task<Response> ChangePasswordAsync(ChangePasswordDto dto);
        public Task<Response> VerifyEmail(string token, string email);
    }
}
