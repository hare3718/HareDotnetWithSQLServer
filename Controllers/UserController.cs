using Microsoft.AspNetCore.Mvc;
using hareDotnetSecondAPI.Models;
using hareDotnetSecondAPI.Data;

namespace hareDotnetSecondAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class UserController : ControllerBase
{
    DataContextDapper _dataContextDapper;

    public UserController(IConfiguration configuration)
    {
        _dataContextDapper = new DataContextDapper(configuration);
    }
    [HttpGet("GetUsers")]
    public IEnumerable<Users> GetUsers()
    {
        string sql = @"SELECT [UserId],
       [FirstName],
       [LastName],
       [Email],
       [Gender],
       [Active]
FROM HareDotnetFirstSchema.Users";
        IEnumerable<Users> users = _dataContextDapper.LoadData<Users>(sql);
        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    public Users GetSingleUser(int userId)
    {
        string sql = @"SELECT [UserId],
       [FirstName],
       [LastName],
       [Email],
       [Gender],
       [Active]
FROM HareDotnetFirstSchema.Users where UserId = " + userId.ToString();
        return _dataContextDapper.LoadDataSingle<Users>(sql);
    }

}