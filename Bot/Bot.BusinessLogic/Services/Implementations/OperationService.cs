using System;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common;
using Bot.Common.Enums;
using Bot.Models.Data;
using Bot.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.BusinessLogic.Services.Implementations
{
	public class OperationService: IOperationService
	{
        public decimal Price { get; set; }

        private readonly ISheetService _sheetService;
        private readonly IUserService _userService;

        private ApplicationContext _context;
        public OperationService(ApplicationContext context
            ,ISheetService sheetService,
            IUserService userService)
        {
            _userService = userService;
            _sheetService = sheetService;
            _context = context;
        }

        public void Add(decimal value, string userName)
		{
            var user = _userService.Get(userName);
            var selectedCategory = ListOfSelectedIndexes.SelectedIndexes.FirstOrDefault(x=>x.Key == user.Id).Key;
            var operation = new Operation { CategoryId = selectedCategory, Price = value, NameUser = userName,UserId = user.Id };
            _context.Operations.Add(operation);
            _context.SaveChanges();

            var category = _context.Categories
                .AsNoTracking().FirstOrDefault(x => x.Id == selectedCategory);
            ListOfSelectedIndexes.SelectedIndexes.Remove(selectedCategory);
            var categoryOperationType = category.Type;

            if (categoryOperationType == OperationType.Discharge)
                _sheetService.AddDischarge(operation);
            _sheetService.AddIncome(operation);

        }


        //public void Get()
        //{
        //    var list = new List<Category> {
        //        new Category {  Name = "🍔 Еда вне дома", Type = OperationType.Discharge },
        //        new Category {  Name = "🛒 Продукты и хозтовары", Type = OperationType.Discharge },
        //        new Category {  Name = "💊 Здоровье и красота", Type = OperationType.Discharge },
        //        new Category {  Name = "🚇 Транспорт", Type = OperationType.Discharge },
        //        new Category { Name = "👕 Одежда, товары", Type = OperationType.Discharge },
        //        new Category {  Name = "🚰 Коммунальные", Type = OperationType.Discharge },
        //        new Category { Name = "🚙 Автомобиль", Type = OperationType.Discharge },
        //        new Category {  Name = "🌐 Интернет и связь", Type = OperationType.Discharge },
        //        new Category {  Name = "📚 Образование", Type = OperationType.Discharge },
        //        new Category {  Name = "👶 Дети", Type = OperationType.Discharge },
        //        new Category {  Name = "✈️ Путешествия", Type = OperationType.Discharge },
        //        new Category {  Name = "🏠 Аренда жилья", Type = OperationType.Discharge },
        //        new Category { Name = "📺 Подписки", Type = OperationType.Discharge },
        //        new Category { Name = "👨‍👨‍👧 Помощь родителям", Type = OperationType.Discharge },
        //        new Category {Name = "🚧 Непредвиденное", Type = OperationType.Discharge },
        //        new Category { Name = "🏡 Дом, ремонт", Type = OperationType.Discharge },
        //        new Category { Name = "🧰 Страховка", Type = OperationType.Discharge },
        //        new Category {  Name = "💸 Крипта", Type = OperationType.Discharge },
        //        new Category {  Name = "🎢 Развлечения", Type = OperationType.Discharge },
        //        new Category {  Name = "🧙‍ Личное", Type = OperationType.Discharge },
        //        new Category {  Name = "💲 Финансовые", Type = OperationType.Discharge },
        //        new Category { Name = "🌎 Прочие", Type = OperationType.Discharge },
        //        new Category {  Name = "💵 Зарплата", Type = OperationType.Income },
        //        new Category {  Name = "👨‍💻 Фриланс", Type = OperationType.Income },
        //        new Category { Name = "📈 Дивиденды", Type = OperationType.Income },
        //        new Category { Name = "🏦 Депозиты", Type = OperationType.Income },
        //        new Category { Name = "🏨 Аренда", Type = OperationType.Income },
        //        new Category { Name = "💸 Крипта", Type = OperationType.Income },
        //        new Category { Name = "🏭 Бизнес", Type = OperationType.Income },
        //        new Category {  Name = "🤝 Услуги", Type = OperationType.Income }
        //        };
        //    foreach (var item in list)
        //    {
        //        _context.Categories.Add(item);
        //    }
        //    _context.SaveChanges();
        //    var list1 = _context.Categories.ToList();
        //}
    }
}

