namespace Vogel.Social.MongoEntities.Users
{
    public class UserRelationshipView
    {
        public string Id { get; set; }
        public List<UserFriendView> Friends { get; set; }
    }

    public class UserFriendView
    {
        public string Id { get; set; }

        public string UserId { get; set; }
    }
}
