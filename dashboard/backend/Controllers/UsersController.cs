using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _service;
        public UsersController(IUserService service)
        {
            _service = service;
        }


        // GET    /api/users           — List all users
        //  GET    /api/users/{id}

        //   Get user by ID


        //    POST / api / users           — Create user



        //    PUT    /api/users/{id


        //     Update user



        //     DELETE /api/users/{id}      — Delete user

    }
}
