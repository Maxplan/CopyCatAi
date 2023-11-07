// Purpose: Handles user CRUD operations and authentication.

using CopyCatAiApi.Data.Contexts;
using CopyCatAiApi.DTOs;
using CopyCatAiApi.Models;
using CopyCatAiApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CopyCatAiApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        // Private fields
        private readonly CopyCatAiContext _context;
        private readonly UserManager<UserModel> _userManager;
        private readonly TokenServices _tokenService;

        // Constructor using dependency injection
        public UserController(CopyCatAiContext context, UserManager<UserModel> userManager, TokenServices tokenService)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        // ---CRUD---
        //Create
        [HttpPost("register")]
        public async Task<IActionResult> Register(CreateUserDTO createUserDTO)
        {
            // Check if user exists
            var emailExists = await _context.Users.SingleOrDefaultAsync(u => u.Email == createUserDTO.Email);

            // Check if username exists
            var usernameExists = await _context.Users.SingleOrDefaultAsync(u => u.UserName == createUserDTO.UserName);

            // Null checks
            if (emailExists != null)
            {
                return BadRequest("User already exists");
            }
            if (usernameExists != null)
            {
                return BadRequest("Username already exists");
            }

            // Create a new UserModel
            var user = new UserModel
            {
                Email = createUserDTO.Email,
                UserName = createUserDTO.UserName
            };
            
            // Create the user with the UserManager
            var result = await _userManager.CreateAsync(user, createUserDTO.Password!);

            // Error handling
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Add the user to the User role
            if (await _userManager.AddToRoleAsync(user, "User") != IdentityResult.Success)
            {
                return BadRequest("Could not add user to role");
            }
            return Ok("User created successfully");
        }

        //Read

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Create a DTO to return, hides data from UserModel
            var userGetDTO = new UserGetDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };

            return Ok(userGetDTO);
        }

        [HttpGet("getbyemail/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Create a DTO to return, hides data from UserModel
            var userGetDTO = new UserGetDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };
            return Ok(userGetDTO);
        }

        [HttpGet("getbyusername/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Create a DTO to return, hides data from UserModel
            var userGetDTO = new UserGetDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };

            return Ok(userGetDTO);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();

            if (users == null)
            {
                return NotFound("No users found");
            }

            var userGetDTOs = new List<UserGetDTO>();

            foreach (var user in users)
            {
                var userGetDTO = new UserGetDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName
                };
                userGetDTOs.Add(userGetDTO);
            }
            return Ok(userGetDTOs);
        }
        //Update

        //Delete
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var exists = await _context.Users.FindAsync(id);

            if (exists == null)
            {
                return NotFound("User not found");
            }

            var user = exists;

            _context.Users.Remove(user);

            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok("User deleted successfully");
            }
            return BadRequest("User could not be deleted");
        }

        // ---Authentication---
        //Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDTO loginUserDTO)
        {
            // Find user by email
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginUserDTO.Email);

            // Check if user exists
            if (user == null)
            {
                return BadRequest("User not found");
            }

            // Check if password is correct
            if (!await _userManager.CheckPasswordAsync(user, loginUserDTO.Password!))
            {
                return BadRequest("Incorrect password");
            }

            // Create a token
            var token = await _tokenService.CreateToken(user);

            // Create a DTO to return, hides data from UserModel
            var userGetDTO = new UserGetDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };

            // Return the token and user
            return Ok(new { token, userGetDTO });
        }
    }
}