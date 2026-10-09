using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IterativeProject1.Repositories;
using IterativeProject1.DTOs;
using System.Threading.Tasks;
using System.Linq;

namespace IterativeProject1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // any logged-in user (Client or Admin) can view the provider list
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetByRole([FromQuery] string role)
        {
            var users = await _userRepository.GetByRole(role);

            var result = users.Select(u => new UserSummaryDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Profession = u.Profession
            });

            return Ok(result);
        }
    }
}