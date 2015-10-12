using Mvc5Project.DAL;
using Mvc5Project.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;

namespace Mvc5Project.Controllers
{
    public class BlogController : Controller
    {
        
        private IBlogRepository _blogRepository;
        public static List<BlogViewModel> postList = new List<BlogViewModel>();
        public static List<AllPostsViewModel> allPostsList = new List<AllPostsViewModel>();
        public static List<AllPostsViewModel> checkCatList = new List<AllPostsViewModel>();
        public static List<AllPostsViewModel> checkTagList = new List<AllPostsViewModel>();


        public BlogController()
        {
            _blogRepository = new BlogRepository(new BlogDbContext());
        }

        public BlogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        #region Index

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(int? page, string sortOrder, string searchString, string[] searchCategory, string[] searchTag)
        {
            checkCatList.Clear();
            checkTagList.Clear();
            CreateCatAndTagList();


            Posts(page, sortOrder, searchString, searchCategory, searchTag);
            return View();
        }

        #endregion Index

        #region Posts/AllPosts

        [ChildActionOnly]
        public ActionResult Posts(int? page, string sortOrder, string searchString, string[] searchCategory, string[] searchTag)
        {
            postList.Clear();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentSearchCategory = searchCategory;
            ViewBag.CurrentSearchTag = searchTag;
            ViewBag.DateSortParm = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.TitleSortParm = sortOrder == "Title" ? "title_desc" : "Title";


            var posts = _blogRepository.GetPosts();
            foreach (var post in posts)
            {
                var postCategories = GetPostCategories(post);
                var postTags = GetPostTags(post);
                var likes = _blogRepository.LikeDislikeCount("postlike", post.Id);
                var dislikes = _blogRepository.LikeDislikeCount("postdislike", post.Id);
                postList.Add(new BlogViewModel() { Post = post, Modified = post.Modified, Title = post.Title, ShortDescription = post.ShortDescription, PostedOn = post.PostedOn, ID = post.Id, PostLikes = likes, PostDislikes = dislikes, PostCategories = postCategories, PostTags = postTags, UrlSlug = post.UrlSeo });
            }

            if (searchString != null)
            {
                postList = postList.Where(x => x.Title.ToLower().Contains(searchString.ToLower())).ToList();
            }

            if (searchCategory != null)
            {
                List<BlogViewModel> newlist = new List<BlogViewModel>();
                foreach (var catName in searchCategory)
                {
                    foreach (var item in postList)
                    {
                        if (item.PostCategories.Where(x => x.Name == catName).Any())
                        {
                            newlist.Add(item);
                        }
                    }
                    foreach (var item in checkCatList)
                    {
                        if (item.Category.Name == catName)
                        {
                            item.Checked = true;
                        }
                    }
                }
                postList = postList.Intersect(newlist).ToList();
            }

            if (searchTag != null)
            {
                List<BlogViewModel> newlist = new List<BlogViewModel>();
                foreach (var tagName in searchTag)
                {
                    foreach (var item in postList)
                    {
                        if (item.PostTags.Where(x => x.Name == tagName).Any())
                        {
                            newlist.Add(item);
                        }
                    }
                    foreach (var item in checkTagList)
                    {
                        if (item.Tag.Name == tagName)
                        {
                            item.Checked = true;
                        }
                    }
                }
                postList = postList.Intersect(newlist).ToList();
            }

            switch (sortOrder)
            {
                case "date_desc":
                    postList = postList.OrderByDescending(x => x.PostedOn).ToList();
                    break;
                case "Title":
                    postList = postList.OrderBy(x => x.Title).ToList();
                    break;
                case "title_desc":
                    postList = postList.OrderByDescending(x => x.Title).ToList();
                    break;
                default:
                    postList = postList.OrderBy(x => x.PostedOn).ToList();
                    break;
            }

            int pageSize = 2;
            int pageNumber = (page ?? 1);

            return PartialView("Posts", postList.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AllPosts(int? page, string sortOrder, string searchString, string[] searchCategory, string[] searchTag)
        {
            allPostsList.Clear();
            checkCatList.Clear();
            checkTagList.Clear();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentSearchCategory = searchCategory;
            ViewBag.CurrentSearchTag = searchTag;
            ViewBag.DateSortParm = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.TitleSortParm = sortOrder == "Title" ? "title_desc" : "Title";

            var posts = _blogRepository.GetPosts();
            foreach (var post in posts)
            {
                var postCategories = GetPostCategories(post);
                var postTags = GetPostTags(post);
                allPostsList.Add(new AllPostsViewModel() { PostId = post.Id, Date = post.PostedOn, Description = post.ShortDescription, Title = post.Title, PostCategories = postCategories, PostTags = postTags, UrlSlug = post.UrlSeo });
            }

            if (searchString != null)
            {
                allPostsList = allPostsList.Where(x => x.Title.ToLower().Contains(searchString.ToLower())).ToList();
            }

            CreateCatAndTagList();

            if (searchCategory != null)
            {
                List<AllPostsViewModel> newlist = new List<AllPostsViewModel>();
                foreach (var catName in searchCategory)
                {
                    foreach (var item in allPostsList)
                    {
                        if (item.PostCategories.Where(x => x.Name == catName).Any())
                        {
                            newlist.Add(item);
                        }
                    }
                    foreach (var item in checkCatList)
                    {
                        if (item.Category.Name == catName)
                        {
                            item.Checked = true;
                        }
                    }
                }
                allPostsList = allPostsList.Intersect(newlist).ToList();
            }

            if (searchTag != null)
            {
                List<AllPostsViewModel> newlist = new List<AllPostsViewModel>();
                foreach (var tagName in searchTag)
                {
                    foreach (var item in allPostsList)
                    {
                        if (item.PostTags.Where(x => x.Name == tagName).Any())
                        {
                            newlist.Add(item);
                        }
                    }
                    foreach (var item in checkTagList)
                    {
                        if (item.Tag.Name == tagName)
                        {
                            item.Checked = true;
                        }
                    }
                }
                allPostsList = allPostsList.Intersect(newlist).ToList();
            }

            switch (sortOrder)
            {
                case "date_desc":
                    allPostsList = allPostsList.OrderByDescending(x => x.Date).ToList();
                    break;
                case "Title":
                    allPostsList = allPostsList.OrderBy(x => x.Title).ToList();
                    break;
                case "title_desc":
                    allPostsList = allPostsList.OrderByDescending(x => x.Title).ToList();
                    break;
                default:
                    allPostsList = allPostsList.OrderBy(x => x.Date).ToList();
                    break;
            }

            int pageSize = 2;
            int pageNumber = (page ?? 1);
            return View("AllPosts", allPostsList.ToPagedList(pageNumber, pageSize));

        }


        #endregion Posts/AllPosts

        #region Post

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Post(string slug)
        {
            PostViewModel model = new PostViewModel();
            var posts = GetPosts();
            var postid = _blogRepository.GetPostIdBySlug(slug);
            var post = _blogRepository.GetPostById(postid);
            var videos = GetPostVideos(post);
            var firstPostId = posts.OrderBy(i => i.PostedOn).First().Id;
            var lastPostId = posts.OrderBy(i => i.PostedOn).Last().Id;
            var nextId = posts.OrderBy(i => i.PostedOn).SkipWhile(i => i.Id != postid).Skip(1).Select(i => i.Id).FirstOrDefault();
            var previousId = posts.OrderBy(i => i.PostedOn).TakeWhile(i => i.Id != postid).Select(i => i.Id).LastOrDefault();
            model.FirstPostId = firstPostId;
            model.LastPostId = lastPostId;
            model.PreviousPostSlug = posts.Where(x => x.Id == previousId).Select(x => x.UrlSeo).FirstOrDefault();
            model.NextPostSlug = posts.Where(x => x.Id == nextId).Select(x => x.UrlSeo).FirstOrDefault();
            model.ID = post.Id;
            model.PostCount = posts.Count();
            model.UrlSeo = post.UrlSeo;
            model.Videos = videos;
            model.Title = post.Title;
            model.Body = post.Body;
            model.PostLikes = _blogRepository.LikeDislikeCount("postlike", post.Id);
            model.PostDislikes = _blogRepository.LikeDislikeCount("postdislike", post.Id);
            return View(model);
        }


        public ActionResult UpdatePostLike(string postid, string slug, string username, string likeordislike, string sortorder)
        {
            _blogRepository.UpdatePostLike(postid, username, likeordislike);
            return RedirectToAction("Post", new { slug = slug, sortorder = sortorder });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditPost(string slug)
        {
            var model = CreatePostViewModel(slug);
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditPost(PostViewModel model)
        {
            var post = _blogRepository.GetPostById(model.ID);
            post.Body = model.Body;
            post.Title = model.Title;
            post.Meta = model.Meta;
            post.UrlSeo = model.UrlSeo;
            post.ShortDescription = model.ShortDescription;
            post.Modified = DateTime.Now;
            _blogRepository.Save();

            return RedirectToAction("Post", new { slug = model.UrlSeo });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddVideoToPost(string postid, string slug)
        {
            PostViewModel model = new PostViewModel();
            model.ID = postid;
            model.UrlSeo = slug;
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddVideoToPost(string postid, string slug, string videoUrl)
        {
            CreatePostViewModel(slug);
            _blogRepository.AddVideoToPost(postid, videoUrl);
            return RedirectToAction("EditPost", new { slug = slug });
        }

        
        [Authorize(Roles = "Admin")]
        public ActionResult RemoveVideoFromPost(string slug, string postid, string videoUrl)
        {
            CreatePostViewModel(slug);
            _blogRepository.RemoveVideoFromPost(postid, videoUrl);
            return RedirectToAction("EditPost", new { slug = slug });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddCategoryToPost(string postid)
        {
            PostViewModel model = new PostViewModel();
            model.ID = postid;
            model.Categories = _blogRepository.GetCategories();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategoryToPost(PostViewModel model)
        {
            var post = _blogRepository.GetPostById(model.ID);
            var postCats = _blogRepository.GetPostCategories(post);
            List<string> pCatIds = new List<string>();
            foreach (var pCat in postCats)
            {
                pCatIds.Add(pCat.Id);
            }
            var newCats = model.Categories.Where(x => x.Checked == true).ToList();
            List<string> nCatIds = new List<string>();
            foreach (var pCat in newCats)
            {
                nCatIds.Add(pCat.Id);
            }
            if (!pCatIds.SequenceEqual(nCatIds))
            {
                foreach (var pCat in postCats)
                {
                    _blogRepository.RemovePostCategories(model.ID, pCat.Id);
                }
                foreach (var cat in model.Categories)
                {
                    PostCategory postCategory = new PostCategory();
                    if (cat.Checked == true)
                    {
                        postCategory.PostId = model.ID;
                        postCategory.CategoryId = cat.Id;
                        postCategory.Checked = true;
                        _blogRepository.AddPostCategories(postCategory);
                    }
                }
                _blogRepository.Save();
            }
            return RedirectToAction("EditPost", new { slug = post.UrlSeo });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RemoveCategoryFromPost(string slug, string postid, string catName)
        {
            CreatePostViewModel(slug);
            _blogRepository.RemoveCategoryFromPost(postid, catName);
            return RedirectToAction("EditPost", new { slug = slug });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddNewCategory(string postid, bool callfrompost)
        {
            if (postid != null && callfrompost)
            {
                PostViewModel model = new PostViewModel();
                model.ID = postid;
                return View(model);
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewCategory(string postid, string catName, string catUrlSeo, string catDesc)
        {
            if (postid != null)
            {
                _blogRepository.AddNewCategory(catName, catUrlSeo, catDesc);
                return RedirectToAction("AddCategoryToPost", new { postid = postid });
            }
            else
            {
                _blogRepository.AddNewCategory(catName, catUrlSeo, catDesc);
                return RedirectToAction("CategoriesAndTags", "Blog");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddTagToPost(string postid)
        {
            PostViewModel model = new PostViewModel();
            model.ID = postid;
            model.Tags = _blogRepository.GetTags();
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTagToPost(PostViewModel model)
        {
            var post = _blogRepository.GetPostById(model.ID);
            var postTags = _blogRepository.GetPostTags(post);
            List<string> pTagIds = new List<string>();
            foreach (var pTag in postTags)
            {
                pTagIds.Add(pTag.Id);
            }
            var newTags = model.Tags.Where(x => x.Checked == true).ToList();
            List<string> nTagIds = new List<string>();
            foreach (var pTag in newTags)
            {
                nTagIds.Add(pTag.Id);
            }
            if (!pTagIds.SequenceEqual(nTagIds))
            {
                foreach (var pTag in postTags)
                {
                    _blogRepository.RemovePostTags(model.ID, pTag.Id);
                }
                foreach (var tag in model.Tags)
                {
                    PostTag postTag = new PostTag();
                    if (tag.Checked == true)
                    {
                        postTag.PostId = model.ID;
                        postTag.TagId = tag.Id;
                        postTag.Checked = true;
                        _blogRepository.AddPostTags(postTag);
                    }
                }
                _blogRepository.Save();
            }
            return RedirectToAction("EditPost", new { slug = post.UrlSeo });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RemoveTagFromPost(string slug, string postid, string tagName)
        {
            CreatePostViewModel(slug);
            _blogRepository.RemoveTagFromPost(postid, tagName);
            return RedirectToAction("EditPost", new { slug = slug });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddNewTag(string postid, bool callfrompost)
        {
            if (postid != null && callfrompost)
            {
                PostViewModel model = new PostViewModel();
                model.ID = postid;
                return View(model);
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewTag(string postid, string tagName, string tagUrlSeo)
        {
            if (postid != null)
            {
                _blogRepository.AddNewTag(tagName, tagUrlSeo);
                return RedirectToAction("AddTagToPost", new { postid = postid });
            }
            else
            {
                _blogRepository.AddNewTag(tagName, tagUrlSeo);
                return RedirectToAction("CategoriesAndTags", "Blog");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult CategoriesAndTags()
        {
            checkCatList.Clear();
            checkTagList.Clear();
            CreateCatAndTagList();
            return View();
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveCatAndTag(string[] categoryNames, string[] tagNames)
        {
            if (categoryNames != null)
            {
                foreach (var catName in categoryNames)
                {
                    var category = _blogRepository.GetCategories().Where(x => x.Name == catName).FirstOrDefault();
                    _blogRepository.RemoveCategory(category);
                }
            }
            if (tagNames != null)
            {
                foreach (var tagName in tagNames)
                {
                    var tag = _blogRepository.GetTags().Where(x => x.Name == tagName).FirstOrDefault();
                    _blogRepository.RemoveTag(tag);
                }
            }
            return RedirectToAction("CategoriesAndTags", "Blog");
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeletePost(PostViewModel model, string postid)
        {
            model.ID = postid;
            return View(model);
        }



        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(string postId)
        {
            _blogRepository.DeletePostandComponents(postId);
            return RedirectToAction("Index", "Blog");
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddNewPost()
        {
            List<int> numlist = new List<int>();
            int num = 0;
            var posts = _blogRepository.GetPosts();
            if (posts.Count != 0)
            {
                foreach (var post in posts)
                {
                    var postid = post.Id;
                    Int32.TryParse(postid, out num);
                    numlist.Add(num);
                }
                numlist.Sort();
                num = numlist.Last();
                num++;
            }
            else
            {
                num = 1;
            }
            var newid = num.ToString();
            PostViewModel model = new PostViewModel();
            model.ID = newid;
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddNewPost(PostViewModel model)
        {
            var post = new Post
            {
                Id = model.ID,
                Body = model.Body,
                Meta = model.Meta,
                PostedOn = DateTime.Now,
                Published = true,
                ShortDescription = model.ShortDescription,
                Title = model.Title,
                UrlSeo = model.UrlSeo
            };
            _blogRepository.AddNewPost(post);
            return RedirectToAction("EditPost", "Blog", new { slug = model.UrlSeo });
        }


        #endregion Post

        #region Rss

        public ActionResult Feed()
        {
            var blogTitle = "Your Blog Title";
            var blogDescription = " Your Blog Description.";
            var blogUrl = "http://yourblog.com ";

            var posts = _blogRepository.GetPosts().Select(
                p => new SyndicationItem(
                    p.Title,
                    p.ShortDescription,
                  new Uri(blogUrl)
                )
                );

            // Create an instance of SyndicationFeed class passing the SyndicationItem collection
            var feed = new SyndicationFeed(blogTitle, blogDescription, new Uri(blogUrl), posts)
            {
                Copyright = new TextSyndicationContent(string.Format("Copyright © {0}", blogTitle)),
                Language = "en-US"
            };

            // Format feed in RSS format through Rss20FeedFormatter formatter
            var feedFormatter = new Rss20FeedFormatter(feed);

            // Call the custom action that write the feed to the response
            return new FeedResult(feedFormatter);

        }

        #endregion Rss




        #region Helpers
        public IList<Post> GetPosts()
        {
            return _blogRepository.GetPosts();
        }
        public IList<Category> GetPostCategories(Post post)
        {
            return _blogRepository.GetPostCategories(post);
        }
        public IList<Tag> GetPostTags(Post post)
        {
            return _blogRepository.GetPostTags(post);
        }
        public IList<PostVideo> GetPostVideos(Post post)
        {
            return _blogRepository.GetPostVideos(post);
        }
        public void CreateCatAndTagList()
        {
            foreach (var ct in _blogRepository.GetCategories())
            {
                checkCatList.Add(new AllPostsViewModel { Category = ct, Checked = false });
            }
            foreach (var tg in _blogRepository.GetTags())
            {
                checkTagList.Add(new AllPostsViewModel { Tag = tg, Checked = false });
            }
        }

        public PostViewModel CreatePostViewModel(string slug)
        {
            PostViewModel model = new PostViewModel();
            var postid = _blogRepository.GetPostIdBySlug(slug);
            var post = _blogRepository.GetPostById(postid);
            model.ID = postid;
            model.Title = post.Title;
            model.Body = post.Body;
            model.Meta = post.Meta;
            model.UrlSeo = post.UrlSeo;
            model.Videos = _blogRepository.GetPostVideos(post).ToList();
            model.PostCategories = _blogRepository.GetPostCategories(post).ToList();
            model.PostTags = _blogRepository.GetPostTags(post).ToList();
            model.ShortDescription = post.ShortDescription;
            return model;
        }


        #endregion
    }
}