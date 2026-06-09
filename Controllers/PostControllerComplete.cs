using hareDotnetSecondAPI.Models;
using hareDotnetSecondAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using hareDotnetSecondAPI.DTOs;


namespace hareDotnetSecondAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostControllerComplete : ControllerBase
    {
        private readonly DataContextDapper _dataContextDapper;
        public PostControllerComplete(IConfiguration configuration)
        {
            _dataContextDapper = new DataContextDapper(configuration);
        }

        [HttpGet("GetPosts/{postId}/{userId}/{searchTerm}")]
        public IEnumerable<Posts> GetPosts(int postId, int userId, string searchTerm = "None")
        {
            string sql = @"EXEC HareDotnetFirstSchema.spPosts_Get";
            string parameters = "";

            if (postId != 0)
                parameters += ", @PostId = " + postId.ToString();
            if (userId != 0)
                parameters += ", @UserId = " + userId.ToString();
            if (searchTerm != "None")
                parameters += ", @SearchValue = '" + searchTerm + "'";

            if (parameters.Length > 0)
                sql += parameters.Substring(1);

            return _dataContextDapper.LoadData<Posts>(sql);
        }

        [HttpGet("GetMyPosts")]
        public IEnumerable<Posts> GetMyPosts()
        {
            string sql = @"EXEC HareDotnetFirstSchema.spPosts_Get @UserId = " + User.FindFirst("UserId")?.Value;
            return _dataContextDapper.LoadData<Posts>(sql);
        }

        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(PostToAddDto postToAddDto)
        {
            string? userIdFromToken = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdFromToken))
                return Unauthorized("User not found in token!");

            string sql = @"EXEC HareDotnetFirstSchema.spPosts_Upsert
            @PostId = " + postToAddDto.PostId.ToString() + @",
            @UserId = " + userIdFromToken + @",
            @PostTitle = '" + postToAddDto.PostTitle + @"',
            @PostContent = '" + postToAddDto.PostContent + @"'";

            if (postToAddDto.PostId <= 0)
            {
                sql = @"EXEC HareDotnetFirstSchema.spPosts_Upsert
                @UserId = " + userIdFromToken + @",
                @PostTitle = '" + postToAddDto.PostTitle + @"',
                @PostContent = '" + postToAddDto.PostContent + @"'";
            }

            if (_dataContextDapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to Upsert post!!");
        }

        [HttpPut("DeletePost/{postId}")]
        public IActionResult DeletePost(int postId)
        {

            string sql = @"EXEC HareDotnetFirstSchema.spPost_Delete";
            string parameters = "";

            if (postId != 0)
                parameters += ", @PostId = " + postId.ToString();
            if (User.FindFirst("UserId")?.Value != null)
                parameters += ", @UserId = " + User.FindFirst("UserId")?.Value;

            if (parameters.Length > 0)
                sql += parameters.Substring(1);


            if (_dataContextDapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to delete the post!!");
        }
    }
}