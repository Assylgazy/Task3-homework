using Reddit.Models;
using Reddit.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reddit;

namespace JwtAuth.Controllers;

[ApiController]
[Route("/api/[contoller]")]

    public class AccountController : ControllerBase
    {
    private readonly UserManager<User> userManager;
    private readonly ApplicationDbContext _context;
    private readonly TokenService _tokenService;

    public AccountController(UserManager<User> userManager , ApplicationDbContext context ,
        TokenService tokenService , ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _context = context;
        _tokenService = tokenService;
    }

    [Htt]
        
    }

