using System;
using System.Collections.Generic;

namespace LumaBay
{
    public static class Localization
    {
        private static readonly Dictionary<string, string> Russian = new Dictionary<string, string>
        {
            ["title"] = "LUMA BAY",
            ["subtitle"] = "Три в ряд • История маяка",
            ["play"] = "ИГРАТЬ",
            ["map"] = "Карта",
            ["levels"] = "Уровни",
            ["settings"] = "Настройки",
            ["level"] = "Уровень {0}",
            ["moves"] = "Ходы: {0}",
            ["goal"] = "Цели",
            ["collect"] = "Соберите {0}: {1}/{2}",
            ["fog"] = "Рассеять туман: {0}/{1}",
            ["locked"] = "Закрыто",
            ["start"] = "НАЧАТЬ",
            ["back"] = "Назад",
            ["win"] = "Блестяще!",
            ["win_subtitle"] = "Маяк становится ярче!",
            ["lose"] = "Ходы закончились",
            ["retry"] = "ЕЩЁ РАЗ",
            ["continue"] = "ПРОДОЛЖИТЬ",
            ["next"] = "СЛЕДУЮЩИЙ УРОВЕНЬ",
            ["coins"] = "Монеты: {0}",
            ["stars"] = "Звёзды: {0}",
            ["available_stars"] = "Доступно звёзд: {0}",
            ["restore"] = "ВОССТАНОВИТЬ ЗА 3 ★",
            ["restore_need"] = "Для ремонта нужно 3 ★",
            ["restoration"] = "Восстановление маяка",
            ["progress"] = "Прогресс: {0}%",
            ["shuffle"] = "Перемешать",
            ["extra_moves"] = "+5 ходов",
            ["booster_lightning"] = "Молния",
            ["booster_anchor"] = "Якорная бомба",
            ["booster_shuffle"] = "Перемешать",
            ["booster_time"] = "+5 ходов",
            ["booster_harpoon"] = "Гарпун",
            ["not_enough"] = "Недостаточно монет",
            ["invalid_move"] = "Нужна комбинация из трёх",
            ["cascade"] = "Каскад ×{0}",
            ["sound"] = "Звук",
            ["vibration"] = "Вибрация",
            ["language"] = "Язык",
            ["reset"] = "Сбросить прогресс",
            ["reset_done"] = "Прогресс сброшен",
            ["about"] = "Версия 0.1.1 Alpha • Norvexa Games",
            ["story_0"] = "Шторм погасил старый маяк. Мира возвращается в Лума-Бэй, чтобы вновь зажечь его свет.",
            ["story_1"] = "Двор расчищен. Под мокрыми досками Мира находит часть необычной звёздной карты.",
            ["story_2"] = "Лестница снова безопасна. На стене виден знак, совпадающий с картой.",
            ["story_3"] = "Механизм очищен от соли. Кто-то отключил его ещё до шторма.",
            ["story_4"] = "Новая линза собирает свет. Вдали появляется слабый ответный сигнал.",
            ["story_5"] = "Комната смотрителя открыта. В журнале упоминается заброшенный остров.",
            ["story_6"] = "Маяк снова работает. Луч указывает прямо на неизвестный остров в заливе.",
            ["piece_0"] = "ракушки",
            ["piece_1"] = "морские звёзды",
            ["piece_2"] = "фонари",
            ["piece_3"] = "компасы",
            ["piece_4"] = "кристаллы",
            ["piece_5"] = "цветы"
        };

        private static readonly Dictionary<string, string> English = new Dictionary<string, string>
        {
            ["title"] = "LUMA BAY",
            ["subtitle"] = "Match 3 • Lighthouse Story",
            ["play"] = "PLAY",
            ["map"] = "Map",
            ["levels"] = "Levels",
            ["settings"] = "Settings",
            ["level"] = "Level {0}",
            ["moves"] = "Moves: {0}",
            ["goal"] = "Goals",
            ["collect"] = "Collect {0}: {1}/{2}",
            ["fog"] = "Clear fog: {0}/{1}",
            ["locked"] = "Locked",
            ["start"] = "START",
            ["back"] = "Back",
            ["win"] = "Brilliant!",
            ["win_subtitle"] = "The lighthouse shines brighter!",
            ["lose"] = "Out of moves",
            ["retry"] = "TRY AGAIN",
            ["continue"] = "CONTINUE",
            ["next"] = "NEXT LEVEL",
            ["coins"] = "Coins: {0}",
            ["stars"] = "Stars: {0}",
            ["available_stars"] = "Available stars: {0}",
            ["restore"] = "RESTORE FOR 3 ★",
            ["restore_need"] = "Restoration requires 3 ★",
            ["restoration"] = "Lighthouse restoration",
            ["progress"] = "Progress: {0}%",
            ["shuffle"] = "Shuffle",
            ["extra_moves"] = "+5 moves",
            ["booster_lightning"] = "Lightning",
            ["booster_anchor"] = "Anchor bomb",
            ["booster_shuffle"] = "Shuffle",
            ["booster_time"] = "+5 moves",
            ["booster_harpoon"] = "Harpoon",
            ["not_enough"] = "Not enough coins",
            ["invalid_move"] = "Make a match of three",
            ["cascade"] = "Cascade ×{0}",
            ["sound"] = "Sound",
            ["vibration"] = "Vibration",
            ["language"] = "Language",
            ["reset"] = "Reset progress",
            ["reset_done"] = "Progress reset",
            ["about"] = "Version 0.1.1 Alpha • Norvexa Games",
            ["story_0"] = "A storm darkened the old lighthouse. Mira returns to Luma Bay to bring its light back.",
            ["story_1"] = "The yard is clear. Under wet boards, Mira finds part of a strange star map.",
            ["story_2"] = "The stairs are safe again. A wall mark matches the symbol on the map.",
            ["story_3"] = "The mechanism is free of salt. Someone disabled it before the storm.",
            ["story_4"] = "The new lens gathers light. A weak answering signal appears offshore.",
            ["story_5"] = "The keeper's room is open. Its journal mentions an abandoned island.",
            ["story_6"] = "The lighthouse works again. Its beam points directly at an unknown island.",
            ["piece_0"] = "shells",
            ["piece_1"] = "starfish",
            ["piece_2"] = "lanterns",
            ["piece_3"] = "compasses",
            ["piece_4"] = "crystals",
            ["piece_5"] = "flowers"
        };

        public static string Language { get; set; } = "ru";

        public static string T(string key, params object[] args)
        {
            Dictionary<string, string> table = Language == "en" ? English : Russian;
            if (!table.TryGetValue(key, out string value)) value = key;
            return args == null || args.Length == 0 ? value : string.Format(value, args);
        }

        public static string PieceName(PieceKind kind)
        {
            return T($"piece_{Math.Max(0, (int)kind)}");
        }
    }
}
