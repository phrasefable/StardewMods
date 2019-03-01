# Aggressive Acorns

__Aggressive Acorns__ allow high customisation of tree behavior.
The main features are to allow spreading trees to replace long grass, and to prevent immature trees from being destroyed with the scythe and other melee weapons.

## Installation
Compatible with version 1.3.33 of Stardew Valley.

For the latest compatibility information visit https://smapi.io/compat#aggressive_acorns

1. Install [SMAPI](https://smapi.io/)
2. Get this mod from Nexus Mods.
3. (Manual installation) Extract the contents of the zip file to `Stardew Valley\Mods`. 
4. Start the game once to create the configuration file. Quit, edit the file (`Stardew Valley\Mods\Aggressive Acorns\config.json`), then play. *All options default to vanilla, so make sure to enable any features you want*.

## Features
All configurable features default to vanilla options. Be sure to enable them if you want them.

#### Prevent destruction with melee tools (optional).
* **Immature trees will no longer be destroyed by the scythe** or melee weapons. This is great for when you are growing hay in your timber plantation.



#### Seeds
* As in vanilla, each day, every tree on the farm has a certain chance to (try to) place a seed around it. (If the chosen location is invalid there is no second chance.)
* Stumps (and therefore hibernating mushroom trees) no longer do this.
* Seed spread uses the held seed (from if the tree wasn't shaken), but *does not require one*.
* :wrench: If a tree wasn't shaken, it looses its held seed overnight anyway (vanilla behaviour). Can retain them (OP).
* :wrench: The chance for a tree to hold (gain) a shakeable seed is configurable.
* :wrench: Daily spread chance is configurable. (Set this to `0` to disable spread)
* :wrench: **Seeds can replace long grass** (doesn't effect manually planting tree seeds).
* :wrench: Spread can be disabled during winter.
* :wrench: Spread can be disabled for tapped trees.

#### Growth
* By default, growth follows vanilla settings.
* :wrench: The daily growth chance is configurable.
* :wrench: By default, tree growth stops during winter. Trees indoors and in the desert will always grow year round (unlike vanilla, where it is only palm trees and the greenhouse. This means palm trees won't grow in winter outside the desert or indoors. This also means that normal trees in the desert will grow during winter). There is an option to allow year round growth for all trees.
* :wrench: Immature trees in the shade of mature trees will not grow into trees (vanilla). The highest stage of growth for shaded trees is configurable. By setting the cap to mature, trees will grow fully right next to each other, but it may look bad.
* :wrench: There is an option to allow immature trees (of any stage) to mature instantly.
* :wrench: By default, only seeds can be walked over. This can be configured to any stage of tree, but has graphical errors in-game.

#### Mushroom Trees

* :wrench: The vanilla hibernation of mushroom trees can be disabled. This also disables the regeneration of mushroom stumps on Spring 1st.
* While hibernating, growth and spread do not occur.
* Even if hibernation is disabled, mushroom trees still respect the normal winter growth rules (ie. won't grow/spread in winter unless they are enabled).
* :wrench: **Mushroom stumps can have a chance to regrow each day**. When enabled, it is half the normal growth chance, or always if instant growth is enabled. It will not occur during hibernation or when normal growth would not occur.

## Configuration

| **Name** | **Type** | Default | **Description** |
| -------- | -------- | ------- | --------------- |
|`PreventScythe`| boolean (`true`, `false`) | `false` | Whether immature trees are destroyed by melee weapons. |
|`SeedsReplaceGrass`| boolean | `false` | Whether trees are able to replace long grass when they spread by dropping seeds. |
|`MaxShadedGrowthStage`| integer (`0` - `4`)| `4` | The highest stage of growth for trees next to any mature tree. |
|`MaxPassibleGrowthStage`| integer (`0` - `4`) | `0` | The highest stage of growth without collision. **Visually broken**|
|`DailyGrowthChance`| float (`0.0` - `1.0`) | `0.20` | Daily chance for a tree to mature by one stage. |
|`DoGrowInWinter`| boolean | `false` | Whether trees grow normally in winter. |
|`DailySpreadChance`| float (`0.0` - `1.0`) | `0.15` | Daily chance (to attempt) to plant a seed nearby. (Does not try again if the position is invalid.) |
|`DoTappedSpread`| boolean | `true` | Whether tapped trees will spread by dropping seeds. |
|`DoSpreadInWinter`| boolean | `true` | Whether trees will spread through dropping seeds in winter |
|`DoGrowInstantly`| boolean | `false` | Whether immature trees (of any stage) grow to full maturity overnight. |
|`DoSeedsPersist`| boolean | `false` | Whether a tree keeps its seed if not shaken. |
|`DailySeedChance`| float (`0.0` - `1.0`) | `0.15` | Daily chance for a tree to gain a seed. (Does not effect spread chance.) |
|`DoMushroomTreesHibernate`| boolean | `true` | Whether mushroom trees will hibernate over winter. Hibernation also prevents growth of immature trees and mature ones from spreading. |
|`DoMushroomTreesRegrow`| boolean | `false` | Whether mushroom tree stumps will regrow if not hibernating. Daily regrow chance is half the daily growth chance. |

#### Growth stages
| **Index** | Description| **[Stage as per wiki](https://stardewvalleywiki.com/Trees#Maple_Tree)** |
| --- | --- | --- |
| `0` | Seed | 1 |
| `1` | Sprout | 2 |
| `2` | Sapling | 3 |
| `3`, `4` | Bush | 4 |
| &ge;`5` | Mature | 5 |
| *Must use these<br>values in config* | | |

#### Default Config
```json
{
  "PreventScythe": true,
  "SeedsReplaceGrass": true,
  "MaxShadedGrowthStage": 4,
  "MaxPassibleGrowthStage": 0,
  "DoGrowInWinter": false,
  "DoSpreadInWinter": true,
  "DailyGrowthChance": 0.2,
  "DoGrowInstantly": false,
  "DoSeedsPersist": true,
  "DoTappedSpread": true,
  "DoMushroomTreesHibernate": true,
  "DoMushroomTreesRegrow": true,
  "DailySeedChance": 0.05,
  "DailySpreadChance": 0.15
}
```
