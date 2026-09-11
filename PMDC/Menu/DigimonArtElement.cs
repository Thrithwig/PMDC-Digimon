using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueElements;
using RogueEssence;
using RogueEssence.Content;
using RogueEssence.Data;
using RogueEssence.Menu;

namespace PMDC.Menu
{
    /// <summary>Original-resolution art for the terminal preview, owned by that menu.</summary>
    public sealed class DigimonArtElement : BaseMenuElement, IDisposable
    {
        private BaseSheet sheet;
        private readonly Loc location;
        private readonly int size;
        public DigimonArtElement(string species, int x, int y, int size)
        {
            location = new Loc(x, y);
            this.size = size;
            int number = DataManager.Instance.GetMonster(species).IndexNum;
            sheet = BaseSheet.Import(PathMod.ModPath("Content/StaticCreature/" + number + ".png"));
        }
        public override void Draw(SpriteBatch spriteBatch, Loc offset)
        {
            if (sheet == null) return;
            float scale = Math.Min((float)size / sheet.Width, (float)size / sheet.Height);
            int width = Math.Max(1, (int)(sheet.Width * scale));
            int height = Math.Max(1, (int)(sheet.Height * scale));
            sheet.Draw(spriteBatch, new Rectangle(location.X + offset.X + (size-width)/2,
                location.Y + offset.Y + (size-height)/2, width, height), null, Color.White);
        }
        public void Dispose() { sheet?.Dispose(); sheet = null; }
    }
}
