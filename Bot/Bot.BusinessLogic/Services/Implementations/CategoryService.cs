using System;
using AutoMapper;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common.Dto;
using Bot.Common.Enums;
using Bot.Models.Data;
using Bot.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.BusinessLogic.Services.Implementations
{
	public class CategoryService: ICategoryService
	{
        private readonly ApplicationContext _context = new ApplicationContext();

        public IMapper Mapper { get; set; }

        public int Add(string category,OperationType type)
        {
            try
            {
                var ct = new Category { Name = category, Type = type };
                _context.Categories.Add(ct);
                _context.SaveChanges();
                return (_context.Categories.FirstOrDefault(c => c.Name == category)!).Id;
            }
            catch { return 0; }
        }

        public List<CategoryDto> Get(int type)
        {
            IQueryable<Category> query = _context.Categories;
            query = query.Where(c => c.Type == (OperationType)type);
            var categories = query.ToList();
            var categoriesDto = Mapper.Map<List<CategoryDto>>(categories);
            return categoriesDto!;
        }
        public List<CategoryDto> Get()
        {
            var categories = _context.Categories.AsNoTracking().ToList();
            var categoriesDto = Mapper.Map<List<CategoryDto>>(categories);
            return categoriesDto;
        }
    }
}

