using System;
using System.Collections.Generic;
using Dapper;
using Npgsql;

namespace DataBase
{
    class Program
    {
        static void Main(string[] args)
        {
            PrepareDatabase();

            using (var conn = CreateConnection("cats"))
            {
                Process(conn);
            }
            
            Console.WriteLine("Press <Enter> to drop database");
            Console.Read();
            
            DropDatabase();
        }

        private static void Process(NpgsqlConnection conn)
        {
            // дока по дапперу https://github.com/StackExchange/Dapper

            conn.Execute("create table cats (id int, name text, size text, bread text, fluffiness text)");

            conn.Execute("insert into cats(id, name, size, bread, fluffiness) values(:id, :name, :size, :bread, :fluffiness)",
                new
                {
                    id = 1,
                    name = "Умка",
                    size = "Не очень большой",
                    bread = "Булочка",
                    fluffiness = "Низкая"
                }
            );

            conn.Execute(
                "insert into cats(id, name, size, bread, fluffiness) values(:id, :name, :size, :bread, :fluffiness)",
                new
                {
                    id = 2,
                    name = "Себастьян",
                    size = "Большой",
                    bread = "Батон",
                    fluffiness = "Слишком высокая"
                }
            );

            IEnumerable<Cat> cats = conn.Query<Cat>("select id, name, size, bread, fluffiness from cats");

            foreach (var cat in cats)
            {
                Console.WriteLine("{0,-5} {1,-80} {2, -40} {3, 1} {4, 20} {4}", cat.Id, cat.Name, cat.Size, cat.Bread, 
                    cat.Fluffiness);
            }
        }

        private static void DropDatabase()
        {
            using (var conn = CreateConnection("postgres"))
            {
                conn.Execute("drop database cats");
            }
        }

        private static void PrepareDatabase()
        {
            using (var conn = CreateConnection("postgres"))
            {
                conn.Execute("create database cats");
            }
        }

        private static NpgsqlConnection CreateConnection(string db)
        {
            // Все возможные параметрты https://www.npgsql.org/doc/connection-string-parameters.html
            var conn = new NpgsqlConnection($"server=localhost;userid=postgres;database={db};Pooling=false");

            conn.Open();
            
            return conn;
        }
    }


    public class Cat
    {
        public Cat(int id, string name, string size, string bread, string fluffiness)
        {
            Id = id;
            Name = name;
            Size = size;
            Bread = bread;
            Fluffiness = fluffiness;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Size { get; set; }

        public string Bread { get; set; }

        public string Fluffiness { get; set; }
    }
}