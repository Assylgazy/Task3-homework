using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Reddit.Dtos;
using Reddit.Mapper;
using Reddit.Models;

namespace Reddit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor(CreateUserDto createAuthorDto)
        {
            var author = new User
            {
                Name = createAuthorDto.Name
            };

            await _context.Users.AddAsync(author);
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAuthors()
        {
            return await _context.Users.ToListAsync();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(int userId,int communityId)
        {
            var community = await _context.Communities.FindAsync(communityId);

            if (community == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.SubscribedCommunities.Add(community);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
        {
            var user = new User
            {
                Name = createUserDto.Name,
               
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Name == loginUserDto.Name);
            if (user == null || .Net.BCrypt.Verify(loginUserDto.Password, user.Password))
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_context["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken()
        {
            // Implement token refresh logic here
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(int userId, int communityId)
        {
            var community = await _context.Communities.FindAsync(communityId);
            if (community == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.SubscribedCommunities.Add(community);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
    
