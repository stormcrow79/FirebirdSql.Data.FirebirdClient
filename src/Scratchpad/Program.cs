/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/FirebirdSQL/NETProvider/raw/master/license.txt.
 *
 *    Software distributed under the License is distributed on
 *    an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *    express or implied. See the License for the specific
 *    language governing rights and limitations under the License.
 *
 *    All Rights Reserved.
 */

//$Authors = Jiri Cincura (jiri@cincura.net)

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Scratchpad;

class Program
{
	static void Main(string[] args)
	{
		var connectionString =
			"DataSource=localhost;Port=3050;" +
			"User=SYSDBA;Password=masterkey;" +
			"Database=employee;" +
			"Character Set=ISO8859_1;";

		var options = new DbContextOptionsBuilder()
			.UseFirebird(connectionString)
			.LogTo(Console.WriteLine, minimumLevel: Microsoft.Extensions.Logging.LogLevel.Information)
			.Options;

		var context = new Context(options);

		context.Database.GetDbConnection().Open();

		context.Test.Add(new Test()
		{
			Name = "Entr\u00E9e",
		});

		// the computed column causes a block to be generated
		// EXECUTE BLOCK (p0 VARCHAR(50) CHARACTER SET UTF8 = @p0)
		// RETURNS ("ID" INT, "MODIFIED" TIMESTAMP)
		// the input parameter should be declared as WIN1252 to match the connection string

		context.SaveChanges();
	}

	[Table("TESTS")]
	public class Test
	{
		[Column("ID")]
		public int Id { get; set; }

		[Column("NAME", TypeName = "varchar(50)")]
		public string Name { get; set; }
	}

	public class Context : DbContext
	{
		public Context(DbContextOptions options) : base(options) { }

		public DbSet<Test> Test { get; set; }
	}
}
