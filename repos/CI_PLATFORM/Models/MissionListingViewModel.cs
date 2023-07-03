using CI_PLATFORM.DataDB;
using System.Collections.Generic;

namespace CI_PLATFORM.Models
{
    public class MissionListingViewModel
    {
        internal MissionMedium image;
        internal Mission Missions;
        internal GoalMission goalMissions;


        public string? Avatar { get; set; }
        public List<City> city { get; set; }

        public List<Country> country { get; set; }
        

        public List<MissionTheme> theme { get; set; }

        public List<Skill> skill { get; set; }

        public List<Mission> mission { get; set;}

        public List<MissionModel> MissionList { get; set; }

        public bool IsFavorite { get; set; }
    }

    public class MissionModel
    {
        public long MissionId { get; set; }

        public string MediaPath { get; set; }

        public string? Avatar { get; set; }

        public string City { get; set; }

        public long CountryId { get; set; }

        public string Theme { get; set; }

        public string? Title { get; set; }

        public string MissionType { get; set; }

        public string Status { get; set; }

        public string? ShortDescription { get; set; }

        public string GoalObjectiveText { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? Deadline { get; set; }

        public string? OrganizationName { get; set; }

        public string? OrganizationDetail { get; set; }

        public string? Avilability { get; set; }

        public string? SeatsLeft { get; set; }

        public string? SkillName { get; set; }

        public bool IsFavorite { get; set; }
        public object MissionSkills { get; internal set; }
        public object Skill { get; internal set; }

        public string Rating { get; set; } = null!;
    }


   

}
