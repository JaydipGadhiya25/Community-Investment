using System.Web.Mvc;

namespace CI_PLATFORM.Models
{
    public class StoriesListingViewModel
    {
       
        public List<StoriesModel> StoriesList { get; set; }

        public List<MissionList> AllMissionList { get; set; }

        public List<DraftSave> DraftSaveModel { get; set; }


        public List<Submitstory> SubmitStoryModel { get; set; }

        public List<StoriesDetail> StoriesDetailModel { get; set; }

        







        public class StoriesModel
        {
            public List<string> path { get; set; }

            public long StoryId { get; set; }

            public long missionId { get; set; }


            public string Type { get; set; } = null!;


            public long userId { get; set; }

            public string? Title { get; set; }

            public string? Description { get; set; }

            public string? FirstName { get; set; }

            public string? LastName { get; set; }

            public string? Avatar { get; set; }

            public string? MediaPath { get; set; }

            public string? ThemeTitle { get; set; }

            public bool isDraft { get; set; }

            public string missionName { get; set; }
            
            public DateTime publishedAt { get; set; }

            

        }


        public class MissionList
        {

            public List<AllMissionName> missions { get; set; }
            public long StoryId { get; set; }

            public long UserId { get; set; }

            public long MissionId { get; set; }

            public string? Title { get; set; }

            public bool isStoryExist { get; set; }


            public class AllMissionName
            {
                public long UserId { get; set; }

                public long StoryId { get; set; }

                public long MissionId { get; set; }

                public string? Title { get; set; }
            }
        }

        public class DraftSave
        {
            public long UserId { get; set; }

            public long MissionId { get; set; }

            public string? Title { get; set; }

            public string? Description { get; set; }

            public string Status { get; set; } = null!;

            public DateTime PublishedAt { get; set; }

            public DateTime CreatedAt { get; set; }

           
        }

        public class Submitstory
        {

            public long StoryId { get; set; }
            public long UserId { get; set; }

            public long MissionId { get; set; }

            public string? Title { get; set; }

            public string? Description { get; set; }

            public string Status { get; set; } = null!;

            public DateTime PublishedAt { get; set; }

            public DateTime CreatedAt { get; set; }
        }


        public class StoriesDetail
        {
            


            public List<UserModel> users { get; set; }

            public long? StoryView { get; set; } = 0;



            public long StoryId { get; set; }


            public long UserId { get; set; }

            public string Email { get; set; } = null!;


            public string? Avatar { get; set; }

            public string? FirstName { get; set; }

            public string? LastName { get; set; }

            public string MissionName { get; set; }

            public long MissionId { get; set; }

            public List<SelectListItem> path { get; set; }

            public string? Title { get; set; }

            public string? Description { get; set; }

            public string Status { get; set; } = null!;

            public DateTime PublishedAt { get; set; }

            public DateTime CreatedAt { get; set; }


            public class UserModel
            {
                public long UserId { get; set; }

                public string Email { get; set; } = null!;


                public string? Avatar { get; set; }

                public string? FirstName { get; set; }

                public string? LastName { get; set; }
            }
        }

        



    }
}
