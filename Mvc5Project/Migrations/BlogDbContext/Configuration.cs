namespace Mvc5Project.Migrations.BlogDbContext
{
    using System;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Models.BlogDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"Migrations\BlogDbContext";
        }

        protected override void Seed(Models.BlogDbContext context)
        {
            context.Categories.AddOrUpdate(new Models.Category { Id = "cat1", Name = "Lorem", UrlSeo = "Lorem", Description = "Lorem Category" });
            context.Categories.AddOrUpdate(new Models.Category { Id = "cat2", Name = "Duis", UrlSeo = "Duis", Description = "Duis Category" });
            context.Categories.AddOrUpdate(new Models.Category { Id = "cat3", Name = "Nulla", UrlSeo = "Nulla", Description = "Nulla Category" });
            context.Categories.AddOrUpdate(new Models.Category { Id = "cat4", Name = "Ipsum", UrlSeo = "Ipsum", Description = "Ipsum Category" });

            context.Posts.AddOrUpdate(new Models.Post { Id = "1", Title = "Lorem", Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus nec dolor metus. Nullam risus nisi, posuere eget consequat ac, lacinia ac arcu. Nulla facilisi. Nunc nec tristique sem, eu pulvinar augue. Morbi at risus eget tortor pharetra cursus eu at ligula. Mauris eu commodo nisl, ac lobortis lectus. Ut rhoncus rutrum elit sed fringilla. Etiam in accumsan purus. Maecenas orci diam, consequat a tellus at, pellentesque ullamcorper elit. Sed quis consequat turpis. Proin lacinia est sit amet felis imperdiet, sit amet convallis nulla imperdiet. Nunc sit amet justo sapien. Nulla pulvinar mi quis dapibus commodo.", ShortDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus nec dolor metus. Nullam risus nisi, posuere eget consequat ac, lacinia ac arcu. Nulla facilisi. Nunc nec tristique sem, eu pulvinar augue. Morbi at risus eget tortor pharetra cursus eu at ligula.", PostedOn = DateTime.Now, Meta = "Lorem", UrlSeo = "Lorem", Published = true });
            context.Posts.AddOrUpdate(new Models.Post { Id = "2", Title = "Duis", Body = "Duis sed bibendum risus, nec porta velit. Proin commodo lectus ut nibh blandit tincidunt ut non nibh. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Aenean pretium, felis eget sollicitudin pharetra, arcu ante commodo eros, ut mattis lacus neque sed augue. Fusce laoreet libero eros, sit amet iaculis mauris tempor sit amet. Donec sollicitudin bibendum sem. Nullam a ligula placerat velit rutrum finibus. Vivamus dapibus diam vel nisi pellentesque, et iaculis tellus commodo. Donec efficitur sapien eget arcu cursus bibendum. Morbi risus risus, pellentesque ac sem a, tempor tristique elit.", ShortDescription = "Duis sed bibendum risus, nec porta velit. Proin commodo lectus ut nibh blandit tincidunt ut non nibh. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.", PostedOn = DateTime.Now, Meta = "Duis", UrlSeo = "Duis", Published = true });
            context.Posts.AddOrUpdate(new Models.Post { Id = "3", Title = "Nulla", Body = "Nulla mattis mi in elementum elementum. Aliquam dictum quam id nibh fermentum maximus. Curabitur facilisis neque eget lorem scelerisque vestibulum. Sed et pulvinar turpis, eu convallis urna. Vivamus consectetur vel lorem ut dictum. Aliquam ac ante eu tortor pharetra efficitur. Fusce mattis lacinia arcu, vel dignissim leo fermentum ac. Donec tincidunt pellentesque tristique. Pellentesque porta faucibus scelerisque. Ut ante mi, iaculis eleifend augue vel, fringilla sodales felis. Pellentesque vehicula metus sapien, eget sagittis augue eleifend ut. Ut sit amet nulla est. Sed vel turpis quis dui lobortis accumsan a a mi. Donec nec sagittis urna.", ShortDescription = "Nulla mattis mi in elementum elementum. Aliquam dictum quam id nibh fermentum maximus. Curabitur facilisis neque eget lorem scelerisque vestibulum.", PostedOn = DateTime.Now, Meta = "Nulla", UrlSeo = "Nulla", Published = true });

            context.PostCategories.AddOrUpdate(new Models.PostCategory { PostId = "1", CategoryId = "cat1" });
            context.PostCategories.AddOrUpdate(new Models.PostCategory { PostId = "1", CategoryId = "cat4" });
            context.PostCategories.AddOrUpdate(new Models.PostCategory { PostId = "2", CategoryId = "cat2" });
            context.PostCategories.AddOrUpdate(new Models.PostCategory { PostId = "3", CategoryId = "cat3" });


            context.PostVideos.AddOrUpdate(new Models.PostVideo { Id = 1, PostId = "1", VideoSiteName = "YouTube", VideoUrl = "https://www.youtube.com/embed/HcSEU_BZwDw", VideoThumbnail = "http://img.youtube.com/vi/HcSEU_BZwDw/0.jpg" });
            context.PostVideos.AddOrUpdate(new Models.PostVideo { Id = 2, PostId = "2", VideoSiteName = "YouTube", VideoUrl = "https://www.youtube.com/embed/HcSEU_BZwDw", VideoThumbnail = "http://img.youtube.com/vi/HcSEU_BZwDw/0.jpg" });
            context.PostVideos.AddOrUpdate(new Models.PostVideo { Id = 3, PostId = "3", VideoSiteName = "YouTube", VideoUrl = "https://www.youtube.com/embed/HcSEU_BZwDw", VideoThumbnail = "http://img.youtube.com/vi/HcSEU_BZwDw/0.jpg" });
            context.PostVideos.AddOrUpdate(new Models.PostVideo { Id = 4, PostId = "1", VideoSiteName = "YouTube", VideoUrl = "https://www.youtube.com/embed/XzAHGhMhl7o", VideoThumbnail = "http://img.youtube.com/vi/XzAHGhMhl7o/0.jpg" });

        }
    }
}
