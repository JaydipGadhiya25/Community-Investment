using CI_PLATFORM.DataDB;
using CI_PLATFORM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CI_PLATFORM.Controllers
{

    [Authorize]
    public class MissionController : Controller
    {
        private readonly CiPlatformContext _db;

        public MissionController(CiPlatformContext db)
        {
            _db = db;

        }



        public IActionResult MissionListing(int? page)
        {

            var missionCount = _db.Missions.Count();
            int rowsPerPage = 03;
            int totalPages = (int)Math.Ceiling((double)missionCount / rowsPerPage);

            var pageNumber = page ?? 1; // set the current page number
            var itemsToSkip = (pageNumber - 1) * rowsPerPage;
            List<Country> countries = new List<Country>();
            countries = _db.Countries.ToList();

            List<City> cities = _db.Cities.ToList();

            List<MissionTheme> themes = new List<MissionTheme>();
            themes = _db.MissionThemes.ToList();

            List<Skill> skills = new List<Skill>();
            skills = _db.Skills.ToList();

            List<Mission> mission = _db.Missions.ToList();
            List<MissionMedium> image = _db.MissionMedia.ToList();
            List<City> city = _db.Cities.ToList();

            List<MissionTheme> theme = _db.MissionThemes.ToList();
            List<GoalMission> goalMissions = _db.GoalMissions.ToList();

            List<FavouriteMission> favouriteMissions = _db.FavouriteMissions.ToList();

            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);


            var Missions = (from m in mission
                            join i in image on m.MissionId equals i.MissionId into data
                            join g in goalMissions on m.MissionId equals g.MissionId into data1
                            join c in cities on m.CityId equals c.CityId into data2
                            join t in theme on m.ThemeId equals t.MissionThemeId into data3
                            from i in data.DefaultIfEmpty().Take(1)
                            from g in data1.DefaultIfEmpty().Take(1)
                            from c in data2.DefaultIfEmpty().Take(1)
                            from t in data3.DefaultIfEmpty().Take(1)
                            select new Models.MissionModel
                            {
                                MissionId = m.MissionId,
                                MediaPath = i.MediaPath,
                                Title = m.Title,
                                ShortDescription = m.ShortDescription,
                                StartDate = m.StartDate,
                                EndDate = m.EndDate,
                                OrganizationName = m.OrganizationName,
                                GoalObjectiveText = g.GoalObjectiveText,
                                Deadline = m.Deadline,
                                MissionType = m.MissionType,
                                SeatsLeft = m.SeatsLeft,
                                City = c.Name,
                                Theme = t.Title,
                                IsFavorite = _db.FavouriteMissions.Any(mi => mi.UserId == userId && mi.MissionId == m.MissionId)
                            }).Skip(itemsToSkip)
                .Take(rowsPerPage).ToList();

            var model = new MissionListingViewModel()
            {
                mission = mission,
                city = cities,
                country = countries,
                theme = themes,
                skill = skills,
                MissionList = Missions
            };
            return View(model);
        }


        [HttpGet]
        [Route("Mission/SearchMissions", Name = "SearchMissions")]
        public IActionResult SearchMissions(string keywords,int? page)
        {
            var missionCount = _db.Missions.Count();
            int rowsPerPage = 03;
            int totalPages = (int)Math.Ceiling((double)missionCount / rowsPerPage);

            var pageNumber = page ?? 1; // set the current page number
            var itemsToSkip = (pageNumber - 1) * rowsPerPage;
            List<Country> countries = new List<Country>();
            countries = _db.Countries.ToList();

            List<City> cities = _db.Cities.ToList();

            List<MissionTheme> themes = new List<MissionTheme>();
            themes = _db.MissionThemes.ToList();

            List<Skill> skills = new List<Skill>();
            skills = _db.Skills.ToList();

            List<Mission> mission = _db.Missions.ToList();
            List<MissionMedium> image = _db.MissionMedia.ToList();
            List<City> city = _db.Cities.ToList();

            List<MissionTheme> theme = _db.MissionThemes.ToList();
            List<GoalMission> goalMissions = _db.GoalMissions.ToList();
            var Missions = (from m in mission
                            where m.Title.ToLower().Contains(String.IsNullOrEmpty(keywords) ? String.Empty : keywords)
                            join i in image on m.MissionId equals i.MissionId into data
                            join g in goalMissions on m.MissionId equals g.MissionId into data1
                            join c in cities on m.CityId equals c.CityId into data2
                            join t in theme on m.ThemeId equals t.MissionThemeId into data3
                            from i in data.DefaultIfEmpty().Take(1)
                            from g in data1.DefaultIfEmpty().Take(1)
                            from c in data2.DefaultIfEmpty().Take(1)
                            from t in data3.DefaultIfEmpty().Take(1)
                            select new Models.MissionModel
                            {
                                MissionId = m.MissionId,
                                MediaPath = i.MediaPath,
                                Title = m.Title,
                                ShortDescription = m.ShortDescription,
                                StartDate = m.StartDate,
                                EndDate = m.EndDate,
                                OrganizationName = m.OrganizationName,
                                GoalObjectiveText = g.GoalObjectiveText,
                                Deadline = m.Deadline,
                                MissionType = m.MissionType,
                                SeatsLeft = m.SeatsLeft,
                                City = c.Name,
                                Theme = t.Title
                            }).Skip(itemsToSkip)
                .Take(rowsPerPage).ToList();

            return PartialView("_MissionListing", Missions);
        }




        [HttpGet]
        [Route("Mission/SearchCity", Name = "SearchCity")]
        public IActionResult SearchCity(string Country)
        {
           
            List<Country> countries = new List<Country>();
            countries = _db.Countries.ToList();

            List<City> cities = new List<City>();
            cities = _db.Cities.ToList();

            int countryID = (int)_db.Countries.Where(c => c.Name == Country)
                .Select(c => c.CountryId).FirstOrDefault();
            var citiesInCountry = (from city in _db.Cities
                                   join country in _db.Countries
                                   on city.CountryId equals country.CountryId
                                   where (country.CountryId == countryID)
                                   select city).ToList();



            return PartialView("_CityDropdown", citiesInCountry);
        }





        [HttpGet]
        [Route("Mission/FilterCountry", Name = "FilterCountry")]
        public IActionResult FilterCountry(string Country, int? page)
        {
            var missionCount = _db.Missions.Count();
            int rowsPerPage = 03;
            int totalPages = (int)Math.Ceiling((double)missionCount / rowsPerPage);

            var pageNumber = page ?? 1; // set the current page number
            var itemsToSkip = (pageNumber - 1) * rowsPerPage;


            List<Country> countries = new List<Country>();
            countries = _db.Countries.ToList();

            List<City> cities = new List<City>();
            cities = _db.Cities.ToList();

            List<MissionTheme> themes = new List<MissionTheme>();
            themes = _db.MissionThemes.ToList();

            List<Skill> skills = new List<Skill>();
            skills = _db.Skills.ToList();

            List<Mission> mission = _db.Missions.ToList();
            List<MissionMedium> image = _db.MissionMedia.ToList();
            List<City> city = _db.Cities.ToList();

            List<MissionTheme> theme = _db.MissionThemes.ToList();
            List<GoalMission> goalMissions = _db.GoalMissions.ToList();

            var allCities = (from City in cities
                             join country in countries on City.CountryId equals country.CountryId
                           where country.Name == Country
                           select City.Name).ToList();
           
                var Missions = (from m in mission
                                where allCities.Contains(m.City.Name)
                                join i in image on m.MissionId equals i.MissionId into data
                                join g in goalMissions on m.MissionId equals g.MissionId into data1
                                join c in cities on m.CityId equals c.CityId into data2
                                join t in theme on m.ThemeId equals t.MissionThemeId into data3
                                from i in data.DefaultIfEmpty().Take(1)
                                from g in data1.DefaultIfEmpty().Take(1)
                                from c in data2.DefaultIfEmpty().Take(1)
                                from t in data3.DefaultIfEmpty().Take(1)
                                select new Models.MissionModel
                                {
                                    MissionId = m.MissionId,
                                    MediaPath = i.MediaPath,
                                    Title = m.Title,
                                    ShortDescription = m.ShortDescription,
                                    StartDate = m.StartDate,
                                    EndDate = m.EndDate,
                                    OrganizationName = m.OrganizationName,
                                    GoalObjectiveText = g.GoalObjectiveText,
                                    MissionType = m.MissionType,
                                    Deadline = m.Deadline,
                                    SeatsLeft = m.SeatsLeft,
                                    City = c.Name,
                                    Theme = t.Title
                                }).ToList();

            return PartialView("_MissionListing", Missions);
        }





        [HttpGet]
        [Route("Mission/FilterTheme", Name = "FilterTheme")]
        public IActionResult FilterTheme(string Theme,string Country, int? page)
        {
            var missionCount = _db.Missions.Count();
            int rowsPerPage = 03;
            int totalPages = (int)Math.Ceiling((double)missionCount / rowsPerPage);

            var pageNumber = page ?? 1; // set the current page number
            var itemsToSkip = (pageNumber - 1) * rowsPerPage;
            List<Country> countries = new List<Country>();
            countries = _db.Countries.ToList();

            List<City> cities = _db.Cities.ToList();

            List<MissionTheme> themes = new List<MissionTheme>();
            themes = _db.MissionThemes.ToList();

            List<Skill> skills = new List<Skill>();
            skills = _db.Skills.ToList();

            List<Mission> mission = _db.Missions.ToList();
            List<MissionMedium> image = _db.MissionMedia.ToList();
            List<City> city = _db.Cities.ToList();

            List<MissionTheme> theme = _db.MissionThemes.ToList();
            List<GoalMission> goalMissions = _db.GoalMissions.ToList();

            int countryID = (int)_db.Countries.Where(c => c.Name == Country)
             .Select(c => c.CountryId).FirstOrDefault();

            var Missions = (from m in mission
                           
                            where (m.Theme.Title.Contains(Theme) && ( countryID == 0 || m.CountryId == countryID))
                            join i in image on m.MissionId equals i.MissionId into data
                            join g in goalMissions on m.MissionId equals g.MissionId into data1
                            join c in cities on m.CityId equals c.CityId into data2
                            join t in theme on m.ThemeId equals t.MissionThemeId into data3
                            from i in data.DefaultIfEmpty().Take(1)
                            from g in data1.DefaultIfEmpty().Take(1)
                            from c in data2.DefaultIfEmpty().Take(1)
                            from t in data3.DefaultIfEmpty().Take(1)
                            select new Models.MissionModel
                            {
                                MissionId = m.MissionId,
                                MediaPath = i.MediaPath,
                                Title = m.Title,
                                ShortDescription = m.ShortDescription,
                                StartDate = m.StartDate,
                                EndDate = m.EndDate,
                                OrganizationName = m.OrganizationName,
                                GoalObjectiveText = g.GoalObjectiveText,
                                MissionType = m.MissionType,
                                SeatsLeft = m.SeatsLeft,
                                Deadline = m.Deadline,
                                City = c.Name,
                                Theme = t.Title
                            }).Skip(itemsToSkip)
                .Take(rowsPerPage).ToList();

            return PartialView("_MissionListing", Missions);
        }






        [HttpGet]
        [Route("Mission/FilterCity", Name = "FilterCity")]
        public IActionResult FilterCity(string City, int? page)
        {
            var missionCount = _db.Missions.Count();
            int rowsPerPage = 03;
            int totalPages = (int)Math.Ceiling((double)missionCount / rowsPerPage);

            var pageNumber = page ?? 1; // set the current page number
            var itemsToSkip = (pageNumber - 1) * rowsPerPage;
            List<Country> countries = new List<Country>();
            countries = _db.Countries.ToList();

            List<City> cities = _db.Cities.ToList();

            List<MissionTheme> themes = new List<MissionTheme>();
            themes = _db.MissionThemes.ToList();

            List<Skill> skills = new List<Skill>();
            skills = _db.Skills.ToList();

            List<Mission> mission = _db.Missions.ToList();
            List<MissionMedium> image = _db.MissionMedia.ToList();
            List<City> city = _db.Cities.ToList();

            List<MissionTheme> theme = _db.MissionThemes.ToList();
            List<GoalMission> goalMissions = _db.GoalMissions.ToList();
            var Missions = (from m in mission
                            where m.City.Name.Contains(City)
                            join i in image on m.MissionId equals i.MissionId into data
                            join g in goalMissions on m.MissionId equals g.MissionId into data1
                            join c in cities on m.CityId equals c.CityId into data2
                            join t in theme on m.ThemeId equals t.MissionThemeId into data3
                            from i in data.DefaultIfEmpty().Take(1)
                            from g in data1.DefaultIfEmpty().Take(1)
                            from c in data2.DefaultIfEmpty().Take(1)
                            from t in data3.DefaultIfEmpty().Take(1)
                            select new Models.MissionModel
                            {
                                MissionId = m.MissionId,
                                MediaPath = i.MediaPath,
                                Title = m.Title,
                                ShortDescription = m.ShortDescription,
                                StartDate = m.StartDate,
                                EndDate = m.EndDate,
                                OrganizationName = m.OrganizationName,
                                GoalObjectiveText = g.GoalObjectiveText,
                                MissionType = m.MissionType,
                                SeatsLeft = m.SeatsLeft,
                                Deadline = m.Deadline,
                                City = c.Name,
                                Theme = t.Title
                            }).Skip(itemsToSkip)
                .Take(rowsPerPage).ToList();
            var filteredMissionCount = Missions.Count();
            ViewBag.FilteredMissionCount = filteredMissionCount;



            return PartialView("_MissionListing", Missions);
        }





        [HttpGet]
        [Route("Mission/FilterSkill", Name = "FilterSkill")]
        public IActionResult FilterSkill(int skillId,string Country, int? page)
        {
            var missionCount = _db.Missions.Count();
            int rowsPerPage = 03;
            int totalPages = (int)Math.Ceiling((double)missionCount / rowsPerPage);

            var pageNumber = page ?? 1; // set the current page number
            var itemsToSkip = (pageNumber - 1) * rowsPerPage;

            List<Country> countries = new List<Country>();
            countries = _db.Countries.ToList();

            List<City> cities = _db.Cities.ToList();

            List<MissionTheme> themes = new List<MissionTheme>();
            themes = _db.MissionThemes.ToList();

            List<Skill> skills = new List<Skill>();
            skills = _db.Skills.ToList();

            List<Mission> mission = _db.Missions.ToList();
            List<MissionMedium> image = _db.MissionMedia.ToList();
            List<City> city = _db.Cities.ToList();

            List<MissionTheme> theme = _db.MissionThemes.ToList();
            List<GoalMission> goalMissions = _db.GoalMissions.ToList();
            int missionID = Convert.ToInt32((from MissionSkill in _db.MissionSkills
                                  where MissionSkill.SkillId == skillId
                                  select MissionSkill.MissionId).FirstOrDefault());

            int countryID = (int)_db.Countries.Where(c => c.Name == Country)
             .Select(c => c.CountryId).FirstOrDefault();

            var Missions = (from m in mission
                            where (m.MissionId.Equals(missionID) && (countryID == 0 || m.CountryId == countryID))
                            join i in image on m.MissionId equals i.MissionId into data
                            join g in goalMissions on m.MissionId equals g.MissionId into data1
                            join c in cities on m.CityId equals c.CityId into data2
                            join t in theme on m.ThemeId equals t.MissionThemeId into data3
                            from i in data.DefaultIfEmpty().Take(1)
                            from g in data1.DefaultIfEmpty().Take(1)
                            from c in data2.DefaultIfEmpty().Take(1)
                            from t in data3.DefaultIfEmpty().Take(1)
                            select new Models.MissionModel
                            {
                                MissionId = m.MissionId,
                                MediaPath = i.MediaPath,
                                Title = m.Title,
                                ShortDescription = m.ShortDescription,
                                StartDate = m.StartDate,
                                EndDate = m.EndDate,
                                OrganizationName = m.OrganizationName,
                                GoalObjectiveText = g.GoalObjectiveText,
                                MissionType = m.MissionType,
                                SeatsLeft = m.SeatsLeft,
                                Deadline = m.Deadline,
                                City = c.Name,
                                Theme = t.Title
                            }).Skip(itemsToSkip)
                .Take(rowsPerPage).ToList();

            return PartialView("_MissionListing", Missions);
        }





        [HttpGet]
        [Route("Mission/SortByCreatedTime", Name = "SortByCreatedTime")]
        public IActionResult SortByCreatedTime(int? page)
        {
            var missionCount = _db.Missions.Count();
            int rowsPerPage = 03;
            int totalPages = (int)Math.Ceiling((double)missionCount / rowsPerPage);

            var pageNumber = page ?? 1; // set the current page number
            var itemsToSkip = (pageNumber - 1) * rowsPerPage;

            List<Country> countries = new List<Country>();
            countries = _db.Countries.ToList();

            List<City> cities = _db.Cities.ToList();

            List<MissionTheme> themes = new List<MissionTheme>();
            themes = _db.MissionThemes.ToList();

            List<Skill> skills = new List<Skill>();
            skills = _db.Skills.ToList();

            List<Mission> mission = _db.Missions.ToList();
            List<MissionMedium> image = _db.MissionMedia.ToList();
            List<City> city = _db.Cities.ToList();

            List<MissionTheme> theme = _db.MissionThemes.ToList();
            List<GoalMission> goalMissions = _db.GoalMissions.ToList();
            var Missions = (from m in mission
                           
                            join i in image on m.MissionId equals i.MissionId into data
                            join g in goalMissions on m.MissionId equals g.MissionId into data1
                            join c in cities on m.CityId equals c.CityId into data2
                            join t in theme on m.ThemeId equals t.MissionThemeId into data3
                            from i in data.DefaultIfEmpty().Take(1)
                            from g in data1.DefaultIfEmpty().Take(1)
                            from c in data2.DefaultIfEmpty().Take(1)
                            from t in data3.DefaultIfEmpty().Take(1)
                            orderby m.CreatedAt descending
                            select new Models.MissionModel
                            {
                                MissionId = m.MissionId,
                                MediaPath = i.MediaPath,
                                Title = m.Title,
                                ShortDescription = m.ShortDescription,
                                StartDate = m.StartDate,
                                EndDate = m.EndDate,
                                OrganizationName = m.OrganizationName,
                                GoalObjectiveText = g.GoalObjectiveText,
                                Deadline = m.Deadline,
                                MissionType = m.MissionType,
                                SeatsLeft = m.SeatsLeft,
                                City = c.Name,
                                Theme = t.Title
                            }).Skip(itemsToSkip)
                .Take(rowsPerPage).ToList();

            return PartialView("_MissionListing", Missions);
        }





        [HttpGet]
        [Route("Mission/SortByOldestTime", Name = "SortByOldestTime")]
        public IActionResult SortByOldestTime(int? page)
        {
            var missionCount = _db.Missions.Count();
            int rowsPerPage = 03;
            int totalPages = (int)Math.Ceiling((double)missionCount / rowsPerPage);

            var pageNumber = page ?? 1; // set the current page number
            var itemsToSkip = (pageNumber - 1) * rowsPerPage;
            List<Country> countries = new List<Country>();
            countries = _db.Countries.ToList();

            List<City> cities = _db.Cities.ToList();

            List<MissionTheme> themes = new List<MissionTheme>();
            themes = _db.MissionThemes.ToList();

            List<Skill> skills = new List<Skill>();
            skills = _db.Skills.ToList();

            List<Mission> mission = _db.Missions.ToList();
            List<MissionMedium> image = _db.MissionMedia.ToList();
            List<City> city = _db.Cities.ToList();

            List<MissionTheme> theme = _db.MissionThemes.ToList();
            List<GoalMission> goalMissions = _db.GoalMissions.ToList();
            var Missions = (from m in mission

                            join i in image on m.MissionId equals i.MissionId into data
                            join g in goalMissions on m.MissionId equals g.MissionId into data1
                            join c in cities on m.CityId equals c.CityId into data2
                            join t in theme on m.ThemeId equals t.MissionThemeId into data3
                            from i in data.DefaultIfEmpty().Take(1)
                            from g in data1.DefaultIfEmpty().Take(1)
                            from c in data2.DefaultIfEmpty().Take(1)
                            from t in data3.DefaultIfEmpty().Take(1)
                            orderby m.CreatedAt ascending
                            select new Models.MissionModel
                            {
                                MissionId = m.MissionId,
                                MediaPath = i.MediaPath,
                                Title = m.Title,
                                ShortDescription = m.ShortDescription,
                                StartDate = m.StartDate,
                                EndDate = m.EndDate,
                                OrganizationName = m.OrganizationName,
                                GoalObjectiveText = g.GoalObjectiveText,
                                Deadline = m.Deadline,
                                MissionType = m.MissionType,
                                SeatsLeft = m.SeatsLeft,
                                City = c.Name,
                                Theme = t.Title
                            }).Skip(itemsToSkip)
                .Take(rowsPerPage).ToList();

            return PartialView("_MissionListing", Missions);
        }






        [HttpGet]
        [Route("Mission/SortByLowestSeats", Name = "SortByLowestSeats")]
        public IActionResult SortByLowestSeats(int? page)
        {
            var missionCount = _db.Missions.Count();
            int rowsPerPage = 03;
            int totalPages = (int)Math.Ceiling((double)missionCount / rowsPerPage);

            var pageNumber = page ?? 1; // set the current page number
            var itemsToSkip = (pageNumber - 1) * rowsPerPage;
            List<Country> countries = new List<Country>();
            countries = _db.Countries.ToList();

            List<City> cities = _db.Cities.ToList();

            List<MissionTheme> themes = new List<MissionTheme>();
            themes = _db.MissionThemes.ToList();

            List<Skill> skills = new List<Skill>();
            skills = _db.Skills.ToList();

            List<Mission> mission = _db.Missions.ToList();
            List<MissionMedium> image = _db.MissionMedia.ToList();
            List<City> city = _db.Cities.ToList();

            List<MissionTheme> theme = _db.MissionThemes.ToList();
            List<GoalMission> goalMissions = _db.GoalMissions.ToList();
            var Missions = (from m in mission

                            join i in image on m.MissionId equals i.MissionId into data
                            join g in goalMissions on m.MissionId equals g.MissionId into data1
                            join c in cities on m.CityId equals c.CityId into data2
                            join t in theme on m.ThemeId equals t.MissionThemeId into data3
                            from i in data.DefaultIfEmpty().Take(1)
                            from g in data1.DefaultIfEmpty().Take(1)
                            from c in data2.DefaultIfEmpty().Take(1)
                            from t in data3.DefaultIfEmpty().Take(1)
                            orderby m.SeatsLeft ascending
                            select new Models.MissionModel
                            {
                                MissionId = m.MissionId,
                                MediaPath = i.MediaPath,
                                Title = m.Title,
                                ShortDescription = m.ShortDescription,
                                StartDate = m.StartDate,
                                EndDate = m.EndDate,
                                OrganizationName = m.OrganizationName,
                                GoalObjectiveText = g.GoalObjectiveText,
                                Deadline = m.Deadline,
                                MissionType = m.MissionType,
                                SeatsLeft = m.SeatsLeft,
                                City = c.Name,
                                Theme = t.Title
                            }).Skip(itemsToSkip)
                .Take(rowsPerPage).ToList();

            return PartialView("_MissionListing", Missions);
        }





        [HttpGet]
        [Route("Mission/SortByHighestSeats", Name = "SortByHighest")]
        public IActionResult SortByHighestSeats(int? page)
        {
            var missionCount = _db.Missions.Count();
            int rowsPerPage = 03;
            int totalPages = (int)Math.Ceiling((double)missionCount / rowsPerPage);

            var pageNumber = page ?? 1; // set the current page number
            var itemsToSkip = (pageNumber - 1) * rowsPerPage;
            List<Country> countries = new List<Country>();
            countries = _db.Countries.ToList();

            List<City> cities = _db.Cities.ToList();

            List<MissionTheme> themes = new List<MissionTheme>();
            themes = _db.MissionThemes.ToList();

            List<Skill> skills = new List<Skill>();
            skills = _db.Skills.ToList();

            List<Mission> mission = _db.Missions.ToList();
            List<MissionMedium> image = _db.MissionMedia.ToList();
            List<City> city = _db.Cities.ToList();

            List<MissionTheme> theme = _db.MissionThemes.ToList();
            List<GoalMission> goalMissions = _db.GoalMissions.ToList();
            var Missions = (from m in mission

                            join i in image on m.MissionId equals i.MissionId into data
                            join g in goalMissions on m.MissionId equals g.MissionId into data1
                            join c in cities on m.CityId equals c.CityId into data2
                            join t in theme on m.ThemeId equals t.MissionThemeId into data3
                            from i in data.DefaultIfEmpty().Take(1)
                            from g in data1.DefaultIfEmpty().Take(1)
                            from c in data2.DefaultIfEmpty().Take(1)
                            from t in data3.DefaultIfEmpty().Take(1)
                            orderby m.SeatsLeft descending
                            select new Models.MissionModel
                            {
                                MissionId = m.MissionId,
                                MediaPath = i.MediaPath,
                                Title = m.Title,
                                ShortDescription = m.ShortDescription,
                                StartDate = m.StartDate,
                                EndDate = m.EndDate,
                                OrganizationName = m.OrganizationName,
                                GoalObjectiveText = g.GoalObjectiveText,
                                Deadline = m.Deadline,
                                MissionType = m.MissionType,
                                SeatsLeft = m.SeatsLeft,
                                City = c.Name,
                                Theme = t.Title
                            }).Skip(itemsToSkip)
                .Take(rowsPerPage).ToList();

            return PartialView("_MissionListing", Missions);
        }





        [HttpGet]
        [Route("Mission/SortByDeadline", Name = "SortByDeadline")]
        public IActionResult SortByDeadline(int? page)
        {
            var missionCount = _db.Missions.Count();
            int rowsPerPage = 03;
            int totalPages = (int)Math.Ceiling((double)missionCount / rowsPerPage);

            var pageNumber = page ?? 1; // set the current page number
            var itemsToSkip = (pageNumber - 1) * rowsPerPage;
            List<Country> countries = new List<Country>();
            countries = _db.Countries.ToList();

            List<City> cities = _db.Cities.ToList();

            List<MissionTheme> themes = new List<MissionTheme>();
            themes = _db.MissionThemes.ToList();

            List<Skill> skills = new List<Skill>();
            skills = _db.Skills.ToList();

            List<Mission> mission = _db.Missions.ToList();
            List<MissionMedium> image = _db.MissionMedia.ToList();
            List<City> city = _db.Cities.ToList();

            List<MissionTheme> theme = _db.MissionThemes.ToList();
            List<GoalMission> goalMissions = _db.GoalMissions.ToList();
            var Missions = (from m in mission

                            join i in image on m.MissionId equals i.MissionId into data
                            join g in goalMissions on m.MissionId equals g.MissionId into data1
                            join c in cities on m.CityId equals c.CityId into data2
                            join t in theme on m.ThemeId equals t.MissionThemeId into data3
                            from i in data.DefaultIfEmpty().Take(1)
                            from g in data1.DefaultIfEmpty().Take(1)
                            from c in data2.DefaultIfEmpty().Take(1)
                            from t in data3.DefaultIfEmpty().Take(1)
                            orderby m.Deadline ascending
                            select new Models.MissionModel
                            {
                                MissionId = m.MissionId,
                                MediaPath = i.MediaPath,
                                Title = m.Title,
                                ShortDescription = m.ShortDescription,
                                StartDate = m.StartDate,
                                EndDate = m.EndDate,
                                OrganizationName = m.OrganizationName,
                                GoalObjectiveText = g.GoalObjectiveText,
                                Deadline = m.Deadline,
                                MissionType = m.MissionType,
                                SeatsLeft = m.SeatsLeft,
                                City = c.Name,
                                Theme = t.Title
                            }).Skip(itemsToSkip)
                .Take(rowsPerPage).ToList();

            return PartialView("_MissionListing", Missions);
        }






       


        [HttpGet]
        [Route("Mission/AddToFavorites", Name = "AddToFavorites")]
        public IActionResult AddToFavorites(int missionId, int userId)
        {
            var existingFavorite = _db.FavouriteMissions.FirstOrDefault(fm => fm.MissionId == missionId && fm.UserId == userId);

            if (existingFavorite == null)
            {
                if (ModelState.IsValid)
                {
                    FavouriteMission favourite = new FavouriteMission()
                    {
                        MissionId = missionId,
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow,
                    };

                    _db.FavouriteMissions.Add(favourite);
                    _db.SaveChanges();
                    return Ok(new { success = true, message = "Favorite added successfully" });
                }

                else
                {

                    return View();

                }
            }
            else
            {
                _db.FavouriteMissions.Remove(existingFavorite);
                _db.SaveChanges();
                return Ok(new { success = true, message = "Favorite removed successfully" });
            }

        }

     



        public IActionResult _NoMissionFound()
        {
            return View();
        }



        
    }
}
