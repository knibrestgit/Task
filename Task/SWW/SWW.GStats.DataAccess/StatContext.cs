using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWW.GStats.DataAccess
{
    public class StatsContext : DbContext
    {
        public DbSet<Endpoint> Endpoints { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Scoreboard> Scoreboards { get; set; }

        private static bool checkFlag = true;

        public StatsContext(DbContextOptions options) : base(options)
        {
            if (checkFlag) {
                checkFlag = false;
                Database.EnsureCreated();
            }
        }

        public async Task<IEnumerable<T>> RunRawSqlAsync<T>(string sql, SqliteParameter[] parameters, Func<SqliteDataReader, T> map) {
            var result = new List<T>(50);
            using (var connection = Database.GetDbConnection() as SqliteConnection) {
                await connection.OpenAsync();
                using (var query = new SqliteCommand(sql, connection)) {

                    if (parameters != null) {
                        query.Parameters.AddRange(parameters);
                    }

                    using (var reader = await query.ExecuteReaderAsync()) {
                        while (reader.Read()) {
                            result.Add(map(reader));
                        }
                    }
                }
            }
            return result;
        }

    }
}
