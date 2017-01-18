using Mvc5Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc5Project.DAL
{
    public interface IBlogRepository : IDisposable
    {
        #region 1
        IList<Post> GetPosts();
        IList<Category> GetPostCategories(Post post);
        IList<Tag> GetPostTags(Post post);
        IList<PostVideo> GetPostVideos(Post post);
        int LikeDislikeCount(string typeAndlike, string id);
        IList<Tag> GetTags();
        IList<Category> GetCategories();
        #endregion

        #region 2
        Post GetPostById(string postid);
        string GetPostIdBySlug(string slug);
        void UpdatePostLike(string postid, string username, string likeordislike);
        void AddVideoToPost(string postid, string videoUrl);
        void RemoveVideoFromPost(string postid, string videoUrl);
        void AddPostCategories(PostCategory postCategory);
        void RemovePostCategories(string postid, string categoryid);
        void RemoveCategoryFromPost(string postid, string catName);
        void AddNewCategory(string catName, string catUrlSeo, string catDesc);
        void RemoveTagFromPost(string postid, string tagName);
        void AddPostTags(PostTag postTag);
        void RemovePostTags(string postid, string tagid);
        void AddNewTag(string tagName, string tagUrlSeo);
        void RemoveCategory(Category category);
        void RemoveTag(Tag tag);
        void DeletePostandComponents(string postid);
        void AddNewPost(Post post);
        #endregion

        IList<Comment> GetPostComments(Post post);
        List<CommentViewModel> GetParentReplies(Comment comment);
        List<CommentViewModel> GetChildReplies(Reply parentReply);
        Reply GetReplyById(string id);
        bool CommentDeleteCheck(string commentid);
        bool ReplyDeleteCheck(string replyid);
        void UpdateCommentLike(string commentid, string username, string likeordislike);
        void UpdateReplyLike(string replyid, string username, string likeordislike);
        Post GetPostByReply(string replyid);
        IList<Comment> GetComments();
        IList<Reply> GetReplies();
        void AddNewComment(Comment comment);
        void AddNewReply(Reply reply);
        Comment GetCommentById(string id);
        void DeleteComment(string commentid);
        void DeleteReply(string replyid);


        void Save();
    }

}
