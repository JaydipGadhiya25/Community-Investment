using CI_PLATFORM.DataDB;
using CI_PLATFORM.Models;
using Microsoft.AspNetCore.Mvc;
using CI_PLATFORM.REPOSITORY.INTERFACE;
using System.Security.Claims;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Configuration;

namespace CI_PLATFORM.Controllers
{
    public class StoriesListingController : Controller
    {
        private readonly CiPlatformContext _db;
        private readonly IStoriesListing _storiesListing;
        private readonly IConfiguration _configuration;




        public StoriesListingController(CiPlatformContext db, IStoriesListing storiesListing, IConfiguration configuration)
        {
            _db = db;
            _storiesListing = storiesListing;
            _configuration = configuration;
            

        }
        public IActionResult StoriesListing(int? page,int? userId)
        {
            userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
            var stories = _storiesListing.GetStoriesCard(page,userId);
            return View(stories);
            
        }


        public IActionResult ShareYourStory()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
            var missions = _storiesListing.GetAllMission(userId);
            
            return View(missions);
        }

        [HttpPost]
        [Route("SaveAsDraft",Name = "SaveAsDraft")]

        public IActionResult SaveAsDraft(int userID, int missionId, string status, string title, DateTime publishedAt,string description, List<string> filePaths,string url)
        {
            //if (ModelState.IsValid)
            //{
                var draft = _storiesListing.SaveDraft(userID, missionId, status, title, publishedAt,description,filePaths,url);
               
            //}
            return Ok(new { success = true, message = "!! Success" });


        }


        [HttpPost]
        [Route("EditAsDraft", Name = "EditAsDraft")]

        public IActionResult EditAsDraft(int userID, int missionId, string status, string title, DateTime publishedAt, string description, List<string> filePaths,string url)
        {
            //if (ModelState.IsValid)
            //{
            var draft = _storiesListing.EditDraft(userID, missionId, status, title, publishedAt, description, filePaths,url);

            //}
            return Ok(new { success = true, message = "!! Success" });


        }


        [HttpPost]
        [Route("SubmitStory",Name = "SubmitStory")]

        public IActionResult SubmitStory(int storyID, int missionId, string status)
        {
            if (ModelState.IsValid)
            {
                var draft = _storiesListing.SubmitStory(storyID, missionId, status);
                
            }
            return RedirectToAction("StoriesListing", "StoriesListing");

        }
        
        
        
        [HttpGet]
        [Route("SearchStories",Name = "SearchStories")]

        public IActionResult SearchStories(string keywords)
        {
               var result = _storiesListing.SearchStories(keywords).ToList();


            return PartialView("_StoriesCard",result);

        }


        [HttpGet]
        [Route("isStoryExist", Name = "isStoryExist")]

        public StoriesListingViewModel.MissionList isStoryExist(int missionId,int userID)
        {
            bool result = _db.Stories.Any(r => r.MissionId == missionId && r.UserId == userID && r.Status == "DRAFT");

            StoriesListingViewModel.MissionList storystatus = new StoriesListingViewModel.MissionList()
            {
                isStoryExist = result,

            };


            return storystatus;

        }


        

        public IActionResult StoriesDetail(string id)
        {

            int userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
            var result = _storiesListing.GetStoriesDetail(id);
            return View(result);

        }

       
        
        [HttpPost]
        [Route("RecommendStory", Name = "RecommendStory")]
        public IActionResult RecommendStory(string Mail, int id)
        {

            string UserName = _configuration.GetSection("credential")["FromEmail"];
            string MyPassword = _configuration.GetSection("credential")["MyPassword"];
            string Port = _configuration.GetSection("credential")["Port"];
            string FromEmail = _configuration.GetSection("credential")["FromEmail"];
            string Host = _configuration.GetSection("credential")["Host"];
            // Generate a unique password reset token

            //string token = Guid.NewGuid().ToString();
            // Construct the reset password link with the token
            var resetLink = Url.Action("StoriesDetail", "StoriesListing", new { id = Convert.ToBase64String(Encoding.UTF8.GetBytes(id.ToString())) }, Request.Scheme);

            // Send the email with the reset password link
            var message = new MailMessage();
            message.From = new MailAddress(UserName);
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





    }
}
