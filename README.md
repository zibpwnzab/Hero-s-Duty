<div align="center">

# ⚔️ HERO'S DUTY — Isometric Grid Puzzle

**Гиперказуальная тактическая головоломка со скольжением по сетке 8x8, интерактивными ловушками и 35+ спроектированными уровнями**

[![Unity](https://img.shields.io/badge/Engine-Unity%203D%20(URP)-black?logo=unity)](https://unity.com/)
[![Language](https://img.shields.io/badge/Language-C%23-239120?logo=c-sharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![Genre](https://img.shields.io/badge/Genre-Hypercasual%20Puzzle%20%7C%20Level%20Design-orange)]()

[**🇷🇺 Русская версия**](#-о-проекте) | [**🇬🇧 English Version (Click below or scroll)**](#-english-version)

</div>

---

<details>
<summary><b>🌐 Switch to English / Read in English (Click to expand)</b></summary>

<br>

## 🎯 Project Overview (English Version)

**Hero's Duty** is an isometric grid-based tactical puzzle game inspired by *Slayaway Camp* and *Friday the 13th: Killer Puzzle*. The player controls a fantasy hero on an **8x8 board** who slides orthogonally like a chess rook—once movement starts, the character continues sliding until colliding with an obstacle, hazard, or enemy. To unlock the exit portal/hatch and earn a 3-star rating, the player must eliminate all enemies within a strict move limit.

### 🔄 Production Case Study: Legacy Refactoring & Grid Hitbox Migration
Originally, I joined this project strictly as a **Level & Game Designer**, designing over **35 levels across 4 chapters (7 visual themes)**. After the original programmer left the project, I took over the engineering side and refactored the core movement and collision architecture:
* **Problem:** Early builds relied on 3D BoxColliders attached directly to decorative 3D environment meshes, causing inconsistent stops and clipping during high-speed `Rigidbody` movement.
* **Solution:** I re-engineered `Player.cs` to use predictive raycasting (`RecalculateStopDistance`) combined with standardized floor-cell colliders, and rebuilt **Chapter 1 (12 levels)** as a polished playable Vertical Slice on the new architecture (`Assets/Levels/Chapter1`). Chapters 2–4 (`Assets/Levels/Chapter2–4`) are preserved in the repository to showcase the full scope of level design, biome progression, and puzzle topologies.

### ⚙️ Mechanics & Interactive Hazards
* **Predictive Sliding Movement (`Player.cs`):** Swipe-driven orthogonal movement with `CollisionDetectionMode.Continuous` and forward raycast distance clamping to prevent physics tunneling.
* **Directional Ice Deflectors (`ice.cs`):** Snaps the sliding player to the tile center and redirects velocity vector by 90° while recalculating forward obstacle distance on the fly (`OnRedirected`).
* **Turn-Based Lightning Traps (`LightningTrap.cs`):** Synchronized with `Player.MoveCounter`, striking every 2nd turn (`MoveCounter % 2 == 0`) with a configurable delay.
* **Destructible / Falling Environment (`FallingObject.cs`):** Trees and pillars that calculate the cardinal impact vector (`Vector3.Cross`), topple 90° around their base pivot, crush enemies along their fall path, and dynamically switch layers to become walkable bridges.
* **Linked Portals (`Portal.cs`):** Preserves incoming momentum and transforms velocity vectors relative to the exit portal's orientation (`Quaternion.Inverse`).

</details>

---

## 🎮 О проекте

**Hero's Duty** — изометрическая тактическая головоломка для мобильных платформ в фэнтези-сеттинге (механика в духе *Slayaway Camp* / *Friday the 13th: Killer Puzzle*). 

Игрок перемещается по клеточному полю **8×8** по принципу шахматной ладьи: начав скольжение по свайпу, герой не может остановиться, пока не врежется в препятствие или врага. Задача — зачистить арену от противников и встать на точку выхода за минимальное количество ходов (система оценки в 1–3 звезды).

---

## 🔧 Инженерный кейс: рефакторинг легаси и миграция хитбоксов

У этого проекта показательная производственная история. Изначально я выступал на проекте исключительно как **геймдизайнер и левел-дизайнер** (спроектировал более **35 уровней в 7 различных визуальных тематиках**, разбитых на 4 главы). 

После ухода основного программиста я полностью забрал на себя C#-разработку и провел техническую реанимацию проекта:
1. **Проблема старой архитектуры:** Изначально коллайдеры препятствий вешались напрямую на 3D-модели декораций разного габарита, из-за чего при быстром движении `Rigidbody` возникали погрешности остановки на соседних клетках.
2. **Инженерное решение:** Я перевел систему коллизий на унифицированные хитбоксы ячеек сетки под моделями и переписал контроллер движения (`Player.cs`) с использованием предварительного рейкаста дистанции остановки (`RecalculateStopDistance`) с учетом экстентов коллайдера.
3. **Структура репозитория:** 
   * **`Assets/Levels/Chapter1` (12 уровней)** — полностью пересобранный на новой системе хитбоксов играбельный вертикальный срез (Vertical Slice).
   * **`Assets/Levels/Chapter2` – `Chapter4`** — сохраненный архив из ~25 дополнительных уровней, демонстрирующий прогрессию сложности, комбинаторику ловушек и работу с окружением.

---

## 🧩 Геймдизайн: матрица механик и интерактивных блоков

Вся глубина пазлов строится на комбинации простого правила движения (скольжение до упора) и блоков-модификаторов на сетке:

| Элемент / Скрипт | Механика работы | Роль в левел-дизайне |
| :--- | :--- | :--- |
| **Базовое скольжение (`Player.cs`)** | Свайп запускает героя вдоль оси `X` или `Z`. Комбинация `Continuous` физики и `Physics.Raycast` предотвращает пролет сквозь тонкие стены. | Задает базовую геометрию маршрута («лабиринт ладьи»). |
| **Ледяной отражатель (`ice.cs`)** | При попадании в радиус центра клетки центрирует игрока и меняет вектор скорости на 90° (`OnRedirected`), пересчитывая луч до следующей стены. | Позволяет строить нелинейные траектории и менять направление движения без потери хода. |
| **Периодическая молния (`LightningTrap.cs`)** | Отслеживает глобальный счетчик ходов (`Player.MoveCounter`) и бьет в клетку каждый четный ход (`MoveCounter % 2 == 0`). | Заставляет игрока планировать четность шагов и делать «холостые» ходы для смены такта. |
| **Падающее дерево (`FallingObject.cs`)** | Определяет сторону удара игрока (`GetCardinalDirection`), падает на 90° вокруг нижней точки (`RotateAround`), убивая врагов на линии падения, и меняет слой на проходимый. | Динамически меняет топологию уровня: убирает преграду, позволяет убить врага дистанционно и создает мост. |
| **Система порталов (`Portal.cs`)** | Переносит игрока в парный портал с пересчетом кватерниона поворота и сохранением инерции (`rotationDifference * velocity`). | Связывает изолированные участки сетки 8×8. |
| **Босс с отскоком (`reaction.cs`, `killboss.cs`)** | Отталкивает игрока в обратном направлении, пока на уровне остаются живые миньоны (`vrags > 0`). | Задает жесткий порядок уничтожения целей (сначала свита, затем босс). |

---

## 📂 Структура проекта

```text
Assets/
├── Levels/
│   ├── Chapter1/        # Полностью играбельный Vertical Slice (12 уровней на новой сетке хитбоксов)
│   ├── Chapter2-4/      # Витрина левел-дизайна и визуальных тем (20+ уровней)
│   └── Templates/       # Шаблоны арен 8x8 для быстрой сборки пазлов
├── Scripts/
│   ├── Player.cs        # Управление свайпами, физика скольжения и рейкаст препятствий
│   ├── ice.cs           # Клетка смены вектора движения
│   ├── LightningTrap.cs # Пошаговая ловушка, завязанная на счетчик ходов
│   ├── FallingObject.cs # Интерактивные разрушаемые объекты (деревья/колонны)
│   ├── Portal.cs        # Двусторонние телепорты с сохранением импульса
│   ├── Enemy.cs         # Логика противников и анимаций смерти
│   └── Finish.cs        # Условия победы, расчет рейтинга (1-3 звезды) и сохранение прогресса
└── Visual / Animations  # 3D-ассеты, VFX ударов, шейдеры и анимации
```
