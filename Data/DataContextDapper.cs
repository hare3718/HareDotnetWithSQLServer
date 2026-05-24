using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;

namespace hareDotnetSecondAPI.Data;

public class DataContextDapper
{
    private readonly IConfiguration _configuration;

    public DataContextDapper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IEnumerable<T> LoadData<T>(string sql)
    {
        IDbConnection dbConnection = CreateConnection();
        return dbConnection.Query<T>(sql);
    }

    public T LoadDataSingle<T>(string sql)
    {
        IDbConnection dbConnection = CreateConnection();
        return dbConnection.QuerySingle<T>(sql);
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }
}
