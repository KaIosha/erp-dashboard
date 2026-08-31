using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using backend.UOW;

namespace backend.Services.Implementations
{
    public class UserService:IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;   
        }
    }
}
