# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

DefinitelyNotFF7 is a learning project: a turn-based roguelite with an ASP.NET Core backend (`backend/`) and a planned TypeScript + Phaser + Vite frontend (`frontend/`, not yet created). The owner is using it to learn C#/.NET, so prefer clear, idiomatic code and explain non-obvious C# or ASP.NET concepts when introducing them. The final game must use original characters, assets and worldbuilding. The "Definitely Not ..." names are placeholder parody names.

## How to work with the owner

The owner writes all game code themselves. Claude acts as a mentor (see `.claude/output-styles/mentor.md`, selected with `/output-style Mentor`):
- Don't create, edit or delete project files, and don't give drop-in solutions. Teach with short, parallel examples and explain *why* they work. Reading files and running `dotnet build` / `dotnet test` is fine.
- Exception: when the owner explicitly writes "write it for me", do that one specific piece. Documentation (`README.md`, this file) has been handled this way.
- Ask **one question per reply** and move in small steps. Stacking several questions at once has been overwhelming.
- When reviewing, read the actual changes (`git diff`) and build before giving feedback. Ask the owner to explain their code before critiquing it.
- Encourage "predict, then test in Swagger" for bugs, and asking what should be tested when game logic changes.
- Concepts covered so far: enums, `required` vs nullable (`?`), class vs instance (`request.Ability`, not `BattleActionRequest.Ability`), guard clauses and nullable flow analysis, `void` vs return types, the ternary operator, `[FromBody]`, `switch` with `break` vs `return`, `try/catch` only catching thrown exceptions.

## Commands

Run from `backend/` (solution file: `backend.slnx`, target `net10.0`):

```sh
dotnet build                                   # build the solution
dotnet run --project DefinitelyNotFF7.api      # run the API (http://localhost:5236)
```

In Development, Swagger UI is served at `/swagger` (OpenAPI doc at `/openapi/v1.json`). Swagger is currently the only client for exercising the game.

There is no test project yet, and no linter or formatter is configured. Once a test project exists, run a single test with `dotnet test --filter "FullyQualifiedName~TestName"`.

## Architecture

All game state is in memory and shared by all callers. `GameStateService` is a **singleton** that holds three objects:
- `CurrentSession` (`GameSession`): the active run. It holds the run's `Character`, `EncounterNumber` and `Potions`. A run ends (`IsActive = false`) when the player loses.
- `CurrentBattle` (`Battle`): the current encounter. Its `Character` is the *same object reference* as the session's character, which is how HP, XP and level carry over between encounters.
- `CurrentProgress` (`PlayerProgress`): meta-progression (unlocked abilities) that survives `run/start` but resets when the API restarts.

All combat actions go through one endpoint, `POST /api/Battle/turn` (`BattleController.TakeTurn`). The body is a `BattleActionRequest`: `Type` (`BattleActionType` enum: `Attack` = 0, `Defend` = 1, `Ability` = 2, `Potion` = 3; `required`) and `Ability` (`string?`, only used when `Type` is `Ability`). Enums bind as numbers because no `JsonStringEnumConverter` is registered. The other battle endpoints are `GET /api/Battle` and `POST /api/Battle/start?enemyType=...`.

`TakeTurn` has three zones:
1. **Shared checks:** an active run, a battle exists, `Winner == null`.
2. **`switch (request.Type)`:** each case does only its own checks and action. A failed check `return`s early, so the enemy does not act. `default` rejects unknown action values.
   - `Attack`: `BattleService.Attack` (+1 ATB).
   - `Defend`: `BattleService.Defend` sets `Character.IsDefending = true` (+1 ATB). It deals no damage itself.
   - `Ability`: rejects a missing name, then a name not in `PlayerProgress.UnlockedAbilities`, then an inner `switch` on the name inside `try/catch` (not enough ATB throws `InvalidOperationException`).
   - `Potion`: rejects when no potions are left or HP is full, then `HealingService.UsePotion` and `Potions--`.
3. **Shared end of turn:**
   - If the enemy is still alive, `BattleService.EnemyAttack`. This is the only place the enemy deals damage. It halves damage when `IsDefending` is set, then clears the flag, so a defense lasts one hit.
   - `BattleService.CheckWinner` sets `Battle.Winner`.
   - `BattleOutcomeService.HandleBattleOutcome()` runs once per battle, guarded by `IsOutcomeProcessed`. On victory it awards XP (`LevelingService`), unlocks abilities by level, restores 25% HP/MP, increments `EncounterNumber`, and rolls a 30% potion drop. On defeat it ends the run.
   - Returns `{ battle, potionsRemaining }` for every action.

To add a new action, add a value to `BattleActionType` and a case to the switch. Effects that change incoming damage should set state that `EnemyAttack` reads, not deal their own enemy hit.

Conventions and gotchas:
- `Battle.Winner` stores the winning combatant's **name** string. Victory and defeat are detected by comparing it to `Character.Name` and `Enemy.Name`.
- Only `GameStateService` (singleton) and `BattleOutcomeService` (scoped) are registered in DI (`Program.cs`). `BattleService`, `HealingService`, `LevelingService` and `EnemyFactory` are created with `new` where they are used.
- Game-rule violations are thrown as `InvalidOperationException` by services and the factory, then caught in controllers and returned as `400 BadRequest`.
- Abilities are identified by string names (`"Braver"`, `"FocusedThrust"`) that must match across the inner ability `switch` in `TakeTurn`, `BattleOutcomeService` (`CreatePlayerProgress` and `CheckAbilityUnlocks`) and `PlayerProgress.UnlockedAbilities`. There is no single list of abilities that exist, so an unknown name like `"Fire"` fails the unlock check ("Ability is not unlocked.") and the inner `default` ("Unknown ability.") is unreachable. A planned next step is an `AbilityType` enum to replace these strings. Abilities spend ATB, which is gained by attacking or defending (`MaxAtb` is 2).
- Action methods in `BattleService` return a damage value that is currently discarded (`Defend` is `void`).
- `EnemyFactory.CreateEnemy(type, floor)` scales enemy HP by +10% and attack by +5% per floor after the first. Floor = `EncounterNumber + 1`.
- Models are returned directly as API responses (no DTOs yet), so new model properties such as `Character.IsDefending` show up in responses. `GET /api/Battle` and `POST /api/Battle/start` return the `Battle` itself, while `/turn` wraps it as `{ battle, potionsRemaining }`.
- `GET /api/Game/character` and `/enemy` return hardcoded sample objects, not live state.

## Status and next steps

Done most recently (commit `9aa4d97`): merged the `attack`, `ability`, `defend` and `potion` endpoints into `POST /api/Battle/turn`, and moved Defend's damage reduction into `EnemyAttack` via `IsDefending` (this fixed Defend dealing 1.5× damage and the defense never wearing off).

Next, in order:
1. **`AbilityType` enum** to replace ability name strings in `BattleActionRequest.Ability`, `PlayerProgress.UnlockedAbilities`, `BattleOutcomeService` and the inner switch. Needs `JsonStringEnumConverter` in `Program.cs` so clients can send `"Braver"`. Open question for the owner: should `Ability` be `AbilityType` or `AbilityType?`
2. Small cleanups in `TakeTurn`: leftover blank lines, the outdated comment on `EnemyAttack`.
3. A test project (xUnit), starting with `BattleService` rules such as Defend halving one hit.
4. Returning a turn result/log (damage dealt and taken) instead of discarding the damage values, for the future Phaser frontend.

Work in progress: the Materia system. `Materia` model plus `Character.MateriaSlots`/`EquippedMateria` exist, and MP is tracked and restored, but nothing equips Materia or spends MP yet. Planned later: EF Core + PostgreSQL (via Docker), auth and per-player state replacing the singleton, DTOs, and automated tests.
