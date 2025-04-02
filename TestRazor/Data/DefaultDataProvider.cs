using TestRazor.Models;

namespace TestRazor.Data
{
    static class DefaultDataProvider
    {
        public static Category[] GetDefaultCategories()
        {
            Category[] default_categories =
            {
                new Category() { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category() { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category() {Id = 3, Name = "History", DisplayOrder = 3},
            };
            return default_categories;
        }
    }
}
