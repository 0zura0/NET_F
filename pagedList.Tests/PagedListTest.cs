using Microsoft.EntityFrameworkCore;
using Reddit;
using Reddit.Models;
using Reddit.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class PagedListTests
{
    private ApplicationDbContext CreateContext()
    {
        var dbName = Guid.NewGuid().ToString(); // Give unique name to the database, so that different tests don't interfere with each other
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.Posts.AddRange(
            new Post { Id = 1, Title = "First Post", Content = "Content 1", Upvotes = 10, Downvotes = 5 },
            new Post { Id = 2, Title = "Second Post", Content = "Content 2", Upvotes = 15, Downvotes = 3 },
            new Post { Id = 3, Title = "Third Post", Content = "Content 3", Upvotes = 5, Downvotes = 2 },
            new Post { Id = 4, Title = "Fourth Post", Content = "Content 3", Upvotes = 5, Downvotes = 2 },
            new Post { Id = 5, Title = "Fifth Post", Content = "Content 3", Upvotes = 5, Downvotes = 2 }
        );

        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task CreateAsync_ReturnsCorrectPagedList()
    {
        using var context = CreateContext();
        var queryable = context.Posts.OrderBy(p => p.Id).AsQueryable();
        var pagedList = await PagedList<Post>.CreateAsync(queryable, 1, 3);

        AssertPagedList(pagedList, 3, 5, 1, 3, true, false);
        AssertItemTitles(pagedList, "First Post", "Second Post", "Third Post");
    }

    [Fact]
    public async Task CreateAsync_HasNextPage_TrueWhenItemsRemain()
    {
        using var context = CreateContext();
        var queryable = context.Posts.OrderBy(p => p.Id).AsQueryable();
        var pagedList = await PagedList<Post>.CreateAsync(queryable, 1, 3);

        Assert.True(pagedList.HasNextPage);
    }

    [Fact]
    public async Task CreateAsync_HasNextPage_FalseWhenNoItemsRemain()
    {
        using var context = CreateContext();
        var queryable = context.Posts.OrderBy(p => p.Id).AsQueryable();
        var pagedList = await PagedList<Post>.CreateAsync(queryable, 2, 3);

        Assert.False(pagedList.HasNextPage);
    }

    [Fact]
    public async Task CreateAsync_HasPreviousPage_TrueWhenNotOnFirstPage()
    {
        using var context = CreateContext();
        var queryable = context.Posts.OrderBy(p => p.Id).AsQueryable();
        var pagedList = await PagedList<Post>.CreateAsync(queryable, 2, 3);

        Assert.True(pagedList.HasPreviousPage);
    }

    [Fact]
    public async Task CreateAsync_HasPreviousPage_FalseWhenOnFirstPage()
    {
        using var context = CreateContext();
        var queryable = context.Posts.OrderBy(p => p.Id).AsQueryable();
        var pagedList = await PagedList<Post>.CreateAsync(queryable, 1, 3);

        Assert.False(pagedList.HasPreviousPage);
    }

    private void AssertPagedList<T>(PagedList<T> pagedList, int expectedItemCount, int expectedTotalCount, int expectedPage, int expectedPageSize, bool expectedHasNextPage, bool expectedHasPreviousPage)
    {
        Assert.Equal(expectedItemCount, pagedList.Items.Count);
        Assert.Equal(expectedTotalCount, pagedList.TotalCount);
        Assert.Equal(expectedPage, pagedList.Page);
        Assert.Equal(expectedPageSize, pagedList.PageSize);
        Assert.Equal(expectedHasNextPage, pagedList.HasNextPage);
        Assert.Equal(expectedHasPreviousPage, pagedList.HasPreviousPage);
    }

    private void AssertItemTitles(PagedList<Post> pagedList, params string[] expectedTitles)
    {
        var actualTitles = pagedList.Items.Select(i => i.Title).ToArray();
        Assert.Equal(expectedTitles, actualTitles);
    }
}
