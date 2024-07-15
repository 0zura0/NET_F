//using Microsoft.EntityFrameworkCore;
//using Reddit.Models;
//using Reddit.Repositories;
//using System;
//using System.Linq;
//using Xunit;

//namespace Reddit.Tests.Repositories
//{
//    public class PostsRepositoryTests
//    {
//        [Fact]
//        public async Task GetPosts_ReturnsCorrectPosts()
//        {
//            // Arrange
//            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())  // Unique DB name for isolation
//                .Options;

//            using (var context = new ApplicationDbContext(options))
//            {
//                // Seed some test data
//                context.Posts.AddRange(new[]
//                {
//                    new Post { Id = 1, Title = "First Post", Content = "Content 1", Upvotes = 10, Downvotes = 5 },
//                    new Post { Id = 2, Title = "Second Post", Content = "Content 2", Upvotes = 15, Downvotes = 3 },
//                    new Post { Id = 3, Title = "Third Post", Content = "Content 3", Upvotes = 5, Downvotes = 2 }
//                });
//                context.SaveChanges();
//            }

//            using (var context = new ApplicationDbContext(options))
//            {
//                // Act
//                var repository = new PostsRepository(context);

//                // Test sorting by "popular"
//                var resultPopular = await repository.GetPosts(1, 10, null, "popular", true);
//                Assert.Equal(3, resultPopular.TotalCount);  // Assuming total 3 posts in the seeded data
//                //Assert.Equal("Second Post", resultPopular.Items.First().Title);  // Example assertion

//                // Test sorting by "positivity"
//                var resultPositivity = await repository.GetPosts(1, 10, null, "positivity", true);
//                Assert.Equal(3, resultPositivity.TotalCount);  // Assuming total 3 posts in the seeded data
//                Assert.Equal("First Post", resultPositivity.Items.First().Title);  // Example assertion

//                // Test searching by searchTerm "Second"
//                var resultSearch = await repository.GetPosts(1, 10, "Second", null, true);
//                Assert.Single(resultSearch.Items);  // Only one post should match
//                Assert.Equal("Second Post", resultSearch.Items.First().Title);  // Example assertion

//                // Test paging
//                var resultPaging = await repository.GetPosts(2, 1, null, null, true);
//                Assert.Equal(1, resultPaging.Items.Count());  // Only one item should be on the second page
//                Assert.Equal("Second Post", resultPaging.Items.First().Title);  // Example assertion

//                // Test invalid page number
//                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetPosts(0, 10, null, null, true));

//                // Test invalid page size
//                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetPosts(1, 0, null, null, true));
//            }
//        }
//        [Fact]
//        public async Task CreateAsync_ReturnsCorrectPagedList()
//        {
//            // Arrange
//            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//                .UseInMemoryDatabase(databaseName: "PagedListTestDatabase")
//                .Options;


//            //var context = new ApplicationDbContext(options);
//            using (var context = new ApplicationDbContext(options))
//            {
//                context.AddRange(new[]
//                {
//                    new Post { Id = 1, Title = "First Post", Content = "Content 1", Upvotes = 10, Downvotes = 5 },
//                    new Post { Id = 2, Title = "Second Post", Content = "Content 2", Upvotes = 15, Downvotes = 3 },
//                    new Post { Id = 3, Title = "Third Post" , Content = "Content 3", Upvotes = 5, Downvotes = 2 },
//                    new Post { Id = 4, Title = "Fourth Post", Content = "Content 3", Upvotes = 5, Downvotes = 2 },
//                    new Post { Id = 5, Title = "Fifth Post", Content = "Content 3", Upvotes = 5, Downvotes = 2 }
//                 });
//                context.SaveChanges();
//            }


//            using (var context = new ApplicationDbContext(options))
//            {
//                // Act
//                var queryable = context.Posts.OrderBy(p => p.Id);
//                var pagedList = await PagedList<Post>.CreateAsync(queryable, 1, 3);

//                // Assert
//                Assert.Equal(5, pagedList.TotalCount);
//                Assert.Equal(1, pagedList.Page);
//                Assert.Equal(3, pagedList.PageSize);
//                Assert.Equal(3, pagedList.Items.Count);
//                Assert.True(pagedList.HasNextPage);
//                Assert.False(pagedList.HasPreviousPage);
//                Assert.Equal("First Post", pagedList.Items[0].Title);
//                Assert.Equal("Second Post", pagedList.Items[1].Title);
//                Assert.Equal("Third Post", pagedList.Items[2].Title);
//            }
//        }
//    }
//}
