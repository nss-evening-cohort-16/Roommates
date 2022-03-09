using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories
{
    public class RoommateRepository : BaseRepository
    {
        public RoommateRepository(string connectionString) : base(connectionString)
        {
        }

        public Roommate GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT rm.Id,
                                               FirstName,
                                               LastName,
                                               RentPortion,
                                               MoveInDate,
                                               r.Name
                                        FROM Roommate rm
                                        LEFT JOIN Room r ON
                                            rm.RoomId = r.Id
                                        WHERE rm.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Roommate roomie = null;

                    if (reader.Read())
                    {
                        roomie = new Roommate()
                        {
                            Id = id,
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                            MovedInDate = reader.GetDateTime(reader.GetOrdinal("MoveInDate")),
                            Room = new Room() { Name = reader.GetString(reader.GetOrdinal("Name"))}
                        };
                    }

                    reader.Close();
                    return roomie;
                }
            }
        }

        public List<Roommate> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT rm.Id,
                                               FirstName,
                                               LastName,
                                               RentPortion,
                                               MoveInDate,
                                               r.Name
                                        FROM Roommate rm
                                        LEFT JOIN Room r ON
                                            rm.RoomId = r.Id";

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Roommate> roommates = new List<Roommate>();

                    while (reader.Read())
                    {
                        Roommate roomie = new Roommate()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                            MovedInDate = reader.GetDateTime(reader.GetOrdinal("MoveInDate")),
                            Room = new Room() { Name = reader.GetString(reader.GetOrdinal("Name")) }
                        };
                        roommates.Add(roomie);
                    }
                    reader.Close();
                    return roommates;
                }
            }
        }
    }
}
