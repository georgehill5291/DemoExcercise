using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Modules.Entities;
using Services.Context;

namespace Services.Services;

public interface IBlogService
{
    IEnumerable<BlogPost> GetBlogPosts();
    Task<bool> CreateBlogPost(BlogPost blogPost, IFormFile? bannerImage, ClaimsPrincipal User);
    Task<bool> EditBlogPost(int id, BlogPost post, IFormFile? bannerImage, ClaimsPrincipal User);
}

public class BlogService(
    ApplicationDbContext context,
    UserManager<IdentityUser> userManager,
    IHostEnvironment environment)
    : IBlogService
{
    private readonly IHostEnvironment _environment = environment;

    public IEnumerable<BlogPost> GetBlogPosts()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> EditBlogPost(int id, BlogPost post, IFormFile? bannerImage, ClaimsPrincipal User)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(bannerImage.FileName).ToLower();
        if (!allowedExtensions.Contains(extension)) return false;

        // Validate file size (max 5MB)
        if (bannerImage.Length > 5 * 1024 * 1024) return false;

        var existingPost = await context.BlogPosts.FindAsync(id);
        if (existingPost == null) return false;

        // Handle Banner Image Upload
        if (bannerImage != null && bannerImage.Length > 0)
        {
            var fileName = Path.GetFileName(bannerImage.FileName);
            var filePath = Path.Combine("wwwroot/uploads", fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await bannerImage.CopyToAsync(stream);
            }

            existingPost.BannerImagePath = "/uploads/" + fileName; // Save the relative path
        }

        existingPost.Title = post.Title;
        existingPost.Content = post.Content;

        if (User.IsInRole("Admin"))
            existingPost.Status = post.Status;
        else
            existingPost.Status = PostStatus.Draft;

        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CreateBlogPost(BlogPost blogPost, IFormFile? bannerImage, ClaimsPrincipal User)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(bannerImage.FileName).ToLower();
        if (!allowedExtensions.Contains(extension)) return false;

        // Validate file size (max 5MB)
        if (bannerImage.Length > 5 * 1024 * 1024) return false;


        // Handle Banner Image Upload
        if (bannerImage != null && bannerImage.Length > 0)
        {
            var fileName = Path.GetFileName(bannerImage.FileName);
            var filePath = Path.Combine("wwwroot/uploads", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await bannerImage.CopyToAsync(stream);
            }

            blogPost.BannerImagePath = "/uploads/" + fileName; // Save the relative path
        }

        var user = await userManager.GetUserAsync(User);
        blogPost.CreatedBy = user.UserName;
        blogPost.CreatedDate = DateTime.Now;
        blogPost.Status = PostStatus.Draft;
        context.BlogPosts.Add(blogPost);
        await context.SaveChangesAsync();

        return true;
    }
}