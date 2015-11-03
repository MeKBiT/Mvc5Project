$(document).ready(function () {


    $('.commentMenu').css("visibility", "hidden");
    $(".commentControl").css("visibility", "hidden");
    commentVisibility(".maincomment");
    commentVisibility(".mainreply");
    commentVisibility(".childReplyCont");


    $(".commentExpCtrl").click(function () {
        var clicks = $(this).data('clicks');
        if (!clicks) {
            commentExpand(this, '.maincomment', '.userComment');
            commentExpand(this, '.mainreply', '.parentReply');
            commentExpand(this, '.childReplyCont', '.childReply');
        } else {
            commentCollapse(this, '.maincomment', '.userComment');
            commentCollapse(this, '.mainreply', '.parentReply');
            commentCollapse(this, '.childReplyCont', '.childReply');
        }
        $(this).data("clicks", !clicks);
    });


    $('.shareChild').css("display", "none");
    $('.shareChild').css("margin-left", "-40px");
    shareMenuVisibility(".shareParent");


   
    $('.postMenuSub').css('visibility', 'hidden');
    $('.postContainer').hover(
        function () {
            $('.postMenuSub').css('visibility', 'visible');
        }, function () {
            $('.postMenuSub').css('visibility', 'hidden');
        }
    );


    $('.comReplyParent').click(function () {
        var clicks = $(this).data('clicks');
        var id = $(this).attr("id");
        var parentid = $('#' + id).closest('.commentExp').attr("id");
        if (!clicks) {
            $("#" + parentid + ">.collapseComment").show();
        }
        else {
            $('.collapseComment').hide();
        }
        $(this).data("clicks", !clicks);
    });


});




function commentVisibility(parent) {
    $(parent).hover(
   function () {
       var mcId = $(this).attr("id");
       var id1 = $("#" + mcId + ">.commentControl").attr("id");
       var id2 = $("#" + mcId + ">.commentExp>.replySubMenu>.commentMenu").attr("id");
       $("#" + id1).css("visibility", "visible");
       $("#" + id2).css("visibility", "visible");
   }, function () {
       var mcId = $(this).attr("id");
       var id1 = $("#" + mcId + ">.commentControl").attr("id");
       var id2 = $("#" + mcId + ">.commentExp>.replySubMenu>.commentMenu").attr("id");
       $("#" + id1).css("visibility", "hidden");
       $("#" + id2).css("visibility", "hidden");
   });
}



function commentExpand(target, parent, grandparent) {
    var id = $(target).attr("id");
    $("#" + id).html("+");
    $("#" + id).css("font-size", "16px");
    var parentid = $(target).closest(parent).attr('id');
    var expid = $("#" + parentid + "> .commentExp").attr("id");
    var grandparentid = $("#" + parentid).closest(grandparent).attr('id');
    var repliesid = $("#" + grandparentid + "> .commentreplies").attr("id");
    $("#" + expid).css("display", "none");
    $("#" + repliesid).css("display", "none");
}
function commentCollapse(target, parent, grandparent) {
    var id = $(target).attr("id");
    $("#" + id).html("&mdash;");
    $("#" + id).css("font-size", "10px");
    var parentid = $(target).closest(parent).attr('id');
    var expid = $("#" + parentid + "> .commentExp").attr("id");
    var grandparentid = $("#" + parentid).closest(grandparent).attr('id');
    var repliesid = $("#" + grandparentid + "> .commentreplies").attr("id");
    $("#" + expid).css("display", "block");
    $("#" + repliesid).css("display", "block");
}

function shareMenuVisibility(parent) {
    $(parent).hover(
        function () {
            var mcId = $(this).attr("id");
            var id = $("#" + mcId + ">div>a").attr("id");
            $("#" + id).css({ 'display': 'block', 'opacity': '0' })
     .animate({ opacity: 1, marginLeft: 0 }, 500);
        }, function () {
            var mcId = $(this).attr("id");
            var id = $("#" + mcId + ">div>a").attr("id");
            $("#" + id).css("display", "none");
            $("#" + id).css("margin-left", "-40px");
        });
}
