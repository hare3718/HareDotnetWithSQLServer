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
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dataContextDapper;
        public PostController(IConfiguration configuration)
        {
            _dataContextDapper = new DataContextDapper(configuration);
        }

        [HttpGet("GetPosts")]
        public IEnumerable<Posts> GetPosts()
        {
            string sql = @"SELECT PostId, UserId, PostTitle, PostContent, PostCreated, PostUpdated FROM HareDotnetFirstSchema.Posts";
            return _dataContextDapper.LoadData<Posts>(sql);
        }

        [HttpGet("GetPost/{postId}")]
        public IEnumerable<Posts> GetPostByPostId(int postId)
        {
            string sql = $@"SELECT PostId, UserId, PostTitle, PostContent, PostCreated, PostUpdated FROM HareDotnetFirstSchema.Posts WHERE PostId = {postId}";
            return _dataContextDapper.LoadData<Posts>(sql);
        }

        [HttpGet("GetPostByUser/{userId}")]
        public IEnumerable<Posts> GetPostByUser(int userId)
        {
            string sql = $@"SELECT PostId, UserId, PostTitle, PostContent, PostCreated, PostUpdated FROM HareDotnetFirstSchema.Posts WHERE UserId = {userId}";
            return _dataContextDapper.LoadData<Posts>(sql);
        }

        [HttpGet("GetMyPosts")]
        public IEnumerable<Posts> GetMyPosts()
        {
            string sql = $@"SELECT PostId, UserId, PostTitle, PostContent, PostCreated, PostUpdated FROM HareDotnetFirstSchema.Posts WHERE UserId = {User.FindFirst("UserId")?.Value}";
            return _dataContextDapper.LoadData<Posts>(sql);
        }

        [HttpGet("GetPostBySearch/{searchTerm}")]
        public IEnumerable<Posts> GetPostBySearch(string searchTerm)
        {
            string sql = $@"SELECT PostId, UserId, PostTitle, PostContent, PostCreated, PostUpdated FROM HareDotnetFirstSchema.Posts
             WHERE PostTitle LIKE '%{searchTerm}%' OR PostContent LIKE '%{searchTerm}%'";
            return _dataContextDapper.LoadData<Posts>(sql);
        }

        [HttpPost("AddPost")]
        public IActionResult AddPost(PostToAddDto postToAddDto)
        {
            string? userIdFromToken = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdFromToken))
                return Unauthorized("User not found in token!");

            string sql = $@"INSERT INTO HareDotnetFirstSchema.Posts 
                    (UserId, PostTitle, PostContent, PostCreated) 
                    VALUES ({userIdFromToken}, '{postToAddDto.PostTitle}', 
                            '{postToAddDto.PostContent}', GETDATE())";

            if (_dataContextDapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to create new post!!");
        }

        [HttpPut("UpdatePost")]
        public IActionResult UpdatePost(PostToEditDto postToEditDto)
        {

            string sql = $@"UPDATE HareDotnetFirstSchema.Posts SET PostTitle = '{postToEditDto.PostTitle}', PostContent = '{postToEditDto.PostContent}', PostUpdated = GETDATE() 
                            WHERE PostId = {postToEditDto.PostId} AND UserId = {User.FindFirst("UserId")?.Value}";
            Console.WriteLine(sql);
            if (_dataContextDapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to update the post!!");
        }

        [HttpPut("DeletePost")]
        public IActionResult DeletePost(int postId)
        {

            string sql = $@"DELETE FROM HareDotnetFirstSchema.Posts 
                            WHERE PostId = {postId} AND UserId = {User.FindFirst("UserId")?.Value}";
            if (_dataContextDapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to delete the post!!");
        }
    }
}