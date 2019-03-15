
  private static StardewValley.Object chooseStoneType(
    double chanceForPurpleStone,
    double chanceForMysticStone,
    double gemStoneChance,
    Vector2 tile)
  {
    int num1 = 1;
    int num2;
    if (this.mineLevel < 40)
    {
      num2 = this.mineRandom.Next(31, 42);
      if (this.mineLevel % 40 < 30 && num2 >= 33 && num2 < 38)
        num2 = this.mineRandom.NextDouble() < 0.5 ? 32 : 38;
      else if (this.mineLevel % 40 >= 30)
        num2 = this.mineRandom.NextDouble() < 0.5 ? 34 : 36;
      if (this.mineLevel != 1 && this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < 0.029)
        return new StardewValley.Object(tile, 751, "Stone", true, false, false, false)
        {
          MinutesUntilReady = 3
        };
    }
    else if (this.mineLevel < 80)
    {
      num2 = this.mineRandom.Next(47, 54);
      num1 = 3;
      if (this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < 0.029)
        return new StardewValley.Object(tile, 290, "Stone", true, false, false, false)
        {
          MinutesUntilReady = 4
        };
    }
    else if (this.mineLevel < 120)
    {
      num1 = 4;
      num2 = this.mineRandom.NextDouble() >= 0.3 || this.isDarkArea()
        ? (this.mineRandom.NextDouble() >= 0.3
          ? (this.mineRandom.NextDouble() >= 0.5 ? 762 : 760)
          : this.mineRandom.Next(55, 58))
        : (this.mineRandom.NextDouble() >= 0.5 ? 32 : 38);
      if (this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < 0.029)
        return new StardewValley.Object(tile, 764, "Stone", true, false, false, false)
        {
          MinutesUntilReady = 8
        };
    }
    else
    {
      num1 = 5;
      num2 = this.mineRandom.NextDouble() >= 0.5
        ? (this.mineRandom.NextDouble() >= 0.5 ? 42 : 40)
        : (this.mineRandom.NextDouble() >= 0.5 ? 32 : 38);
      double num3 = 0.02 + (double) (this.mineLevel - 120) * 0.0005;
      if (this.mineLevel >= 130)
        num3 += 0.01 * ((double) (this.mineLevel - 120 - 10) / 10.0);
      double val1 = 0.0;
      if (this.mineLevel >= 130)
        val1 += 0.001 * ((double) (this.mineLevel - 120 - 10) / 10.0);
      double num4 = Math.Min(val1, 0.004);
      if (this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < num3)
      {
        double num5 = (double) (this.mineLevel - 120) * (0.0003 + num4);
        double num6 = 0.01 + (double) (this.mineLevel - 120) * 0.0005;
        double num7 = Math.Min(0.5, 0.1 + (double) (this.mineLevel - 120) * 0.005);
        if (this.mineRandom.NextDouble() < num5)
          return new StardewValley.Object(tile, 765, "Stone", true, false, false, false)
          {
            MinutesUntilReady = 16
          };
        if (this.mineRandom.NextDouble() < num6)
          return new StardewValley.Object(tile, 764, "Stone", true, false, false, false)
          {
            MinutesUntilReady = 8
          };
        if (this.mineRandom.NextDouble() < num7)
          return new StardewValley.Object(tile, 290, "Stone", true, false, false, false)
          {
            MinutesUntilReady = 4
          };
        return new StardewValley.Object(tile, 751, "Stone", true, false, false, false)
        {
          MinutesUntilReady = 2
        };
      }
    }

    double num8 = Game1.dailyLuck / 2.0 + (double) Game1.player.MiningLevel * 0.005;
    if (this.mineLevel > 50 &&
        this.mineRandom.NextDouble() < 0.00025 + (double) this.mineLevel / 120000.0 + 0.0005 * num8 / 2.0)
    {
      num2 = 2;
      num1 = 10;
    }
    else if (gemStoneChance != 0.0 && this.mineRandom.NextDouble() <
             gemStoneChance + gemStoneChance * num8 + (double) this.mineLevel / 24000.0)
      return new StardewValley.Object(tile, this.getRandomGemRichStoneForThisLevel(this.mineLevel), "Stone", true,
        false, false, false)
      {
        MinutesUntilReady = 5
      };

    if (this.mineRandom.NextDouble() < chanceForPurpleStone / 2.0 +
        chanceForPurpleStone * (double) Game1.player.MiningLevel * 0.008 +
        chanceForPurpleStone * (Game1.dailyLuck / 2.0))
      num2 = 44;
    if (this.mineLevel > 100 && this.mineRandom.NextDouble() < chanceForMysticStone +
        chanceForMysticStone * (double) Game1.player.MiningLevel * 0.008 +
        chanceForMysticStone * (Game1.dailyLuck / 2.0))
      num2 = 46;
    int parentSheetIndex = num2 + num2 % 2;
    if (this.mineRandom.NextDouble() < 0.1 && this.getMineArea(-1) != 40)
      return new StardewValley.Object(tile, this.mineRandom.NextDouble() < 0.5 ? 668 : 670, "Stone", true, false, false,
        false)
      {
        MinutesUntilReady = 2,
        Flipped = this.mineRandom.NextDouble() < 0.5
      };
    return new StardewValley.Object(tile, parentSheetIndex, "Stone", true, false, false, false)
    {
      MinutesUntilReady = num1
    };
  }