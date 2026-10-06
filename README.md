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

- C#
- ASP.NET Core
- REST API using Controllers

Planned as the project develops:

- Entity Framework Core
- PostgreSQL
- Docker
- Authentication
- DTOs and service layers
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

The ASP.NET Core backend has been created and the first API endpoint is working.

Current endpoint:

```http
GET /api/Game/status
```

Example response:

```json
{
  "game": "Definitely Not FF7",
  "status": "Definitely in development"
}
```

## 🧠 Why I'm Building This

I'm currently studying software development and have previously worked with technologies including **JavaScript, TypeScript, React, Node.js, Express, PostgreSQL, Prisma and Docker**.

I'm increasingly interested in **backend and full-stack development**, so this project is my playground for gaining more practical experience with the .NET ecosystem.

Rather than building everything at once, I'm developing the project incrementally and introducing new technologies when the project actually needs them.

## 🗺️ What's Next?

The next steps are focused on building the core game domain:

- Characters
- Enemies
- Abilities
- Combat
- Damage calculation
- Battle state
- Run progression

Database persistence, authentication and the Phaser frontend will be introduced later as the project grows.

## ⚠️ Disclaimer

Despite the extremely suspicious repository name, this project is not affiliated with or endorsed by Square Enix.

The final game will use original characters, assets, abilities and worldbuilding.

Any questionable similarities during early development are clearly the work of **Definitely Not Cloud** and his lawyer.
