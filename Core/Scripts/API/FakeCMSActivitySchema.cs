using System.Collections.Generic;
using Bogus;
using dreamcube.unity.Core.Scripts.API.CMSSchemaPlaceholder;

namespace dreamcube.unity.Core.Scripts.API
{
    public static class GenerateFakeCMSSchemas
    {

        public static CMSActivitySchema.LocalizedText GenerateFakeLocalizedText()
        {
            Faker faker = new Faker();
            var jpIpsum = new Bogus.DataSets.Lorem("ko");

            CMSActivitySchema.LocalizedText text = new CMSActivitySchema.LocalizedText
            {
                Category = faker.Lorem.Word(),
                FieldName = faker.Lorem.Word(),
                En = faker.Lorem.Word(),
                Zh = jpIpsum.Word()
            };
            return text;
        }

        public static CMSActivitySchema.ActivityOption GenerateFakeActivityOption(string type)
        {
            var faker = new Faker();
            CMSActivitySchema.ActivityOption activityOption = new CMSActivitySchema.ActivityOption
            {
                ID = faker.IndexFaker,
                ActivityType = type,
                Header = GenerateFakeLocalizedText(),
                Title = GenerateFakeLocalizedText(),
                PrefabFileName = faker.Hacker.Adjective(),
                LinearMediaUrl = faker.Internet.Url()
            };
            return activityOption;
        }


        public static CMSActivitySchema.ActivitySet GenerateActivitySet(string type)
        {
            var faker = new Faker();
            var jpIpsum = new Bogus.DataSets.Lorem("ko");
            CMSActivitySchema.ActivitySet activitySet = new CMSActivitySchema.ActivitySet
            {
                ActivityType = type,
                Title = GenerateFakeLocalizedText(),
                Description = new CMSActivitySchema.LocalizedText()
                {
                    Category = faker.Lorem.Word(),
                    FieldName = faker.Lorem.Word(),
                    En = faker.Lorem.Sentence(20),
                    Zh = jpIpsum.Sentence(20)
                }
            };

            for (var i = 0; i < 4; i++)
            {
                CMSActivitySchema.ActivityOption activityOption = GenerateFakeActivityOption(type);
                activitySet.activityOptionSet.Add(activityOption);
            }

            return activitySet;

        }


        public static CMSActivitySchema.ActivitySets GenerateActivitySets(string type)
        {
            CMSActivitySchema.ActivitySets ActivitySets = new CMSActivitySchema.ActivitySets();
            ActivitySets.AllActivitySets = new List<CMSActivitySchema.ActivitySet>();
            for (var i = 0; i < 4; i++)
            {
                ActivitySets.AllActivitySets.Add(GenerateActivitySet(type));
            }

            return ActivitySets;
        }

    }
}