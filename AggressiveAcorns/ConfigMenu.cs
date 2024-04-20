using GenericModConfigMenu;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns
{
    internal class ConfigMenu
    {

        private readonly IGenericModConfigMenuApi _api;
        private readonly IManifest _manifest;
        private readonly ModConfig _modConfig;
        private readonly List<(string key, string name, Action contents)> Pages = [];


        public ConfigMenu(IGenericModConfigMenuApi api, IManifest manifest, ModConfig modConfig)
        {
            this._api = api;
            this._manifest = manifest;
            this._modConfig = modConfig;

            this.Pages.Add((key: "pf.aa.page_general", name: "General", contents: this.Page_General));
            this.Pages.Add((key: "pf.aa.page_growth", name: "Growth", contents: this.Page_Growth));
        }


        public void Build(Action saveConfig, Action resetConfig)
        {
            this.Register(reset: resetConfig, save: saveConfig);

            // ==== Home Page ====
            foreach (var (key, name, _) in this.Pages)
            {
                this.AddPageLink(key, () => "> " + name);
            }

            this.AddParagraph(() => "");
            this.AddSectionTitle(() => "Growth stages:");
            this.AddParagraph(() =>
                "0 ------- seed\n" +
                "1 ------- sprout\n" +
                "2 ------- sapling\n" +
                "3 - 4 --- bush\n" +
                "5 - 13 -- tree\n" +
                "14 - 15 - tree, can grow moss"
            );

            // ==== Pages ====
            foreach (var (key, name, contents) in this.Pages)
            {
                this.AddPage(pageId: key, pageTitle: () => name);
                contents();
            }
        }


        // ================= General ==============================================================
        private void Page_General()
        {


            this.AddBoolOption(
                name: () => "Melee weapons destroy saplings",
                getValue: () => this._modConfig.DoMeleeWeaponsDestroySeedlings,
                setValue: value => this._modConfig.DoMeleeWeaponsDestroySeedlings = value
            );

            this.AddNumberOption(
                name: () => "Max passable growth stage",
                getValue: () => this._modConfig.MaxPassableGrowthStage,
                setValue: value => this._modConfig.MaxPassableGrowthStage = value,
                min: -1,
                max: 15,
                formatValue: Format_GrowthStage
            );
        }


        // ================= Growth ===============================================================
        private void Page_Growth()
        {
            this.AddParagraph(() => "Use these options to set the growth chances for all trees.\n" + 
                                    "Growth chances for individual types of trees may be overridden below.");

            this.AddParagraph(() => "When a percentage-chance config option is set below zero, Aggressive Acorns will not apply any changes to it  - it is \"unchanged\" by this mod. " +
                                    "This means that the value will be as set by Stardew Valley or any other installed mods.\n" +
                                    "This is mainly useful in overrides, to make a given type of tree ignore the general setting and revert to baseline behaviour.");
            
            this.AddParagraph(() => "To add more tree types to the override lists, edit Mods/AggressiveAcorns/config.json when the game is closed.");

            this.AddPercentageOption(
                name: () => "Growth",
                getValue: () => this._modConfig.ChanceGrowth,
                setValue: value => this._modConfig.ChanceGrowth = value
            );

            this.AddPercentageOption(
                name: () => "Fertilized Growth",
                getValue: () => this._modConfig.ChanceGrowthFertilized,
                setValue: value => this._modConfig.ChanceGrowthFertilized = value
            );

            this.AddParagraph(() => "");
            this.AddSectionTitle(() => "Overrides:");

            string[] overriddenTypes = this._modConfig.ChanceGrowth_Overrides.Keys
                .Union(this._modConfig.ChanceGrowthFertilized_Overrides.Keys)
                .OrderBy(s => s)
                .ToArray();

            foreach (string treeType in overriddenTypes)
            {
                this.AddSectionTitle(() => GetTypeNameElseQuote(treeType));
                if (this._modConfig.ChanceGrowth_Overrides.ContainsKey(treeType))
                {
                    this.AddPercentageOption(
                        name: () => " . . . Growth",
                        getValue: () => this._modConfig.ChanceGrowth_Overrides[treeType],
                        setValue: value => this._modConfig.ChanceGrowth_Overrides[treeType] = value
                    );
                }
                if (this._modConfig.ChanceGrowthFertilized_Overrides.ContainsKey(treeType))
                {
                    this.AddPercentageOption(
                        name: () => " . . . Fertilized Growth",
                        getValue: () => this._modConfig.ChanceGrowthFertilized_Overrides[treeType],
                        setValue: value => this._modConfig.ChanceGrowthFertilized_Overrides[treeType] = value
                    );
                }
            }
        }


        // ============================ UTILITY + WRAPPERS =====================

        private static string Format_Percentage(int value)
        {
            return value switch
            {
                -1 => "(unchanged)",
                _ => value.ToString() + "%",
            };
        }


        private static string Format_GrowthStage(int stage)
        {
            string desc = stage switch
            {
                >= Tree.stageForMossGrowth => "tree",
                >= Tree.treeStage => "tree",
                >= Tree.bushStage => "bush",
                Tree.saplingStage => "sapling",
                Tree.sproutStage => "sprout",
                Tree.seedStage => "seed",
                < 0 => "none",
            };

            return $"{stage} : {desc}";
        }


        private static string GetTypeNameElseQuote(string typeId)
        {
            if (ModConfigUtils.TreeNames.TryGetValue(typeId, out string result)) return $"{result} ('{typeId}')";
            return $"'{typeId}'";
        }


        private void AddPercentageOption(Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null)
        {
            this.AddNumberOption(getValue, setValue, name, tooltip, min: -1, max: 100, formatValue: Format_Percentage, fieldId: fieldId);
        }

        /*********
        ** Methods
        *********/

        /// <summary>Register a mod whose config can be edited through the UI.</summary>
        /// <param name="reset">Reset the mod's config to its default values.</param>
        /// <param name="save">Save the mod's current config to the <c>config.json</c> file.</param>
        /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
        /// <remarks>Each mod can only be registered once, unless it's deleted via <see cref="Unregister"/> before calling this again.</remarks>
        private void Register(Action reset, Action save, bool titleScreenOnly = false) => this._api.Register(this._manifest, reset, save, titleScreenOnly);


        /****
        ** Basic options
        ****/

        /// <summary>Add a section title at the current position in the form.</summary>
        /// <param name="text">The title text shown in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the title, or <c>null</c> to disable the tooltip.</param>
        private void AddSectionTitle(Func<string> text, Func<string> tooltip = null)
        {
            this._api.AddSectionTitle(this._manifest, text, tooltip);
        }

        /// <summary>Add a paragraph of text at the current position in the form.</summary>
        /// <param name="text">The paragraph text to display.</param>
        private void AddParagraph(Func<string> text)
        {
            this._api.AddParagraph(this._manifest, text);
        }

        /// <summary>Add an image at the current position in the form.</summary>
        /// <param name="texture">The image texture to display.</param>
        /// <param name="texturePixelArea">The pixel area within the texture to display, or <c>null</c> to show the entire image.</param>
        /// <param name="scale">The zoom factor to apply to the image.</param>
        private void AddImage(Func<Texture2D> texture, Rectangle? texturePixelArea = null, int scale = Game1.pixelZoom)
        {
            this._api.AddImage(this._manifest, texture, texturePixelArea, scale);
        }

        /// <summary>Add a boolean option at the current position in the form.</summary>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        private void AddBoolOption(Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null)
        {
            this._api.AddBoolOption(this._manifest, getValue, setValue, name, tooltip, fieldId);
        }

        /// <summary>Add an integer option at the current position in the form.</summary>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="min">The minimum allowed value, or <c>null</c> to allow any.</param>
        /// <param name="max">The maximum allowed value, or <c>null</c> to allow any.</param>
        /// <param name="interval">The interval of values that can be selected.</param>
        /// <param name="formatValue">Get the display text to show for a value, or <c>null</c> to show the number as-is.</param>
        /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        private void AddNumberOption(Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null)
        {
            this._api.AddNumberOption(this._manifest, getValue, setValue, name, tooltip, min, max, interval, formatValue, fieldId);
        }

        /// <summary>Add a float option at the current position in the form.</summary>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="min">The minimum allowed value, or <c>null</c> to allow any.</param>
        /// <param name="max">The maximum allowed value, or <c>null</c> to allow any.</param>
        /// <param name="interval">The interval of values that can be selected.</param>
        /// <param name="formatValue">Get the display text to show for a value, or <c>null</c> to show the number as-is.</param>
        /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        private void AddNumberOption(Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null)
        {
            this._api.AddNumberOption(this._manifest, getValue, setValue, name, tooltip, min, max, interval, formatValue, fieldId);
        }

        /// <summary>Add a string option at the current position in the form.</summary>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="allowedValues">The values that can be selected, or <c>null</c> to allow any.</param>
        /// <param name="formatAllowedValue">Get the display text to show for a value from <paramref name="allowedValues"/>, or <c>null</c> to show the values as-is.</param>
        /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        private void AddTextOption(Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null)
        {
            this._api.AddTextOption(this._manifest, getValue, setValue, name, tooltip, allowedValues, formatAllowedValue, fieldId);
        }

        /// <summary>Add a key binding at the current position in the form.</summary>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        private void AddKeybind(Func<SButton> getValue, Action<SButton> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null)
        {
            this._api.AddKeybind(this._manifest, getValue, setValue, name, tooltip, fieldId);
        }

        /// <summary>Add a key binding list at the current position in the form.</summary>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        private void AddKeybindList(Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null)
        {
            this._api.AddKeybindList(this._manifest, getValue, setValue, name, tooltip, fieldId);
        }


        /****
        ** Multi-page management
        ****/
        /// <summary>Start a new page in the mod's config UI, or switch to that page if it already exists. All options registered after this will be part of that page.</summary>
        /// <param name="pageId">The unique page ID.</param>
        /// <param name="pageTitle">The page title shown in its UI, or <c>null</c> to show the <paramref name="pageId"/> value.</param>
        /// <remarks>You must also call <see cref="AddPageLink"/> to make the page accessible. This is only needed to set up a multi-page config UI. If you don't call this method, all options will be part of the mod's main config UI instead.</remarks>
        private void AddPage(string pageId, Func<string> pageTitle = null)
        {
            this._api.AddPage(this._manifest, pageId, pageTitle);
        }

        /// <summary>Add a link to a page added via <see cref="AddPage"/> at the current position in the form.</summary>
        /// <param name="pageId">The unique ID of the page to open when the link is clicked.</param>
        /// <param name="text">The link text shown in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the link, or <c>null</c> to disable the tooltip.</param>
        private void AddPageLink(string pageId, Func<string> text, Func<string> tooltip = null)
        {
            this._api.AddPageLink(this._manifest, pageId, text, tooltip);
        }


        ///****
        //** Advanced
        //****/
        ///// <summary>Add an option at the current position in the form using custom rendering logic.</summary>
        ///// <param name="mod">The mod's manifest.</param>
        ///// <param name="name">The label text to show in the form.</param>
        ///// <param name="draw">Draw the option in the config UI. This is called with the sprite batch being rendered and the pixel position at which to start drawing.</param>
        ///// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        ///// <param name="beforeMenuOpened">A callback raised just before the menu containing this option is opened.</param>
        ///// <param name="beforeSave">A callback raised before the form's current values are saved to the config (i.e. before the <c>save</c> callback passed to <see cref="Register"/>).</param>
        ///// <param name="afterSave">A callback raised after the form's current values are saved to the config (i.e. after the <c>save</c> callback passed to <see cref="Register"/>).</param>
        ///// <param name="beforeReset">A callback raised before the form is reset to its default values (i.e. before the <c>reset</c> callback passed to <see cref="Register"/>).</param>
        ///// <param name="afterReset">A callback raised after the form is reset to its default values (i.e. after the <c>reset</c> callback passed to <see cref="Register"/>).</param>
        ///// <param name="beforeMenuClosed">A callback raised just before the menu containing this option is closed.</param>
        ///// <param name="height">The pixel height to allocate for the option in the form, or <c>null</c> for a standard input-sized option. This is called and cached each time the form is opened.</param>
        ///// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        ///// <remarks>The custom logic represented by the callback parameters is responsible for managing its own state if needed. For example, you can store state in a static field or use closures to use a state variable.</remarks>
        //void AddComplexOption(IManifest mod, Func<string> name, Action<SpriteBatch, Vector2> draw, Func<string> tooltip = null, Action beforeMenuOpened = null, Action beforeSave = null, Action afterSave = null, Action beforeReset = null, Action afterReset = null, Action beforeMenuClosed = null, Func<int> height = null, string fieldId = null);

        ///// <summary>Set whether the options registered after this point can only be edited from the title screen.</summary>
        ///// <param name="mod">The mod's manifest.</param>
        ///// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
        ///// <remarks>This lets you have different values per-field. Most mods should just set it once in <see cref="Register"/>.</remarks>
        //void SetTitleScreenOnlyForNextOptions(IManifest mod, bool titleScreenOnly);

        ///// <summary>Register a method to notify when any option registered by this mod is edited through the config UI.</summary>
        ///// <param name="mod">The mod's manifest.</param>
        ///// <param name="onChange">The method to call with the option's unique field ID and new value.</param>
        ///// <remarks>Options use a randomized ID by default; you'll likely want to specify the <c>fieldId</c> argument when adding options if you use this.</remarks>
        //void OnFieldChanged(IManifest mod, Action<string, object> onChange);

        ///// <summary>Open the config UI for a specific mod.</summary>
        ///// <param name="mod">The mod's manifest.</param>
        //void OpenModMenu(IManifest mod);

        ///// <summary>Get the currently-displayed mod config menu, if any.</summary>
        ///// <param name="mod">The manifest of the mod whose config menu is being shown, or <c>null</c> if not applicable.</param>
        ///// <param name="page">The page ID being shown for the current config menu, or <c>null</c> if not applicable. This may be <c>null</c> even if a mod config menu is shown (e.g. because the mod doesn't have pages).</param>
        ///// <returns>Returns whether a mod config menu is being shown.</returns>
        //bool TryGetCurrentMenu(out IManifest mod, out string page);

        ///// <summary>Remove a mod from the config UI and delete all its options and pages.</summary>
        ///// <param name="mod">The mod's manifest.</param>
        //void Unregister(IManifest mod);
    }
}
