# Aujurlandar

A grid-based, Nuzlocke-style creature battler set in the empire's heyday, where demons roam Aujurlandar.

## Demons

**Demon**:
A creature that fights in battles, defined by its species, level and investments.
_Avoid_: Monster, creature, mon

**Species**:
The kind of demon, fixing its type, its base stats and its place in an adaptation line.

**Adaptation**:
A demon changing into the next species in its line; the game's form of evolution.
_Avoid_: Evolution

**Adaptation line**:
The one to three species a demon passes through by adaptation.

**Base stat total (BST)**:
The sum of a species' six base stats, fixed by its stage and line length (400, 500 or 600).

**Party**:
The up to five demons the player carries and fights with.
_Avoid_: Team, roster

**Banished**:
The permanent loss of a demon that reaches 0 HP; it leaves the party for good.
_Avoid_: Fainted, knocked out, dead

## Battles

**Battle**:
A turn-based fight with one active demon on each side.

**Active demon**:
The demon currently fighting for its side.

**Turn**:
One round of a battle in which both sides choose an action and the actions then resolve, faster demon first.

**Switch**:
Replacing the active demon with another from the party.

**Flee**:
Leaving a wild encounter before it is decided; it always succeeds.

**Game over**:
The end of a run when every demon in the party has been banished.

**Wild encounter**:
A battle against a lone roaming demon rather than a trainer; the only kind a player can flee.

**Opponent**:
The side the player fights against in a battle.
_Avoid_: Enemy

## Stats

**Stat pair**:
Two opposed stats, one aggression and one durability: Physical Attack and Magic Defense, Magic Attack and Physical Defense, Speed and Health.

**Investment**:
A 10-point shift from one stat to its opposite, chosen by the player, which never changes the BST.
_Avoid_: EV, training

**Bank**:
Investment points a demon has earned but not yet spent.

**Respec**:
Undoing or changing a demon's investments.

**Stat step**:
A temporary in-battle change to a stat, from −4 to +4, that resets on switch-out.
_Avoid_: Stat stage, boost

**Extra HP**:
A separate pool of HP, capped at a quarter of maximum HP, that takes damage before real HP.
_Avoid_: Shield, overheal

## Types and damage

**Type**:
One of twelve elemental or spiritual affinities, one per Architect except Reality.

**Effectiveness**:
How a move's type fares against a demon's type: super effective, neutral, resisted or no effect.

**STAB**:
The damage bonus a move gets when its type matches its user's type.

**Speed modifier**:
The damage bonus for the faster demon and penalty for the slower one, scaled by their speed ratio; it replaces critical hits.
_Avoid_: Crit, critical hit

**Output randomness**:
Chance that affects the outcome after a player has chosen an action; the game has none.

**Input randomness**:
Chance in what the player faces, such as encounters, opposing teams and AI choices; the game allows it.

## Moves

**Move**:
An action a demon knows, of category Physical, Magic, Positive or Negative.
_Avoid_: Attack, skill, ability

**Contact move**:
A Physical or Magic move that strikes the target directly.

**Projectile move**:
A Physical or Magic move that strikes from range.

**Fast move**:
A move that acts before normal moves regardless of Speed.
_Avoid_: Priority move

**Energy**:
A move's own pool, spent when used and recovered by using other moves; low energy weakens the move.
_Avoid_: PP

**Status condition**:
A lasting effect on a demon, one per Element and per spirit-quad type; a demon holds at most one.
_Avoid_: Status ailment
