using System.Collections.Generic;
using System.Linq;
using Bogus;
using dreamcube.unity.Core.Scripts.API.ContentAPI.Activity;
using dreamcube.unity.Core.Scripts.API.ContentAPI.Category;
using dreamcube.unity.Core.Scripts.API.ContentAPI.Screen;
using dreamcube.unity.Core.Scripts.Models.ContentModels;
using Serilog;

namespace dreamcube.unity.Core.Scripts.Stores
{
    public static class TitleFaker
    {
        public static SharedSchema.Title GenerateFakeTitle()
        {
            var englishFaker = new Faker();
            var chineseFaker = new Faker("zh_CN");
            SharedSchema.Title title = new SharedSchema.Title
            {
                en = englishFaker.Lorem.Sentence(3),
                zh = chineseFaker.Name.FullName()
            };
            return title;
        }

    }

    public static class CategoryDataStore
    {
        public static List<Category> Categories { get; set; } = new List<Category>();

        public static Category GetCategoryWithID(string ID)
        {
            var categoriesWithID = Categories.Where(x => x.ID == ID).ToList();
            return categoriesWithID.FirstOrDefault();
        }

        public static SharedSchema.Title GetCategoryTitleWithID(string ID)
        {
            var category = GetCategoryWithID(ID);
            if (category == null)
            {
                Log.Warning($"{nameof(GetCategoryTitleWithID)} {ID} not found");
                return TitleFaker.GenerateFakeTitle();
            }
            return category.Text.Title;
        }
    }

    public static class ActivityDataStore
    {
        public static List<Activity> Activities { get; set; } = new List<Activity>();

        public static Activity GetActivityWithID(string ID)
        {
            var activitiesWithID = Activities.Where(x => x.ID == ID).ToList();
            return activitiesWithID.FirstOrDefault();
        }

        public static SharedSchema.Title GetActivityTitleWithID(string ID)
        {
            var activity = GetActivityWithID(ID);

            if (activity == null)
            {
                Log.Warning($"{nameof(GetActivityTitleWithID)} {ID} not found");
                return TitleFaker.GenerateFakeTitle();
            }

            return activity.Text.Title;
        }
    }


    public static class ScreenDataStore
    {
        public static List<Screen> Screens { get; set; } = new List<Screen>();

        public static Screen GetScreenWithID(string ID)
        {
            var screensWithID = Screens.Where(x => x.ID == ID).ToList();
            return screensWithID.FirstOrDefault();
        }

        public static SharedSchema.Title GetScreenTitleWithID(string ID)
        {
            var screen = GetScreenWithID(ID);

            if (screen == null)
            {
                Log.Warning($"{nameof(GetScreenTitleWithID)} {ID} not found");
                return TitleFaker.GenerateFakeTitle();
            }

            return screen.Text.Title;
        }

        public static Video GetScreenVideoURLWithID(string ID)
        {
            var screen = GetScreenWithID(ID);

            if (screen == null)
            {
                Log.Warning($"{nameof(GetScreenVideoURLWithID)} {ID} not found");
                return new Video();
            }

            return screen.Media.Video;
        }
    }
}