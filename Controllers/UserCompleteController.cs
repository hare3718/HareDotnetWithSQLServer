using Microsoft.AspNetCore.Mvc;
using hareDotnetSecondAPI.Models;
using hareDotnetSecondAPI.Data;
using hareDotnetSecondAPI.DTOs;

namespace hareDotnetSecondAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class UserCompleteController : ControllerBase
{
    DataContextDapper _dataContextDapper;

    public UserCompleteController(IConfiguration configuration)
    {
        _dataContextDapper = new DataContextDapper(configuration);
    }
    [HttpGet("GetUsers/{userId}/{isActive}")]
    public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
    {
        string sql = @"EXEC HareDotnetFirstSchema.spUsers_Get";

        string parameters = "";

        if (userId != 0)
            parameters += ", @UserId = " + userId.ToString();

        if (isActive)
            parameters += ", @Active = 1";

        // Remove leading comma if exists
        if (parameters.StartsWith(", "))
            parameters = parameters.Substring(2);

        if (!string.IsNullOrEmpty(parameters))
            sql += " " + parameters;

        return _dataContextDapper.LoadData<UserComplete>(sql);
    }

    [HttpPut("UpsertUser")]

    public IActionResult UpsertUser(UserComplete user)
    {
        string sql = $@"EXEC HareDotnetFirstSchema.spUser_Upsert
         @UserId = {user.UserId},
         @FirstName = '{user.FirstName}',
         @LastName = '{user.LastName}',
         @Email = '{user.Email}',
         @Gender = '{user.Gender}',
         @jobTitle = '{user.JobTitle}',
         @Department = '{user.Department}',
         @Salary = {user.Salary},
         @Active = {user.Active}";
        //  WHERE UserId = {user.UserId}";
        //  Console.WriteLine(sql);
        if (_dataContextDapper.ExecuteSql(sql)) return Ok();
        throw new Exception("Sorry Hare bro, Something went wrong while updating the user");
    }

    [HttpDelete("DeleteUser/{userId}")]

    public IActionResult DeleteUser(int userId)
    {
        string sql = @"EXEC HareDotnetFirstSchema.spUser_Delete @UserId = " + userId.ToString();
        if (_dataContextDapper.ExecuteSql(sql)) return Ok();
        throw new Exception("Sorry Hare bro, Something went wrong while deleting the user");
    }

}