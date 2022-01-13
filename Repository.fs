module GiraffeApi.Repository
open GiraffeApi.Models
open Npgsql.FSharp
open System

let connStr = "Host=localhost;Database=blog;Username=anthony;Password=itb"

// Posts table
let findAllPostsAsync () =
    connStr
    |> Sql.connect
    |> Sql.query "select * from posts"
    |> Sql.executeAsync (fun read ->
        {
            Id = read.uuid "post_id"
            Title = read.text "post_title"
            Content = read.text "post_content"
            Comments = []
        })

let findPostByIdAsync (postId: Guid) =
    connStr
    |> Sql.connect
    |> Sql.query "select post_id, post_content, post_title from posts where post_id = @postId"
    |> Sql.parameters ["@postId", Sql.uuid postId]
    |> Sql.executeAsync (fun x ->
        {
            Id = x.uuid "post_id"
            Title = x.text "post_title"
            Content = x.text "post_content"
            Comments = []
        })
    
let insertPostAsync post =
    connStr
    |> Sql.connect
    |> Sql.query "insert into posts
                 (post_id, post_title, post_content)
                 values (@id, @title, @content)"
    |> Sql.parameters [
        "@id", Sql.uuid post.Id
        "@title", Sql.text post.Title
        "@content", Sql.text post.Content
    ]
    |> Sql.executeNonQueryAsync
    
let updatePostByIdAsync post =
    connStr
    |> Sql.connect
    |> Sql.query "update posts set post_title = @title, post_content = @content
                  where post_id = @postId"
    |> Sql.parameters [
        "@title", Sql.text post.Title
        "@content", Sql.text post.Content
        "@postId", Sql.uuid post.Id
    ]
    |> Sql.executeNonQueryAsync
    
let deletePostByIdAsync postId =
    connStr
    |> Sql.connect
    |> Sql.query "delete from posts where post_id = @postId"
    |> Sql.parameters ["@postId", Sql.uuid postId]
    |> Sql.executeNonQueryAsync
    
// Comments Table
let findAllCommentsAsync postId =
    connStr
    |> Sql.connect
    |> Sql.query "select comment_id, comment_content, c.post_id
                 from posts p
                 join comments c on p.post_id = c.post_id
                 where c.post_id = @postId"
    |> Sql.parameters ["@postId", Sql.uuid postId]
    |> Sql.executeAsync (fun read ->
        {
              CommentId = read.uuid "comment_id"
              Content = read.string "comment_content"
              PostId = read.uuid "post_id"
        })
    
let findCommentByIdAsync commentId =
    connStr
    |> Sql.connect
    |> Sql.query "select comment_id, comment_content from comments where comment_id = @id"
    |> Sql.parameters ["@id", Sql.uuid commentId]
    |> Sql.executeAsync (fun read ->
        {
            CommentId = read.uuid "comment_id"
            Content = read.text "comment_content"
            PostId = read.uuid "post_id"
        })
    
let insertCommentAsync comment =
    connStr
    |> Sql.connect
    |> Sql.query "insert into comments (comment_id, comment_content, post_id) values (@id, @content, @pid)"
    |> Sql.parameters [
        "@id", Sql.uuid comment.CommentId
        "@content", Sql.text comment.Content
        "@pid", Sql.uuid comment.PostId
    ]
    |> Sql.executeNonQueryAsync
    
let deleteCommentAsync commentId =
    connStr
    |> Sql.connect
    |> Sql.query "delete from comments where comment_id = @id"
    |> Sql.parameters ["@id", Sql.uuid commentId]
    |> Sql.executeNonQueryAsync
    
let mapCommentsToPost post (commentList: Comment list) =
    let posto = post |> List.head
    { posto with
        Comments =
        commentList
        |> List.append posto.Comments }