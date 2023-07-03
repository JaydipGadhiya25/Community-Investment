using CI_PLATFORM.DataDB;
using CI_PLATFORM.Models;
using CI_PLATFORM.REPOSITORY.INTERFACE;
using System.Security.Claims;
using System.Text;
using System.Web.Mvc;

namespace CI_PLATFORM.REPOSITORY.SERVICES
{
    public class StoriesListing : IStoriesListing
    {
        private readonly DataDB.CiPlatformContext _db;
       

        public StoriesListing(DataDB.CiPlatformContext db)
        {
            _db = db;

        }
        public List<StoriesListingViewModel.StoriesModel> GetStoriesCard(int? page,int? userId)
        {

            var StoriesCount = _db.Stories.Count();
            int rowsPerPage = 06;
            int totalPages = (int)Math.Ceiling((double)StoriesCount / rowsPerPage);

            var pageNumber = page ?? 1; // set the current page number
            var itemsToSkip = (pageNumber - 1) * rowsPerPage;
            var userID = userId;


            List<DataDB.Story> story = _db.Stories.ToList();
            List<DataDB.Mission> mission = _db.Missions.ToList();
            List<User> users = _db.Users.ToList();
            List<MissionMedium> image = _db.MissionMedia.ToList();
            List<MissionTheme> missionThemes = _db.MissionThemes.ToList();
            List<StoryMedium> storyMedia = _db.StoryMedia.ToList();


            

             var Stories = (from s in story
                            where  s.Status == "PUBLISHED" || s.UserId == userId && s.Status == "DRAFT"
                           join i in image on s.MissionId equals i.MissionId into data1
                           join u in users on s.UserId equals u.UserId into data2
                           join m in mission on s.MissionId equals m.MissionId into data3
                           join t in missionThemes on data3.FirstOrDefault().ThemeId equals t.MissionThemeId into data4
                           join sm in storyMedia on s.StoryId equals sm.StoryId into data5

                           from i in data1.DefaultIfEmpty().Take(1)
                           from u in data2.DefaultIfEmpty().Take(1)
                           from m in data3.DefaultIfEmpty().Take(1)
                           from t in data4.DefaultIfEmpty().Take(1)
                           




                            select new Models.StoriesListingViewModel.StoriesModel
                           {
                               Title = s.Title,
                               Description = s.Description,
                               FirstName = u.FirstName,
                               LastName = u.LastName,
                               Avatar = u.Avatar,
                               MediaPath = i.MediaPath,
                               ThemeTitle = t.Title,
                               isDraft = _db.Stories.Any(d => d.StoryId == s.StoryId && d.UserId == userID && d.Status == "DRAFT"),
                               missionName = m.Title,
                               publishedAt = s.PublishedAt,
                               missionId = s.MissionId,
                               path = data5.Select(sm => sm.Path).ToList(),
                               StoryId = s.StoryId,

                               

                           }).Skip(itemsToSkip).Take(rowsPerPage).ToList();

            return Stories;
        }


        public List<StoriesListingViewModel.StoriesModel> SearchStories(string keywords)
        {

            //var StoriesCount = _db.Stories.Count();
            //int rowsPerPage = 03;
            //int totalPages = (int)Math.Ceiling((double)StoriesCount / rowsPerPage);

            //var pageNumber = page ?? 1; // set the current page number
            //var itemsToSkip = (pageNumber - 1) * rowsPerPage;


            List<DataDB.Story> story = _db.Stories.ToList();
            List<DataDB.Mission> mission = _db.Missions.ToList();
            List<User> users = _db.Users.ToList();
            List<MissionMedium> image = _db.MissionMedia.ToList();
            List<MissionTheme> missionThemes = _db.MissionThemes.ToList();
            List<StoryMedium> storyMedia = _db.StoryMedia.ToList();

            var Stories = (from s in story
                           where s.Title.ToLower().Contains(String.IsNullOrEmpty(keywords) ? String.Empty : keywords)
                           join i in image on s.MissionId equals i.MissionId into data1
                           join u in users on s.UserId equals u.UserId into data2
                           join m in mission on s.MissionId equals m.MissionId into data3
                           join t in missionThemes on data3.FirstOrDefault().ThemeId equals t.MissionThemeId into data4

                           from i in data1.DefaultIfEmpty().Take(1)
                           from u in data2.DefaultIfEmpty().Take(1)
                           from m in data3.DefaultIfEmpty().Take(1)
                           from t in data4.DefaultIfEmpty().Take(1)


                           select new Models.StoriesListingViewModel.StoriesModel
                           {
                               Title = s.Title,
                               Description = s.Description,
                               FirstName = u.FirstName,
                               LastName = u.LastName,
                               Avatar = u.Avatar,
                               MediaPath = i.MediaPath,
                               ThemeTitle = t.Title,


                           }).ToList();

            return Stories;
        }



        public StoriesListingViewModel.MissionList GetAllMission(int userId)
        {
            List<Mission> missions = _db.Missions.ToList();
            List<MissionApplication> missionApplications = _db.MissionApplications.ToList();
            List<Story> story = _db.Stories.ToList();



            var Mission = (from m in missions
                           join a in missionApplications on m.MissionId equals a.MissionId into data1
                           join s in story on m.MissionId equals s.MissionId into data2


                           from a in data1.DefaultIfEmpty().Take(6)
                           from s in data2.DefaultIfEmpty().Take(1)

                           where (a != null && a.UserId == userId)

                           select new Models.StoriesListingViewModel.MissionList.AllMissionName
                           {
                               Title = m.Title,
                               MissionId = m.MissionId,
                               UserId = userId,
                               StoryId = s.StoryId,
                               
                           }

                ).ToList();
            

            return new StoriesListingViewModel.MissionList()
            {
               missions = Mission,
              
            };
        }



        public  StoriesListingViewModel.DraftSave EditDraft(int userID, int missionId,string status, string title, DateTime publishedAt , string description, List<string> filePaths,string url)
        {
            var isExistStory = _db.Stories.Where(s => s.UserId == userID && s.MissionId == missionId).FirstOrDefault();

           
                isExistStory.UserId = userID;
                isExistStory.MissionId = missionId;
                isExistStory.Status = status;
                isExistStory.Title = title;
                isExistStory.PublishedAt = publishedAt;
                isExistStory.CreatedAt = DateTime.UtcNow;
                isExistStory.Description = description;
                

                _db.Stories.Update(isExistStory);
                _db.SaveChanges();


                var storyid = isExistStory.StoryId;

                var existingitem = _db.StoryMedia
    .Where(sm => sm.StoryId == storyid && sm.Type == ".jpg")
    .ToList();
            
                if (existingitem != null)
                {
                    foreach (var photo in existingitem)
                    {
                        _db.StoryMedia.Remove(photo);
                    }
                }

            var isExisturl = _db.StoryMedia.Where(s => s.StoryId == storyid && s.Type == ".url" ).FirstOrDefault();

            if(isExisturl != null)
            {
                isExisturl.Path = url;
                _db.StoryMedia.Update(isExisturl);
                _db.SaveChanges();
            }

         


               foreach (var item in filePaths)
            {
                DataDB.StoryMedium image = new StoryMedium()
                {

                    Path = item,
                    StoryId = storyid,
                    CreatedAt = DateTime.UtcNow,
                    Type = ".jpg",

               
            };
            _db.StoryMedia.Add(image);
        }

        _db.SaveChanges();
            return new StoriesListingViewModel.DraftSave()
            {
                UserId = userID,
                MissionId = missionId,
                Status = status,
                Title = title,
                PublishedAt = publishedAt,
                Description = description,
             
                
                
            };

        }


        public StoriesListingViewModel.DraftSave SaveDraft(int userID, int missionId, string status, string title, DateTime publishedAt, string description, List<string> filePaths, string url)
        {



            DataDB.Story story = new Story()
            {
                UserId = userID,
                MissionId = missionId,
                Status = status,
                Title = title,
                PublishedAt = publishedAt,
                CreatedAt = DateTime.UtcNow,
                Description = description,
            };

            _db.Stories.Add(story);
            _db.SaveChanges();


            var storyid = story.StoryId;


            DataDB.StoryMedium image = new StoryMedium()
            {
                Path = url,
                StoryId = storyid,
                CreatedAt = DateTime.UtcNow,
                Type = ".url",
            };
            _db.StoryMedia.Add(image);
            _db.SaveChanges();

            foreach (var path in filePaths)
            {


                image.Path = path;
                image.StoryId = storyid;
                image.CreatedAt = DateTime.UtcNow;
                image.Type = ".jpg";

                _db.StoryMedia.Add(image);
            }

            
            _db.SaveChanges();
        
        


            return new StoriesListingViewModel.DraftSave()
            {
                UserId = userID,
                MissionId = missionId,
                Status = status,
                Title = title,
                PublishedAt = publishedAt,
                Description = description,



            };
        }


        public StoriesListingViewModel.Submitstory SubmitStory(int storyID, int missionId, string status)
        {
            var isExistStory = _db.Stories.Where(s => s.StoryId == storyID && s.MissionId == missionId).FirstOrDefault();

            if (isExistStory != null)
            {


                isExistStory.Status = status;
                isExistStory.CreatedAt = DateTime.UtcNow;
                _db.Stories.Update(isExistStory);
                _db.SaveChanges();
            }
           
            return new StoriesListingViewModel.Submitstory()
            {
                StoryId = storyID,
                MissionId = missionId,
                Status = status,
                
            };

        }



        public StoriesListingViewModel.StoriesDetail GetStoriesDetail(string id)
        {

            byte[] idBytes = Convert.FromBase64String(id);
            string idString = Encoding.UTF8.GetString(idBytes);
            int decryptedId = int.Parse(idString);


            List<DataDB.Story> story = _db.Stories.ToList();
            List<DataDB.Mission> mission = _db.Missions.ToList();
            List<User> users = _db.Users.ToList();
            List<MissionMedium> image = _db.MissionMedia.ToList();
            List<StoryMedium> StoryMedia = _db.StoryMedia.ToList();
            List<StoryMedium> storyMedia = _db.StoryMedia.ToList();


            List<StoriesListingViewModel.StoriesDetail.UserModel> userModels = users.Select(u => new StoriesListingViewModel.StoriesDetail.UserModel
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Avatar = u.Avatar,


            }).ToList();
            var storycount = _db.Stories.FirstOrDefault(s => s.StoryId == decryptedId);

            if (story != null)
            {
                // Increment the total views of the story
                storycount.StoryView++;

                // Save the changes to the database
                _db.SaveChanges();
            }

            var Stories = (from s in story
                           where s.StoryId.Equals(decryptedId)
                           join i in image on s.MissionId equals i.MissionId into data1
                           join u in users on s.UserId equals u.UserId into data2
                           join m in mission on s.MissionId equals m.MissionId into data3
                           join sm in storyMedia on s.StoryId equals sm.StoryId into data4

                           from i in data1.DefaultIfEmpty().Take(1)
                           from u in data2.DefaultIfEmpty().Take(1)
                           from m in data3.DefaultIfEmpty().Take(1)
                           from sm in data4.DefaultIfEmpty().Take(1)

            


            select new Models.StoriesListingViewModel.StoriesDetail
                           {
                               Title = s.Title,
                               Description = s.Description,
                               Avatar = u.Avatar,
                               FirstName = u.FirstName,
                               LastName = u.LastName,
                               MissionName = m.Title,
                               path = data4.Select<StoryMedium,SelectListItem>(sm =>
                               {
                                   return new SelectListItem
                                   {
                                       Text = sm.Path,
                                       Value = sm.Type
                                   };
                               }).ToList(),
                               Email = u.Email,
                               users = userModels,
                               StoryId = s.StoryId,
                               MissionId = s.MissionId,
                               StoryView = s.StoryView,
                               

            }).FirstOrDefault();



            return Stories;
        }


       
    }
}
