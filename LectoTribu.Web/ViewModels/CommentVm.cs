namespace LectoTribu.Web.ViewModels;

public record CommentVm(Guid UserId, string UserName, string Content, DateTime CreatedAtUtc);

