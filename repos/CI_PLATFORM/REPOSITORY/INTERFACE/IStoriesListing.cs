using CI_PLATFORM.Models;
using System.Security.Claims;
namespace CI_PLATFORM.REPOSITORY.INTERFACE
{

    public interface IStoriesListing
    {
        List<StoriesListingViewModel.StoriesModel> GetStoriesCard(int? page,int? userId);

        List<StoriesListingViewModel.StoriesModel> SearchStories(string keywords);

        StoriesListingViewModel.MissionList GetAllMission(int userId);

        StoriesListingViewModel.DraftSave EditDraft(int userID, int missionId, string status, string title, DateTime publishedAt,string description, List<string> filePaths,string url);

        StoriesListingViewModel.DraftSave SaveDraft(int userID, int missionId, string status, string title, DateTime publishedAt, string description, List<string> filePaths,string url);


        StoriesListingViewModel.Submitstory SubmitStory(int storyID, int missionId, string status);

        StoriesListingViewModel.StoriesDetail GetStoriesDetail(string id);

        

        
    }

  
}
