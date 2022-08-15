using System;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Models.Data;
using Bot.Models.Models;

namespace Bot.BusinessLogic.Services.Implementations
{
	public class OperationService: IOperationService
	{
		public static int CategoryId { get; set; }
        public decimal Price { get; set; }

        private ApplicationContext _context = new ApplicationContext();

		public void Add(decimal price,string name = "Kirill")
		{
            var operation = new Operation { CategoryId = CategoryId, Price = price, NameUser = "Kirill" };
            _context.Operations.Add(operation);
            _context.SaveChanges();
        }


        public List<Operation> Get()
        {
            //var list = new List<Category> {
            //    new Category {  Name = "Еда вне дома", Type = OperationType.Discharge },
            //    new Category {  Name = "Продукты и хоз товары", Type = OperationType.Discharge },
            //    new Category {  Name = "Здоровье и красота", Type = OperationType.Discharge },
            //    new Category {  Name = "Транспорт", Type = OperationType.Discharge },
            //    new Category { Name = "Одежда, товары", Type = OperationType.Discharge },
            //    new Category {  Name = "Коммунальные", Type = OperationType.Discharge },
            //    new Category { Name = "Автомобиль", Type = OperationType.Discharge },
            //    new Category {  Name = "Интернет и связь", Type = OperationType.Discharge },
            //    new Category {  Name = "Образование", Type = OperationType.Discharge },
            //    new Category {  Name = "Дети", Type = OperationType.Discharge },
            //    new Category {  Name = "Путешествия", Type = OperationType.Discharge },
            //    new Category {  Name = "Аренда жилья", Type = OperationType.Discharge },
            //    new Category { Name = "Подписки", Type = OperationType.Discharge },
            //    new Category { Name = "Помощь родителям", Type = OperationType.Discharge },
            //    new Category {Name = "Непредвиденное", Type = OperationType.Discharge },
            //    new Category { Name = "Дом, ремонт", Type = OperationType.Discharge },
            //    new Category { Name = "Страховка", Type = OperationType.Discharge },
            //    new Category {  Name = "Крипта", Type = OperationType.Discharge },
            //    new Category {  Name = "Развлечения", Type = OperationType.Discharge },
            //    new Category {  Name = "Личное", Type = OperationType.Discharge },
            //    new Category {  Name = "Финансовые", Type = OperationType.Discharge },
            //    new Category { Name = "Прочие", Type = OperationType.Discharge },
            //    new Category {  Name = "Зарплата", Type = OperationType.Income },
            //    new Category {  Name = "Фриланс", Type = OperationType.Income },
            //    new Category { Name = "Дивиденды", Type = OperationType.Income },
            //    new Category { Name = "Депозиты", Type = OperationType.Income },
            //    new Category { Name = "Аренда", Type = OperationType.Income },
            //    new Category { Name = "Крипта", Type = OperationType.Income },
            //    new Category { Name = "Бизнес", Type = OperationType.Income },
            //    new Category {  Name = "Услуги", Type = OperationType.Income }
            //    };
            //foreach (var item in list)
            //{
            //    _context.Categories.Add(item);
            //}
            //_context.SaveChanges();
            var list1 = _context.Categories.ToList();
            return null;
        }
    }
}

