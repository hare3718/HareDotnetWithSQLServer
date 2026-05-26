using Microsoft.AspNetCore.Mvc;
using hareDotnetSecondAPI.Models;
using hareDotnetSecondAPI.Data;
using hareDotnetSecondAPI.DTOs;

namespace hareDotnetSecondAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class UserEFController : ControllerBase
{
    DataContextEF _dataContextEF;

    public UserEFController(IConfiguration configuration)
    {
        _dataContextEF = new DataContextEF(configuration);
    }
    [HttpGet("GetUsers")]
    public IEnumerable<Users> GetUsers()
    {
       IEnumerable<Users> users = _dataContextEF.Users.ToList<Users>();
       if (users == null) throw new Exception("Sorry Hare bro, Something went wrong while fetching the users");
        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    public Users GetSingleUser(int userId)
    {
        Users user = _dataContextEF.Users.Where(u => u.UserId == userId).FirstOrDefault();
        if (user == null) throw new Exception("Sorry Hare bro, Something went wrong while fetching the user");
        return user;
    }

    [HttpPut("UpdateUser")]

    public IActionResult UpdateUser(Users user)
    {
        Users existingUser = _dataContextEF.Users.Where(u => u.UserId == user.UserId).FirstOrDefault();

        if (existingUser != null){
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Gender = user.Gender;
            existingUser.Active = user.Active;

            if (_dataContextEF.SaveChanges() > 0) return Ok();
            throw new Exception("Sorry Hare bro, Something went wrong while updating the user");
        }
        throw new Exception("Sorry Hare bro, User not found");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UsersToAddDto user)
    {
        Users userToAdd = new Users();
        userToAdd.FirstName = user.FirstName;
        userToAdd.LastName = user.LastName;
        userToAdd.Email = user.Email;
        userToAdd.Gender = user.Gender;
        userToAdd.Active = user.Active;

        _dataContextEF.Users.Add(userToAdd);
        if (_dataContextEF.SaveChanges() > 0) return Ok();
        throw new Exception("Sorry Hare bro, Something went wrong while adding the user");
    }

    [HttpDelete("DeleteUser/{userId}")]

    public IActionResult DeleteUser(int userId)
    {
        Users existingUser = _dataContextEF.Users.Where(u => u.UserId == userId).FirstOrDefault();

        if (existingUser != null){
            _dataContextEF.Users.Remove(existingUser);
            if (_dataContextEF.SaveChanges() > 0) return Ok();
            throw new Exception("Sorry Hare bro, Something went wrong while deleting the user");
        }
        throw new Exception("Sorry Hare bro, User not found");
    }

    // New endpoints for UserSalary and UserJobInfo

    [HttpGet("GetUsersJobInfo")]
    public IEnumerable<UserJobInfo> GetUsersJobInfo()
    {
       IEnumerable<UserJobInfo> users = _dataContextEF.UserJobInfo.ToList<UserJobInfo>();
       if (users == null) throw new Exception("Sorry Hare bro, Something went wrong while fetching the users");
        return users;
    }

}