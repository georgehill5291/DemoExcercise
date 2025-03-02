using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modules.Entities;
using Services.Context;
using Services.Services;

namespace DemoExcercise.Controllers;

[Authorize(Roles = "Admin,Editor")]
public class BlogController : Controller
{
    private readonly IBlogService _blogService;
    private readonly ApplicationDbContext _context;
    private readonly IHostEnvironment _environment;
    private readonly UserManager<IdentityUser> _userManager;

    public BlogController(ApplicationDbContext context, UserManager<IdentityUser> userManager,
        IHostEnvironment environment, IBlogService blogService)
    {
        _context = context;
        _userManager = userManager;
        _environment = environment;
        _blogService = blogService;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _context.BlogPosts.ToListAsync();
        return View(posts);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(BlogPost model, IFormFile? bannerImage)
    {
        await _blogService.CreateBlogPost(model, bannerImage, User);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var post = await _context.BlogPosts.FindAsync(id);
        if (post == null) return NotFound();

        return View(post);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, BlogPost post, IFormFile? bannerImage)
    {
        if (id != post.Id) return NotFound();

        try
        {
            var result = await _blogService.EditBlogPost(id, post, bannerImage, User);
            if (result)
                return RedirectToAction("Index");
            return NotFound();
        }
        catch (Exception e)
        {
            return RedirectToAction("Index");
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var post = await _context.BlogPosts.FindAsync(id);
        if (post != null)
        {
            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}