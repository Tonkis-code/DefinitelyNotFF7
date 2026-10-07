# DefinitelyNotFF7

> Definitely not Final Fantasy VII.

**DefinitelyNotFF7** is a learning project where I'm building a turn-based roguelite inspired by classic RPG combat.

The project is primarily an opportunity for me to improve my skills in **C# and ASP.NET Core** while gradually building a complete game with a separate frontend and backend.

The project is currently in early development.

## 🎮 The Idea

The goal is to build a small roguelite centered around turn-based combat.

A run will eventually consist of encounters where the player fights enemies, earns upgrades and develops their character before facing increasingly difficult opponents.

The initial focus is deliberately small:

1. Choose a character
2. Enter a battle
3. Attack or use abilities
4. Defeat the enemy
5. Choose an upgrade
6. Continue to the next encounter
7. Eventually fight a boss — or die trying

No massive open world.

No 200-hour story.

Probably at least one suspiciously long sword.

## 🛠️ Tech Stack

### Backend

Currently:

- C#
- ASP.NET Core
- REST API using Controllers
- Swagger UI / OpenAPI

Planned as the project develops:

- Entity Framework Core
- PostgreSQL
- Docker
- Authentication
- DTOs
- Automated tests

### Frontend

Planned:

- TypeScript
- Phaser
- Vite

## 📁 Project Structure

```text
DefinitelyNotFF7/
├── backend/
│   └── DefinitelyNotFF7.api/
├── frontend/                 # Planned
├── docker-compose.yml        # Planned
└── README.md
```

## 🚧 Current Progress

The ASP.NET Core backend is up and running, and development of the core turn-based combat system has started.

Currently implemented:

- ASP.NET Core REST API using Controllers
- Character model
- Enemy model
- Battle model
- Battle service for combat logic
- Stateful battles across API requests
- Basic character attacks
- Enemy attacks
- Defending with reduced incoming damage
- Damage calculation
- Health prevented from dropping below zero
- Winner tracking
- Prevention of further actions after a battle has ended
- Swagger UI for testing the API

Current endpoints:

```http
GET  /api/Game/status
GET  /api/Game/character
GET  /api/Game/enemy

POST /api/Game/battle/start
GET  /api/Game/battle
POST /api/Game/battle/attack
POST /api/Game/battle/defend
```

A battle can currently be started between **Definitely Not Cloud** and **Definitely Not Guard Scorpion**.

Each combat action is handled through a separate API request while the current battle state is preserved between turns.

The player can attack:

```http
POST /api/Game/battle/attack
```

This damages the enemy before it retaliates.

The player can also defend:

```http
POST /api/Game/battle/defend
```

Defending skips the player's attack but reduces the incoming enemy damage.

The current battle can be inspected without performing an action:

```http
GET /api/Game/battle
```

Example battle state after one attack and one defend:

```json
{
  "character": {
    "name": "Definitely Not Cloud",
    "maxHealth": 100,
    "currentHealth": 78,
    "attack": 20
  },
  "enemy": {
    "name": "Definitely Not Guard Scorpion",
    "maxHealth": 150,
    "currentHealth": 130,
    "attack": 15
  },
  "winner": null
}
```

It's still not exactly the most advanced combat system ever created.

But now the scorpion can hit back.

## 🧠 Why I'm Building This

I'm currently studying software development and have previously worked with technologies including **JavaScript, TypeScript, React, Node.js, Express, PostgreSQL, Prisma and Docker**.

I'm increasingly interested in **backend and full-stack development**, so this project is my playground for gaining more practical experience with the .NET ecosystem.

Rather than building everything at once, I'm developing the project incrementally and introducing new technologies when the project actually needs them.

## 🗺️ What's Next?

The immediate focus is expanding the combat system:

- More player actions and abilities
- Turn management
- More interesting damage calculation
- Battle logs and combat feedback
- Different characters and enemies
- Battle progression
- Run progression and upgrades

Once the core game logic is taking shape, the project will gradually introduce:

- Database persistence with Entity Framework Core
- PostgreSQL
- Docker
- Authentication
- Phaser frontend
- Automated tests

## ⚠️ Disclaimer

Despite the extremely suspicious repository name, this project is not affiliated with or endorsed by Square Enix.

The final game will use original characters, assets, abilities and worldbuilding.

Any questionable similarities during early development are clearly the work of **Definitely Not Cloud** and his lawyer.
