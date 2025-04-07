using SocialWave.Models.AbstractClasses;

namespace SocialWave.Models.ConcreteClasses
{
    public class SavedPost
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid PostId { get; set; }
        public Post Post { get; set; }
    }
}
