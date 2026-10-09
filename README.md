# DefinitelyNotFF7

> Definitely not Final Fantasy VII.

**DefinitelyNotFF7** is a learning project where I'm building a turn-based roguelite inspired by classic RPG combat.

The project is primarily an opportunity for me to improve my skills in **C# and ASP.NET Core** while gradually building a complete game with a separate frontend and backend.

The backend currently features a playable combat prototype with an ATB-inspired ability system, enemy encounters, experience points, character leveling and basic roguelite run progression.

The project is still in early development.

## 🎮 The Idea

The goal is to build a small roguelite centered around turn-based combat, character progression and increasingly difficult encounters.

Players will start a run with a fresh character, battle enemies, earn experience and grow stronger as they progress.

Unlike traditional RPG progression, character levels and temporary stats reset when a run ends.

However, the long-term goal is to introduce **permanent progression**, allowing players to retain unlocked abilities and eventually improve how much their stats increase when leveling up.

The planned gameplay loop looks something like this:

1. Choose a character
2. Start a new run
3. Enter an enemy encounter
4. Attack, defend or use abilities
5. Defeat enemies and earn experience
6. Level up and improve character stats
7. Unlock abilities and permanent upgrades
8. Continue through increasingly difficult encounters
9. Eventually defeat a boss — or die trying

No massive open world.

No 200-hour story.

Probably at least one suspiciously long sword.

## 🛠️ Tech Stack

### Backend

Currently implemented using:

- C#
- ASP.NET Core
- REST API using Controllers
- Swagger UI / OpenAPI
- Object-oriented programming
- Service classes for combat and leveling logic
- Factory pattern for enemy creation

Planned as the project develops:

- Entity Framework Core
- PostgreSQL
- Docker
- Authentication and player accounts
- DTOs
- Automated tests

### Frontend

Planned:

- TypeScript
- Phaser
- Vite

The frontend will eventually provide a proper interface for combat, character progression, abilities and run management instead of interacting directly with Swagger.

## 📁 Project Structure

```text
DefinitelyNotFF7/
├── backend/
│   └── DefinitelyNotFF7.api/
│       ├── Controllers/
│       │   └── GameController.cs
│       ├── Factories/
│       │   └── EnemyFactory.cs
│       ├── Models/
│       │   ├── Battle.cs
│       │   ├── Character.cs
│       │   ├── Enemy.cs
│       │   └── GameSession.cs
│       └── Services/
│           ├── BattleService.cs
│           └── LevelingService.cs
├── frontend/                 # Planned
├── docker-compose.yml        # Planned
└── README.md
```

## 🚧 Current Progress

The ASP.NET Core backend is running, and the first version of the core combat and roguelite progression systems has been implemented.

### ⚔️ Combat System

Currently implemented:

- Stateful turn-based battles across API requests
- Basic character attacks
- Enemy retaliation
- Defending to reduce incoming damage
- Damage calculations
- Health prevented from dropping below zero
- ATB generation through attacking and defending
- ATB-based special abilities
- ATB costs and validation
- Winner detection
- Prevention of combat actions after a battle has ended
- Prevention of starting another encounter during an unfinished battle

### ⚡ ATB and Abilities

Characters have an ATB gauge with a current and maximum value.

Attacking or defending generates ATB, which can then be spent on special abilities.

Currently implemented abilities:

| Ability | ATB Cost | Effect |
|---|---|---|
| Braver | 1 | Deals 2× normal attack damage |
| Focused Thrust | 2 | Deals 4× normal attack damage |

Abilities are executed through a single API endpoint:

```http
POST /api/Game/battle/ability?ability=Braver
```

```http
POST /api/Game/battle/ability?ability=FocusedThrust
```

Attempting to use an ability without enough ATB returns a `400 Bad Request` response.

Invalid ability names are also handled.

### 👾 Enemy System

Enemy creation is handled through an `EnemyFactory`.

Instead of hardcoding enemies inside the controller, the factory creates enemies based on the requested enemy type.

Currently available enemies:

| Enemy | Max HP | Attack | XP Reward |
|---|---:|---:|---:|
| Definitely Not Shinra Grunt | 60 | 10 | 40 |
| Definitely Not Scorpion | 125 | 15 | 100 |

Enemies can be selected when starting a battle:

```http
POST /api/Game/battle/start?enemyType=Grunt
```

```http
POST /api/Game/battle/start?enemyType=Scorpion
```

Invalid enemy types return `400 Bad Request`.

The factory makes it easier to introduce new enemy types without repeatedly modifying the controller.

### 📈 Experience and Leveling

Defeating an enemy awards experience points based on its XP reward.

Experience and level-up calculations are handled by a separate `LevelingService`.

Currently implemented:

- XP rewards after defeating enemies
- XP carried over between levels
- Multiple level-ups from a single XP reward
- Increasing XP requirements
- Character stat increases on level-up
- Shared XP reward handling for regular attacks and abilities

Characters currently start with:

| Stat | Starting Value |
|---|---:|
| Level | 1 |
| Max HP | 100 |
| Attack | 20 |
| Current XP | 0 |
| XP to next level | 100 |

Every level-up currently grants:

- +10 Max HP
- +10 Current HP
- +3 Attack
- +50 to the XP requirement for the following level

For example, a character earning 300 XP from level 1 would reach level 3 with 50 XP remaining and require another 200 XP for the next level.

These values are temporary balancing choices and will likely change as development continues.

### 🔄 Roguelite Run System

A basic run lifecycle has been implemented using a `GameSession` model.

The system separates starting a new run from starting individual battles.

**Starting a run:**

```http
POST /api/Game/run/start
```

This creates a fresh level 1 character with starting stats and an active game session.

**Starting an encounter:**

```http
POST /api/Game/battle/start?enemyType=Scorpion
```

The battle uses the character stored in the current game session rather than creating a new character.

This means that:

- Character levels persist between encounters.
- Earned XP carries over between battles.
- Increased HP and Attack values are retained.
- Current health is preserved between encounters.
- Players cannot skip unfinished battles by starting new encounters.
- Players cannot start battles without an active run.
- Losing a battle ends the current run.
- Starting a new run resets temporary character progression.

The run system currently operates in memory using a single shared game session.

Database persistence and individual player sessions have not yet been implemented.

## 🌐 API Endpoints

The current API can be tested through Swagger UI.

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Game/status` | Returns API/game status |
| GET | `/api/Game/character` | Returns a sample character |
| GET | `/api/Game/enemy` | Returns a sample enemy |
| POST | `/api/Game/run/start` | Starts a new run |
| POST | `/api/Game/battle/start?enemyType=Grunt` | Starts an encounter |
| GET | `/api/Game/battle` | Returns the current battle |
| POST | `/api/Game/battle/attack` | Performs a basic attack |
| POST | `/api/Game/battle/defend` | Defends against an enemy attack |
| POST | `/api/Game/battle/ability?ability=Braver` | Uses a selected ability |

The character and enemy GET endpoints currently return sample objects rather than the active session's character or selected enemy.

### Example Battle Response

A battle response after defeating a Scorpion and leveling up:

```json
{
  "character": {
    "name": "Definitely Not Cloud",
    "maxHealth": 110,
    "currentHealth": 45,
    "attack": 23,
    "currentAtb": 2,
    "maxAtb": 2,
    "level": 2,
    "currentXp": 40,
    "xpToNextLevel": 150
  },
  "enemy": {
    "name": "Definitely Not Scorpion",
    "maxHealth": 125,
    "currentHealth": 0,
    "attack": 15,
    "xpReward": 100
  },
  "winner": "Definitely Not Cloud"
}
```

The character's stats reflect progression earned during the current run.

It's still not exactly the most advanced combat system ever created.

But now the scorpion can hit back, Cloud can level up, and dying actually has consequences.

Progress.

## 🧠 Why I'm Building This

I'm currently studying software development and have previously worked with technologies including **JavaScript, TypeScript, React, Node.js, Express, PostgreSQL, Prisma and Docker**.

I'm increasingly interested in **backend and full-stack development**, so this project is my playground for gaining more practical experience with the .NET ecosystem.

Some of the concepts I'm practicing include:

- Object-oriented programming in C#
- Classes, properties and methods
- Nullable reference types
- REST API design
- HTTP response handling
- Exception handling
- Service-oriented code organization
- Factory pattern
- Game state management
- Character progression systems
- Separating business logic from controllers

Rather than building everything at once, I'm developing the project incrementally and introducing new technologies when the project actually needs them.

The goal isn't just to finish a game, but to understand how the systems behind it work.

## 🗺️ What's Next?

### Gameplay and Combat

- Expand the available abilities
- Introduce ability unlocks based on character levels
- Improve turn management and ATB mechanics
- Add combat logs and feedback
- Expand enemy variety
- Introduce randomized encounters
- Add more interesting damage calculations
- Introduce bosses and encounter progression
- Add more playable characters

### Roguelite Progression

- Separate permanent player progression from temporary run progression
- Retain unlocked abilities between runs
- Introduce permanent stat-growth upgrades
- Add rewards and upgrade choices between encounters
- Expand run management
- Introduce difficulty scaling and progression through floors

### Backend and Infrastructure

- Database persistence with Entity Framework Core
- PostgreSQL integration
- Docker configuration
- Authentication and player accounts
- Individual saved player progression
- DTOs and improved API structure
- Automated testing
- Refactoring in-memory game state into a more scalable design

### Frontend

- Create a Phaser frontend using TypeScript and Vite
- Display characters, enemies and health bars
- Implement interactive combat controls
- Display ATB gauges and available abilities
- Show XP gains and level-up feedback
- Build a run progression interface

## ⚠️ Disclaimer

Despite the extremely suspicious repository name, this project is not affiliated with or endorsed by Square Enix.

The final game will use original characters, assets, abilities and worldbuilding.

Any questionable similarities during early development are clearly the work of **Definitely Not Cloud** and his lawyer.
