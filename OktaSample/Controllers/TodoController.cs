using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OktaSample.Models;

namespace OktaSample.Controllers
{
    public class TodoController : Controller
    {
        private readonly string _connectionString;

        protected SqlConnection Connection => new SqlConnection(_connectionString);

        public TodoController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // GET: Todo
        public ActionResult Index()
        {
            using var conn = Connection;
            conn.Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT Id, [Name], [Status], CreatedBy FROM Todo WHERE CreatedBy = @userId";

            cmd.Parameters.AddWithValue("@userId", GetUserId());

            var reader = cmd.ExecuteReader();
            var todos = new List<Todo>();

            while (reader.Read())
            {
                todos.Add(new Todo
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy"))
                });
            }

            reader.Close();
            return View(todos);

        }

        // GET: Todo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Todo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Todo todo)
        {
            try
            {
                using var conn = Connection;
                conn.Open();
                using var cmd = conn.CreateCommand();

                cmd.CommandText = @"INSERT INTO Todo (Name, Status, CreatedBy) VALUES (@name, @status, @createdBy)";

                cmd.Parameters.AddWithValue("@name", todo.Name);
                cmd.Parameters.AddWithValue("@status", todo.Status);
                cmd.Parameters.AddWithValue("@createdBy", GetUserId());

                cmd.ExecuteNonQuery();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private string GetUserId() => User.FindFirstValue("sub");
    }
}