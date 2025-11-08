using ConsultTechApp.Core.Abstractions.Repositories;
using ConsultTechApp.Core.Context;
using ConsultTechApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultTechApp.Core.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationStoreContext _context;
        public CategoryRepository(ApplicationStoreContext context)
        {
            _context = context;
        }

        public List<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategory(Guid id)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == id);
        }

        public void CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }

        public void DeleteCategory(Category category)
        {
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }
    }
}
