module GiraffeApi.Handlers

open Giraffe
open GiraffeApi.Dtos
open GiraffeApi.Models
open GiraffeApi.Repository
open Microsoft.AspNetCore.Http
open Microsoft.FSharp.Collections

let getAllPostsHandler next ctx = task {
    let! all = findAllPostsAsync ()
    return! json all next ctx
}

let createBlogPost =
    fun next (ctx: HttpContext) -> task {
        let! dto = ctx.BindJsonAsync<PostDto> ()
        let newPost = Post.fromDto dto
        let! insert = insertPostAsync newPost
        return! json insert next ctx
    }
    
let getPostHandler id next ctx = task {
        let! post = findPostByIdAsync(id)
        let head = post |> List.head
        return! json head next ctx
}

let updatePostHandlerAsync postId next (ctx:HttpContext) = task {
    let! dto = ctx.BindJsonAsync<PostDto> ()
    let! found = findPostByIdAsync postId
    let head = found |> List.head
    let updatedPost =
        { Id = head.Id
          Title = dto.Title
          Content = dto.Content
          Comments = [] }
    let! result = updatePostByIdAsync updatedPost
    return! Successful.OK result next ctx
}

let deletePostHandlerAsync postId next ctx = task {
    let! result = deletePostByIdAsync postId
    return! json result next ctx
}

let getCommentsHandler postId next ctx = task {
    let! comments = findAllCommentsAsync postId
    return! json comments next ctx
}

let createCommentHandler postId next (ctx: HttpContext) = task {
    let! dto = ctx.BindJsonAsync<CommentDto> ()
    let newComment = Comment.fromDto postId dto
    let! insert = insertCommentAsync newComment
    return! json insert next ctx
}

let getCommentHandlerAsync commentId next ctx = task {
    let! comment = findCommentByIdAsync commentId
    return! json comment next ctx
}

let updateCommentHandler commentId next (ctx: HttpContext) = task {
    let! dto = ctx.BindJsonAsync<CommentDto> ()
    let! commentList = findCommentByIdAsync commentId
    let comment = commentList |> List.head
    let updatedComment = {
        CommentId = comment.CommentId
        Content = dto.Content
        PostId = comment.PostId
    }
    let! update = updateCommentAsync updatedComment
    return! json update next ctx
}

let deleteCommentHandler commentId next ctx = task {
    let! result = deleteCommentAsync commentId
    return! json result next ctx
}