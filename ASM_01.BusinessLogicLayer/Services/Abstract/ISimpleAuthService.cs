using ASM_01.BusinessLayer.DTOs;

namespace ASM_01.BusinessLayer.Services.Abstract
{
    public interface ISimpleAuthService
    {
        public AuthDto Login(string username, string password);
    }
}