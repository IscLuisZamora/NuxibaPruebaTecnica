using Microsoft.EntityFrameworkCore;
using AppTestNuxiba.Models;
using MySql.Data.MySqlClient;
namespace AppTestNuxiba.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
            
        }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.UserId);
            });
        }

        public async Task<List<Usuario>> GetAllUsuariosAsync()
        {
            return await Usuarios.FromSqlRaw(@"SELECT u.userId, u.Login, u.Nombre, u.Paterno,u.Materno, e.Sueldo, e.FechaIngreso FROM usuarios u INNER JOIN empleados e ON u.userId = e.userId;").ToListAsync();
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            var sql = @"
        SELECT u.userId, u.Login, u.Nombre, u.Paterno, u.Materno, e.Sueldo, e.FechaIngreso
        FROM usuarios u
        INNER JOIN empleados e ON u.userId = e.userId
        WHERE u.userId = @UserId";
            var parameters = new[] { new MySqlParameter("@UserId", id) };
            return await Usuarios.FromSqlRaw(sql, parameters).FirstOrDefaultAsync();
        }

        public async Task EditSueldoAsync(int userId, double nuevoSueldo)
        {
            await Database.ExecuteSqlRawAsync(@"
                UPDATE empleados
                SET Sueldo = {0}
                WHERE userId = {1};
            ", nuevoSueldo, userId);
        }

        public async Task InsertAllData(Usuario usuario)
        {
            await Database.ExecuteSqlRawAsync(@"
                INSERT INTO usuarios (Login, Nombre, Paterno, Materno)
                VALUES ({0}, {1}, {2}, {3});

                INSERT INTO empleados (userId, Sueldo, FechaIngreso)
                VALUES (LAST_INSERT_ID(), {4}, {5});
            ", usuario.Login, usuario.Nombre, usuario.Paterno, usuario.Materno, usuario.Sueldo, usuario.FechaIngreso);
        }


    }
}
