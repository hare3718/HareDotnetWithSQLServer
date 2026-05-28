using Microsoft.AspNetCore.Mvc;
using hareDotnetSecondAPI.DTOs;
using hareDotnetSecondAPI.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using hareDotnetSecondAPI.Helpers;


namespace hareDotnetSecondAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dataContextDapper;
        private readonly IConfiguration _config;

        private readonly AuthHelper _authHelper;
        public AuthController(IConfiguration config, DataContextDapper dataContextDapper)
        {
            _dataContextDapper = new DataContextDapper(config);
            _config = config;
            _authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sql = @"SELECT Email FROM HareDotnetFirstSchema.Auth where Email = '" + userForRegistration.Email + "'";
                IEnumerable<string> emails = _dataContextDapper.LoadData<string>(sql);
                if (emails.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (var rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);
                    string addAuth = @"INSERT INTO HareDotnetFirstSchema.Auth (Email, PasswordHash, PasswordSalt) VALUES
                     (@Email, @PasswordHash, @PasswordSalt)";

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter emailParam = new SqlParameter("@Email", SqlDbType.VarChar);
                    emailParam.Value = userForRegistration.Email;
                    sqlParameters.Add(emailParam);

                    SqlParameter passwordHashParam = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParam.Value = passwordHash;

                    SqlParameter passwordSaltParam = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParam.Value = passwordSalt;

                    sqlParameters.Add(passwordHashParam);
                    sqlParameters.Add(passwordSaltParam);

                    if (_dataContextDapper.ExecuteSqlWithParameters(addAuth, sqlParameters))
                    {
                        string sqlAddUserDetail = $@"INSERT INTO HareDotnetFirstSchema.Users (FirstName, LastName, Email, Gender)
                       VALUES ('{userForRegistration.FirstName}', '{userForRegistration.LastName}', '{userForRegistration.Email}', '{userForRegistration.Gender}')";
                        if (_dataContextDapper.ExecuteSql(sqlAddUserDetail))
                        {
                            return Ok();
                        }
                        throw new Exception("Something went wrong while registering the user");
                    }
                }
                throw new Exception("User with this email already exists");

            }
            return BadRequest("Password and Password Confirmation do not match");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @"SELECT PasswordHash, PasswordSalt FROM HareDotnetFirstSchema.Auth where Email = '" + userForLogin.Email + "'";
            UserForLoginConfirmation userForLoginConfirmation = _dataContextDapper.LoadDataSingle<UserForLoginConfirmation>(sqlForHashAndSalt);

            if (userForLoginConfirmation == null)
                return StatusCode(404, "User not found!");

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

            if (passwordHash.SequenceEqual(userForLoginConfirmation.PasswordHash))
            {
                int userId = _dataContextDapper.LoadDataSingle<int>(@"SELECT UserId FROM HareDotnetFirstSchema.Users where Email = '" + userForLogin.Email + "'");
                return Ok(new Dictionary<string, string> { { "token", _authHelper.CreateToken(userId.ToString()) } });
            }
            return Unauthorized("Password is incorrect!. Please try again.");
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            // Logic to refresh the token
            string userId = User.FindFirst("UserId")?.Value + "";

            string sql = @"SELECT UserId FROM HareDotnetFirstSchema.Users where UserId = " + userId;
            int userIdFromDb = _dataContextDapper.LoadDataSingle<int>(sql);
            return Ok(new Dictionary<string, string> { { "token", _authHelper.CreateToken(userIdFromDb.ToString()) } });
        }
    }
}
