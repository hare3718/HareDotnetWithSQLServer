using Microsoft.AspNetCore.Mvc;
using hareDotnetSecondAPI.Models;
using hareDotnetSecondAPI.Data;
using hareDotnetSecondAPI.DTOs;

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

    [HttpPut("UpdateUser")]

    public IActionResult UpdateUser(Users user)
    {
        string sql = $@"UPDATE HareDotnetFirstSchema.Users
         SET FirstName = '{user.FirstName}',
             LastName = '{user.LastName}',
             Email = '{user.Email}',
             Gender = '{user.Gender}',
             Active = {(user.Active ? 1 : 0)}
        //  WHERE UserId = {user.UserId}";
        //  Console.WriteLine(sql);
       if (_dataContextDapper.ExecuteSql(sql)) return Ok();
        throw new Exception("Sorry Hare bro, Something went wrong while updating the user");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UsersToAddDto user)
    {
        string sql = $@"INSERT INTO HareDotnetFirstSchema.Users (FirstName, LastName, Email, Gender, Active)
                       VALUES ('{user.FirstName}', '{user.LastName}', '{user.Email}', '{user.Gender}', {(user.Active ? 1 : 0)})";
        if (_dataContextDapper.ExecuteAddSql(sql) > 0) return Ok();
        throw new Exception("Sorry Hare bro, Something went wrong while adding the user");
    }

}