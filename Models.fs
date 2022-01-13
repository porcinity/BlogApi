module GiraffeApi.Models
open GiraffeApi.Dtos
open System

type Comment = {
    CommentId : Guid
    Content : string
    PostId : Guid
}

type Post = {
    Id : Guid
    Title : string
    Content : string
    Comments : Comment list
}

module Post =
    let fromDto (dto: PostDto) =
        { Id = Guid.NewGuid()
          Title = dto.Title
          Content = dto.Content
          Comments = [] }
        
module Comment =
    let fromDto postId (dto: CommentDto) =
        { CommentId = Guid.NewGuid()
          Content = dto.Content
          PostId = postId }