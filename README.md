# DefinitelyNotFF7

> Definitely not Final Fantasy VII.

**DefinitelyNotFF7** is a learning project where I'm building a turn-based roguelite inspired by classic RPG combat.

The project is primarily an opportunity for me to improve my skills in **C# and ASP.NET Core** while gradually building a complete game with a separate frontend and backend.

The backend currently features a playable combat prototype with an ATB-inspired ability system, enemy encounters, experience points, character leveling, healing, consumable items, floor-based enemy scaling and ability unlocks retained between runs.

The project is still in early development.

## 🎮 The Idea

The goal is to build a small roguelite centered around turn-based combat, character progression and increasingly difficult encounters.

Players will start a run with a fresh character, battle enemies, earn experience and grow stronger as they progress.

Unlike traditional RPG progression, character levels and temporary stats reset when a new run begins.

Ability unlocks currently persist between runs while the API is running. The long-term goal is to save permanent progression to a database and introduce upgrades that improve stat growth on level-up.

The planned gameplay loop looks something like this:

1. Choose a character
2. Start a new run
3. Enter an enemy encounter
4. Attack, defend, use abilities or consume items
5. Defeat enemies and earn experience
6. Level up and improve character stats
7. Unlock abilities and permanent upgrades
8. Collect rewards and continue through increasingly difficult encounters
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
- Dependency injection
- Service-oriented architecture
- Singleton and scoped service lifetimes
- In-memory game state management
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
│       │   ├── BattleController.cs
│       │   └── GameController.cs
│       ├── Factories/
│       │   └── EnemyFactory.cs
│       ├── Models/
│       │   ├── Battle.cs
│       │   ├── Character.cs
│       │   ├── Enemy.cs
│       │   ├── GameSession.cs
│       │   ├── Materia.cs
│       │   └── PlayerProgress.cs
│       ├── Services/
│       │   ├── BattleOutcomeService.cs
│       │   ├── BattleService.cs
│       │   ├── GameStateService.cs
│       │   ├── HealingService.cs
│       │   └── LevelingService.cs
│       └── Program.cs
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
- Winner detection and one-time battle outcome processing
- Prevention of combat actions after a battle has ended
- Prevention of starting another encounter during an unfinished battle
- Potion usage during combat

Combat logic is handled by `BattleService`, while `BattleOutcomeService` processes the consequences of winning or losing an encounter.

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
POST /api/Battle/ability?ability=Braver
```

```http
POST /api/Battle/ability?ability=FocusedThrust
```

Attempting to use an ability without enough ATB returns a `400 Bad Request` response.

Abilities must also be unlocked before they can be used. **Braver** is available from the start, while **Focused Thrust** unlocks when the character reaches level 3.

Once unlocked, an ability remains available in later runs while the API is running.

Unknown or locked ability names are rejected.

### 👾 Enemy System

Enemy creation is handled through an `EnemyFactory`.

Instead of hardcoding enemies inside the controller, the factory creates enemies based on the requested enemy type.

Currently available enemies:

| Enemy | Base Max HP | Base Attack | XP Reward |
|---|---:|---:|---:|
| Definitely Not Shinra Grunt | 60 | 10 | 40 |
| Definitely Not Scorpion | 125 | 15 | 100 |

Enemies can be selected when starting a battle:

```http
POST /api/Battle/start?enemyType=Grunt
```

```http
POST /api/Battle/start?enemyType=Scorpion
```

Invalid enemy types return `400 Bad Request`.

#### Floor-Based Scaling

Enemy stats scale as the player progresses through encounters.

The current scaling system increases:

- Enemy maximum HP by 10% per floor after the first
- Enemy attack by 5% per floor after the first

The current floor is calculated from the number of completed encounters.

This allows existing enemy types to become progressively more challenging without requiring a completely new enemy for every encounter.

The scaling values are temporary and will likely be adjusted as development continues.

### 📈 Experience and Leveling

Defeating an enemy awards experience points based on its XP reward.

Experience and level-up calculations are handled by `LevelingService`.

Currently implemented:

- XP rewards after defeating enemies
- XP carried over between levels
- Multiple level-ups from a single XP reward
- Increasing XP requirements
- Character stat increases on level-up
- Ability unlock checks after leveling
- Shared XP reward handling regardless of the finishing action

Characters currently start with:

| Stat | Starting Value |
|---|---:|
| Level | 1 |
| Max HP | 100 |
| Max MP | 35 |
| Attack | 20 |
| Current XP | 0 |
| XP to next level | 100 |
| Max ATB | 2 |

Every level-up currently grants:

- +10 Max HP
- +10 Current HP
- +5 Max MP
- +5 Current MP
- +3 Attack
- +50 to the XP requirement for the following level

For example, defeating a Scorpion at level 1 awards 100 XP, enough to reach level 2. The next level then requires 150 XP.

These values are temporary balancing choices and will likely change as development continues.

### ❤️ Healing and Consumable Items

Healing is handled by a dedicated `HealingService`.

#### Post-Battle Restoration

After a victory, the game automatically restores:

- 25% of the character's maximum HP
- 25% of the character's maximum MP

Restoration is applied after XP and level-up processing, so it uses the updated maximum values if the character levels up.

HP and MP cannot exceed their respective maximum values.

#### Potions

Players currently start each run with **3 Potions**.

Potions can be used during combat through:

```http
POST /api/Battle/potion
```

Using a Potion:

- Restores 30% of the character's maximum HP
- Consumes one Potion
- Uses the player's combat action, allowing the enemy to retaliate

Potion usage is rejected when:

- No run is active
- No battle is active
- The battle has already ended
- No Potions remain
- The character already has full health

#### Potion Drops

Defeating an enemy currently has a **30% chance** to award an additional Potion.

The battle response includes a `potionDropped` property indicating whether the reward was received.

This introduces a simple consumable resource system that players must manage across encounters.

### 🏆 Battle Outcome Processing

Battle outcomes are handled by `BattleOutcomeService`.

After a victory, the service:

1. Awards experience points
2. Checks for newly unlocked abilities
3. Restores HP and MP
4. Increments the completed encounter count
5. Rolls for a possible Potion drop

If the character loses, the current run is marked as inactive.

Battle outcomes are processed only once using `IsOutcomeProcessed`, preventing duplicate XP, healing and rewards.

This logic is now separated from the controller rather than being handled directly inside HTTP endpoints.

### 🔄 Roguelite Run System

A basic run lifecycle has been implemented using a `GameSession` model.

The system separates starting a new run from starting individual battles.

**Starting a run:**

```http
POST /api/Game/run/start
```

This creates a fresh level 1 character with starting stats, Potions and an active game session.

**Starting an encounter:**

```http
POST /api/Battle/start?enemyType=Scorpion
```

The battle uses the character stored in the current game session rather than creating a new character.

This means that:

- Character levels persist between encounters
- Earned XP carries over between battles
- Increased HP, MP and Attack values are retained
- Current health and MP are preserved between encounters
- Potions are shared across encounters
- Enemy difficulty increases with floor progression
- Players cannot skip unfinished battles by starting new encounters
- Players cannot start battles without an active run
- Losing a battle ends the current run
- Starting a new run resets temporary character progression
- Unlocked abilities persist across runs using a separate `PlayerProgress` object

The run system currently operates in memory using a single shared game session.

Ability unlocks survive starting a new run, **but they reset when the API restarts**.

Database persistence and individual player sessions have not yet been implemented.

### 🔮 Materia System — Early Development

A basic `Materia` model has been introduced as preparation for a future magic and equipment system.

The model currently defines:

- Materia name
- MP cost
- Base damage
- Element

Characters also have:

- A number of available Materia slots
- A collection of equipped Materia

Characters currently start with **2 Materia slots**.

The initial planned Materia is Fire, with an MP cost and elemental damage.

Materia equipping, spellcasting and elemental interactions have not yet been implemented.

Eventually, the system should allow players to obtain, equip and use different Materia during combat.

## 🏗️ Backend Architecture

One of the latest development milestones was refactoring the original `GameController`, which had grown to nearly 400 lines.

Previously, a single controller handled run management, combat endpoints, game state and battle outcome processing.

The backend now separates these responsibilities.

### Controllers

**GameController**

Responsible for:

- Game status
- Starting new runs
- Temporary sample character and enemy endpoints

**BattleController**

Responsible for:

- Starting encounters
- Retrieving the current battle
- Attacking
- Defending
- Using abilities
- Using Potions

### Services

| Service | Responsibility |
|---|---|
| GameStateService | Stores the current game session, battle and permanent progression in memory |
| BattleService | Handles combat actions, damage, ATB and winner detection |
| BattleOutcomeService | Handles XP rewards, ability unlocks, healing, loot and run endings |
| LevelingService | Calculates XP and character level-ups |
| HealingService | Handles HP/MP restoration and Potion healing |

`EnemyFactory` handles enemy creation and floor-based scaling.

### Dependency Injection

The project now uses ASP.NET Core's built-in dependency injection.

`GameStateService` is registered as a singleton so multiple controllers and services can access the same in-memory game state.

`BattleOutcomeService` is registered as a scoped service and receives the shared `GameStateService` through constructor injection.

This allows controllers to delegate gameplay processing to services rather than containing all the business logic themselves.

The current singleton game state is suitable for a single-player prototype, but it is not intended as the final architecture for multiple authenticated users.

The longer-term plan is to replace this shared state approach with player-specific persistence and session management.

## 🌐 API Endpoints

The current API can be tested through Swagger UI.

### Game

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Game/status` | Returns API/game status and current encounter count |
| GET | `/api/Game/character` | Returns a sample character |
| GET | `/api/Game/enemy` | Returns a sample enemy |
| POST | `/api/Game/run/start` | Starts a new run |

### Battle

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Battle` | Returns the current battle |
| POST | `/api/Battle/start?enemyType=Grunt` | Starts an encounter |
| POST | `/api/Battle/attack` | Performs a basic attack |
| POST | `/api/Battle/ability?ability=Braver` | Uses a selected ability |
| POST | `/api/Battle/defend` | Defends against an enemy attack |
| POST | `/api/Battle/potion` | Uses a Potion during combat |

The character and enemy GET endpoints currently return sample objects rather than the active session's character or selected enemy.

### Example Battle Response

An actual Swagger response after defeating a Scorpion, leveling up, receiving post-battle restoration and obtaining a Potion:

```json
{
  "character": {
    "name": "Definitely Not Cloud",
    "maxHealth": 110,
    "currentHealth": 78,
    "attack": 23,
    "currentAtb": 1,
    "maxAtb": 2,
    "level": 2,
    "currentXp": 0,
    "xpToNextLevel": 150,
    "maxMp": 40,
    "currentMp": 40,
    "materiaSlots": 2,
    "equippedMateria": []
  },
  "enemy": {
    "name": "Definitely Not Scorpion",
    "maxHealth": 125,
    "currentHealth": 0,
    "attack": 15,
    "xpReward": 100
  },
  "winner": "Definitely Not Cloud",
  "isOutcomeProcessed": true,
  "currentFloor": 1,
  "potionDropped": true
}
```

The response confirms that:

- Cloud defeated the Scorpion
- The 100 XP reward triggered a level-up
- Maximum HP, MP and Attack increased
- Post-battle restoration was applied
- The battle outcome was processed
- A Potion dropped
- The character has 2 Materia slots, with nothing equipped yet

A separate request to:

```http
GET /api/Game/status
```

Confirmed:

```json
{
  "game": "Definitely Not FF7",
  "status": "Definitely in development",
  "runActive": true,
  "encounterNumber": 1
}
```

The first encounter was completed successfully, and the run remained active.

It's still not exactly the most advanced combat system ever created.

But now the Scorpion can hit back, Cloud can level up, recover HP and MP, collect Potions, unlock abilities and face increasingly difficult encounters.

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
- Dependency injection
- Singleton and scoped service lifetimes
- Service-oriented architecture
- Factory pattern
- Game state management
- One-time battle outcome processing
- In-memory progression across runs
- Character progression systems
- Separating business logic from controllers
- Refactoring and organizing an expanding codebase

Rather than building everything at once, I'm developing the project incrementally and introducing new technologies when the project actually needs them.

The goal isn't just to finish a game, but to understand how the systems behind it work.

## 🗺️ What's Next?

### Database and Infrastructure

The next major development milestone is introducing persistent storage.

Planned:

- Set up PostgreSQL using Docker
- Integrate Entity Framework Core
- Create database models and migrations
- Persist player progression across API restarts
- Introduce authentication and player accounts
- Associate progression with individual players
- Replace the shared in-memory game state with a more scalable approach
- Introduce DTOs
- Add automated testing

### Gameplay and Combat

- Expand the available abilities
- Add more level-based ability unlocks
- Expose unlocked abilities through an API endpoint
- Improve turn management and ATB mechanics
- Add combat logs and feedback
- Expand enemy variety
- Introduce randomized encounters
- Add more interesting damage calculations
- Introduce bosses and encounter progression
- Add more playable characters

### Materia and Equipment

- Implement equipping and unequipping Materia
- Introduce Fire and additional elemental spells
- Implement MP costs for spellcasting
- Add elemental damage and interactions
- Introduce equipment that increases Materia slots
- Expand character customization

### Roguelite Progression

- Introduce permanent stat-growth upgrades
- Replace automatic post-battle healing with selectable rewards (healing, Gil, Materia)
- Add rewards and upgrade choices between encounters
- Expand run management
- Introduce more varied floor progression
- Balance enemy scaling and resource management

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
