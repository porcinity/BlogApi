module GiraffeApi.Handlers
open Giraffe
open GiraffeApi.Dtos
open GiraffeApi.Models
open GiraffeApi.Repository
open Microsoft.AspNetCore.Http

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