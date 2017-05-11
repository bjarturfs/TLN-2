using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeDbSet;
using System.Data.Entity;
using CodingDocs.Models;
using CodingDocs.Models.Entities;

namespace CodingDocs.Tests
{
    /// <summary>
    /// This is an example of how we'd create a fake database by implementing the 
    /// same interface that the BookeStoreEntities class implements.
    /// </summary>
    public class MockDatabase : IAppDataContext
    {
        /// <summary>
        /// Sets up the fake database.
        /// </summary>
        public MockDatabase()
        {
            // We're setting our DbSets to be InMemoryDbSets rather than using SQL Server.
            this.Users = new InMemoryDbSet<ApplicationUser>();
            this.Projects = new InMemoryDbSet<Project>();
            this.UsersInProjects = new InMemoryDbSet<UsersInProject>();
            this.Files = new InMemoryDbSet<File>();
        }

        public IDbSet<ApplicationUser> Users { get; set; }
        public IDbSet<Project> Projects { get; set; }
        public IDbSet<UsersInProject> UsersInProjects { get; set; }
        public IDbSet<File> Files { get; set; }

        public int SaveChanges()
        {
            // Pretend that each entity gets a database id when we hit save.
            int changes = 0;
            //changes += DbSetHelper.IncrementPrimaryKey<Author>(x => x.AuthorId, this.Authors);
            //changes += DbSetHelper.IncrementPrimaryKey<Book>(x => x.BookId, this.Books);

            return changes;
        }

        public void Dispose()
        {
            // Do nothing!
        }
    }
}