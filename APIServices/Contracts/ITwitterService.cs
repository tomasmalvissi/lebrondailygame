namespace LBJ.APIServices.Contracts;

public interface ITwitterService
{
    Task<string> PublishTweetAsync(string message);
}
