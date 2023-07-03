using CI_PLATFORM.DataDB;
using CI_PLATFORM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using static CI_PLATFORM.Models.MissionVolunteeringViewModel;

namespace CI_PLATFORM.Controllers
{
    public class MissionVolunteeringController : Controller
    {
        private readonly CiPlatformContext _db;
        private readonly IConfiguration _configuration;

        public MissionVolunteeringController(CiPlatformContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;

        }
        public IActionResult MissionVolunteering(string id)
        {
            byte[] idBytes = Convert.FromBase64String(id);
            string idString = Encoding.UTF8.GetString(idBytes);
            int decryptedId = int.Parse(idString);


            //var mission = _db.Missions.Where(m => m.MissionId == id).FirstOrDefault();

            List<Mission> mission = _db.Missions.ToList();
            List<MissionMedium> image = _db.MissionMedia.ToList();
            List<User> users = _db.Users.ToList();
            List<City> city = _db.Cities.ToList();
            List<MissionSkill> missionskill = _db.MissionSkills.ToList();
            List<Skill> skill = _db.Skills.ToList();
            List<Comment> comment = _db.Comments.ToList();
            List<MissionTheme> theme = _db.MissionThemes.ToList();
            List<GoalMission> goalMissions = _db.GoalMissions.ToList();
            List<MissionRating> Ratings = _db.MissionRatings.ToList();
            List<MissionApplication> missionapplications = _db.MissionApplications.ToList();
            List<DataDB.MissionDocument> missionDocuments = _db.MissionDocuments.ToList();

            var Missions = (from m in mission
                            where m.MissionId.Equals(decryptedId)
                            join i in image on m.MissionId equals i.MissionId into data
                            join g in goalMissions on m.MissionId equals g.MissionId into data1
                            join c in city on m.CityId equals c.CityId into data2
                            join t in theme on m.ThemeId equals t.MissionThemeId into data3
                            join ms in missionskill on m.MissionId equals ms.MissionId into data4
                            join s in skill on data4.FirstOrDefault().SkillId equals s.SkillId into data5
                            join u in users on m.MissionId equals u.UserId into data6
                            


                            from i in data.DefaultIfEmpty().Take(1)
                            from g in data1.DefaultIfEmpty().Take(1)
                            from c in data2.DefaultIfEmpty().Take(1)
                            from t in data3.DefaultIfEmpty().Take(1)
                            from ms in data4.DefaultIfEmpty().Take(1)
                            from s in data5.DefaultIfEmpty().Take(1)
                           

                            select new Models.MissionModel
                            {
                                CountryId = m.CountryId,
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
                                MissionSkills = data4.ToList(),
                                SkillName = s.SkillName,
                                
                               



                            }).FirstOrDefault();
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);


            var relatedMissions = (from m in mission
                                   join g in goalMissions on m.MissionId equals g.MissionId into data1
                                   join i in image on m.MissionId equals i.MissionId into data2
                                   join t in theme on m.ThemeId equals t.MissionThemeId into data3
                                   from g in data1.DefaultIfEmpty()
                                   from i in data2.DefaultIfEmpty()
                                   from t in data3.DefaultIfEmpty()
                                   where (m.City.Name == Missions.City || m.CountryId == Missions.CountryId || m.Theme.Title == Missions.Theme) && m.MissionId != Missions.MissionId
                                   select new Models.MissionVolunteeringViewModel.ReleatedMissions
                                   {
                                       CountryId = m.CountryId,
                                       MissionId = m.MissionId,
                                       Title = m.Title,
                                       ShortDescription = m.ShortDescription,
                                       StartDate = m.StartDate,
                                       EndDate = m.EndDate,
                                       OrganizationName = m.OrganizationName,
                                       Deadline = m.Deadline,
                                       MissionType = m.MissionType,
                                       SeatsLeft = m.SeatsLeft,
                                       City = m.City.Name,
                                       MediaPath = i.MediaPath,
                                       Theme = t.Title,
                                       IsRFavorite = _db.FavouriteMissions.Any(mi => mi.UserId == userId && mi.MissionId == m.MissionId),
                                       GoalObjectiveText = g.GoalObjectiveText,



                                   }).Take(3);




            var comments = (from c in comment
                            join u in users on c.UserId equals u.UserId into data1

                            from u in data1.DefaultIfEmpty()
                            orderby c.CreatedAt descending

                            select new Models.MissionVolunteeringViewModel.CommentModel
                            {

                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                CommentText = c.CommentText,
                                CreatedAt = c.CreatedAt,

                            }).Take(5).ToList();
          
            var recentvolunteers = (from r in missionapplications
                                  
                                    join u in users on r.UserId equals u.UserId into data1
                                    

                            from u in data1.DefaultIfEmpty()
                            where (r.UserId != userId)
                            select new Models.MissionVolunteeringViewModel.RecentVolunteer
                            {
                                UserId = r.UserId,
                                FirstName = u.FirstName,
                                LastName = u.LastName,


                            }).DistinctBy(m => m.UserId).Take(1)
                                                    .ToList();


            var document = (from d in missionDocuments


                            select new Models.MissionVolunteeringViewModel.MissionDocument
                                    {
                                       DocumentPath = d.DocumentPath,


                                    }).ToList();


            List<userModel> userModels = users.Select(u => new userModel
            {
               FirstName = u.FirstName,
               LastName = u.LastName,
               Email = u.Email,
               Avatar = u.Avatar,


            }).ToList();


            //var avgrating = _db.MissionRatings.Where(r => r.MissionId == Missions.MissionId).Average(r => Convert.ToDouble(r.Rating)).DefaultIfEmpty(0);
            var avgratingrow = _db.MissionRatings.Where(r => r.MissionId == Missions.MissionId).FirstOrDefault();
            var avgRating = 0;
            if (avgratingrow != null)
            {
                 avgRating = (int)_db.MissionRatings.Average(r => Convert.ToDouble(r.Rating));
                
            }

            var model = new Models.MissionVolunteeringViewModel
            {

                Title = Missions.Title,
                ShortDescription = Missions.ShortDescription,
                seatsLeft = Missions.SeatsLeft,
                OrganizationName = Missions.OrganizationName,
                City = Missions.City,
                Theme = Missions.Theme,
                StartDate = Missions.StartDate,
                EndDate = Missions.EndDate,
                GoalObjectiveText = Missions.GoalObjectiveText,
                Status = Missions.Status,
                MissionType = Missions.MissionType,
                Deadline = Missions.Deadline,
                MissionId = Missions.MissionId,
                IsFavorite = _db.FavouriteMissions.Any(m => m.UserId == userId && m.MissionId == decryptedId),
                CountryId = Missions.CountryId,
                RelatedMissions = relatedMissions.ToList(),
                Comment = comments,
                users = userModels,
                SkillName = Missions.SkillName,
                Rating = _db.MissionRatings.OrderBy(r => r.CreatedAt).LastOrDefault(r => r.MissionId == Missions.MissionId && r.UserId == userId)?.Rating,
                avgRating = avgRating,
                isApplied = _db.MissionApplications.Any(a => a.MissionId == Missions.MissionId && a.UserId == userId),
                recentVolunteers = recentvolunteers,
                MissionDocuments = document,
                
            };
           

            return View(model);
        }

        [HttpPost]
        [Route("PostComment", Name = "PostComment")]
        public IActionResult PostComment(int missionId, int userId, string commentText)
        {

            var model = new MissionVolunteeringViewModel.CommentModel {
                CommentText = commentText ,
                MissionId = missionId ,


            };

            // Save the comment to the database using the provided mission ID, user ID, and comment text
            if (ModelState.IsValid)
            {
                TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                Comment comment = new Comment()
                {
                    MissionId = missionId,
                    UserId = userId,
                    CommentText = commentText,
                    CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, istZone),
                };

                _db.Comments.Add(comment);
                _db.SaveChanges();
                List<Comment> commentList = _db.Comments.ToList();
                List<User> users = _db.Users.ToList();
                var comments = (from c in commentList
                                join u in users on c.UserId equals u.UserId into data1
                                orderby c.CreatedAt descending
                                from u in data1.DefaultIfEmpty()

                                select new Models.MissionVolunteeringViewModel.CommentModel
                                {

                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    CommentText = c.CommentText,
                                    CreatedAt = c.CreatedAt,

                                }).Take(5).ToList();

                return PartialView("_CommentSection", comments);
            }

            else
            {

                return Ok(new { success = true, message = "!! Error" });

            }
            
        }

        [HttpGet]
        [Route("LoadMoreComments", Name = "LoadMoreComments")]
        public IActionResult LoadMoreComments(int clickCount)
        {
            var itemtoskip = 5 * clickCount;
                List<Comment> commentList = _db.Comments.ToList();
                List<User> users = _db.Users.ToList();
                var comments = (from c in commentList
                                join u in users on c.UserId equals u.UserId into data1
                                orderby c.CreatedAt descending
                                from u in data1.DefaultIfEmpty()

                                select new Models.MissionVolunteeringViewModel.CommentModel
                                {

                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    CommentText = c.CommentText,
                                    CreatedAt = c.CreatedAt,

                                }).Skip(itemtoskip).Take(5).ToList();

                return PartialView("_CommentSection", comments);
          
        }


        [HttpGet]
        [Route("MissionVolunteering/favorites", Name = "favorites")]
        public IActionResult favorites(int missionId, int userId)
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
                    return Ok(new { success = true , message = "Favorite added successfully" });
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


        [HttpPost]
        [Route("/MissionVolunteering/Rating", Name = "Rating")]
        public IActionResult Rating(string rating, int missionId, int userId)
        {
            if (ModelState.IsValid)
            {
               
                var isRatingExistByUser = _db.MissionRatings.Where(r => r.MissionId == missionId && r.UserId == userId).FirstOrDefault();
               
                if (isRatingExistByUser != null)
                {

                    int ratingValue = int.Parse(rating);
                    isRatingExistByUser.Rating = ratingValue.ToString();
                    _db.MissionRatings.Update(isRatingExistByUser);
                    _db.SaveChanges();
                    return Ok(new { success = true, message = "update added successfully" });
                }
                else
                {
                    MissionRating Rating = new MissionRating()
                    {
                        MissionId = missionId,
                        UserId = userId,
                        Rating = rating,
                        CreatedAt = DateTime.UtcNow,
                    };

                    _db.MissionRatings.Add(Rating);
                    _db.SaveChanges();
                    return Ok(new { success = true, message = "Favorite added successfully" });
                }

                }

            else
            {

                return Ok(new { success = true, message = "error " });

            }
        }

       
        
        public IActionResult Recommend(string Mail,int id)
        {
            string UserName = _configuration.GetSection("credential")["FromEmail"];
            string MyPassword = _configuration.GetSection("credential")["MyPassword"];
            string Port = _configuration.GetSection("credential")["Port"];
            string FromEmail = _configuration.GetSection("credential")["FromEmail"];
            string Host = _configuration.GetSection("credential")["Host"];

            // Generate a unique password reset token

            //string token = Guid.NewGuid().ToString();
            // Construct the reset password link with the token
            var resetLink = Url.Action("MissionVolunteering", "MissionVolunteering", new { id = Convert.ToBase64String(Encoding.UTF8.GetBytes(id.ToString())) }, Request.Scheme);

            // Send the email with the reset password link
            var message = new MailMessage();
            message.From = new MailAddress(FromEmail);
            message.To.Add(new MailAddress(Mail));
            message.Subject = "Recommnendation Of Mission";
            message.Body = $"Please click the following link to Participate in this Mission : {resetLink}";
            message.IsBodyHtml = true;

            using var smtp = new SmtpClient(Host, int.Parse(Port))
            {
                Credentials = new NetworkCredential(UserName, MyPassword),
                EnableSsl = true
            };

            try
            {
                smtp.Send(message);

                return Ok(new { success = true, message = "mail has been sent" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        [Route("ApplyMission", Name = "AppplyMission")]
        public IActionResult ApplyMission(int missionId , int UserId)
        {
            if (ModelState.IsValid)
            {
                MissionApplication missionApplication = new MissionApplication() { 

                    MissionId = missionId,
                    UserId = UserId,
                    AppliedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                };
                _db.MissionApplications.Add(missionApplication);
                _db.SaveChanges();

                return Ok(new { success = true, message = "Succesfully applied" });
            }
            else
            {
                return Ok(new { success = true, message = "Error while applying" });
            }


           
        }

        [HttpGet]
        [Route("GetRecentVolunteer",Name = "GetRecentVolunteer")]

        public IActionResult GetRecentVolunteer(int page)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
            List<MissionApplication> missionapplications = _db.MissionApplications.ToList();
            List<User> users = _db.Users.ToList();

            int volunteersCount = _db.MissionApplications.Count();
            int rowsPerPage = 01;
            int totalPages = (int)Math.Ceiling((double)volunteersCount / rowsPerPage);
            var pageNumber = page; // set the current page number
            var itemsToSkip = (pageNumber - 1) * rowsPerPage;

            var recentvolunteers = (from r in missionapplications

                                    join u in users on r.UserId equals u.UserId into data1


                                    from u in data1.DefaultIfEmpty()
                                    where (r.UserId != userId)
                                    select new Models.MissionVolunteeringViewModel.RecentVolunteer
                                    {
                                        UserId = r.UserId,
                                        FirstName = u.FirstName,
                                        LastName = u.LastName,


                                    }).DistinctBy(m => m.UserId).Skip((int)itemsToSkip).Take(rowsPerPage)
                                                    .ToList();
            return PartialView("_RecentVolunteer", recentvolunteers);

        }


    }
}
