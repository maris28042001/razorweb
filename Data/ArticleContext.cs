using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using razor.models;

    public class ArticleContext : DbContext
    {
        public ArticleContext (DbContextOptions<ArticleContext> options)
            : base(options)
        {
        }

        public DbSet<razor.models.Article> Article { get; set; } = default!;
    }
