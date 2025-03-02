using Microsoft.Build.Framework;

namespace DemoExcercise.Entities;

public enum PostStatus
{
    Draft,
    Published,
    Rejected
}

public class BlogPost
{
    public int Id { get; set; }

    [Required] public string Title { get; set; }

    [Required] public string BannerImagePath { get; set; }

    [Required] public string Content { get; set; }

    [Required] public PostStatus Status { get; set; }

    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}