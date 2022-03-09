using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories
{
    public class ChoreRepository : BaseRepository
    {
        public ChoreRepository(string connectionString) : base(connectionString)
        {
        }

        public List<Chore> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id,
                                               Name
                                        FROM Chore";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Chore> chores = new List<Chore>();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("Id"));
                        string name = reader.GetString(reader.GetOrdinal("Name"));

                        Chore chore = new Chore()
                        {
                            Id = id,
                            Name = name
                        };
                        chores.Add(chore);  
                    }
                    reader.Close();
                    return chores;
                }
            }
        }

        public Chore GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Name
                                        FROM Chore
                                        WHERE Id = @id;";
                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    Chore chore = null;

                    if (reader.Read())
                    {
                        chore = new Chore()
                        {
                            Id = id,
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                    }
                    reader.Close();
                    return chore;
                }
            }
        }

        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Chore
                                            (Name)
                                        OUTPUT INSERTED.Id
                                        VALUES
                                            (@name);";
                    cmd.Parameters.AddWithValue("@name", chore.Name);

                    int id = (int)cmd.ExecuteScalar();

                    chore.Id = id;
                }
            }
        }

        public List<Chore> GetUnassignedChores()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id,
                                               Name
                                        FROM Chore
                                        LEFT JOIN RoommateChore ON ChoreId = Chore.Id
                                        WHERE ChoreId IS NULL";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Chore> chores = new List<Chore>();

                    while (reader.Read())
                    {
                        Chore chore = new Chore()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                        chores.Add(chore);
                    }
                    reader.Close();
                    return chores;
                }
            }
        }

        public List<Chore> GetAllChoresForAssignment()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id,
                                               CASE
                                                  WHEN EXISTS (SELECT *
                                                                 FROM Chore other
                                                                 LEFT JOIN RoommateChore ON ChoreId = other.Id
                                                                 WHERE ChoreId IS NULL
                                                                   AND other.Id = c.Id)
                                                    THEN Name + ' *'
                                                    ELSE Name
                                               END AS Name
                                        FROM Chore c";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Chore> chores = new List<Chore>();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("Id"));
                        string name = reader.GetString(reader.GetOrdinal("Name"));

                        Chore chore = new Chore()
                        {
                            Id = id,
                            Name = name
                        };
                        chores.Add(chore);
                    }
                    reader.Close();
                    return chores;
                }
            }
        }

        /// <summary>
        /// A method for assigning a chore to a roommate
        /// </summary>
        /// <param name="choreId">The ID of the chore to be assigned</param>
        /// <param name="roommateId">The ID of the roommate to whom the chore is being assigned</param>
        /// <returns>A boolean indicating if the assignment was successful</returns>
        public bool AssignChore(int choreId,int roommateId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO RoommateChore
                                            (ChoreId,
                                             RoommateId)
                                        VALUES
                                            (@choreId,
                                             @roommateId)";

                    cmd.Parameters.AddWithValue("@choreId", choreId);
                    cmd.Parameters.AddWithValue("@roommateId", roommateId);

                    //int numRowsAff = cmd.ExecuteNonQuery();
                    //bool success = numRowsAff > 0;
                    //return success;

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
