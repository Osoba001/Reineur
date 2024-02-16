namespace Users.Application.Requests.UserRequests
{
    public class FetchUsersRequest : Request
    {
        public required int PageSize { get; set; }
        public required int Page { get; set; }
    }
}
