using System;
using System.Collections.Generic;
using UnityEngine;

namespace LumaBay
{
    [Serializable]
    public sealed class LighthouseTask
    {
        public int Id;
        public int UnlockLevel;
        public int StarCost;
        public int VisualState;
        public string ChapterRu;
        public string ChapterEn;
        public string TitleRu;
        public string TitleEn;
        public string DescriptionRu;
        public string DescriptionEn;

        public string Chapter => Localization.Language == "en" ? ChapterEn : ChapterRu;
        public string Title => Localization.Language == "en" ? TitleEn : TitleRu;
        public string Description => Localization.Language == "en" ? DescriptionEn : DescriptionRu;
    }

    public static class LighthouseTaskCatalog
    {
        private static readonly List<LighthouseTask> Tasks = Build();

        public static int Count => Tasks.Count;

        public static LighthouseTask Get(int index)
        {
            if (Tasks.Count == 0) return null;
            return Tasks[Mathf.Clamp(index, 0, Tasks.Count - 1)];
        }

        public static bool IsComplete(int index) => index >= Tasks.Count;

        public static float Progress01(int index) => Mathf.Clamp01(index / (float)Tasks.Count);

        public static int VisualState(int index)
        {
            if (index <= 0) return 0;
            if (index >= Tasks.Count) return 31;
            return Get(index).VisualState;
        }

        private static List<LighthouseTask> Build()
        {
            var result = new List<LighthouseTask>(48);
            AddChapter(result, "Спасение берега", "Saving the Shore", new[]
            {
                T("Осмотреть разрушения", "Survey the damage", "Мира оценивает последствия шторма и отмечает опасные участки.", "Mira surveys the storm damage and marks unsafe areas."),
                T("Расчистить тропу", "Clear the path", "К маяку снова можно пройти со стороны бухты.", "The lighthouse can be reached from the bay again."),
                T("Убрать плавник и мусор", "Remove driftwood", "Двор освобождён от плавника, сетей и разбитых ящиков.", "The yard is cleared of driftwood, nets and broken crates."),
                T("Укрепить край утёса", "Secure the cliff edge", "Опасный край укреплён канатами и новыми опорами.", "The dangerous cliff edge is secured with ropes and supports."),
                T("Восстановить ворота", "Restore the gate", "Старинные ворота снова открывают путь к башне.", "The old gate once again opens the way to the tower."),
                T("Поставить рабочие фонари", "Install work lights", "Тёплый свет позволяет продолжать ремонт после заката.", "Warm work lights allow repairs to continue after sunset."),
                T("Собрать строительные леса", "Raise the scaffolding", "Леса охватывают повреждённую сторону башни.", "Scaffolding now surrounds the damaged side of the tower."),
                T("Открыть главный вход", "Open the main entrance", "Заклинившая дверь поддалась, внутри найден журнал смотрителя.", "The jammed door opens, revealing the keeper's journal."),
            }, 1, 8, 0);

            AddChapter(result, "Возвращение башни", "Restoring the Tower", new[]
            {
                T("Высушить нижний этаж", "Dry the ground floor", "Из башни откачана вода, а стены защищены от сырости.", "Water is pumped out and the walls are protected from damp."),
                T("Заменить каменную кладку", "Replace damaged masonry", "Новые камни возвращают основанию прочность.", "Fresh stonework restores strength to the base."),
                T("Починить винтовую лестницу", "Repair the spiral stairs", "Путь к верхним этажам снова безопасен.", "The way to the upper floors is safe again."),
                T("Восстановить окна", "Restore the windows", "В башню возвращается дневной свет.", "Daylight returns to the tower."),
                T("Починить дверь смотрителя", "Repair the keeper's door", "Комната смотрителя снова защищена от ветра.", "The keeper's room is sheltered from the wind again."),
                T("Очистить фасад", "Clean the facade", "Соль и копоть смыты с внешних стен.", "Salt and soot are washed from the outer walls."),
                T("Загрунтовать башню", "Prime the tower", "Поверхность подготовлена к новой морской окраске.", "The surface is prepared for a new coastal paint scheme."),
                T("Вернуть красные полосы", "Restore the red bands", "Знаменитый силуэт маяка снова виден издалека.", "The lighthouse's famous silhouette can be seen from afar again."),
            }, 9, 20, 7);

            AddChapter(result, "Сердце света", "Heart of the Light", new[]
            {
                T("Починить внешний балкон", "Repair the gallery", "Балкон укреплён и окружён новой оградой.", "The gallery is reinforced and fitted with a new railing."),
                T("Восстановить купол", "Restore the lantern roof", "Медный купол очищен и защищён от шторма.", "The copper roof is cleaned and storm-proofed."),
                T("Заменить стекло фонаря", "Replace lantern glass", "Новые стёкла выдержат сильный ветер и соль.", "New panes will withstand strong wind and salt."),
                T("Починить поворотный механизм", "Repair the rotation gear", "Механизм снова способен плавно вращать линзу.", "The mechanism can smoothly rotate the lens again."),
                T("Очистить линзу Френеля", "Clean the Fresnel lens", "Старинная линза вновь прозрачна.", "The historic lens is clear once more."),
                T("Восстановить противовес", "Restore the counterweight", "Привод механизма работает без рывков.", "The drive mechanism now runs without jolts."),
                T("Проложить новую проводку", "Install new wiring", "Энергия безопасно подаётся к фонарю и комнатам.", "Power safely reaches the lantern and rooms."),
                T("Зажечь пробный огонь", "Light the test flame", "Первый тёплый отблеск появляется над бухтой.", "The first warm glow appears above the bay."),
            }, 21, 32, 15);

            AddChapter(result, "Первый луч", "The First Beam", new[]
            {
                T("Настроить фокус", "Calibrate the focus", "Луч становится ярким и узким.", "The beam becomes bright and focused."),
                T("Синхронизировать вращение", "Synchronize rotation", "Свет проходит над морем с ровным ритмом.", "The light sweeps across the sea in a steady rhythm."),
                T("Восстановить туманный колокол", "Restore the fog bell", "Бухта снова услышит предупреждение в тумане.", "The bay will hear its warning again in heavy fog."),
                T("Починить сигнальные флаги", "Repair signal flags", "Дневные сигналы возвращаются на балкон.", "Daytime signals return to the gallery."),
                T("Обновить аварийный генератор", "Upgrade the emergency generator", "Маяк не погаснет при следующем шторме.", "The lighthouse will stay lit through the next storm."),
                T("Проверить морскую карту", "Verify the sea chart", "Нора отмечает безопасный коридор через рифы.", "Nora marks a safe route through the reefs."),
                T("Запустить маяк", "Commission the lighthouse", "Главный луч впервые за много лет освещает Лума-Бэй.", "The main beam lights Luma Bay for the first time in years."),
                T("Провести праздник света", "Hold the Festival of Light", "Жители собираются у башни, чтобы отметить возвращение маяка.", "The town gathers to celebrate the lighthouse's return."),
            }, 33, 42, 23);

            AddChapter(result, "Дом смотрителя", "Keeper's Quarters", new[]
            {
                T("Восстановить комнату смотрителя", "Restore the keeper's room", "Старая комната превращается в уютное рабочее место.", "The old room becomes a warm and useful workspace."),
                T("Открыть архив", "Open the archive", "Карты и журналы получают безопасные шкафы.", "Charts and journals receive secure storage."),
                T("Собрать радиостанцию", "Rebuild the radio station", "Маяк снова может связаться с кораблями.", "The lighthouse can contact ships again."),
                T("Оборудовать метеостанцию", "Install a weather station", "Новые приборы отслеживают ветер, давление и волны.", "New instruments track wind, pressure and waves."),
                T("Создать музейный уголок", "Create a museum corner", "История маяка становится частью новой экспозиции.", "The lighthouse's history becomes a small exhibition."),
                T("Восстановить библиотеку", "Restore the library", "Морские книги и звёздные карты возвращаются на полки.", "Sea books and star charts return to the shelves."),
                T("Открыть смотровую комнату", "Open the viewing room", "Гости смогут наблюдать закат над бухтой.", "Visitors can watch the sunset over the bay."),
                T("Улучшить линзу", "Upgrade the lens", "Новая оптика увеличивает дальность и экономит энергию.", "New optics extend the range while using less power."),
            }, 43, 54, 26);

            AddChapter(result, "Живая бухта", "A Living Bay", new[]
            {
                T("Восстановить пирс", "Restore the pier", "У подножия маяка снова могут швартоваться лодки.", "Boats can dock beneath the lighthouse again."),
                T("Починить лодочную мастерскую", "Repair the boat workshop", "Лео получает место для ремонта спасательных лодок.", "Leo gains a workshop for rescue boats."),
                T("Посадить прибрежный сад", "Plant a coastal garden", "Ветер наполняется ароматом лаванды и морских трав.", "The wind carries lavender and coastal herbs."),
                T("Открыть чайную террасу", "Open the tea terrace", "У маяка появляется уютное место для жителей и гостей.", "A cosy terrace welcomes locals and visitors."),
                T("Построить площадку наблюдений", "Build a lookout deck", "С площадки видны рифы, остров и проходящие корабли.", "The deck overlooks reefs, the island and passing ships."),
                T("Установить спасательный пост", "Install a rescue station", "Бухта получает современное аварийное оборудование.", "The bay receives modern emergency equipment."),
                T("Открыть малую обсерваторию", "Open the small observatory", "Звёздная карта из журнала наконец получает смысл.", "The star map from the journal finally begins to make sense."),
                T("Отправить луч к острову", "Send the beam to the island", "Улучшенный свет отвечает загадочному сигналу за горизонтом.", "The upgraded beam answers the mysterious signal beyond the horizon."),
            }, 55, 60, 29);

            return result;
        }

        private static void AddChapter(List<LighthouseTask> list, string chapterRu, string chapterEn,
            IReadOnlyList<TaskText> tasks, int firstLevel, int lastLevel, int firstVisualState)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                float levelT = tasks.Count <= 1 ? 0f : i / (float)(tasks.Count - 1);
                int unlockLevel = Mathf.RoundToInt(Mathf.Lerp(firstLevel, lastLevel, levelT));
                int visualState = Mathf.Clamp(firstVisualState + Mathf.FloorToInt(i * 4f / tasks.Count), 0, 31);
                int starCost = i == tasks.Count - 1 ? 3 : (i % 3 == 2 ? 2 : 1);
                TaskText text = tasks[i];
                list.Add(new LighthouseTask
                {
                    Id = list.Count + 1,
                    UnlockLevel = unlockLevel,
                    StarCost = starCost,
                    VisualState = visualState,
                    ChapterRu = chapterRu,
                    ChapterEn = chapterEn,
                    TitleRu = text.TitleRu,
                    TitleEn = text.TitleEn,
                    DescriptionRu = text.DescriptionRu,
                    DescriptionEn = text.DescriptionEn
                });
            }
        }

        private static TaskText T(string titleRu, string titleEn, string descriptionRu, string descriptionEn)
        {
            return new TaskText(titleRu, titleEn, descriptionRu, descriptionEn);
        }

        private readonly struct TaskText
        {
            public readonly string TitleRu;
            public readonly string TitleEn;
            public readonly string DescriptionRu;
            public readonly string DescriptionEn;

            public TaskText(string titleRu, string titleEn, string descriptionRu, string descriptionEn)
            {
                TitleRu = titleRu;
                TitleEn = titleEn;
                DescriptionRu = descriptionRu;
                DescriptionEn = descriptionEn;
            }
        }
    }
}
