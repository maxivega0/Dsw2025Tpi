using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Services
{
    public class AuthenticateManagementService : IAuthenticateManagementService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtTokenService _jwtTokenService;
        private readonly ICustomersManagementService _customersManagementService;
        private readonly ILogger<IAuthenticateManagementService> _logger;

        public AuthenticateManagementService(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            JwtTokenService jwtTokenService,
            ICustomersManagementService customersManagementService,
            ILogger<IAuthenticateManagementService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _customersManagementService = customersManagementService;
            _logger = logger;
        }

        public async Task<LoginResponse> Login(LoginModel request)
        {
            _logger.LogInformation("Logica para el inicio de sesion");
            var user = await _userManager.FindByNameAsync(request.Username);
            
            if (user == null)
                throw new EntityNotFoundException("Usuario o contraseña incorrecta");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                throw new EntityNotFoundException("Usuario o contraseña incorrecta");
            
            var role = await _roleManager.FindByNameAsync((await _userManager.GetRolesAsync(user)).First());
            
            var token = _jwtTokenService.GenerateToken(user.UserName, role.Name); 

            return new LoginResponse(Token: token, User: user, Role: role );
        }

        public async Task<RegisterResponse> Register(RegisterModel request)
        {
            _logger.LogInformation("Logica para el registro de usuario");
            var userExists = await _userManager.FindByNameAsync(request.Username);
            if (userExists != null)
                throw new DuplicatedEntityException("El usuario ya existe");

            var roleName = string.IsNullOrWhiteSpace(request.Role) ? "Customer" : request.Role;

            if (!await _roleManager.RoleExistsAsync(roleName))
                throw new EntityNotFoundException($"El rol '{roleName}' no existe.");

            var user = new IdentityUser
            {
                UserName = request.Username,
                Email = request.Email
            };

            var userResult = await _userManager.CreateAsync(user, request.Password);
            
            if (!userResult.Succeeded)
                throw new ArgumentException(string.Join(", ",userResult.Errors.Select(e => e.Description )));

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                throw new ArgumentException("No se pudo asignar el rol.");
            }

            try
            {
                if (roleName == "Customer")
                {
                    var customerModel = new CustomerModel.CreateCustomerRequest(user.UserName, user.Email, user.Id);

                    var customer = await _customersManagementService.CreateCustomer(customerModel);

                }
            }
            catch
            {
                await _userManager.DeleteAsync(user);
                throw;
            }

            var role = await _roleManager.FindByNameAsync((await _userManager.GetRolesAsync(user)).First());

            var token = _jwtTokenService.GenerateToken(user.UserName, role.Name);

            return new RegisterResponse(Token: token, User: user, Role: role);

        }
    }
}
