# How to Give Enchantments to Players

## Quick Methods

### Method 1: `/enchant` Command (Easiest) ⭐

**Requirements:** Player must be holding the item you want to enchant.

**Command:**
```
/enchant <player> <enchantment> [level]
```

**Examples:**
```
/enchant Steve sharpness 5
/enchant Steve efficiency 3
/enchant Steve mending 1
/enchant Steve protection 4
```

**Note:** Player must be holding the item in their hand (main hand or off-hand).

---

### Method 2: `/give` Command with Enchantments

**Give an enchanted item directly to a player:**

```
/give <player> <item>{Enchantments:[{id:"<enchantment>",lvl:<level>}]}
```

**Examples:**
```
/give Steve minecraft:diamond_sword{Enchantments:[{id:"minecraft:sharpness",lvl:5}]}

/give Steve minecraft:diamond_pickaxe{Enchantments:[{id:"minecraft:efficiency",lvl:5},{id:"minecraft:unbreaking",lvl:3},{id:"minecraft:mending",lvl:1}]}
```

**Multiple enchantments:**
```
/give Steve minecraft:diamond_sword{Enchantments:[{id:"minecraft:sharpness",lvl:5},{id:"minecraft:fire_aspect",lvl:2},{id:"minecraft:unbreaking",lvl:3}]}
```

---

### Method 3: Give Experience Levels (For Enchanting Table)

**Give experience levels so player can enchant themselves:**

```
/xp add <player> <levels> levels
```

**Examples:**
```
/xp add Steve 30 levels
/xp add Steve 50 levels
```

**Or give experience points:**
```
/xp add Steve 1000 points
```

---

## Common Enchantments

### Weapons (Swords, Axes)
- `sharpness` - Increases damage (max level 5)
- `smite` - Extra damage to undead (max level 5)
- `bane_of_arthropods` - Extra damage to spiders/arthropods (max level 5)
- `fire_aspect` - Sets enemies on fire (max level 2)
- `looting` - More drops from mobs (max level 3)
- `sweeping_edge` - More area damage (max level 3)
- `unbreaking` - Durability lasts longer (max level 3)
- `mending` - Repairs with XP (level 1 only)
- `knockback` - Pushes enemies away (max level 2)

### Tools (Pickaxe, Axe, Shovel, Hoe)
- `efficiency` - Faster mining (max level 5)
- `silk_touch` - Drops block itself (level 1 only)
- `fortune` - More drops (max level 3)
- `unbreaking` - Durability lasts longer (max level 3)
- `mending` - Repairs with XP (level 1 only)

### Armor (Helmet, Chestplate, Leggings, Boots)
- `protection` - Reduces all damage (max level 4)
- `fire_protection` - Reduces fire damage (max level 4)
- `blast_protection` - Reduces explosion damage (max level 4)
- `projectile_protection` - Reduces arrow damage (max level 4)
- `feather_falling` - Reduces fall damage (max level 4)
- `respiration` - Longer underwater breathing (max level 3)
- `aqua_affinity` - Faster underwater mining (level 1 only)
- `thorns` - Damages attackers (max level 3)
- `unbreaking` - Durability lasts longer (max level 3)
- `mending` - Repairs with XP (level 1 only)

### Boots Only
- `depth_strider` - Faster underwater movement (max level 3)
- `frost_walker` - Freezes water (max level 2)
- `soul_speed` - Faster on soul sand/soul soil (max level 3)
- `swift_sneak` - Faster sneaking (max level 3)

### Bows/Crossbows
- `power` - More arrow damage (max level 5)
- `punch` - Pushes enemies away (max level 2)
- `flame` - Arrows set targets on fire (level 1 only)
- `infinity` - Unlimited arrows (level 1 only)
- `unbreaking` - Durability lasts longer (max level 3)
- `mending` - Repairs with XP (level 1 only)

### Fishing Rods
- `luck_of_the_sea` - Better fishing loot (max level 3)
- `lure` - Faster fishing (max level 3)
- `unbreaking` - Durability lasts longer (max level 3)
- `mending` - Repairs with XP (level 1 only)

---

## Enchantment Names (For Commands)

Use these exact names in commands:

| Display Name | Command Name |
|--------------|--------------|
| Sharpness | `sharpness` |
| Efficiency | `efficiency` |
| Protection | `protection` |
| Unbreaking | `unbreaking` |
| Mending | `mending` |
| Fortune | `fortune` |
| Silk Touch | `silk_touch` |
| Fire Aspect | `fire_aspect` |
| Looting | `looting` |
| Power | `power` |
| Infinity | `infinity` |
| Feather Falling | `feather_falling` |
| Depth Strider | `depth_strider` |
| Frost Walker | `frost_walker` |
| Soul Speed | `soul_speed` |
| Swift Sneak | `swift_sneak` |

---

## Examples

### Give Sharpness V Sword
```
/enchant Steve sharpness 5
```
*(Player must be holding a sword)*

### Give Full Enchanted Diamond Armor Set
```
/give Steve minecraft:diamond_helmet{Enchantments:[{id:"minecraft:protection",lvl:4},{id:"minecraft:unbreaking",lvl:3},{id:"minecraft:mending",lvl:1}]}

/give Steve minecraft:diamond_chestplate{Enchantments:[{id:"minecraft:protection",lvl:4},{id:"minecraft:unbreaking",lvl:3},{id:"minecraft:mending",lvl:1}]}

/give Steve minecraft:diamond_leggings{Enchantments:[{id:"minecraft:protection",lvl:4},{id:"minecraft:unbreaking",lvl:3},{id:"minecraft:mending",lvl:1}]}

/give Steve minecraft:diamond_boots{Enchantments:[{id:"minecraft:protection",lvl:4},{id:"minecraft:feather_falling",lvl:4},{id:"minecraft:depth_strider",lvl:3},{id:"minecraft:unbreaking",lvl:3},{id:"minecraft:mending",lvl:1}]}
```

### Give Experience for Enchanting
```
/xp add Steve 30 levels
```

---

## Troubleshooting

### "Nothing changed. Targets either have no item in their hands"
- Player must be holding the item you want to enchant
- Item must be compatible with the enchantment (e.g., can't put Sharpness on armor)

### "Cannot support that enchantment"
- The enchantment doesn't work on that item type
- Example: Can't put Sharpness on a pickaxe, only on swords/axes

### "Level is higher than the maximum"
- Check max level for that enchantment
- Example: Mending is always level 1, Sharpness max is 5

### In Console (No `/` prefix)
```
enchant Steve sharpness 5
give Steve minecraft:diamond_sword{Enchantments:[{id:"minecraft:sharpness",lvl:5}]}
xp add Steve 30 levels
```

---

## Quick Reference

**Enchant item player is holding:**
```
/enchant <player> <enchantment> <level>
```

**Give enchanted item:**
```
/give <player> <item>{Enchantments:[{id:"<enchantment>",lvl:<level>}]}
```

**Give experience:**
```
/xp add <player> <amount> levels
```

