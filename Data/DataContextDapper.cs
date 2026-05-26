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
        return dbConnection.QuerySingleOrDefault<T>(sql);
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }

    public bool ExecuteSql(string sql)
    {
        IDbConnection dbConnection = CreateConnection();
        return dbConnection.Execute(sql) > 0;
    }

    public int ExecuteAddSql(string sql)
    {
        IDbConnection dbConnection = CreateConnection();
        return dbConnection.Execute(sql);
    }

    public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> parameters)
    {
        SqlCommand commandWithParams = new SqlCommand(sql);
        foreach (SqlParameter param in parameters)
        {
            commandWithParams.Parameters.Add(param);
        }

        SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        commandWithParams.Connection = sqlConnection;
        sqlConnection.Open();
        int rowsAffected = commandWithParams.ExecuteNonQuery();
        sqlConnection.Close();
        return rowsAffected > 0;
    }
}
