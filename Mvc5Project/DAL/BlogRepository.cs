using Mvc5Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc5Project.DAL
{
    public class BlogRepository : IBlogRepository, IDisposable
    {
        private BlogDbContext _context;
        public BlogRepository(BlogDbContext context)
        {
            _context = context;
        }

        #region 1
        public IList<Post> GetPosts()
        {
            return _context.Posts.ToList();
        }
        public IList<Tag> GetTags()
        {
            return _context.Tags.ToList();
        }
        public IList<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }
        public string GetUrlSeoByReply(Reply reply)
        {
            var postId = _context.Comments.Where(x => x.Id == reply.CommentId).Select(x => x.PageId).FirstOrDefault();
            return _context.Posts.Where(x => x.Id == postId).Select(x => x.UrlSeo).FirstOrDefault();
        }

        public IList<Category> GetPostCategories(Post post)
        {
            var categoryIds = _context.PostCategories.Where(p => p.PostId == post.Id).Select(p => p.CategoryId).ToList();
            List<Category> categories = new List<Category>();
            foreach (var catId in categoryIds)
            {
                categories.Add(_context.Categories.Where(p => p.Id == catId).FirstOrDefault());
            }
            return categories;
        }
        public IList<Tag> GetPostTags(Post post)
        {
            var tagIds = _context.PostTags.Where(p => p.PostId == post.Id).Select(p => p.TagId).ToList();
            List<Tag> tags = new List<Tag>();
            foreach (var tagId in tagIds)
            {
                tags.Add(_context.Tags.Where(p => p.Id == tagId).FirstOrDefault());
            }
            return tags;
        }
        public IList<PostVideo> GetPostVideos(Post post)
        {
            var postUrls = _context.PostVideos.Where(p => p.PostId == post.Id).ToList();
            List<PostVideo> videos = new List<PostVideo>();
            foreach (var url in postUrls)
            {
                videos.Add(url);
            }
            return videos;
        }
        public int LikeDislikeCount(string typeAndlike, string id)
        {
            switch (typeAndlike)
            {
                case "postlike":
                    return _context.PostLikes.Where(p => p.PostId == id && p.Like == true).Count();
                case "postdislike":
                    return _context.PostLikes.Where(p => p.PostId == id && p.Dislike == true).Count();
                case "commentlike":
                    return _context.CommentLikes.Where(p => p.CommentId == id && p.Like == true).Count();
                case "commentdislike":
                    return _context.CommentLikes.Where(p => p.CommentId == id && p.Dislike == true).Count();
                case "replylike":
                    return _context.ReplyLikes.Where(p => p.ReplyId == id && p.Like == true).Count();
                case "replydislike":
                    return _context.ReplyLikes.Where(p => p.ReplyId == id && p.Dislike == true).Count();
                default:
                    return 0;
            }
        }
        #endregion

        #region 2
        public Post GetPostById(string id)
        {
            return _context.Posts.Find(id);
        }
        public string GetPostIdBySlug(string slug)
        {
            return _context.Posts.Where(x => x.UrlSeo == slug).FirstOrDefault().Id;
        }
        public void UpdatePostLike(string postid, string username, string likeordislike)
        {
            var postLike = _context.PostLikes.Where(x => x.Username == username && x.PostId == postid).FirstOrDefault();
            if (postLike != null)
            {
                switch (likeordislike)
                {
                    case "like":
                        if (postLike.Like == false) { postLike.Like = true; postLike.Dislike = false; }
                        else postLike.Like = false;
                        break;
                    case "dislike":
                        if (postLike.Dislike == false) { postLike.Dislike = true; postLike.Like = false; }
                        else postLike.Dislike = false;
                        break;
                }
                if (postLike.Like == false && postLike.Dislike == false) _context.PostLikes.Remove(postLike);
            }
            else
            {
                switch (likeordislike)
                {
                    case "like":
                        postLike = new PostLike() { PostId = postid, Username = username, Like = true, Dislike = false };
                        _context.PostLikes.Add(postLike);
                        break;
                    case "dislike":
                        postLike = new PostLike() { PostId = postid, Username = username, Like = false, Dislike = true };
                        _context.PostLikes.Add(postLike);
                        break;
                }
            }
            var post = _context.Posts.Where(x => x.Id == postid).FirstOrDefault();
            post.NetLikeCount = LikeDislikeCount("postlike", postid) - LikeDislikeCount("postdislike", postid);
            Save();
        }
        public void AddVideoToPost(string postid, string videoUrl)
        {
            List<int> numlist = new List<int>();
            int num = 1;
            string siteName = null;
            string thumbUrl = null;
            var check = _context.PostVideos.Where(x => x.PostId == postid && x.VideoUrl == videoUrl).Any();
            if (!check)
            {
                while (_context.PostVideos.Where(x => x.Id == num).Any())
                {
                    num++;
                }
                if (videoUrl.Contains("youtube.com") || videoUrl.Contains("youtu.be"))
                {
                    int pos = videoUrl.LastIndexOf("/") + 1;
                    var result = videoUrl.Substring(pos, videoUrl.Length - pos);
                    thumbUrl = "https://img.youtube.com/vi/" + result + "/0.jpg";
                    siteName = "YouTube";
                }
                var video = new PostVideo { Id = num, PostId = postid, VideoUrl = videoUrl, VideoSiteName = siteName, VideoThumbnail = thumbUrl };
                _context.PostVideos.Add(video);
                Save();
            }

        }
        public void RemoveVideoFromPost(string postid, string videoUrl)
        {
            var video = _context.PostVideos.Where(x => x.PostId == postid && x.VideoUrl == videoUrl).FirstOrDefault();
            _context.PostVideos.Remove(video);
            Save();
        }
        public void AddPostCategories(PostCategory postCategory)
        {
            _context.PostCategories.Add(postCategory);
        }
        public void RemovePostCategories(string postid, string categoryid)
        {
            PostCategory postCategory = _context.PostCategories.Where(x => x.PostId == postid && x.CategoryId == categoryid).FirstOrDefault();
            _context.PostCategories.Remove(postCategory);
            Save();
        }
        public void RemoveCategoryFromPost(string postid, string catName)
        {
            var catid = _context.Categories.Where(x => x.Name == catName).Select(x => x.Id).FirstOrDefault();
            var cat = _context.PostCategories.Where(x => x.PostId == postid && x.CategoryId == catid).FirstOrDefault();
            _context.PostCategories.Remove(cat);
            Save();
        }
        public void AddNewCategory(string catName, string catUrlSeo, string catDesc)
        {
            List<int> numlist = new List<int>();
            int num = 0;
            var categories = _context.Categories.ToList();
            foreach (var cat in categories)
            {
                var catid = cat.Id;
                Int32.TryParse(catid.Replace("cat", ""), out num);
                numlist.Add(num);
            }
            numlist.Sort();
            num = numlist.Last();
            num++;
            var newid = "cat" + num.ToString();
            var category = new Category { Id = newid, Name = catName, Description = catDesc, UrlSeo = catUrlSeo, Checked = false };
            _context.Categories.Add(category);
            Save();
        }
        public void RemoveTagFromPost(string postid, string tagName)
        {
            var tagid = _context.Tags.Where(x => x.Name == tagName).Select(x => x.Id).FirstOrDefault();
            var tag = _context.PostTags.Where(x => x.PostId == postid && x.TagId == tagid).FirstOrDefault();
            _context.PostTags.Remove(tag);
            Save();
        }
        public void AddPostTags(PostTag postTag)
        {
            _context.PostTags.Add(postTag);
        }
        public void RemovePostTags(string postid, string tagid)
        {
            PostTag postTag = _context.PostTags.Where(x => x.PostId == postid && x.TagId == tagid).FirstOrDefault();
            _context.PostTags.Remove(postTag);
            Save();
        }
        public void AddNewTag(string tagName, string tagUrlSeo)
        {
            List<int> numlist = new List<int>();
            int num = 0;
            var tags = _context.Tags.ToList();
            if (tags.Count() != 0)
            {
                foreach (var tg in tags)
                {
                    var tagid = tg.Id;
                    Int32.TryParse(tagid.Replace("tag", ""), out num);
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
            var newid = "tag" + num.ToString();
            var tag = new Tag { Id = newid, Name = tagName, UrlSeo = tagUrlSeo, Checked = false };
            _context.Tags.Add(tag);
            Save();
        }
        public void RemoveCategory(Category category)
        {
            var postCategories = _context.PostCategories.Where(x => x.CategoryId == category.Id).ToList();
            foreach (var postCat in postCategories)
            {
                _context.PostCategories.Remove(postCat);
            }
            _context.Categories.Remove(category);
            Save();
        }
        public void RemoveTag(Tag tag)
        {
            var postTags = _context.PostTags.Where(x => x.TagId == tag.Id).ToList();
            foreach (var postTag in postTags)
            {
                _context.PostTags.Remove(postTag);
            }
            _context.Tags.Remove(tag);
            Save();
        }
        public void DeletePostandComponents(string postid)
        {
            var postCategories = _context.PostCategories.Where(p => p.PostId == postid).ToList();
            var postLikes = _context.PostLikes.Where(p => p.PostId == postid).ToList();
            var postTags = _context.PostTags.Where(p => p.PostId == postid).ToList();
            var postVideos = _context.PostVideos.Where(p => p.PostId == postid).ToList();
            var postComments = _context.Comments.Where(p => p.PageId == postid).ToList();
            List<Reply> postReplies = new List<Reply>();
            foreach (var comment in postComments)
            {
                var replies = _context.Replies.Where(x => x.CommentId == comment.Id).ToList();
                foreach (var reply in replies)
                {
                    postReplies.Add(reply);
                }
            }
            var post = _context.Posts.Find(postid);
            foreach (var pc in postCategories) _context.PostCategories.Remove(pc);
            foreach (var pl in postLikes) _context.PostLikes.Remove(pl);
            foreach (var pt in postTags) _context.PostTags.Remove(pt);
            foreach (var pv in postVideos) _context.PostVideos.Remove(pv);
            foreach (var pcom in postComments)
            {
                var commentLikes = _context.CommentLikes.Where(x => x.CommentId == pcom.Id).ToList();
                foreach (var cl in commentLikes) _context.CommentLikes.Remove(cl);
                _context.Comments.Remove(pcom);
            }
            foreach (var pr in postReplies)
            {
                var replyLikes = _context.ReplyLikes.Where(x => x.ReplyId == pr.Id).ToList();
                foreach (var rl in replyLikes) _context.ReplyLikes.Remove(rl);
                _context.Replies.Remove(pr);
            }
            _context.Posts.Remove(post);
            Save();
        }
        public void AddNewPost(Post post)
        {
            _context.Posts.Add(post);
            Save();
        }
        #endregion

        public IList<Comment> GetPostComments(Post post)
        {
            return _context.Comments.Where(p => p.PageId == post.Id).ToList();
        }
        public IList<Comment> GetCommentsByPageId(string pageId)
        {
            return _context.Comments.Where(p => p.PageId == pageId).ToList();
        }
        public List<CommentViewModel> GetParentReplies(Comment comment)
        {
            var parentReplies = _context.Replies.Where(p => p.CommentId == comment.Id && p.ParentReplyId == null).ToList();
            List<CommentViewModel> parReplies = new List<CommentViewModel>();
            foreach (var pr in parentReplies)
            {
                var chReplies = GetChildReplies(pr);
                parReplies.Add(new CommentViewModel() { Body = pr.Body, ParentReplyId = pr.ParentReplyId, DateTime = pr.DateTime, Id = pr.Id, UserName = pr.UserName, ChildReplies = chReplies });
            }
            return parReplies;
        }

        public List<CommentViewModel> GetChildReplies(Reply parentReply)
        {
            List<CommentViewModel> chldReplies = new List<CommentViewModel>();
            if (parentReply != null)
            {
                var childReplies = _context.Replies.Where(p => p.ParentReplyId == parentReply.Id).ToList();
                foreach (var reply in childReplies)
                {
                    var chReplies = GetChildReplies(reply);
                    chldReplies.Add(new CommentViewModel() { Body = reply.Body, ParentReplyId = reply.ParentReplyId, DateTime = reply.DateTime, Id = reply.Id, UserName = reply.UserName, ChildReplies = chReplies });
                }
            }
            return chldReplies;
        }


        public Reply GetReplyById(string id)
        {
            return _context.Replies.Where(p => p.Id == id).FirstOrDefault();
        }


        public bool CommentDeleteCheck(string commentid)
        {
            return _context.Comments.Where(x => x.Id == commentid).Select(x => x.Deleted).FirstOrDefault();
        }
        public bool ReplyDeleteCheck(string replyid)
        {
            return _context.Replies.Where(x => x.Id == replyid).Select(x => x.Deleted).FirstOrDefault();
        }


        public void UpdateCommentLike(string commentid, string username, string likeordislike)
        {
            var commentLike = _context.CommentLikes.Where(x => x.Username == username && x.CommentId == commentid).FirstOrDefault();
            if (commentLike != null)
            {
                switch (likeordislike)
                {
                    case "like":
                        if (commentLike.Like == false) { commentLike.Like = true; commentLike.Dislike = false; }
                        else commentLike.Like = false;
                        break;
                    case "dislike":
                        if (commentLike.Dislike == false) { commentLike.Dislike = true; commentLike.Like = false; }
                        else commentLike.Dislike = false;
                        break;
                }
                if (commentLike.Like == false && commentLike.Dislike == false) _context.CommentLikes.Remove(commentLike);
            }
            else
            {
                switch (likeordislike)
                {
                    case "like":
                        commentLike = new CommentLike() { CommentId = commentid, Username = username, Like = true, Dislike = false };
                        _context.CommentLikes.Add(commentLike);
                        break;
                    case "dislike":
                        commentLike = new CommentLike() { CommentId = commentid, Username = username, Like = false, Dislike = true };
                        _context.CommentLikes.Add(commentLike);
                        break;
                }
            }
            var comment = _context.Comments.Where(x => x.Id == commentid).FirstOrDefault();
            comment.NetLikeCount = LikeDislikeCount("commentlike", commentid) - LikeDislikeCount("commentdislike", commentid);
            Save();
        }

        public void UpdateReplyLike(string replyid, string username, string likeordislike)
        {
            var replyLike = _context.ReplyLikes.Where(x => x.Username == username && x.ReplyId == replyid).FirstOrDefault();
            if (replyLike != null)
            {
                switch (likeordislike)
                {
                    case "like":
                        if (replyLike.Like == false) { replyLike.Like = true; replyLike.Dislike = false; }
                        else replyLike.Like = false;
                        break;
                    case "dislike":
                        if (replyLike.Dislike == false) { replyLike.Dislike = true; replyLike.Like = false; }
                        else replyLike.Dislike = false;
                        break;
                }
                if (replyLike.Like == false && replyLike.Dislike == false) _context.ReplyLikes.Remove(replyLike);
            }
            else
            {
                switch (likeordislike)
                {
                    case "like":
                        replyLike = new ReplyLike() { ReplyId = replyid, Username = username, Like = true, Dislike = false };
                        _context.ReplyLikes.Add(replyLike);
                        break;
                    case "dislike":
                        replyLike = new ReplyLike() { ReplyId = replyid, Username = username, Like = false, Dislike = true };
                        _context.ReplyLikes.Add(replyLike);
                        break;
                }
            }
            Save();
        }

        public Post GetPostByReply(string replyid)
        {
            var commentId = _context.Replies.Where(x => x.Id == replyid).Select(x => x.CommentId).FirstOrDefault();
            var postId = _context.Comments.Where(x => x.Id == commentId).Select(x => x.PageId).FirstOrDefault();
            return _context.Posts.Where(x => x.Id == postId).FirstOrDefault();
        }




        public IList<Comment> GetComments()
        {
            return _context.Comments.ToList();
        }
        public IList<Reply> GetReplies()
        {
            return _context.Replies.ToList();
        }
        public void AddNewComment(Comment comment)
        {
            _context.Comments.Add(comment);
            Save();
        }
        public void AddNewReply(Reply reply)
        {
            _context.Replies.Add(reply);
            Save();
        }



        public Comment GetCommentById(string id)
        {
            return _context.Comments.Where(p => p.Id == id).FirstOrDefault();
        }

        public void DeleteComment(string commentid)
        {
            var comment = _context.Comments.Where(x => x.Id == commentid).FirstOrDefault();
            _context.Comments.Remove(comment);
            Save();
        }
        public void DeleteReply(string replyid)
        {
            var reply = _context.Replies.Where(x => x.Id == replyid).FirstOrDefault();
            _context.Replies.Remove(reply);
            Save();
        }




        public void Save()
        {
            _context.SaveChanges();
        }


        #region dispose
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion dispose
    }

}
