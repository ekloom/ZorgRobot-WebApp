using Microsoft.Data.SqlClient;
using System.Reflection;

namespace ZorgRobotWebApp.Services.Datainterface
{
    /// <summary>
    /// Base class for saving data to a sql data base and getting data 
    /// </summary>
    public class SqlInterface
    {
        private readonly string _connectionString;

        public SqlInterface(IConfiguration _config)
        {
            _connectionString = _config.GetSection("DB")["ConnectionString"];
        }

        public void DeleteData<T>(string tableName, string condition)
        {

            var type = typeof(T);  // Gets the type of 'T'
            tableName = string.IsNullOrEmpty(tableName) ? type.Name : tableName;  // Uses the class name as the table name. Example: User or the tablename in param

            var deleteQuery = $"DELETE FROM [{tableName}] WHERE {condition}";

            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand(deleteQuery, connection);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Returns the data with type 'T'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public T GetData<T>(string tableName, string condition) where T : class, new()
        {
            var type = typeof(T);  // Gets the type of 'T'
            tableName = string.IsNullOrEmpty(tableName) ? type.Name : tableName;  // Uses the class name as the table name. Example: User or the tablename in param
            var properties = type.GetProperties(BindingFlags.Public); // Gets the properties in the class 'T'


            /* This will form a query with the variables: tableName, columns and parameters.
              Example: SELECT * FROM [User] VALUES (@Name, @Age, @IsActive)
            */
            var selectQuery = $"SELECT * FROM [{tableName}] {condition}";

            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand(selectQuery, connection);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                // Creates a new object of type 'T' this is why T needs to be able to be Instantiable
                var obj = new T();


                foreach (var prop in properties)
                {
                    // Als de data niet null is dan wordt deze if statement true
                    if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                    {
                        // gets the value which has the property name in reader and assigns the value to the matching property in 'obj'
                        prop.SetValue(obj, reader[prop.Name]);
                    }
                }

                // Returns the object with values
                return obj;
            }
            return default;
        }

        public List<T> GetListOfData<T>(string tableName, string condition) where T : class, new()
        {
            var type = typeof(T);  // Gets the type of 'T'
            tableName = string.IsNullOrEmpty(tableName) ? type.Name : tableName;  // Uses the class name as the table name. Example: User or the tablename in param
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance); // Gets the properties in the class 'T'


            /* This will form a query with the variables: tableName, columns and parameters.
              Example: SELECT * FROM [User] VALUES (@Name, @Age, @IsActive)
            */
            var selectQuery = $"SELECT * FROM [{tableName}] {condition}";

            var resultList = new List<T>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand(selectQuery, connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                // Creates a new object of type 'T' this is why T needs to be able to be Instantiable
                var obj = new T();


                foreach (var prop in properties)
                {
                    // Als de data niet null is dan pas wordt deze if statement gerunned
                    if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                    {
                        // gets the value which has the property name in reader and assigns the value to the matching property in 'obj'
                        prop.SetValue(obj, reader[prop.Name]);
                    }
                }

                // adds the object with values to the list
                resultList.Add(obj);
            }
            return resultList;
        }

        public void SaveData<T>(string tableName, T data) where T : class, new()
        {

            var type = typeof(T);  // Gets the type of 'T'

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance); // Gets the properties in the class 'T'

            tableName = string.IsNullOrEmpty(tableName) ? type.Name : tableName;  // Uses the class name as the table name. Example: User or the tablename in param
            var columns = string.Join(", ", properties.Select(p => p.Name)); // Uses the properties name to create columns. Example: (Name, Age, IsActive) 
            var parameters = string.Join(", ", properties.Select(p => $"@{p.Name}")); // Uses the properties name to create params. Example: (@Name, @Age, @IsActive) 


            /* This will form a query with the variables: tableName, columns and parameters.
               Example: INSERT INTO [User] (Name, Age, IsActive) VALUES (@Name, @Age, @IsActive)
             */
            var insertQuery = $"INSERT INTO [{tableName}] ({columns}) VALUES ({parameters})";

            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand(insertQuery, connection);

            foreach (var prop in properties)
            {

                var value = prop.GetValue(data) ?? DBNull.Value; // This will get the value of the property in 'data' which is getting passed in the parameter if it has no value it will just be a null value
                command.Parameters.AddWithValue($"@{prop.Name}", value); // This will add the property name, Example: '@Name' and the value to the parameters
            }

            command.ExecuteNonQuery();
        }
    }
}
