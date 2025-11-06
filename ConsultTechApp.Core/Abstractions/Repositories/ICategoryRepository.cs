using ConsultTechApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultTechApp.Core.Abstractions.Repositories
{
    public interface ICategoryRepository
    {
        public List<Category> GetAllCategories();

        public Category GetCategory(Guid id);

        public void CreateCategory(Category category);

        public void UpdateCategory(Category category);

        public void DeleteCategory(Category category);
    }
}
