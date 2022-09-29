# Twitch Integration for BiBites

This is a [MelonLoader](https://github.com/LavaGang/MelonLoader) mod for BiBites the genetic algorithm Simulation
made by Leo at https://leocaussan.itch.io/




The mod adds various fun functions including (as the name implies)
witch integration and was originally created for [this twitch stream](https://www.twitch.tv/artificiallifeandchill).

## Feature List

- Twitch integration

- [Twitch chat commands](#commands)

- Display generation info

- Launch BiBites

- Make Bibites lay an egg

- Change BiBites cap

- Change simulation speed

- Various camera controls (Zoom in/out, Select Closest/Random, Cinematic Mode, Center)

## Currently WIP
- Statistcs (Highest generation [Living, Session, All time])

## Planned/To-do
- Commands for Statistics

- Random Buffs for all/focused BiBites ( refill food, heal, refill stamina etc)

- Live food/plant density/size rate/spawn rate adjustments

- Twitch cheering integration

- External GUI or console menu
## Commands
#### User
| Command | Arguement(s) | Description                                                     |
|:--------|:-------------|:----------------------------------------------------------------|
| `ping`  | `none`       | Test wether the connection between Bibites and twitch is active |

#### Moderator
| Command     | Arguement(s)                | Description                                |
|:------------|:----------------------------|:-------------------------------------------|
| `lay`       | `(closest/all)`             | Make all or closest Bibite lay an egg      |
| `launch`    | `(closest/all) (intensity)` | Launch all or closest Bibite               |
| `select`    | `(closest/random/none)`     | Select closest or random Bibite or unfocus |
| `setspeed`  | `<intensity>`               | Set the speed of the simulation            |
| `setcap`    | `<amount>`                  | Set the maximum number of Bibites          |
| `zoom`      | `(in/out/<number>)`         | Zoom in or out                             |
| `cinematic` | `none`                      | Toggle cinematic mode                      |
| `center`    | `none`                      | Center the camera                          |



#### Broadcaster
| Command  | Arguement(s) | Description        |
|:---------|:-------------|:-------------------|
| `reload` | `none`       | Reload config file |


## Installation

- Install [MelonLoader](https://github.com/LavaGang/MelonLoader/releases/tag/v0.5.3) v0.5.3 or higher
- Get [latest release DLL](../../../releases/latest):
  - Put `TwitchIntegration.dll` into the new `Mods` folder. **Not** in `The Bibites_Data\Mods`
- After launching the game edit the TwitchIntegration.json [config](#Configuration) file

## Configuration

| Config           | Value         | Description                                                               |
|:-----------------|:--------------|:--------------------------------------------------------------------------|
| `TwitchUsername` | `username`    | Your twitch username                                                      |
| `TwitchOAuth`    | `oauth:token` | Your twitch oauth token. [Generate one here](https://twitchapps.com/tmi/) |
| `CommandPrefix`  | `!`           | Prefix for chat commands                                                  |
| `stats`          | `none`        | Send stats in chat                                                        |
