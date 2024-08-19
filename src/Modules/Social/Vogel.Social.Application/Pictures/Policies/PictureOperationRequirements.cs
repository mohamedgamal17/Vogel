namespace Vogel.Social.Application.Pictures.Policies
{
    public static class PictureOperationRequirements
    {
        public static IsPictureOwnerAuthorizationRequirmenet IsPictureOwner = new IsPictureOwnerAuthorizationRequirmenet();
    }
}
