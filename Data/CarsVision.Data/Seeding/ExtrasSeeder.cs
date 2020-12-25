namespace CarsVision.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CarsVision.Data.Models;

    public class ExtrasSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Extras.Any())
            {
                return;
            }

            var extrasList = new List<Extra>()
            {
                new Extra { Name = "GPS система за проследяване", },
                new Extra { Name = "Автоматичен контрол на стабилността", },
                new Extra { Name = "Адаптивни предни светлини", },
                new Extra { Name = "Антиблокираща система", },
                new Extra { Name = "Въздушни възглавници - Задни", },
                new Extra { Name = "Въздушни възглавници - Предни", },
                new Extra { Name = "Въздушни възглавници - Странични", },
                new Extra { Name = "Ел. разпределяне на спирачното усилие", },
                new Extra { Name = "Електронна програма за стабилизиране", },
                new Extra { Name = "Контрол на налягането на гумите", },
                new Extra { Name = "Парктроник", },
                new Extra { Name = "Система ISOFIX", },
                new Extra { Name = "Система за динамична устойчивост", },
                new Extra { Name = "Система за защита от пробуксуване", },
                new Extra { Name = "Система за изсушаване на накладките", },
                new Extra { Name = "Система за контрол на дистанцията", },
                new Extra { Name = "Система за контрол на спускането", },
                new Extra { Name = "Система за подпомагане на спирането", },
                new Extra { Name = "Auto Start Stop function", },
                new Extra { Name = "Bluetooth / handsfree система", },
                new Extra { Name = "DVD, TV", },
                new Extra { Name = "Steptronic, Tiptronic", },
                new Extra { Name = "USB, audio/video, IN/AUX изводи", },
                new Extra { Name = "Адаптивно въздушно окачване", },
                new Extra { Name = "Безключово палене", },
                new Extra { Name = "Блокаж на диференциала", },
                new Extra { Name = "Бордкомпютър", },
                new Extra { Name = "Бързи / бавни скорости", },
                new Extra { Name = "Датчик за светлина", },
                new Extra { Name = "Ел. Огледала", },
                new Extra { Name = "Ел. Стъкла", },
                new Extra { Name = "Ел. регулиране на окачването", },
                new Extra { Name = "Ел. регулиране на седалките", },
                new Extra { Name = "Ел. усилвател на волана", },
                new Extra { Name = "Климатик", },
                new Extra { Name = "Климатроник", },
                new Extra { Name = "Мултифункционален волан", },
                new Extra { Name = "Навигация", },
                new Extra { Name = "Отопление на волана", },
                new Extra { Name = "Печка", },
                new Extra { Name = "Подгряване на предното стъкло", },
                new Extra { Name = "Подгряване на седалките", },
                new Extra { Name = "Регулиране на волана", },
                new Extra { Name = "Сензор за дъжд", },
                new Extra { Name = "Серво усилвател на волана", },
                new Extra { Name = "Система за измиване на фаровете", },
                new Extra { Name = "Система за контрол на скоростта (автопилот)", },
                new Extra { Name = "Стерео уредба", },
                new Extra { Name = "Филтър за твърди частици", },
                new Extra { Name = "Хладилна жабка", },
                new Extra { Name = "4x4", },
                new Extra { Name = "7 места", },
                new Extra { Name = "Buy back", },
                new Extra { Name = "Бартер", },
                new Extra { Name = "Газова уредба", },
                new Extra { Name = "Дълга база", },
                new Extra { Name = "Капариран/Продаден", },
                new Extra { Name = "Катастрофирал", },
                new Extra { Name = "Къса база", },
                new Extra { Name = "Лизинг", },
                new Extra { Name = "Метанова уредба", },
                new Extra { Name = "На части", },
                new Extra { Name = "Напълно обслужен", },
                new Extra { Name = "Нов внос", },
                new Extra { Name = "С право на дан.к-т", },
                new Extra { Name = "С регистрация", },
                new Extra { Name = "Сервизна книжка", },
                new Extra { Name = "Тунинг", },
                new Extra { Name = "2(3) Врати", },
                new Extra { Name = "4(5) Врати", },
                new Extra { Name = "LED фарове", },
                new Extra { Name = "Ксенонови фарове", },
                new Extra { Name = "Лети джанти", },
                new Extra { Name = "Металик", },
                new Extra { Name = "Отопляеми чистачки", },
                new Extra { Name = "Панорамен люк", },
                new Extra { Name = "Рейлинг на покрива", },
                new Extra { Name = "Ролбари", },
                new Extra { Name = "Спойлери", },
                new Extra { Name = "Теглич", },
                new Extra { Name = "Халогенни фарове", },
                new Extra { Name = "Шибедах", },
                new Extra { Name = "OFFROAD пакет", },
                new Extra { Name = "Аларма", },
                new Extra { Name = "Брониран", },
                new Extra { Name = "Имобилайзер", },
                new Extra { Name = "Каско", },
                new Extra { Name = "Лебедка", },
                new Extra { Name = "Подсилени стъкла", },
                new Extra { Name = "Централно заключване", },
                new Extra { Name = "Велурен салон", },
                new Extra { Name = "Десен волан", },
                new Extra { Name = "Кожен салон", },
                new Extra { Name = "TAXI", },
                new Extra { Name = "За хора с увреждания", },
                new Extra { Name = "Катафалка", },
                new Extra { Name = "Линейка", },
                new Extra { Name = "Учебен", },
                new Extra { Name = "Хладилен", },
                new Extra { Name = "Хомологация N1", },
            };

            foreach (var ex in extrasList)
            {
                await dbContext.Extras.AddAsync(ex);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
