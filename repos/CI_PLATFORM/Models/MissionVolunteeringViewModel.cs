using CI_PLATFORM.DataDB;
namespace CI_PLATFORM.Models
{

   
    
        public class MissionVolunteeringViewModel
        {
            internal MissionMedium image;
            internal Mission Missions;
            internal GoalMission goalMissions;
        internal List<Comment> Comments;

        public List<ReleatedMissions> RelatedMissions { get; set; }
        public List<userModel> users { get; set; }
        public List<CommentModel> Comment { get; set; }

        public List<MissionDocument> MissionDocuments { get; set; }

        public List<RecentVolunteer> recentVolunteers { get; set; }
        public string Title { get; set; }

            public long UserId { get; set; }

        public string? Avatar { get; set; }

        public long MissionId { get; set; }

            public string City { get; set; }

            public string Theme { get; set; }

            public string? seatsLeft { get; set; }

            public string MissionType { get; set; }

            public string Status { get; set; }

            public string? ShortDescription { get; set; }

            public string GoalObjectiveText { get; set; }

            public string? Description { get; set; }

            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }

            public DateTime? Deadline { get; set; }

            public string? OrganizationName { get; set; }

            public string? skill { get; set; }
            public string? SkillName { get; set; }


            public bool IsFavorite { get; set; }

            public long CountryId { get; set; }

            public string isRatingExistByUser { get; set; }

            public string Rating { get; set; } = null!;

            public int avgRating { get; set; }

        public bool isApplied { get; set; }





        public class favourites
            {
                public long MissionId { get; set; }

                public long UserId { get; set; }

                public DateTime? CreatedAt { get; set; }


            }
        public class ReleatedMissions
        {
            public string Title { get; set; }
            public Mission Mission { get; internal set; }
            public GoalMission GoalMission { get; internal set; }
            public MissionMedium MissionMedium { get; internal set; }

         
            public string MediaPath { get; set; }

            public long? UserId { get; set; }

            public long? MissionId { get; set; }

            public string City { get; set; }

            public string Theme { get; set; }

            public string? SeatsLeft { get; set; }

            public string MissionType { get; set; }

            public string Status { get; set; }

            public string? ShortDescription { get; set; }

            public string GoalObjectiveText { get; set; }

            public string? Description { get; set; }

            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }

            public DateTime? Deadline { get; set; }

            public string? OrganizationName { get; set; }

            public string? skill { get; set; }

            public bool IsRFavorite { get; set; }

            public long? CountryId { get; set; }

           
        }

        public class RecentVolunteer
        {
            public string? FirstName { get; set; }

            public string? LastName { get; set; }
            public object UserId { get; internal set; }
        }

            public class CommentModel
        {
            public long? UserId { get; set; }

            public string? FirstName { get; set; }

            public string? LastName { get; set; }

            public long? MissionId { get; set; }

            public DateTime CreatedAt { get; set; }

            public string CommentText { get; set; } = null!;

           
        }

        public class userModel
        {
            public long? UserId { get; set; }

            public string? Avatar { get; set; }

            public string? FirstName { get; set; }

            public string? LastName { get; set; }

            public string Email { get; set; } = null!;

        }

        public class MissionDocument
        {
            public string? DocumentPath { get; set; }
        }


    }

       
    

   
}
